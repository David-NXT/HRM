namespace HR.Application.DTOs.Contracts;

public sealed record EmploymentContractDto(
    Guid ContractId,
    Guid EmployeeId,
    string ContractNumber,
    string ContractType,
    DateOnly StartDate,
    DateOnly? EndDate,
    string Status,
    string? Notes,
    Guid? RenewalOfContractId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateOnly? ActualEndDate = null,
    string? TerminationReason = null);

public sealed record CreateEmploymentContractRequest(
    string ContractNumber,
    string ContractType,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Notes,
    Guid? RenewalOfContractId = null);

public sealed record UpdateEmploymentContractRequest(
    string ContractNumber,
    string ContractType,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Notes);

public sealed record WorkHistoryDto(
    Guid WorkHistoryId,
    Guid EmployeeId,
    Guid DepartmentId,
    string DepartmentCode,
    string DepartmentName,
    Guid PositionId,
    string PositionCode,
    string PositionName,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Notes);

public sealed record CreateWorkAssignmentRequest(
    Guid DepartmentId,
    Guid PositionId,
    DateOnly EffectiveDate,
    string? Notes);
