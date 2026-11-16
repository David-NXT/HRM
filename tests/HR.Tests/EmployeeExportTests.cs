using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using HR.API.Auth;
using HR.API.Controllers;
using HR.Application.DTOs.Employees;
using HR.Application.Interfaces.Services;
using HR.Domain.Entities;
using HR.Infrastructure.Exports;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HR.Tests;

public sealed class EmployeeExportTests
{
    [Fact]
    public async Task Export_IncludesEveryMatchingPage_AndPreservesTextAndSalaryTypes()
    {
        var rows = Enumerable.Range(1, 205).Select(index => Sample(index)).ToList();
        rows[0] = rows[0] with { FullName = "=HYPERLINK(\"https://example.com\")", EmployeeCode = "000001" };
        rows[1] = rows[1] with { EmploymentType = "PART_TIME", BaseSalary = null, PartTimeHourlyRate = 30000m, Status = "LOCKED" };
        rows.Add(Sample(999) with { Email = "excluded@example.com" });
        var source = new EmployeeSource(rows);
        var file = await new EmployeeExportService(source).ExportAsync("  matching  ", CancellationToken.None);

        Assert.EndsWith(".xlsx", file.FileName);
        Assert.Equal(new[] { 1, 2, 3 }, source.Requests.Select(request => request.Page));
        Assert.All(source.Requests, request =>
        {
            Assert.Equal("matching", request.Keyword);
            Assert.Equal(100, request.Size);
        });
        using var stream = new MemoryStream(file.Content);
        using (var document = SpreadsheetDocument.Open(stream, false))
            Assert.Empty(new OpenXmlValidator().Validate(document));
        stream.Position = 0;
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet(1);
        Assert.Equal("Tổng số nhân viên: 205", sheet.Cell("A4").GetString());
        Assert.Equal("Mã nhân viên", sheet.Cell("B6").GetString());
        Assert.Equal("000001", sheet.Cell("B7").GetString());
        Assert.Equal("0900123456", sheet.Cell("H7").GetString());
        Assert.Equal(XLDataType.Text, sheet.Cell("B7").DataType);
        Assert.Equal(XLDataType.Text, sheet.Cell("H7").DataType);
        Assert.StartsWith("=HYPERLINK", sheet.Cell("C7").GetString());
        Assert.False(sheet.Cell("C7").HasFormula);
        Assert.Equal(XLDataType.Number, sheet.Cell("J7").DataType);
        Assert.Equal(12000000m, sheet.Cell("J7").GetValue<decimal>());
        Assert.Equal("VND/tháng", sheet.Cell("K7").GetString());
        Assert.Equal(30000m, sheet.Cell("J8").GetValue<decimal>());
        Assert.Equal("VND/giờ", sheet.Cell("K8").GetString());
        Assert.Equal("Đã khóa", sheet.Cell("I8").GetString());
        Assert.Equal("NV0205", sheet.Cell("B211").GetString());
        Assert.Equal(11, sheet.LastColumnUsed()!.ColumnNumber());
        Assert.Equal(211, sheet.LastRowUsed()!.RowNumber());
        Assert.Equal(6, sheet.SheetView.SplitRow);
        Assert.True(sheet.AutoFilter.IsEnabled);
    }

    [Fact]
    public async Task Export_NoMatches_ProducesReadableWorkbookWithHeadersAndZeroTotal()
    {
        var file = await new EmployeeExportService(new EmployeeSource([]))
            .ExportAsync(null, CancellationToken.None);
        using var stream = new MemoryStream(file.Content);
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet(1);
        Assert.Equal("Tổng số nhân viên: 0", sheet.Cell("A4").GetString());
        Assert.Equal("Từ khóa: Tất cả nhân viên", sheet.Cell("A3").GetString());
        Assert.Equal("Đơn vị lương", sheet.Cell("K6").GetString());
        Assert.True(sheet.Cell("A7").IsEmpty());
    }

    [Fact]
    public async Task Export_CancelledRequest_DoesNotFetchEmployeeData()
    {
        var source = new EmployeeSource([Sample(1)]);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new EmployeeExportService(source).ExportAsync(null, cancellation.Token));
        Assert.Empty(source.Requests);
    }

    [Fact]
    public async Task ExportEndpoint_ReturnsExcelAttachment_AndRetainsHrRoleFilter()
    {
        var source = new EmployeeSource([Sample(1)]);
        var result = Assert.IsType<FileContentResult>(await new EmployeesController(source)
            .Export(null, new EmployeeExportService(source), CancellationToken.None));
        Assert.Equal(EmployeeExportFile.ContentType, result.ContentType);
        Assert.EndsWith(".xlsx", result.FileDownloadName);
        Assert.Contains(typeof(EmployeesController).GetCustomAttributes(typeof(ServiceFilterAttribute), true),
            attribute => ((ServiceFilterAttribute)attribute).ServiceType == typeof(HrRoleFilter));
    }

    private static EmployeeDto Sample(int index) => new(
        Guid.NewGuid(), $"NV{index:D4}", $"Nhân viên {index}", new DateOnly(2000, 1, 1), "Other",
        $"matching-{index}@example.com", "0900123456", "000000000001", null, "ACTIVE",
        DateTime.UtcNow, DateTime.UtcNow, null, null, null, null, "FULL_TIME", 12000000m, null);

    private sealed class EmployeeSource(IReadOnlyList<EmployeeDto> employees) : IEmployeeService
    {
        public List<(string? Keyword, int Page, int Size)> Requests { get; } = [];
        public Task<EmployeeListDto> ListAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Requests.Add((keyword, page, pageSize));
            var matches = employees.Where(employee => string.IsNullOrEmpty(keyword)
                || employee.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToArray();
            return Task.FromResult(new EmployeeListDto(matches.Skip((page - 1) * pageSize).Take(pageSize).ToArray(),
                page, pageSize, matches.Length));
        }

        public Task<EmployeeStatisticsDto> GetStatisticsAsync(CancellationToken token) => throw new NotSupportedException();
        public Task<EmployeeDirectoryListDto> ListActiveDirectoryAsync(string? keyword, int page, int pageSize, CancellationToken token) => throw new NotSupportedException();
        public Task<EmployeeDirectoryEntry> GetActiveDirectoryEntryAsync(Guid id, CancellationToken token) => throw new NotSupportedException();
        public Task<EmployeeDto> GetAsync(Guid id, CancellationToken token) => throw new NotSupportedException();
        public Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken token) => throw new NotSupportedException();
        public Task DeleteAsync(Guid id, CancellationToken token) => throw new NotSupportedException();
        public Employee PrepareNewEmployee(CreateEmployeeRequest request) => throw new NotSupportedException();
        public Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken token) => throw new NotSupportedException();
        public Task<EmployeeDto> UpdateSalaryAsync(Guid id, UpdateEmployeeSalaryRequest request, CancellationToken token) => throw new NotSupportedException();
        public void ValidatePersonalInfoUpdate(UpdateEmployeeRequest request) => throw new NotSupportedException();
        public void ApplyPersonalInfoUpdate(Employee employee, UpdateEmployeeRequest request) => throw new NotSupportedException();
        public Task<EmployeeDto> LockAsync(Guid id, CancellationToken token) => throw new NotSupportedException();
    }
}
