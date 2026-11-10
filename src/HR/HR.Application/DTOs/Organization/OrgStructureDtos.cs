namespace HR.Application.DTOs.Organization;

public sealed record DepartmentDto(
    Guid DepartmentId,
    string Code,
    string Name,
    string? Description,
    Guid? ParentDepartmentId,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record DepartmentTreeDto(
    Guid DepartmentId,
    string Code,
    string Name,
    string Status,
    IReadOnlyList<DepartmentTreeDto> Children);

public sealed record CreateDepartmentRequest(
    string Code,
    string Name,
    string? Description,
    Guid? ParentDepartmentId);

public sealed record UpdateDepartmentRequest(
    string Code,
    string Name,
    string? Description,
    Guid? ParentDepartmentId);

public sealed record PositionDto(
    Guid PositionId,
    string Code,
    string Name,
    string? Description,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record CreatePositionRequest(string Code, string Name, string? Description);

public sealed record UpdatePositionRequest(string Code, string Name, string? Description);
