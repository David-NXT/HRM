namespace Payroll.Domain.Entities;

public class PayCut
{
    public Guid PayCutID { get; set; } = Guid.NewGuid();
    public Guid PayrollID { get; set; }
    public string Status { get; set; } = "active";
    
    // Relationships
    public Payroll? Payroll { get; set; }
    public List<PayCutDetail> Details { get; set; } = new();

    public decimal CalculateTotal()
    {
        if (!string.Equals(Status, "active", StringComparison.OrdinalIgnoreCase))
            return 0m;

        return Details.Sum(d => d.Amount);
    }

    public void AddDetail(PayCutDetail detail)
    {
        ArgumentNullException.ThrowIfNull(detail);

        detail.PayCutID = PayCutID;
        detail.PayCut = this;
        Details.Add(detail);
    }

    public void Cancel()
    {
        Status = "cancelled";
    }
}
