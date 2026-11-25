namespace Payroll.Domain.Entities;

public class AllowanceDetail
{
    public Guid AllowanceDetailID { get; set; } = Guid.NewGuid();
    public Guid AllowanceID { get; set; }
    public Guid AllowanceTypeID { get; set; }
    public string Description { get; set; } = "";
    public decimal RateApplied { get; set; }
    public decimal Amount { get; set; }

    // Relationships
    public Allowance? Allowance { get; set; }
    public AllowanceType? AllowanceType { get; set; }

    public decimal ComputeAmount(decimal baseSalary)
    {
        if (AllowanceType != null && AllowanceType.IsPercentage())
            return Math.Round(baseSalary * RateApplied, 2, MidpointRounding.AwayFromZero);

        return Amount != 0m ? Amount : RateApplied;
    }
}
