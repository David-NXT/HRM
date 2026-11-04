namespace HR.Core.DTOs;

public sealed record CreateEmployeeRequest(
    string EmployeeCode,
    string FullName,
    DateOnly DateOfBirth,
    string Gender,
    string Email,
    string Phone,
    string CitizenId,
    string? Address);

public sealed record UpdateEmployeeRequest(
    string FullName,
    DateOnly DateOfBirth,
    string Gender,
    string Email,
    string Phone,
    string CitizenId,
    string? Address);

public sealed record EmployeeDto(
    int EmployeeId,
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
    DateTime UpdatedAt);

public sealed record EmployeeListDto(
    IReadOnlyList<EmployeeDto> Items,
    int Page,
    int PageSize,
    int Total);
