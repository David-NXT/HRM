namespace Payroll.Domain.Entities;

public class PayCutReason
{
    public Guid PayCutReasonID { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string CalculationType { get; set; } = "percentage"; // percentage, fixed, formula
    public decimal DefaultRate { get; set; }
    public string Status { get; set; } = "active";

    // Relationships
    public List<PayCutDetail> PayCutDetails { get; set; } = new();
}
