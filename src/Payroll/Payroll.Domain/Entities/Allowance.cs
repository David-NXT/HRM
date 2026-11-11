namespace Payroll.Domain.Entities;

public class Allowance
{
    public Guid AllowanceID { get; set; } = Guid.NewGuid();
    public Guid PayrollID { get; set; }
    public string Status { get; set; } = "active";

    // Relationships
    public Payroll? Payroll { get; set; }
    public List<AllowanceDetail> AllowanceDetails { get; set; } = new();
}
