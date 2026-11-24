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

    public bool IsActive()
    {
        return string.Equals(Status, "active", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsPercentage()
    {
        return string.Equals(CalculationType, "percentage", StringComparison.OrdinalIgnoreCase);
    }
}
