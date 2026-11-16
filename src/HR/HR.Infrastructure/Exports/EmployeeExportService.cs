using ClosedXML.Excel;
using HR.Application.DTOs.Employees;
using HR.Application.Interfaces.Services;
using HR.Domain.Constants;

namespace HR.Infrastructure.Exports;

public sealed class EmployeeExportService(IEmployeeService employees) : IEmployeeExportService
{
    public async Task<EmployeeExportFile> ExportAsync(string? keyword, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        keyword = keyword?.Trim();
        var exportedAt = DateTimeOffset.Now;
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Danh sách nhân viên");
        sheet.Style.Font.FontName = "Arial";
        sheet.Style.Font.FontSize = 11;
        sheet.Range("A1:K1").Merge().Value = "DANH SÁCH NHÂN VIÊN";
        sheet.Cell("A1").Style.Font.SetBold().Font.SetFontSize(16);
        sheet.Range("A2:K2").Merge().Value = $"Thời điểm xuất: {exportedAt:dd/MM/yyyy HH:mm:ss zzz}";
        sheet.Range("A3:K3").Merge().Value = "Từ khóa: " + (string.IsNullOrEmpty(keyword) ? "Tất cả nhân viên" : keyword);

        string[] headers = ["STT", "Mã nhân viên", "Họ tên", "Loại nhân viên", "Phòng ban", "Chức vụ",
            "Email", "Điện thoại", "Trạng thái", "Mức lương", "Đơn vị lương"];
        double[] widths = [7, 22, 30, 18, 26, 26, 36, 20, 22, 22, 18];
        for (var column = 1; column <= headers.Length; column++)
        {
            sheet.Cell(6, column).Value = headers[column - 1];
            sheet.Column(column).Width = widths[column - 1];
        }
        sheet.Range("A6:K6").Style.Fill.BackgroundColor = XLColor.FromArgb(35, 83, 166);
        sheet.Range("A6:K6").Style.Font.FontColor = XLColor.White;
        sheet.Range("A6:K6").Style.Font.Bold = true;
        sheet.Row(6).Height = 26;
        sheet.Column(2).Style.NumberFormat.Format = "@";
        sheet.Column(8).Style.NumberFormat.Format = "@";
        sheet.Column(10).Style.NumberFormat.Format = "#,##0.##";
        sheet.SheetView.FreezeRows(6);
        sheet.SheetView.FreezeColumns(2);

        const int pageSize = 100;
        var row = 7;
        for (var page = 1; ; page++)
        {
            var batch = await employees.ListAsync(keyword, page, pageSize, cancellationToken);
            foreach (var employee in batch.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var partTime = employee.EmploymentType == EmploymentTypes.PartTime;
                sheet.Cell(row, 1).Value = row - 6;
                // Assign strings as values, never formulas, including user-supplied names and codes.
                sheet.Cell(row, 2).Value = employee.EmployeeCode;
                sheet.Cell(row, 3).Value = employee.FullName;
                sheet.Cell(row, 4).Value = partTime ? "Part-time" : "Full-time";
                sheet.Cell(row, 5).Value = employee.DepartmentName ?? "Chưa phân công";
                sheet.Cell(row, 6).Value = employee.PositionName ?? "Chưa phân công";
                sheet.Cell(row, 7).Value = employee.Email;
                sheet.Cell(row, 8).Value = employee.Phone;
                sheet.Cell(row, 9).Value = employee.Status switch
                {
                    EmployeeStatuses.Active => "Đang hoạt động",
                    EmployeeStatuses.Locked => "Đã khóa",
                    EmployeeStatuses.Resigned => "Đã nghỉ việc",
                    _ => employee.Status
                };
                var salary = partTime ? employee.PartTimeHourlyRate : employee.BaseSalary;
                if (salary.HasValue) sheet.Cell(row, 10).Value = salary.Value;
                sheet.Cell(row, 11).Value = partTime ? "VND/giờ" : "VND/tháng";
                row++;
            }
            if (batch.Items.Count == 0 || (long)page * pageSize >= batch.Total) break;
        }

        sheet.Range("A4:K4").Merge().Value = $"Tổng số nhân viên: {row - 7}";
        sheet.Range(6, 1, Math.Max(6, row - 1), 11).SetAutoFilter();
        cancellationToken.ThrowIfCancellationRequested();
        using var output = new MemoryStream();
        workbook.SaveAs(output);
        cancellationToken.ThrowIfCancellationRequested();
        return new EmployeeExportFile(output.ToArray(), $"DanhSachNhanVien_{exportedAt:yyyyMMdd_HHmmss}.xlsx");
    }
}
