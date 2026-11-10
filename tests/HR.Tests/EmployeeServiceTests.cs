using HR.Application.DTOs.Employees;
using HR.Application.Interfaces.Repositories;
using HR.Application.Services.Employees;
using Xunit;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
namespace HR.Tests;

public class EmployeeServiceTests
{
    [Fact]
    public async Task Create_ValidProfile_SavesActiveEmployee()
    {
        var repository = new FakeEmployeeRepository();
        var service = new EmployeeService(repository);

        var created = await service.CreateAsync(Sample("nv001"), CancellationToken.None);

        Assert.Equal("NV001", created.EmployeeCode);
        Assert.Equal(EmployeeStatuses.Active, created.Status);
        Assert.Equal(Genders.Male, created.Gender);
        Assert.Equal("0901234567", created.Phone);
        Assert.Equal(1, repository.SaveCount);
    }

    [Fact]
    public async Task Create_InvalidProfile_DoesNotSave()
    {
        var repository = new FakeEmployeeRepository();
        var service = new EmployeeService(repository);
        var request = Sample("NV001") with { Email = "khong-dung", FullName = "A" };

        var error = await Assert.ThrowsAsync<HrException>(() => service.CreateAsync(request, CancellationToken.None));

        Assert.Equal(400, error.StatusCode);
        Assert.Equal("ValidationError", error.Code);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task Create_DuplicateCode_ReturnsConflict()
    {
        var repository = new FakeEmployeeRepository();
        var service = new EmployeeService(repository);
        await service.CreateAsync(Sample("NV001"), CancellationToken.None);

        var error = await Assert.ThrowsAsync<HrException>(() => service.CreateAsync(Sample("nv001"), CancellationToken.None));

        Assert.Equal(409, error.StatusCode);
        Assert.Equal("DuplicateEmployeeCode", error.Code);
    }

    [Fact]
    public async Task Update_MissingEmployee_ReturnsNotFound()
    {
        var service = new EmployeeService(new FakeEmployeeRepository());

        var error = await Assert.ThrowsAsync<HrException>(() =>
            service.UpdateAsync(Id(9), SampleUpdate(), CancellationToken.None));

        Assert.Equal(404, error.StatusCode);
        Assert.Equal("EmployeeNotFound", error.Code);
    }

    [Fact]
    public async Task Update_ExistingEmployee_KeepsCodeAndStatus()
    {
        var repository = new FakeEmployeeRepository();
        var service = new EmployeeService(repository);
        var created = await service.CreateAsync(Sample("NV001"), CancellationToken.None);

        var updated = await service.UpdateAsync(
            created.EmployeeId,
            SampleUpdate() with { FullName = "Trần Thị B", Gender = "Nữ" },
            CancellationToken.None);

        Assert.Equal("NV001", updated.EmployeeCode);
        Assert.Equal("Trần Thị B", updated.FullName);
        Assert.Equal(Genders.Female, updated.Gender);
        Assert.Equal(EmployeeStatuses.Active, updated.Status);
    }

    [Fact]
    public async Task Lock_MissingEmployee_ReturnsNotFound()
    {
        var service = new EmployeeService(new FakeEmployeeRepository());

        var error = await Assert.ThrowsAsync<HrException>(() => service.LockAsync(Id(4), CancellationToken.None));

        Assert.Equal(404, error.StatusCode);
    }

    [Fact]
    public async Task Lock_ExistingEmployee_SetsLocked()
    {
        var repository = new FakeEmployeeRepository();
        var service = new EmployeeService(repository);
        var created = await service.CreateAsync(Sample("NV002"), CancellationToken.None);

        var locked = await service.LockAsync(created.EmployeeId, CancellationToken.None);

        Assert.Equal(EmployeeStatuses.Locked, locked.Status);
    }

    [Fact]
    public async Task Lock_ResignedEmployee_KeepsResignedStatus()
    {
        var repository = new FakeEmployeeRepository();
        var service = new EmployeeService(repository);
        var created = await service.CreateAsync(Sample("NV002"), CancellationToken.None);
        repository.Items[0].Status = EmployeeStatuses.Resigned;

        var error = await Assert.ThrowsAsync<HrException>(() => service.LockAsync(created.EmployeeId, CancellationToken.None));

        Assert.Equal(409, error.StatusCode);
        Assert.Equal(EmployeeStatuses.Resigned, repository.Items[0].Status);
        Assert.Equal(1, repository.SaveCount);
    }

    [Fact]
    public async Task Create_SaveFailure_ReturnsServerError()
    {
        var repository = new FakeEmployeeRepository { FailOnSave = true };
        var service = new EmployeeService(repository);

        var error = await Assert.ThrowsAsync<HrException>(() => service.CreateAsync(Sample("NV003"), CancellationToken.None));

        Assert.Equal(500, error.StatusCode);
        Assert.Equal("SaveError", error.Code);
    }

    [Fact]
    public async Task InternalDirectory_ReturnsGuidAndOrganizationWithoutPersonalData()
    {
        var repository = new FakeEmployeeRepository();
        var employee = new Employee { EmployeeId = Id(23), EmployeeCode = "NV001", FullName = "Nguyễn Văn A",
            CitizenId = "079198001234", Email = "private@example.com", Department = new Department { Name = "Nhân sự" }, Position = new Position { Name = "Chuyên viên" } };
        repository.Items.Add(employee);
        var dto = await new EmployeeService(repository).GetActiveDirectoryEntryAsync(employee.EmployeeId, CancellationToken.None);
        Assert.Equal(employee.EmployeeId, dto.EmployeeId);
        Assert.Equal("Nhân sự", dto.DepartmentName);
        Assert.Equal("Chuyên viên", dto.PositionName);
        var json = System.Text.Json.JsonSerializer.Serialize(dto);
        Assert.DoesNotContain(employee.CitizenId, json);
        Assert.DoesNotContain(employee.Email, json);
        Assert.Equal(6, System.Text.Json.JsonDocument.Parse(json).RootElement.EnumerateObject().Count());
    }

    [Theory]
    [InlineData(EmployeeStatuses.Locked)]
    [InlineData(EmployeeStatuses.Resigned)]
    public async Task InternalDirectory_NonActiveEmployeeReturns404(string status)
    {
        var repository = new FakeEmployeeRepository();
        var employee = new Employee { EmployeeId = Id(1), Status = status }; repository.Items.Add(employee);
        var error = await Assert.ThrowsAsync<HrException>(() => new EmployeeService(repository)
            .GetActiveDirectoryEntryAsync(employee.EmployeeId, CancellationToken.None));
        Assert.Equal(404, error.StatusCode);
    }

    [Fact]
    public async Task List_InvalidPage_ReturnsValidationError()
    {
        var service = new EmployeeService(new FakeEmployeeRepository());

        var error = await Assert.ThrowsAsync<HrException>(() => service.ListAsync(null, 0, 20, CancellationToken.None));

        Assert.Equal(400, error.StatusCode);
    }

    private static CreateEmployeeRequest Sample(string code) =>
        new(code, "Nguyễn Văn A", new DateOnly(1998, 5, 12), "Nam", "vana@example.com", "+84901234567", "079198001234", "Quận 1");

    private static UpdateEmployeeRequest SampleUpdate() =>
        new("Nguyễn Văn A", new DateOnly(1998, 5, 12), "Male", "vana@example.com", "0901234567", "079198001234", "Quận 1");

    private sealed class FakeEmployeeRepository : IEmployeeRepository
    {
        public List<Employee> Items { get; } = [];

        public int SaveCount { get; private set; }

        public bool FailOnSave { get; set; }

        public Task<(IReadOnlyList<Employee> Items, int Total)> SearchAsync(
            string? keyword,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var items = Items.Where(employee =>
                    string.IsNullOrWhiteSpace(keyword)
                    || employee.EmployeeCode.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || employee.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            return Task.FromResult(((IReadOnlyList<Employee>)items, items.Length));
        }

        public Task<(IReadOnlyList<Employee> Items, int Total)> SearchActiveAsync(
            string? keyword,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var items = Items.Where(employee =>
                    employee.Status == EmployeeStatuses.Active
                    && (string.IsNullOrWhiteSpace(keyword)
                        || employee.EmployeeCode.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                        || employee.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
            return Task.FromResult(((IReadOnlyList<Employee>)items, items.Length));
        }

        public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Items.FirstOrDefault(employee => employee.EmployeeId == id));

        public Task<bool> CodeExistsAsync(string employeeCode, CancellationToken cancellationToken) =>
            Task.FromResult(Items.Any(employee => employee.EmployeeCode == employeeCode));

        public Task AddAsync(Employee employee, CancellationToken cancellationToken)
        {
            employee.EmployeeId = Id(Items.Count + 1);
            Items.Add(employee);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveCount++;
            if (FailOnSave)
            {
                throw new HrException(500, "SaveError", "Không thể lưu hồ sơ. Vui lòng thử lại.");
            }

            return Task.CompletedTask;
        }
    }
}
