using HR.Domain.Constants;
namespace HR.Domain.Entities;

public sealed class ContractAlert
{
    public Guid ContractAlertId { get; set; } = Guid.NewGuid();
    public Guid ContractId { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public string Status { get; set; } = ContractAlertStatuses.New;
    public int Version { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public EmploymentContract? Contract { get; set; }
}
