using HR.Application.DTOs.Contracts;
using HR.Application.Interfaces.Repositories;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
namespace HR.Application.Services.Contracts;

public sealed class ContractService
{
    private readonly IEmploymentRepository _repository;

    public ContractService(IEmploymentRepository repository) => _repository = repository;

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

    public async Task<EmploymentContractDto> CreateContractAsync(
        Guid employeeId,
        CreateEmploymentContractRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureEmployeeExistsAsync(employeeId, cancellationToken);
        var values = NormalizeContract(request.ContractNumber, request.ContractType, request.StartDate, request.EndDate, request.Notes);
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
            if (values.StartDate <= previous.StartDate
                || (previous.EndDate.HasValue && values.StartDate <= previous.EndDate.Value))
            {
                throw new HrException(400, "InvalidRenewalDates", "Ngày bắt đầu hợp đồng gia hạn phải sau thời gian của hợp đồng trước.");
            }
        }

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
            if (!previous.EndDate.HasValue)
            {
                previous.EndDate = values.StartDate.AddDays(-1);
            }
            previous.UpdatedAt = now;
            await _repository.SaveChangesInTransactionAsync(cancellationToken);
        }
        else
        {
            await _repository.SaveChangesAsync(cancellationToken);
        }
        return ToDto(contract);
    }

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
        var values = NormalizeContract(request.ContractNumber, request.ContractType, request.StartDate, request.EndDate, request.Notes);
        await EnsureContractNumberAvailableAsync(values.ContractNumber, contractId, cancellationToken);
        contract.ContractNumber = values.ContractNumber;
        contract.ContractType = values.ContractType;
        contract.StartDate = values.StartDate;
        contract.EndDate = values.EndDate;
        contract.Notes = values.Notes;
        contract.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
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

    private async Task<Employee> EnsureEmployeeExistsAsync(Guid employeeId, CancellationToken cancellationToken) =>
        await _repository.GetEmployeeAsync(employeeId, cancellationToken)
        ?? throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ nhân viên.");

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

    private static EmploymentContractDto ToDto(EmploymentContract contract) =>
        new(contract.ContractId, contract.EmployeeId, contract.ContractNumber, contract.ContractType,
            contract.StartDate, contract.EndDate, contract.Status, contract.Notes,
            contract.RenewalOfContractId, contract.CreatedAt, contract.UpdatedAt);

    private static WorkHistoryDto ToDto(WorkHistory history) =>
        new(history.WorkHistoryId, history.EmployeeId, history.DepartmentId,
            history.Department?.Code ?? string.Empty, history.Department?.Name ?? string.Empty,
            history.PositionId, history.Position?.Code ?? string.Empty, history.Position?.Name ?? string.Empty,
            history.StartDate, history.EndDate, history.Notes);
}
