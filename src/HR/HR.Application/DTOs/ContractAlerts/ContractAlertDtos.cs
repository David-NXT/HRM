namespace HR.Application.DTOs.ContractAlerts;

public sealed record ContractAlertDto(
    Guid ContractAlertId,
    Guid ContractId,
    Guid EmployeeId,
    string EmployeeCode,
    string EmployeeName,
    string EmployeeStatus,
    string ContractNumber,
    string ContractType,
    string ContractStatus,
    DateOnly ContractStartDate,
    DateOnly? CurrentContractEndDate,
    DateOnly ExpiryDate,
    int DaysRemaining,
    bool IsCurrentCycle,
    string Status,
    int Version,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record ContractAlertListDto(IReadOnlyList<ContractAlertDto> Items, int Page, int PageSize, int Total);

public sealed record UpdateContractAlertStatusRequest(string Status, int ExpectedVersion);

public sealed record ContractAlertScanResult(DateOnly ScanDate, int ScannedCount, int CreatedCount, int SkippedCount, int FailedCount);
