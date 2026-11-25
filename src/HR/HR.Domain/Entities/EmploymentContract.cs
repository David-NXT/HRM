using HR.Domain.Constants;
namespace HR.Domain.Entities;

public sealed class EmploymentContract
{
    public Guid ContractId { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    // Ngày hết hạn đã ký được giữ ở EndDate; ngày chấm dứt thực tế được lưu riêng.
    public DateOnly? ActualEndDate { get; set; }
    public string? TerminationReason { get; set; }
    public string Status { get; set; } = ContractStatuses.Active;
    public string? Notes { get; set; }
    public Guid? RenewalOfContractId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Employee? Employee { get; set; }
    public EmploymentContract? RenewalOfContract { get; set; }
    public EmploymentContract? RenewedByContract { get; set; }
}
