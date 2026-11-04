using HR.Core.DTOs;
using Xunit;
using HR.Core.Employees;
using HR.Core.Entities;
using HR.Core.Exceptions;
using HR.Core.Interfaces;
using HR.Core.Services;

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
            service.UpdateAsync(9, SampleUpdate(), CancellationToken.None));

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

        var error = await Assert.ThrowsAsync<HrException>(() => service.LockAsync(4, CancellationToken.None));

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
    public async Task Create_SaveFailure_ReturnsServerError()
    {
        var repository = new FakeEmployeeRepository { FailOnSave = true };
        var service = new EmployeeService(repository);

        var error = await Assert.ThrowsAsync<HrException>(() => service.CreateAsync(Sample("NV003"), CancellationToken.None));

        Assert.Equal(500, error.StatusCode);
        Assert.Equal("SaveError", error.Code);
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

        public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Items.FirstOrDefault(employee => employee.EmployeeId == id));

        public Task<bool> CodeExistsAsync(string employeeCode, CancellationToken cancellationToken) =>
            Task.FromResult(Items.Any(employee => employee.EmployeeCode == employeeCode));

        public Task AddAsync(Employee employee, CancellationToken cancellationToken)
        {
            employee.EmployeeId = Items.Count + 1;
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
