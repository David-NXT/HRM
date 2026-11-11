using HR.Application.DTOs.Employees;
using HR.Application.DTOs.Organization;
using HR.Domain.Entities;
namespace HR.Application.DTOs.Lifecycle;

public sealed record LifecycleOptionsDto(
    IReadOnlyList<DepartmentDto> Departments,
    IReadOnlyList<PositionDto> Positions);

public sealed record StartOnboardingRequest(
    CreateEmployeeRequest Employee,
    DateOnly StartDate,
    Guid DepartmentId,
    Guid PositionId,
    string? Notes);

public sealed record OnboardingResultDto(
    Guid EmployeeId,
    string EmployeeCode,
    string Status,
    Guid WorkHistoryId,
    DateOnly StartDate,
    Guid DepartmentId,
    Guid PositionId,
    string EmploymentType);

public sealed record CreateOffboardingChecklistItem(string Title, bool IsRequired);

public sealed record StartOffboardingRequest(
    Guid EmployeeId,
    DateOnly LastWorkingDate,
    string? Notes,
    IReadOnlyList<CreateOffboardingChecklistItem> Checklist);

public sealed record UpdateOffboardingRequest(DateOnly LastWorkingDate, string? Notes, int ExpectedVersion);

public sealed record UpdateOffboardingChecklistRequest(bool IsCompleted, string? CompletionNote, int ExpectedVersion);

public sealed record CompleteOffboardingRequest(int ExpectedVersion);

public sealed record OffboardingChecklistItemDto(
    Guid OffboardingChecklistItemId,
    string Title,
    bool IsRequired,
    bool IsCompleted,
    string? CompletionNote,
    DateTime? CompletedAt);

public sealed record OffboardingDto(
    Guid OffboardingRecordId,
    Guid EmployeeId,
    string EmployeeCode,
    string EmployeeName,
    string EmployeeStatus,
    DateOnly LastWorkingDate,
    string Status,
    string? Notes,
    int Version,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? CompletedAt,
    IReadOnlyList<OffboardingChecklistItemDto> Checklist);
