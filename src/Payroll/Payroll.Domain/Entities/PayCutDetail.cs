namespace Payroll.Domain.Entities;

public class PayCutDetail
{
    public Guid PayCutDetailID { get; set; } = Guid.NewGuid();
    public Guid PayCutID { get; set; }
    public Guid PayCutReasonID { get; set; }
    public string Description { get; set; } = "";
    public decimal RateApplied { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public PayCut? PayCut { get; set; }
    public PayCutReason? PayCutReason { get; set; }
}
