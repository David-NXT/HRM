using HR.Application.DTOs.Lifecycle;
using HR.Application.DTOs.Organization;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
namespace HR.Application.Services.Lifecycle;

public sealed class LifecycleService(
    ILifecycleRepository lifecycle,
    IEmployeeRepository employees,
    IEmployeeService employeeService,
    IOrgStructureRepository organization,
    TimeProvider clock)
{
    public async Task<LifecycleOptionsDto> GetOptionsAsync(CancellationToken cancellationToken)
    {
        var departments = await organization.ListDepartmentsAsync(cancellationToken);
        var positions = await organization.ListPositionsAsync(cancellationToken);
        return new LifecycleOptionsDto(
            departments.Where(item => item.Status == OrgStatuses.Active).Select(item => new DepartmentDto(
                item.DepartmentId, item.Code, item.Name, item.Description, item.ParentDepartmentId,
                item.Status, item.CreatedAt, item.UpdatedAt)).ToArray(),
            positions.Where(item => item.Status == OrgStatuses.Active).Select(item => new PositionDto(
                item.PositionId, item.Code, item.Name, item.Description, item.Status,
                item.CreatedAt, item.UpdatedAt)).ToArray());
    }

    public async Task<OnboardingResultDto> StartOnboardingAsync(StartOnboardingRequest request, CancellationToken cancellationToken)
    {
        if (request.Employee is null)
        {
            throw new HrException(400, "ValidationError", "Thiếu thông tin hồ sơ nhân viên.");
        }
        var employee = employeeService.PrepareNewEmployee(request.Employee);
        if (request.StartDate.Year < 1900 || request.StartDate <= employee.DateOfBirth)
        {
            throw new HrException(400, "InvalidStartDate", "Ngày bắt đầu làm việc phải hợp lệ và sau ngày sinh.");
        }
        var department = await organization.GetDepartmentAsync(request.DepartmentId, cancellationToken);
        if (department is null || department.Status != OrgStatuses.Active)
        {
            throw new HrException(400, "InvalidDepartment", "Phòng ban không tồn tại hoặc đã ngừng hoạt động.");
        }
        var position = await organization.GetPositionAsync(request.PositionId, cancellationToken);
        if (position is null || position.Status != OrgStatuses.Active)
        {
            throw new HrException(400, "InvalidPosition", "Chức vụ không tồn tại hoặc đã ngừng hoạt động.");
        }
        if (await employees.CodeExistsAsync(employee.EmployeeCode, cancellationToken))
        {
            throw new HrException(409, "DuplicateEmployeeCode", "Mã nhân viên đã tồn tại.");
        }

        var notes = NormalizeNote(request.Notes);
        var now = clock.GetUtcNow().UtcDateTime;
        employee.CreatedAt = employee.UpdatedAt = now;
        if (request.StartDate <= Today())
        {
            employee.DepartmentId = department.DepartmentId;
            employee.PositionId = position.PositionId;
        }
        var firstAssignment = new WorkHistory
        {
            Employee = employee,
            DepartmentId = department.DepartmentId,
            PositionId = position.PositionId,
            StartDate = request.StartDate,
            Notes = notes,
            CreatedAt = now
        };
        await lifecycle.AddOnboardingAsync(employee, firstAssignment, cancellationToken);
        await lifecycle.SaveChangesInTransactionAsync(cancellationToken);
        return new OnboardingResultDto(employee.EmployeeId, employee.EmployeeCode,
            employee.Status, firstAssignment.WorkHistoryId, firstAssignment.StartDate,
            firstAssignment.DepartmentId, firstAssignment.PositionId, employee.EmploymentType);
    }

    public async Task<OffboardingDto> StartOffboardingAsync(StartOffboardingRequest request, CancellationToken cancellationToken)
    {
        var employee = await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        EnsureNotResigned(employee);
        await ValidateLastWorkingDateAsync(employee, request.LastWorkingDate, cancellationToken);
        if (await lifecycle.HasPendingOffboardingAsync(employee.EmployeeId, cancellationToken))
        {
            throw new HrException(409, "OffboardingAlreadyStarted", "Nhân viên đã có hồ sơ nghỉ việc đang xử lý.");
        }
        var checklist = NormalizeChecklist(request.Checklist);
        var notes = NormalizeNote(request.Notes);
        var now = clock.GetUtcNow().UtcDateTime;
        var record = new OffboardingRecord
        {
            EmployeeId = employee.EmployeeId,
            Employee = employee,
            LastWorkingDate = request.LastWorkingDate,
            Notes = notes,
            Checklist = checklist,
            CreatedAt = now,
            UpdatedAt = now
        };
        // Also participates in employee concurrency checks against resignation/assignment changes.
        employee.UpdatedAt = now;
        await lifecycle.AddOffboardingAsync(record, cancellationToken);
        await lifecycle.SaveChangesInTransactionAsync(cancellationToken);
        return ToDto(record);
    }

    public async Task<IReadOnlyList<OffboardingDto>> ListOffboardingsAsync(Guid? employeeId, string? status, CancellationToken cancellationToken)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? null : status.Trim().ToUpperInvariant();
        if (normalizedStatus is not (null or LifecycleStatuses.InProgress or LifecycleStatuses.Completed)
            || employeeId == Guid.Empty)
        {
            throw new HrException(400, "ValidationError", "Bộ lọc hồ sơ nghỉ việc không hợp lệ.");
        }
        var records = await lifecycle.ListOffboardingsAsync(employeeId, normalizedStatus, cancellationToken);
        return records.Select(ToDto).ToArray();
    }

    public async Task<OffboardingDto> GetOffboardingAsync(Guid id, CancellationToken cancellationToken) =>
        ToDto(await GetRecordAsync(id, cancellationToken));

    public async Task<OffboardingDto> UpdateOffboardingAsync(Guid id, UpdateOffboardingRequest request, CancellationToken cancellationToken)
    {
        var record = await GetRecordAsync(id, cancellationToken);
        EnsureEditable(record, request.ExpectedVersion);
        var employee = record.Employee ?? await GetEmployeeAsync(record.EmployeeId, cancellationToken);
        EnsureNotResigned(employee);
        await ValidateLastWorkingDateAsync(employee, request.LastWorkingDate, cancellationToken);
        var notes = NormalizeNote(request.Notes);
        record.LastWorkingDate = request.LastWorkingDate;
        record.Notes = notes;
        record.UpdatedAt = clock.GetUtcNow().UtcDateTime;
        await lifecycle.SaveChangesInTransactionAsync(cancellationToken);
        return ToDto(record);
    }

    public async Task<OffboardingDto> UpdateChecklistAsync(
        Guid id, Guid itemId, UpdateOffboardingChecklistRequest request, CancellationToken cancellationToken)
    {
        var record = await GetRecordAsync(id, cancellationToken);
        EnsureEditable(record, request.ExpectedVersion);
        var item = record.Checklist.FirstOrDefault(item => item.OffboardingChecklistItemId == itemId)
            ?? throw new HrException(404, "ChecklistItemNotFound", "Không tìm thấy hạng mục trong hồ sơ nghỉ việc này.");
        var note = NormalizeNote(request.CompletionNote);
        var now = clock.GetUtcNow().UtcDateTime;
        item.IsCompleted = request.IsCompleted;
        item.CompletedAt = request.IsCompleted ? item.CompletedAt ?? now : null;
        item.CompletionNote = note;
        record.UpdatedAt = now;
        await lifecycle.SaveChangesInTransactionAsync(cancellationToken);
        return ToDto(record);
    }

    public async Task<OffboardingDto> CompleteOffboardingAsync(Guid id, CompleteOffboardingRequest request, CancellationToken cancellationToken)
    {
        var record = await GetRecordAsync(id, cancellationToken);
        if (record.Status == LifecycleStatuses.Completed)
        {
            return ToDto(record);
        }
        EnsureEditable(record, request.ExpectedVersion);
        if (record.LastWorkingDate > Today())
        {
            throw new HrException(409, "OffboardingNotEffective", "Chưa đến ngày làm việc cuối cùng; chưa thể xác nhận nghỉ việc.");
        }
        var missing = record.Checklist.Where(item => item.IsRequired && !item.IsCompleted).Select(item => item.Title).ToArray();
        if (missing.Length > 0)
        {
            throw new HrException(422, "ChecklistIncomplete", "Chưa hoàn thành hạng mục bắt buộc: " + string.Join(", ", missing));
        }
        var employee = record.Employee ?? await GetEmployeeAsync(record.EmployeeId, cancellationToken);
        EnsureNotResigned(employee);
        var history = await ValidateLastWorkingDateAsync(employee, record.LastWorkingDate, cancellationToken);
        foreach (var assignment in history.Where(item => item.EndDate == null || item.EndDate > record.LastWorkingDate))
        {
            assignment.EndDate = record.LastWorkingDate;
        }
        var now = clock.GetUtcNow().UtcDateTime;
        employee.Status = EmployeeStatuses.Resigned;
        employee.UpdatedAt = now;
        record.Status = LifecycleStatuses.Completed;
        record.CompletedAt = now;
        record.UpdatedAt = now;
        await lifecycle.SaveChangesInTransactionAsync(cancellationToken);
        return ToDto(record);
    }

    private async Task<IReadOnlyList<WorkHistory>> ValidateLastWorkingDateAsync(
        Employee employee, DateOnly date, CancellationToken cancellationToken)
    {
        if (date.Year < 1900 || date <= employee.DateOfBirth)
        {
            throw new HrException(400, "InvalidLastWorkingDate", "Ngày làm việc cuối cùng không hợp lệ.");
        }
        var history = await lifecycle.GetWorkHistoryAsync(employee.EmployeeId, cancellationToken);
        if (history.Any(item => item.StartDate > date))
        {
            throw new HrException(409, "AssignmentAfterDeparture", "Có bản ghi công tác bắt đầu sau ngày nghỉ dự kiến. Cần xử lý lịch công tác trước khi nghỉ việc.");
        }
        return history;
    }

    private async Task<Employee> GetEmployeeAsync(Guid id, CancellationToken cancellationToken) =>
        await lifecycle.GetEmployeeAsync(id, cancellationToken)
        ?? throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ nhân viên.");

    private async Task<OffboardingRecord> GetRecordAsync(Guid id, CancellationToken cancellationToken) =>
        await lifecycle.GetOffboardingAsync(id, cancellationToken)
        ?? throw new HrException(404, "OffboardingNotFound", "Không tìm thấy hồ sơ nghỉ việc.");

    private DateOnly Today() => DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);

    private static void EnsureNotResigned(Employee employee)
    {
        if (employee.Status == EmployeeStatuses.Resigned)
        {
            throw new HrException(409, "EmployeeResigned", "Nhân viên đã được xác nhận nghỉ việc.");
        }
    }

    private static void EnsureEditable(OffboardingRecord record, int expectedVersion)
    {
        if (record.Status != LifecycleStatuses.InProgress)
        {
            throw new HrException(409, "OffboardingCompleted", "Hồ sơ nghỉ việc đã hoàn tất, không thể chỉnh sửa.");
        }
        if (expectedVersion != record.Version)
        {
            throw new HrException(409, "LifecycleChanged", "Hồ sơ đã thay đổi. Tải lại dữ liệu và thử lại.");
        }
    }

    private static List<OffboardingChecklistItem> NormalizeChecklist(IReadOnlyList<CreateOffboardingChecklistItem>? checklist)
    {
        if (checklist is null || checklist.Count is < 1 or > 50)
        {
            throw new HrException(400, "InvalidChecklist", "Checklist cần từ 1 đến 50 hạng mục bàn giao.");
        }
        var titles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var result = new List<OffboardingChecklistItem>();
        for (var index = 0; index < checklist.Count; index++)
        {
            var input = checklist[index];
            var title = input?.Title?.Trim() ?? string.Empty;
            if (title.Length is < 1 or > 200 || !titles.Add(title))
            {
                throw new HrException(400, "InvalidChecklist", "Tên hạng mục phải từ 1 đến 200 ký tự và không được trùng.");
            }
            result.Add(new OffboardingChecklistItem { Title = title, IsRequired = input!.IsRequired, SortOrder = index });
        }
        return result;
    }

    private static string? NormalizeNote(string? value)
    {
        var note = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        if (note is { Length: > 1000 })
        {
            throw new HrException(400, "ValidationError", "Ghi chú tối đa 1000 ký tự.");
        }
        return note;
    }

    private static OffboardingDto ToDto(OffboardingRecord record) => new(
        record.OffboardingRecordId, record.EmployeeId, record.Employee?.EmployeeCode ?? string.Empty,
        record.Employee?.FullName ?? string.Empty, record.Employee?.Status ?? string.Empty,
        record.LastWorkingDate, record.Status, record.Notes, record.Version,
        record.CreatedAt, record.UpdatedAt, record.CompletedAt,
        record.Checklist.OrderBy(item => item.SortOrder).Select(item => new OffboardingChecklistItemDto(
            item.OffboardingChecklistItemId, item.Title, item.IsRequired, item.IsCompleted,
            item.CompletionNote, item.CompletedAt)).ToArray());
}
