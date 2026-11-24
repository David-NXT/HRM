using HR.Application.DTOs.Contracts;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
using HR.Domain.Rules;
namespace HR.Application.Services.Contracts;

public sealed class ContractService
{
    private readonly IEmploymentRepository _repository;
    private readonly IContractAlertGenerator _alerts;

    public ContractService(IEmploymentRepository repository, IContractAlertGenerator alerts)
    {
        _repository = repository;
        _alerts = alerts;
    }

    public async Task<IReadOnlyList<EmploymentContractDto>> GetContractsAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        await EnsureEmployeeExistsAsync(employeeId, cancellationToken);
        var contracts = await _repository.GetContractsAsync(employeeId, cancellationToken);
        return contracts.Select(ToDto).ToArray();
    }

    public async Task<IReadOnlyList<WorkHistoryDto>> GetWorkHistoryAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        await EnsureEmployeeExistsAsync(employeeId, cancellationToken);
        var history = await _repository.GetWorkHistoryAsync(employeeId, cancellationToken);
        return history.Select(ToDto).ToArray();
    }

    // Tạo hợp đồng chính không chồng khoảng hiệu lực; nhân viên đã nghỉ không được thêm hợp đồng, gia hạn phải nối sau tiền nhiệm có hạn.
    public async Task<EmploymentContractDto> CreateContractAsync(
        Guid employeeId,
        CreateEmploymentContractRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await EnsureEmployeeExistsAsync(employeeId, cancellationToken);
        var values = NormalizeContract(request.ContractNumber, request.ContractType, request.StartDate, request.EndDate, request.Notes);
        await EnsureEmploymentDatesAllowedAsync(employee, values.StartDate, cancellationToken);
        await EnsureContractNumberAvailableAsync(values.ContractNumber, null, cancellationToken);

        EmploymentContract? previous = null;
        if (request.RenewalOfContractId.HasValue)
        {
            previous = await _repository.GetContractAsync(request.RenewalOfContractId.Value, cancellationToken)
                ?? throw new HrException(404, "ContractNotFound", "Không tìm thấy hợp đồng cần gia hạn.");
            if (previous.EmployeeId != employeeId)
            {
                throw new HrException(400, "InvalidRenewal", "Hợp đồng gia hạn phải thuộc cùng nhân viên.");
            }
            if (previous.Status != ContractStatuses.Active)
            {
                throw new HrException(409, "InvalidContractState", "Chỉ có thể gia hạn hợp đồng đang hoạt động.");
            }
            if (await _repository.RenewalAlreadyExistsAsync(previous.ContractId, cancellationToken))
            {
                throw new HrException(409, "ContractAlreadyRenewed", "Hợp đồng này đã có hợp đồng gia hạn.");
            }
            if (!previous.EndDate.HasValue)
            {
                throw new HrException(409, "IndefiniteContractRenewal", "Hợp đồng không xác định thời hạn không có ngày hết hạn để gia hạn. Không tự thay đổi ngày hết hạn đã ký.");
            }
            if (values.StartDate <= previous.StartDate
                || (previous.EndDate.HasValue && values.StartDate <= previous.EndDate.Value))
            {
                throw new HrException(400, "InvalidRenewalDates", "Ngày bắt đầu hợp đồng gia hạn phải sau thời gian của hợp đồng trước.");
            }
        }

        await EnsureNoOverlapAsync(employeeId, values.StartDate, values.EndDate, null, cancellationToken);

        var now = DateTime.UtcNow;
        var contract = new EmploymentContract
        {
            EmployeeId = employeeId,
            ContractNumber = values.ContractNumber,
            ContractType = values.ContractType,
            StartDate = values.StartDate,
            EndDate = values.EndDate,
            Status = ContractStatuses.Active,
            Notes = values.Notes,
            RenewalOfContractId = previous?.ContractId,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _repository.AddContractAsync(contract, cancellationToken);
        if (previous is not null)
        {
            previous.Status = ContractStatuses.Renewed;
            previous.UpdatedAt = now;
            await _repository.SaveChangesInTransactionAsync(cancellationToken);
        }
        else
        {
            await _repository.SaveChangesAsync(cancellationToken);
        }
        // The contract must be persisted before the alert's foreign key can reference it.
        await _alerts.GenerateForContractAsync(contract, cancellationToken);
        return ToDto(contract);
    }

    // Sửa hợp đồng ACTIVE và kiểm tra lại toàn bộ điều kiện ngày, tiền nhiệm và khoảng hiệu lực, trước khi thay dữ liệu đã lưu.
    public async Task<EmploymentContractDto> UpdateContractAsync(
        Guid contractId,
        UpdateEmploymentContractRequest request,
        CancellationToken cancellationToken)
    {
        var contract = await _repository.GetContractAsync(contractId, cancellationToken)
            ?? throw new HrException(404, "ContractNotFound", "Không tìm thấy hợp đồng.");
        if (contract.Status != ContractStatuses.Active)
        {
            throw new HrException(409, "InvalidContractState", "Chỉ có thể cập nhật hợp đồng đang hoạt động.");
        }
        var employee = await EnsureEmployeeExistsAsync(contract.EmployeeId, cancellationToken);
        var values = NormalizeContract(request.ContractNumber, request.ContractType, request.StartDate, request.EndDate, request.Notes);
        await EnsureEmploymentDatesAllowedAsync(employee, values.StartDate, cancellationToken);
        await EnsureContractNumberAvailableAsync(values.ContractNumber, contractId, cancellationToken);
        if (contract.RenewalOfContractId.HasValue)
        {
            var previous = await _repository.GetContractAsync(contract.RenewalOfContractId.Value, cancellationToken)
                ?? throw new HrException(409, "InvalidRenewal", "Không tìm thấy hợp đồng tiền nhiệm để đối chiếu ngày.");
            if (previous.EmployeeId != contract.EmployeeId || !previous.EndDate.HasValue
                || values.StartDate <= previous.EndDate.Value)
            {
                throw new HrException(400, "InvalidRenewalDates", "Hợp đồng gia hạn phải thuộc cùng nhân viên và bắt đầu sau ngày hết hạn hợp đồng trước.");
            }
        }
        await EnsureNoOverlapAsync(contract.EmployeeId, values.StartDate, values.EndDate, contractId, cancellationToken);
        contract.ContractNumber = values.ContractNumber;
        contract.ContractType = values.ContractType;
        contract.StartDate = values.StartDate;
        contract.EndDate = values.EndDate;
        contract.Notes = values.Notes;
        contract.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        await _alerts.GenerateForContractAsync(contract, cancellationToken);
        return ToDto(contract);
    }

    public async Task<WorkHistoryDto> RecordAssignmentAsync(
        Guid employeeId,
        CreateWorkAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await EnsureEmployeeExistsAsync(employeeId, cancellationToken);
        if (employee.Status == HR.Domain.Constants.EmployeeStatuses.Resigned)
        {
            throw new HrException(409, "EmployeeResigned", "Không thể ghi công tác mới cho nhân viên đã nghỉ việc.");
        }
        if (request.EffectiveDate.Year < 1900)
        {
            throw new HrException(400, "ValidationError", "Ngày hiệu lực không hợp lệ.");
        }
        var department = await _repository.GetActiveDepartmentAsync(request.DepartmentId, cancellationToken)
            ?? throw new HrException(400, "InvalidDepartment", "Phòng ban không tồn tại hoặc đã ngừng hoạt động.");
        var position = await _repository.GetActivePositionAsync(request.PositionId, cancellationToken)
            ?? throw new HrException(400, "InvalidPosition", "Chức vụ không tồn tại hoặc đã ngừng hoạt động.");
        if (employee.EmploymentType == EmploymentTypes.PartTime
            && !PositionSeniorityLevels.IsEligibleForPartTime(position.SeniorityLevel))
        {
            throw new HrException(400, "PartTimePositionNotAllowed",
                position.SeniorityLevel is null
                    ? "Chức vụ chưa được phân loại cấp bậc. Hãy phân loại trong Organization trước khi chọn cho nhân viên part-time."
                    : "Nhân viên part-time chỉ được đảm nhiệm chức vụ cấp STAFF; không được giữ chức vụ quản lý trở lên.");
        }
        if (position.SeniorityLevel == PositionSeniorityLevels.Director
            && await _repository.HasDirectorAssignmentInDepartmentAsync(
                department.DepartmentId, request.EffectiveDate, employeeId, cancellationToken))
        {
            throw new HrException(409, "DepartmentDirectorAlreadyAssigned",
                "Phòng ban đã có Giám đốc được phân công trong khoảng thời gian này.");
        }
        var notes = NormalizeNotes(request.Notes);
        var history = await _repository.GetWorkHistoryAsync(employeeId, cancellationToken);
        var latest = history.OrderByDescending(item => item.StartDate).FirstOrDefault();
        if (latest is not null && (request.EffectiveDate <= latest.StartDate
            || (latest.EndDate.HasValue && request.EffectiveDate <= latest.EndDate.Value)))
        {
            throw new HrException(409, "InvalidEffectiveDate", "Ngày hiệu lực phải sau quá trình công tác gần nhất.");
        }

        var openHistory = await _repository.GetOpenWorkHistoryAsync(employeeId, cancellationToken);
        if (openHistory is not null)
        {
            openHistory.EndDate = request.EffectiveDate.AddDays(-1);
        }
        var newHistory = new WorkHistory
        {
            EmployeeId = employeeId,
            DepartmentId = department.DepartmentId,
            PositionId = position.PositionId,
            IsDirector = position.SeniorityLevel == PositionSeniorityLevels.Director,
            StartDate = request.EffectiveDate,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
        if (request.EffectiveDate <= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            employee.DepartmentId = department.DepartmentId;
            employee.PositionId = position.PositionId;
        }
        employee.UpdatedAt = DateTime.UtcNow;
        await _repository.AddWorkHistoryAsync(newHistory, cancellationToken);
        await _repository.SaveChangesInTransactionAsync(cancellationToken);

        newHistory.Department = department;
        newHistory.Position = position;
        return ToDto(newHistory);
    }

    // Nạp entity có tracking trước khi kiểm tra hợp đồng để Employee.Version bảo vệ cả tạo/sửa hợp đồng đồng thời với nghỉ việc.
    private async Task<Employee> EnsureEmployeeExistsAsync(Guid employeeId, CancellationToken cancellationToken) =>
        await _repository.GetEmployeeAsync(employeeId, cancellationToken)
        ?? throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ nhân viên.");

    // Ngày bắt đầu hợp đồng phải sau ngày sinh và không trước công tác đầu tiên đã biết; hồ sơ đã nghỉ không được tạo/sửa hợp đồng.
    private async Task EnsureEmploymentDatesAllowedAsync(Employee employee, DateOnly start, CancellationToken cancellationToken)
    {
        if (employee.Status == EmployeeStatuses.Resigned)
        {
            throw new HrException(409, "EmployeeResigned", "Không thể tạo hoặc sửa hợp đồng cho nhân viên đã nghỉ việc.");
        }
        if (start <= employee.DateOfBirth)
        {
            throw new HrException(400, "InvalidContractStart", "Ngày bắt đầu hợp đồng phải sau ngày sinh của nhân viên.");
        }
        var history = await _repository.GetWorkHistoryAsync(employee.EmployeeId, cancellationToken);
        if (history.Count > 0 && start < history.Min(item => item.StartDate))
        {
            throw new HrException(400, "InvalidContractStart", "Ngày hợp đồng không được trước ngày bắt đầu công tác đầu tiên đã ghi nhận.");
        }
    }

    // So sánh các khoảng ngày có tính cả ngày đầu/cuối; RENEWED vẫn bao phủ thời gian đã ký, TERMINATED giữ thời gian thực tế trước chấm dứt.
    private async Task EnsureNoOverlapAsync(Guid employeeId, DateOnly start, DateOnly? end, Guid? excludingId, CancellationToken cancellationToken)
    {
        var contracts = await _repository.GetContractsAsync(employeeId, cancellationToken);
        if (excludingId.HasValue)
        {
            // Entity hợp đồng được đọc trước Employee.Version có thể đã cũ. Danh sách không tracking này được đọc sau version nhân viên;
            // các thay đổi từ trước lần đọc version được thấy ở đây, còn thay đổi về sau sẽ bị concurrency token chặn khi lưu.
            var current = contracts.FirstOrDefault(item => item.ContractId == excludingId.Value)
                ?? throw new HrException(404, "ContractNotFound", "Hợp đồng đã bị xóa. Tải lại danh sách trước khi chỉnh sửa.");
            if (current.Status != ContractStatuses.Active)
            {
                throw new HrException(409, "InvalidContractState", "Hợp đồng đã được gia hạn, chấm dứt hoặc hủy. Tải lại dữ liệu trước khi chỉnh sửa.");
            }
        }
        var overlapping = contracts.FirstOrDefault(item => item.ContractId != excludingId && ContractRules.ConflictsWith(item, start, end));
        if (overlapping is not null)
        {
            throw new HrException(409, "ContractOverlap", $"Khoảng hiệu lực chồng với hợp đồng {overlapping.ContractNumber}. Hợp đồng tiếp theo phải bắt đầu sau ngày kết thúc hợp đồng trước.");
        }
    }

    private async Task EnsureContractNumberAvailableAsync(string contractNumber, Guid? excludingId, CancellationToken cancellationToken)
    {
        if (await _repository.ContractNumberExistsAsync(contractNumber, excludingId, cancellationToken))
        {
            throw new HrException(409, "DuplicateContractNumber", "Số hợp đồng đã tồn tại.");
        }
    }

    private static (string ContractNumber, string ContractType, DateOnly StartDate, DateOnly? EndDate, string? Notes)
        NormalizeContract(string? contractNumber, string? contractType, DateOnly startDate, DateOnly? endDate, string? notes)
    {
        var number = (contractNumber ?? string.Empty).Trim();
        var type = (contractType ?? string.Empty).Trim();
        var normalizedNotes = NormalizeNotes(notes);
        var errors = new List<string>();
        if (number.Length is < 1 or > 50)
        {
            errors.Add("Số hợp đồng phải từ 1 đến 50 ký tự.");
        }
        if (type.Length is < 1 or > 50)
        {
            errors.Add("Loại hợp đồng phải từ 1 đến 50 ký tự.");
        }
        if (startDate.Year < 1900)
        {
            errors.Add("Ngày bắt đầu hợp đồng không hợp lệ.");
        }
        if (endDate.HasValue && startDate >= endDate.Value)
        {
            errors.Add("Ngày bắt đầu phải trước ngày kết thúc hợp đồng.");
        }
        if (errors.Count > 0)
        {
            throw new HrException(400, "ValidationError", string.Join(" ", errors));
        }
        return (number, type, startDate, endDate, normalizedNotes);
    }

    private static string? NormalizeNotes(string? value)
    {
        var notes = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        if (notes is { Length: > 1000 })
        {
            throw new HrException(400, "ValidationError", "Ghi chú tối đa 1000 ký tự.");
        }
        return notes;
    }

    // Tách ngày hết hạn đã ký với ngày chấm dứt thực tế để FE không nhầm hợp đồng đã chấm dứt là còn hiệu lực.
    private static EmploymentContractDto ToDto(EmploymentContract contract) =>
        new(contract.ContractId, contract.EmployeeId, contract.ContractNumber, contract.ContractType,
            contract.StartDate, contract.EndDate, contract.Status, contract.Notes,
            contract.RenewalOfContractId, contract.CreatedAt, contract.UpdatedAt, contract.ActualEndDate, contract.TerminationReason);

    private static WorkHistoryDto ToDto(WorkHistory history) =>
        new(history.WorkHistoryId, history.EmployeeId, history.DepartmentId,
            history.Department?.Code ?? string.Empty, history.Department?.Name ?? string.Empty,
            history.PositionId, history.Position?.Code ?? string.Empty, history.Position?.Name ?? string.Empty,
            history.StartDate, history.EndDate, history.Notes);
}
