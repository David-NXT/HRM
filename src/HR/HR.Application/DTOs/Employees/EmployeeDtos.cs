using HR.Domain.Constants;
using System.Text.Json.Serialization;

namespace HR.Application.DTOs.Employees;

// Part-time rate values are VND/hour. Keep the JSON key partTimeSalary for existing consumers such as Payroll.
public sealed record CreateEmployeeRequest(
    string EmployeeCode,
    string FullName,
    DateOnly DateOfBirth,
    string Gender,
    string Email,
    string Phone,
    string CitizenId,
    string? Address,
    string EmploymentType = EmploymentTypes.FullTime,
    decimal? BaseSalary = null,
    [property: JsonPropertyName("partTimeSalary")] decimal? PartTimeHourlyRate = null);

public sealed record UpdateEmployeeSalaryRequest(
    decimal? BaseSalary = null,
    [property: JsonPropertyName("partTimeSalary")] decimal? PartTimeHourlyRate = null);

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
    string EmploymentType,
    decimal? BaseSalary,
    [property: JsonPropertyName("partTimeSalary")] decimal? PartTimeHourlyRate,
    int Version = 1);

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

public sealed record EmployeeStatisticsDto(
    int TotalEmployees,
    int ActiveEmployees,
    int LockedEmployees,
    int ResignedEmployees,
    int FullTimeEmployees,
    int PartTimeEmployees);
