using HR.Application.DTOs.Employees;
using HR.Application.DTOs.Lifecycle;
using HR.Application.Interfaces.Repositories;
using HR.Application.Services.Employees;
using HR.Application.Services.Lifecycle;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
using Xunit;

namespace HR.Tests;

public sealed class LifecycleServiceTests
{
    private static readonly DateOnly Today = new(2026, 11, 10);

    [Fact]
    public async Task Onboarding_PartTimeCreatesSubtypeAndWorkHistoryInTheSameTransaction()
    {
        var store = new FakeHrStore();
        var request = Onboarding();
        request = request with { Employee = request.Employee with { EmploymentType = EmploymentTypes.PartTime } };
        var result = await Service(store).StartOnboardingAsync(request, CancellationToken.None);

        var employee = Assert.IsType<PartTimeEmployee>(Assert.Single(store.Employees));
        Assert.Equal(EmploymentTypes.PartTime, result.EmploymentType);
        Assert.Equal(employee.EmployeeId, result.EmployeeId);
        Assert.Equal(employee.EmployeeId, Assert.Single(store.History).EmployeeId);
        Assert.Equal(1, store.TransactionSaves);
    }

    [Fact]
    public async Task Onboarding_CreatesNormalizedActiveEmployeeAndFirstAssignmentTogether()
    {
        var store = new FakeHrStore();
        var result = await Service(store).StartOnboardingAsync(Onboarding(), CancellationToken.None);

        var employee = Assert.Single(store.Employees);
        var history = Assert.Single(store.History);
        Assert.Equal("NV001", employee.EmployeeCode);
        Assert.Equal("0901234567", employee.Phone);
        Assert.Equal(EmployeeStatuses.Active, result.Status);
        Assert.NotEqual(Guid.Empty, result.EmployeeId);
        Assert.Equal(employee.EmployeeId, history.EmployeeId);
        Assert.Equal(store.Department.DepartmentId, employee.DepartmentId);
        Assert.Equal(store.Position.PositionId, history.PositionId);
        Assert.Null(history.EndDate);
        Assert.Equal(1, store.TransactionSaves);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(PositionSeniorityLevels.Manager)]
    [InlineData(PositionSeniorityLevels.Director)]
    [InlineData(PositionSeniorityLevels.Executive)]
    public async Task Onboarding_PartTimeRequiresStaffSeniorityBeforeSaving(string? seniorityLevel)
    {
        var store = new FakeHrStore();
        store.Position.SeniorityLevel = seniorityLevel;
        var request = Onboarding();
        request = request with { Employee = request.Employee with { EmploymentType = EmploymentTypes.PartTime } };

        var error = await Assert.ThrowsAsync<HrException>(() =>
            Service(store).StartOnboardingAsync(request, CancellationToken.None));

        Assert.Equal(400, error.StatusCode);
        Assert.Equal("PartTimePositionNotAllowed", error.Code);
        Assert.Empty(store.Employees);
        Assert.Empty(store.History);
        Assert.Equal(0, store.TransactionSaves);
    }

    [Fact]
    public async Task Onboarding_FutureStartKeepsCurrentOrganizationEmptyUntilEffectiveDate()
    {
        var store = new FakeHrStore();
        var result = await Service(store).StartOnboardingAsync(Onboarding() with { StartDate = Today.AddDays(10) }, CancellationToken.None);

        Assert.Equal(EmployeeStatuses.Active, result.Status);
        Assert.Null(Assert.Single(store.Employees).DepartmentId);
        Assert.Null(Assert.Single(store.Employees).PositionId);
        Assert.Equal(Today.AddDays(10), Assert.Single(store.History).StartDate);
    }

    [Fact]
    public async Task Onboarding_InactiveDepartmentDoesNotCreatePartialEmployee()
    {
        var store = new FakeHrStore();
        store.Department.Status = OrgStatuses.Inactive;
        var error = await Assert.ThrowsAsync<HrException>(() => Service(store).StartOnboardingAsync(Onboarding(), CancellationToken.None));

        Assert.Equal(400, error.StatusCode);
        Assert.Equal("InvalidDepartment", error.Code);
        Assert.Empty(store.Employees);
        Assert.Empty(store.History);
        Assert.Equal(0, store.TransactionSaves);
    }

    [Fact]
    public async Task Onboarding_DuplicateNormalizedCodeDoesNotCreateHistory()
    {
        var store = new FakeHrStore();
        store.Employees.Add(Employee());
        var error = await Assert.ThrowsAsync<HrException>(() => Service(store).StartOnboardingAsync(Onboarding(), CancellationToken.None));

        Assert.Equal(409, error.StatusCode);
        Assert.Equal("DuplicateEmployeeCode", error.Code);
        Assert.Single(store.Employees);
        Assert.Empty(store.History);
    }

    [Fact]
    public async Task StartOffboarding_PreservesActiveStatusAndHrSuppliedChecklist()
    {
        var store = ExistingEmployee();
        var result = await Service(store).StartOffboardingAsync(StartOffboarding(), CancellationToken.None);

        Assert.Equal(LifecycleStatuses.InProgress, result.Status);
        Assert.Equal(EmployeeStatuses.Active, result.EmployeeStatus);
        Assert.Equal("Hạng mục do HR nhập", Assert.Single(result.Checklist).Title);
        Assert.True(result.Checklist[0].IsRequired);
        Assert.False(result.Checklist[0].IsCompleted);
        Assert.Null(Assert.Single(store.History).EndDate);
    }

    [Fact]
    public async Task StartOffboarding_DuplicatePendingRecordReturnsConflict()
    {
        var store = ExistingEmployee();
        var service = Service(store);
        await service.StartOffboardingAsync(StartOffboarding(), CancellationToken.None);
        var error = await Assert.ThrowsAsync<HrException>(() => service.StartOffboardingAsync(StartOffboarding(), CancellationToken.None));

        Assert.Equal(409, error.StatusCode);
        Assert.Single(store.Offboardings);
        Assert.Equal(1, store.TransactionSaves);
    }

    [Fact]
    public async Task StartOffboarding_EmptyChecklistDoesNotCreateRecord()
    {
        var store = ExistingEmployee();
        var error = await Assert.ThrowsAsync<HrException>(() => Service(store).StartOffboardingAsync(
            StartOffboarding() with { Checklist = [] }, CancellationToken.None));

        Assert.Equal("InvalidChecklist", error.Code);
        Assert.Empty(store.Offboardings);
        Assert.Equal(0, store.TransactionSaves);
    }

    [Fact]
    public async Task Complete_MissingRequiredItemReturns422WithoutResigningOrClosingHistory()
    {
        var store = ExistingEmployee();
        var record = PendingRecord(store);
        var error = await Assert.ThrowsAsync<HrException>(() => Service(store).CompleteOffboardingAsync(
            record.OffboardingRecordId, new(record.Version), CancellationToken.None));

        Assert.Equal(422, error.StatusCode);
        Assert.Equal("ChecklistIncomplete", error.Code);
        Assert.Contains("Bàn giao", error.Message);
        Assert.Equal(EmployeeStatuses.Active, store.Employees[0].Status);
        Assert.Null(store.History[0].EndDate);
        Assert.Equal(LifecycleStatuses.InProgress, record.Status);
        Assert.Equal(0, store.TransactionSaves);
    }

    [Fact]
    public async Task Complete_AllRequiredDonePreservesHistoryAndIdentityAndIgnoresOptionalItem()
    {
        var store = ExistingEmployee();
        var employee = store.Employees[0];
        var employeeGuid = employee.EmployeeId;
        var previous = new WorkHistory { WorkHistoryId = Id(8), EmployeeId = employee.EmployeeId,
            DepartmentId = Id(1), PositionId = Id(1), StartDate = Today.AddYears(-2), EndDate = Today.AddYears(-1).AddDays(-1) };
        store.History.Add(previous);
        var record = PendingRecord(store, requiredComplete: true);
        record.Checklist.Add(new OffboardingChecklistItem { Title = "Tùy chọn", IsRequired = false });

        var result = await Service(store).CompleteOffboardingAsync(record.OffboardingRecordId, new(record.Version), CancellationToken.None);

        Assert.Equal(LifecycleStatuses.Completed, result.Status);
        Assert.Equal(EmployeeStatuses.Resigned, employee.Status);
        Assert.Equal(employeeGuid, employee.EmployeeId);
        Assert.Equal("NV001", employee.EmployeeCode);
        Assert.Equal(Today, store.History[0].EndDate);
        Assert.Equal(Today.AddYears(-1).AddDays(-1), previous.EndDate);
        Assert.Equal(2, store.History.Count);
        Assert.Single(store.Employees);
        Assert.NotNull(record.CompletedAt);
        Assert.Equal(1, store.TransactionSaves);
    }

    [Fact]
    public async Task Complete_FutureLastWorkingDateDoesNotResignEarly()
    {
        var store = ExistingEmployee();
        var record = PendingRecord(store, requiredComplete: true);
        record.LastWorkingDate = Today.AddDays(1);
        var error = await Assert.ThrowsAsync<HrException>(() => Service(store).CompleteOffboardingAsync(
            record.OffboardingRecordId, new(record.Version), CancellationToken.None));

        Assert.Equal("OffboardingNotEffective", error.Code);
        Assert.Equal(EmployeeStatuses.Active, store.Employees[0].Status);
        Assert.Null(store.History[0].EndDate);
        Assert.Equal(0, store.TransactionSaves);
    }

    [Fact]
    public async Task Complete_RechecksAssignmentAddedAfterOffboardingStarted()
    {
        var store = ExistingEmployee();
        var record = PendingRecord(store, requiredComplete: true);
        store.History.Add(new WorkHistory { EmployeeId = record.EmployeeId, StartDate = Today.AddDays(5) });
        var error = await Assert.ThrowsAsync<HrException>(() => Service(store).CompleteOffboardingAsync(
            record.OffboardingRecordId, new(record.Version), CancellationToken.None));

        Assert.Equal("AssignmentAfterDeparture", error.Code);
        Assert.Equal(EmployeeStatuses.Active, store.Employees[0].Status);
        Assert.Equal(0, store.TransactionSaves);
    }

    [Fact]
    public async Task Complete_ExistingHr01EmployeeWithoutHistoryCanResign()
    {
        var store = ExistingEmployee();
        store.History.Clear();
        var record = PendingRecord(store, requiredComplete: true);
        await Service(store).CompleteOffboardingAsync(record.OffboardingRecordId, new(record.Version), CancellationToken.None);

        Assert.Equal(EmployeeStatuses.Resigned, store.Employees[0].Status);
        Assert.Empty(store.History);
    }

    [Fact]
    public async Task Complete_RepeatedRequestIsIdempotent()
    {
        var store = ExistingEmployee();
        var record = PendingRecord(store, requiredComplete: true);
        var service = Service(store);
        await service.CompleteOffboardingAsync(record.OffboardingRecordId, new(record.Version), CancellationToken.None);
        var firstCompletion = record.CompletedAt;
        await service.CompleteOffboardingAsync(record.OffboardingRecordId, new(0), CancellationToken.None);

        Assert.Equal(1, store.TransactionSaves);
        Assert.Equal(firstCompletion, record.CompletedAt);
    }

    [Fact]
    public async Task Checklist_CanCompleteAndReopenItemBeforeFinalizing()
    {
        var store = ExistingEmployee();
        var record = PendingRecord(store);
        var service = Service(store);
        var updated = await service.UpdateChecklistAsync(record.OffboardingRecordId, Id(1),
            new(true, "Đã nhận bàn giao", record.Version), CancellationToken.None);

        Assert.True(updated.Checklist[0].IsCompleted);
        Assert.NotNull(updated.Checklist[0].CompletedAt);
        Assert.Equal(EmployeeStatuses.Active, store.Employees[0].Status);
        Assert.Equal(LifecycleStatuses.InProgress, record.Status);

        var reopened = await service.UpdateChecklistAsync(record.OffboardingRecordId, Id(1),
            new(false, "Cần bổ sung", record.Version), CancellationToken.None);

        Assert.False(reopened.Checklist[0].IsCompleted);
        Assert.Null(reopened.Checklist[0].CompletedAt);
        Assert.Equal(2, store.TransactionSaves);
    }

    [Fact]
    public async Task Checklist_StaleVersionCannotChangeCompletion()
    {
        var store = ExistingEmployee();
        var record = PendingRecord(store);
        var error = await Assert.ThrowsAsync<HrException>(() => Service(store).UpdateChecklistAsync(
            record.OffboardingRecordId, record.Checklist[0].OffboardingChecklistItemId,
            new(true, null, record.Version + 1), CancellationToken.None));

        Assert.Equal(409, error.StatusCode);
        Assert.False(record.Checklist[0].IsCompleted);
        Assert.Equal(0, store.TransactionSaves);
    }

    [Fact]
    public async Task Checklist_CompletedRecordCannotBeEdited()
    {
        var store = ExistingEmployee();
        var record = PendingRecord(store);
        record.Status = LifecycleStatuses.Completed;
        var error = await Assert.ThrowsAsync<HrException>(() => Service(store).UpdateChecklistAsync(
            record.OffboardingRecordId, record.Checklist[0].OffboardingChecklistItemId,
            new(true, null, record.Version), CancellationToken.None));

        Assert.Equal("OffboardingCompleted", error.Code);
        Assert.False(record.Checklist[0].IsCompleted);
    }

    [Fact]
    public async Task StartOffboarding_ResignedEmployeeReturnsConflict()
    {
        var store = ExistingEmployee();
        store.Employees[0].Status = EmployeeStatuses.Resigned;
        var error = await Assert.ThrowsAsync<HrException>(() => Service(store).StartOffboardingAsync(StartOffboarding(), CancellationToken.None));

        Assert.Equal("EmployeeResigned", error.Code);
        Assert.Empty(store.Offboardings);
    }

    [Fact]
    public async Task Complete_SynchronizesContractsInTheSameSave_AndPreservesSignedDatesAndSalary()
    {
        var store = ExistingEmployee();
        ((FullTimeEmployee)store.Employees[0]).BaseSalary = 12_000_000m;
        var record = PendingRecord(store, requiredComplete: true);
        var expired = new EmploymentContract { EmployeeId = Id(1), ContractNumber = "OLD", StartDate = Today.AddYears(-2), EndDate = Today.AddYears(-1) };
        var current = new EmploymentContract { EmployeeId = Id(1), ContractNumber = "CURRENT", StartDate = Today.AddMonths(-2), EndDate = Today.AddMonths(2), Status = ContractStatuses.Renewed };
        var future = new EmploymentContract { EmployeeId = Id(1), ContractNumber = "FUTURE", StartDate = Today.AddMonths(2).AddDays(1), EndDate = Today.AddYears(1) };
        var indefinite = new EmploymentContract { EmployeeId = Id(1), ContractNumber = "INDEFINITE", StartDate = Today.AddMonths(-1) };
        store.Contracts.AddRange([expired, current, future, indefinite]);

        await Service(store).CompleteOffboardingAsync(record.OffboardingRecordId, new(record.Version), CancellationToken.None);

        Assert.Equal(EmployeeStatuses.Resigned, store.Employees[0].Status);
        Assert.Equal(12_000_000m, ((FullTimeEmployee)store.Employees[0]).BaseSalary);
        Assert.Equal(ContractStatuses.Active, expired.Status);
        Assert.Null(expired.ActualEndDate);
        Assert.Equal(ContractStatuses.Terminated, current.Status);
        Assert.Equal(Today, current.ActualEndDate);
        Assert.Equal(Today.AddMonths(2), current.EndDate);
        Assert.Equal(ContractStatuses.Cancelled, future.Status);
        Assert.Null(future.ActualEndDate);
        Assert.Equal(Today.AddYears(1), future.EndDate);
        Assert.Equal(ContractStatuses.Terminated, indefinite.Status);
        Assert.Equal(Today, indefinite.ActualEndDate);
        Assert.Null(indefinite.EndDate);
        Assert.NotNull(current.TerminationReason);
        Assert.NotNull(future.TerminationReason);
        Assert.Equal(1, store.TransactionSaves);
    }

    [Fact]
    public async Task Complete_MissingChecklistDoesNotCloseOrCancelAnyContract()
    {
        var store = ExistingEmployee();
        var record = PendingRecord(store, requiredComplete: false);
        store.Contracts.Add(new() { EmployeeId = Id(1), StartDate = Today.AddDays(-10), EndDate = Today.AddDays(30) });
        store.Contracts.Add(new() { EmployeeId = Id(1), StartDate = Today.AddDays(31), EndDate = Today.AddYears(1) });

        await Assert.ThrowsAsync<HrException>(() => Service(store).CompleteOffboardingAsync(record.OffboardingRecordId,
            new(record.Version), CancellationToken.None));

        Assert.All(store.Contracts, contract => Assert.Equal(ContractStatuses.Active, contract.Status));
        Assert.All(store.Contracts, contract => Assert.Null(contract.ActualEndDate));
        Assert.Equal(0, store.TransactionSaves);
    }

    [Fact]
    public async Task Complete_ContractEndingOnLastWorkingDayIsTerminated_AndAnotherEmployeeIsUntouched()
    {
        var store = ExistingEmployee();
        var record = PendingRecord(store, requiredComplete: true);
        var current = new EmploymentContract { EmployeeId = Id(1), StartDate = Today.AddMonths(-1), EndDate = Today };
        var other = new EmploymentContract { EmployeeId = Id(2), StartDate = Today.AddMonths(-1), EndDate = Today.AddMonths(1) };
        store.Contracts.AddRange([current, other]);

        await Service(store).CompleteOffboardingAsync(record.OffboardingRecordId, new(record.Version), CancellationToken.None);

        Assert.Equal(ContractStatuses.Terminated, current.Status);
        Assert.Equal(Today, current.EndDate);
        Assert.Equal(Today, current.ActualEndDate);
        Assert.Equal(ContractStatuses.Active, other.Status);
        Assert.Null(other.ActualEndDate);
    }

    private static LifecycleService Service(FakeHrStore store) =>
        new(store, store, new EmployeeService(store), store, new FixedTimeProvider());

    private static StartOnboardingRequest Onboarding() => new(
        new("nv001", "Nguyễn Văn A", new(1998, 5, 12), "Nam", "vana@example.com", "+84901234567", "079198001234", "Quận 1"),
        Today, Id(1), Id(1), "Tiếp nhận");

    private static StartOffboardingRequest StartOffboarding() =>
        new(Id(1), Today, "Nghỉ việc", [new("Hạng mục do HR nhập", true)]);

    private static Employee Employee() => new FullTimeEmployee()
    {
        EmployeeId = Id(1), EmployeeCode = "NV001", FullName = "Nguyễn Văn A", DateOfBirth = new(1998, 5, 12),
        Status = EmployeeStatuses.Active, DepartmentId = Id(1), PositionId = Id(1)
    };

    private static FakeHrStore ExistingEmployee()
    {
        var store = new FakeHrStore();
        store.Employees.Add(Employee());
        store.History.Add(new WorkHistory { WorkHistoryId = Id(1), EmployeeId = Id(1), DepartmentId = Id(1), PositionId = Id(1), StartDate = Today.AddYears(-1) });
        return store;
    }

    private static OffboardingRecord PendingRecord(FakeHrStore store, bool requiredComplete = false)
    {
        var record = new OffboardingRecord { OffboardingRecordId = Id(1), EmployeeId = Id(1),
            Employee = store.Employees[0], LastWorkingDate = Today, Version = 3,
            Checklist = [new() { OffboardingChecklistItemId = Id(1), Title = "Bàn giao", IsRequired = true, IsCompleted = requiredComplete }] };
        store.Offboardings.Add(record);
        return record;
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(2026, 11, 10, 12, 0, 0, TimeSpan.Zero);
    }

    private sealed class FakeHrStore : ILifecycleRepository, IEmployeeRepository, IOrgStructureRepository
    {
        public List<Employee> Employees { get; } = [];
        public List<EmploymentContract> Contracts { get; } = [];
        public List<WorkHistory> History { get; } = [];
        public List<OffboardingRecord> Offboardings { get; } = [];
        public Department Department { get; } = new() { DepartmentId = Id(1), Code = "HR", Name = "Nhân sự" };
        public Position Position { get; } = new()
        {
            PositionId = Id(1), Code = "STAFF", Name = "Nhân viên",
            SeniorityLevel = PositionSeniorityLevels.Staff
        };
        public int TransactionSaves { get; private set; }

        public Task<Employee?> GetEmployeeAsync(Guid id, CancellationToken ct) => Task.FromResult(Employees.FirstOrDefault(item => item.EmployeeId == id));
        public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct) => GetEmployeeAsync(id, ct);
        public Task<Employee?> GetReadOnlyByIdAsync(Guid id, CancellationToken ct) => GetByIdAsync(id, ct);
        public Task<IReadOnlyList<EmploymentContract>> GetContractsAsync(Guid id, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<EmploymentContract>>(Contracts.Where(item => item.EmployeeId == id).ToArray());
        public Task<bool> CodeExistsAsync(string code, CancellationToken ct) => Task.FromResult(Employees.Any(item => item.EmployeeCode == code));
        public Task<IReadOnlyList<WorkHistory>> GetWorkHistoryAsync(Guid id, CancellationToken ct) => Task.FromResult<IReadOnlyList<WorkHistory>>(History.Where(item => item.EmployeeId == id).ToArray());
        public Task<bool> HasPendingOffboardingAsync(Guid id, CancellationToken ct) => Task.FromResult(Offboardings.Any(item => item.EmployeeId == id && item.Status == LifecycleStatuses.InProgress));
        public Task<OffboardingRecord?> GetOffboardingAsync(Guid id, CancellationToken ct) => Task.FromResult(Offboardings.FirstOrDefault(item => item.OffboardingRecordId == id));
        public Task<IReadOnlyList<OffboardingRecord>> ListOffboardingsAsync(Guid? id, string? status, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<OffboardingRecord>>(Offboardings.Where(item => (!id.HasValue || item.EmployeeId == id) && (status is null || item.Status == status)).ToArray());
        public Task AddOnboardingAsync(Employee employee, WorkHistory history, CancellationToken ct)
        {
            employee.EmployeeId = Id(Employees.Count + 1);
            Employees.Add(employee);
            history.EmployeeId = employee.EmployeeId;
            history.WorkHistoryId = Id(History.Count + 1);
            History.Add(history);
            return Task.CompletedTask;
        }
        public Task AddOffboardingAsync(OffboardingRecord record, CancellationToken ct)
        {
            record.OffboardingRecordId = Id(Offboardings.Count + 1);
            Offboardings.Add(record);
            return Task.CompletedTask;
        }
        public Task SaveChangesInTransactionAsync(CancellationToken ct) { TransactionSaves++; return Task.CompletedTask; }
        public Task<Department?> GetDepartmentAsync(Guid id, CancellationToken ct) => Task.FromResult(id == Department.DepartmentId ? Department : null);
        public Task<Position?> GetPositionAsync(Guid id, CancellationToken ct) => Task.FromResult(id == Position.PositionId ? Position : null);
        public Task<IReadOnlyList<Department>> ListDepartmentsAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<Department>>([Department]);
        public Task<IReadOnlyList<Position>> ListPositionsAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<Position>>([Position]);

        public Task<(IReadOnlyList<Employee> Items, int Total)> SearchAsync(string? keyword, int page, int pageSize, CancellationToken ct) => throw new NotSupportedException();
        public Task<(IReadOnlyList<Employee> Items, int Total)> SearchActiveAsync(string? keyword, int page, int pageSize, CancellationToken ct) => throw new NotSupportedException();
        public Task<EmployeeStatisticsDto> GetStatisticsAsync(CancellationToken ct)
        {
            var stats = new EmployeeStatisticsDto(
                Employees.Count,
                Employees.Count(item => item.Status == EmployeeStatuses.Active),
                Employees.Count(item => item.Status == EmployeeStatuses.Locked),
                Employees.Count(item => item.Status == EmployeeStatuses.Resigned),
                Employees.Count(item => item.EmploymentType == EmploymentTypes.FullTime),
                Employees.Count(item => item.EmploymentType == EmploymentTypes.PartTime));
            return Task.FromResult(stats);
        }
        public Task<bool> HasRelatedRecordsAsync(Guid employeeId, CancellationToken ct) =>
            Task.FromResult(History.Any(item => item.EmployeeId == employeeId)
                || Offboardings.Any(item => item.EmployeeId == employeeId));
        public Task AddAsync(Employee employee, CancellationToken ct) => throw new NotSupportedException();
        public void Remove(Employee employee) => Employees.Remove(employee);
        public Task SaveChangesAsync(CancellationToken ct) => throw new NotSupportedException();
        public Task AddDepartmentAsync(Department department, CancellationToken ct) => throw new NotSupportedException();
        public Task AddPositionAsync(Position position, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> DepartmentCodeExistsAsync(string code, Guid? id, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> PositionCodeExistsAsync(string code, Guid? id, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> DepartmentHasActiveChildrenAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> HasActiveEmployeesInDepartmentsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> HasActiveEmployeesInPositionAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> HasOpenWorkHistoryInDepartmentsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> HasOpenWorkHistoryInPositionAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> HasCurrentWorkHistoryInDepartmentsAsync(IReadOnlyCollection<Guid> ids, DateOnly date, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> HasCurrentWorkHistoryInPositionAsync(Guid id, DateOnly date, CancellationToken ct) => throw new NotSupportedException();
    }
}
