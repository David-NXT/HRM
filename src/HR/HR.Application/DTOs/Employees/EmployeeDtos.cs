using HR.Domain.Constants;

namespace HR.Application.DTOs.Employees;

public sealed record CreateEmployeeRequest(
    string EmployeeCode,
    string FullName,
    DateOnly DateOfBirth,
    string Gender,
    string Email,
    string Phone,
    string CitizenId,
    string? Address,
    string EmploymentType = EmploymentTypes.FullTime);

public sealed record UpdateEmployeeRequest(
    string FullName,
    DateOnly DateOfBirth,
    string Gender,
    string Email,
    string Phone,
    string CitizenId,
    string? Address);

public sealed record EmployeeDto(
    Guid EmployeeId,
    string EmployeeCode,
    string FullName,
    DateOnly DateOfBirth,
    string Gender,
    string Email,
    string Phone,
    string CitizenId,
    string? Address,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid? DepartmentId,
    Guid? PositionId,
    string? DepartmentName,
    string? PositionName,
    string EmploymentType);

public sealed record EmployeeDirectoryEntry(
    Guid EmployeeId,
    string EmployeeCode,
    string FullName,
    string Status,
    string? DepartmentName,
    string? PositionName,
    string EmploymentType = EmploymentTypes.FullTime);

public sealed record EmployeeDirectoryListDto(
    IReadOnlyList<EmployeeDirectoryEntry> Items,
    int Page,
    int PageSize,
    int Total);

public sealed record EmployeeListDto(
    IReadOnlyList<EmployeeDto> Items,
    int Page,
    int PageSize,
    int Total);
