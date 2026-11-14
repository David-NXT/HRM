using HR.Application.DTOs.Organization;
using HR.Application.Interfaces.Repositories;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
namespace HR.Application.Services.Organization;

public sealed class OrgStructureService
{
    private readonly IOrgStructureRepository _repository;

    public OrgStructureService(IOrgStructureRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<DepartmentDto>> ListDepartmentsAsync(CancellationToken cancellationToken)
    {
        var departments = await _repository.ListDepartmentsAsync(cancellationToken);
        return departments.Select(ToDto).ToArray();
    }

    public async Task<IReadOnlyList<DepartmentTreeDto>> GetDepartmentTreeAsync(CancellationToken cancellationToken)
    {
        var departments = await _repository.ListDepartmentsAsync(cancellationToken);
        var byParent = departments.GroupBy(item => item.ParentDepartmentId ?? Guid.Empty).ToDictionary(group => group.Key, group => group.ToArray());
        return BuildChildren(Guid.Empty, byParent, new HashSet<Guid>());
    }

    public async Task<IReadOnlyList<PositionDto>> ListPositionsAsync(CancellationToken cancellationToken)
    {
        var positions = await _repository.ListPositionsAsync(cancellationToken);
        return positions.Select(ToDto).ToArray();
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var values = Normalize(request.Code, request.Name, request.Description);
        await ValidateDepartmentCodeAsync(values.Code, null, cancellationToken);
        await ValidateParentAsync(request.ParentDepartmentId, null, cancellationToken);

        var now = DateTime.UtcNow;
        var department = new Department
        {
            Code = values.Code,
            Name = values.Name,
            Description = values.Description,
            ParentDepartmentId = request.ParentDepartmentId,
            Status = OrgStatuses.Active,
            CreatedAt = now,
            UpdatedAt = now
        };
        var entity = department;
        await AddDepartmentAsync(entity, cancellationToken);
        return ToDto(entity);
    }

    public async Task<DepartmentDto> UpdateDepartmentAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var department = await _repository.GetDepartmentAsync(id, cancellationToken)
            ?? throw new HrException(404, "DepartmentNotFound", "Không tìm thấy phòng ban.");
        var values = Normalize(request.Code, request.Name, request.Description);
        await ValidateDepartmentCodeAsync(values.Code, id, cancellationToken);
        await ValidateParentAsync(request.ParentDepartmentId, id, cancellationToken);

        department.Code = values.Code;
        department.Name = values.Name;
        department.Description = values.Description;
        department.ParentDepartmentId = request.ParentDepartmentId;
        department.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        return ToDto(department);
    }

    public async Task<DepartmentDto> DeactivateDepartmentAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = await _repository.GetDepartmentAsync(id, cancellationToken)
            ?? throw new HrException(404, "DepartmentNotFound", "Không tìm thấy phòng ban.");
        if (department.Status == OrgStatuses.Inactive)
        {
            return ToDto(department);
        }

        var all = await _repository.ListDepartmentsAsync(cancellationToken);
        var descendants = GetDescendantIds(id, all);
        if (await _repository.DepartmentHasActiveChildrenAsync(id, cancellationToken)
            || await _repository.HasActiveEmployeesInDepartmentsAsync(descendants, cancellationToken)
            || await _repository.HasOpenWorkHistoryInDepartmentsAsync(descendants, cancellationToken)
            || await _repository.HasCurrentWorkHistoryInDepartmentsAsync(
                descendants, DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken))
        {
            throw new HrException(409, "DepartmentInUse", "Không thể ngừng hoạt động phòng ban đang có nhân viên hoặc đơn vị cấp dưới đang hoạt động.");
        }

        department.Status = OrgStatuses.Inactive;
        department.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        return ToDto(department);
    }

    public async Task<PositionDto> CreatePositionAsync(CreatePositionRequest request, CancellationToken cancellationToken)
    {
        var values = Normalize(request.Code, request.Name, request.Description);
        var seniorityLevel = NormalizeSeniorityLevel(request.SeniorityLevel);
        await ValidatePositionCodeAsync(values.Code, null, cancellationToken);
        var now = DateTime.UtcNow;
        var position = new Position
        {
            Code = values.Code,
            Name = values.Name,
            Description = values.Description,
            SeniorityLevel = seniorityLevel,
            Status = OrgStatuses.Active,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _repository.AddPositionAsync(position, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return ToDto(position);
    }

    public async Task<PositionDto> UpdatePositionAsync(Guid id, UpdatePositionRequest request, CancellationToken cancellationToken)
    {
        var position = await _repository.GetPositionAsync(id, cancellationToken)
            ?? throw new HrException(404, "PositionNotFound", "Không tìm thấy chức vụ.");
        var values = Normalize(request.Code, request.Name, request.Description);
        var seniorityLevel = NormalizeSeniorityLevel(request.SeniorityLevel);
        await ValidatePositionCodeAsync(values.Code, id, cancellationToken);
        if (position.Code == PositionCodes.DefaultStaff
            && (values.Code != PositionCodes.DefaultStaff || seniorityLevel != PositionSeniorityLevels.Staff))
        {
            throw new HrException(409, "DefaultStaffPositionProtected",
                "Chức vụ Nhân viên mặc định phải giữ mã SYS_STAFF và cấp STAFF.");
        }
        if (seniorityLevel != PositionSeniorityLevels.Staff
            && await _repository.HasPartTimeAssignmentsInPositionAsync(id, cancellationToken))
        {
            throw new HrException(409, "PositionHasPartTimeEmployees",
                "Không thể đặt chức vụ từ cấp quản lý trở lên khi đang có nhân viên part-time đảm nhiệm. Hãy điều chuyển họ trước.");
        }
        if (seniorityLevel == PositionSeniorityLevels.Director
            && await _repository.HasMultipleOpenDirectorAssignmentsAsync(id, cancellationToken))
        {
            throw new HrException(409, "DepartmentDirectorAlreadyAssigned",
                "Có phòng ban đang có nhiều hơn một nhân viên ở cấp Giám đốc. Hãy xử lý phân công hiện tại trước.");
        }
        position.Code = values.Code;
        position.Name = values.Name;
        position.Description = values.Description;
        position.SeniorityLevel = seniorityLevel;
        position.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateOpenWorkHistoryDirectorFlagsAsync(
            id, seniorityLevel == PositionSeniorityLevels.Director, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return ToDto(position);
    }

    public async Task<PositionDto> DeactivatePositionAsync(Guid id, CancellationToken cancellationToken)
    {
        var position = await _repository.GetPositionAsync(id, cancellationToken)
            ?? throw new HrException(404, "PositionNotFound", "Không tìm thấy chức vụ.");
        if (position.Code == PositionCodes.DefaultStaff)
        {
            throw new HrException(409, "DefaultStaffPositionProtected",
                "Không thể ngừng hoạt động chức vụ Nhân viên mặc định.");
        }
        if (position.Status == OrgStatuses.Inactive)
        {
            return ToDto(position);
        }
        if (await _repository.HasActiveEmployeesInPositionAsync(id, cancellationToken)
            || await _repository.HasOpenWorkHistoryInPositionAsync(id, cancellationToken)
            || await _repository.HasCurrentWorkHistoryInPositionAsync(
                id, DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken))
        {
            throw new HrException(409, "PositionInUse", "Không thể ngừng hoạt động chức vụ đang có nhân viên đảm nhiệm.");
        }

        position.Status = OrgStatuses.Inactive;
        position.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        return ToDto(position);
    }

    private async Task AddDepartmentAsync(Department department, CancellationToken cancellationToken)
    {
        await _repository.AddDepartmentAsync(department, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateDepartmentCodeAsync(string code, Guid? excludingId, CancellationToken cancellationToken)
    {
        if (await _repository.DepartmentCodeExistsAsync(code, excludingId, cancellationToken))
        {
            throw new HrException(409, "DuplicateDepartmentCode", "Mã phòng ban đã tồn tại.");
        }
    }

    private async Task ValidatePositionCodeAsync(string code, Guid? excludingId, CancellationToken cancellationToken)
    {
        if (await _repository.PositionCodeExistsAsync(code, excludingId, cancellationToken))
        {
            throw new HrException(409, "DuplicatePositionCode", "Mã chức vụ đã tồn tại.");
        }
    }

    private static string NormalizeSeniorityLevel(string? value)
    {
        var normalized = (value ?? string.Empty).Trim().ToUpperInvariant();
        if (!PositionSeniorityLevels.IsValid(normalized))
        {
            throw new HrException(400, "InvalidPositionSeniorityLevel",
                "Cấp chức vụ phải là STAFF, MANAGER, DIRECTOR hoặc EXECUTIVE.");
        }
        return normalized;
    }

    private async Task ValidateParentAsync(Guid? parentId, Guid? departmentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue)
        {
            return;
        }
        if (parentId == departmentId)
        {
            throw new HrException(422, "InvalidDepartmentTree", "Phòng ban không thể là cấp trên của chính nó.");
        }
        var parent = await _repository.GetDepartmentAsync(parentId.Value, cancellationToken);
        if (parent is null || parent.Status != OrgStatuses.Active)
        {
            throw new HrException(400, "InvalidDepartmentParent", "Phòng ban cấp trên không tồn tại hoặc đã ngừng hoạt động.");
        }
        if (departmentId.HasValue)
        {
            var all = await _repository.ListDepartmentsAsync(cancellationToken);
            if (GetDescendantIds(departmentId.Value, all).Contains(parentId.Value))
            {
                throw new HrException(422, "InvalidDepartmentTree", "Không thể chuyển phòng ban vào bên dưới đơn vị cấp dưới của chính nó.");
            }
        }
    }

    private static HashSet<Guid> GetDescendantIds(Guid departmentId, IReadOnlyList<Department> departments)
    {
        var childrenByParent = departments.Where(item => item.ParentDepartmentId.HasValue)
            .GroupBy(item => item.ParentDepartmentId!.Value)
            .ToDictionary(group => group.Key, group => group.Select(item => item.DepartmentId).ToArray());
        var descendants = new HashSet<Guid>();
        var pending = new Stack<Guid>();
        pending.Push(departmentId);
        while (pending.TryPop(out var current))
        {
            if (!childrenByParent.TryGetValue(current, out var children))
            {
                continue;
            }
            foreach (var child in children)
            {
                if (descendants.Add(child))
                {
                    pending.Push(child);
                }
            }
        }
        descendants.Add(departmentId);
        return descendants;
    }

    private static IReadOnlyList<DepartmentTreeDto> BuildChildren(
        Guid parentId,
        IReadOnlyDictionary<Guid, Department[]> byParent,
        HashSet<Guid> path)
    {
        if (!byParent.TryGetValue(parentId, out var children))
        {
            return [];
        }
        var result = new List<DepartmentTreeDto>();
        foreach (var item in children)
        {
            if (!path.Add(item.DepartmentId))
            {
                throw new HrException(500, "InvalidDepartmentTree", "Phát hiện vòng lặp trong cơ cấu phòng ban.");
            }
            result.Add(new DepartmentTreeDto(item.DepartmentId, item.Code, item.Name, item.Status,
                BuildChildren(item.DepartmentId, byParent, path)));
            path.Remove(item.DepartmentId);
        }
        return result;
    }

    private static (string Code, string Name, string? Description) Normalize(string? code, string? name, string? description)
    {
        var normalizedCode = (code ?? string.Empty).Trim().ToUpperInvariant();
        var normalizedName = (name ?? string.Empty).Trim();
        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        var errors = new List<string>();
        if (normalizedCode.Length is < 2 or > 20 || !normalizedCode.All(character => char.IsLetterOrDigit(character) || character is '_' or '-'))
        {
            errors.Add("Mã phải từ 2 đến 20 ký tự chữ, số, gạch dưới hoặc gạch ngang.");
        }
        if (normalizedName.Length is < 2 or > 150)
        {
            errors.Add("Tên phải từ 2 đến 150 ký tự.");
        }
        if (normalizedDescription is { Length: > 500 })
        {
            errors.Add("Mô tả tối đa 500 ký tự.");
        }
        if (errors.Count > 0)
        {
            throw new HrException(400, "ValidationError", string.Join(" ", errors));
        }
        return (normalizedCode, normalizedName, normalizedDescription);
    }

    private static DepartmentDto ToDto(Department item) =>
        new(item.DepartmentId, item.Code, item.Name, item.Description, item.ParentDepartmentId, item.Status, item.CreatedAt, item.UpdatedAt);

    private static PositionDto ToDto(Position item) =>
        new(item.PositionId, item.Code, item.Name, item.Description, item.Status, item.CreatedAt, item.UpdatedAt,
            item.SeniorityLevel);
}
