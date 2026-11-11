namespace Payroll.Domain.Entities;

public class PayCut
{
    public Guid PayCutID { get; set; } = Guid.NewGuid();
    public Guid PayrollID { get; set; }
    public string Status { get; set; } = "active";
    
    // Relationships
    public Payroll? Payroll { get; set; }
    public List<PayCutDetail> Details { get; set; } = new();
}
