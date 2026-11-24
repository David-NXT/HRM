namespace Payroll.Domain.Entities;

public class Allowance
{
    public Guid AllowanceID { get; set; } = Guid.NewGuid();
    public Guid PayrollID { get; set; }
    public string Status { get; set; } = "active";

    // Relationships
    public Payroll? Payroll { get; set; }
    public List<AllowanceDetail> AllowanceDetails { get; set; } = new();

    public decimal CalculateTotal()
    {
        if (!string.Equals(Status, "active", StringComparison.OrdinalIgnoreCase))
            return 0m;

        return AllowanceDetails.Sum(d => d.Amount);
    }

    public void AddDetail(AllowanceDetail detail)
    {
        ArgumentNullException.ThrowIfNull(detail);

        detail.AllowanceID = AllowanceID;
        detail.Allowance = this;
        AllowanceDetails.Add(detail);
    }

    public void Cancel()
    {
        Status = "cancelled";
    }
}
