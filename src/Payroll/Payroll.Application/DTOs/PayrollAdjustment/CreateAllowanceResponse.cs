namespace Payroll.Application.DTOs.PayrollAdjustment;

public class CreateAllowanceResponse
{
    public Guid AllowanceDetailID { get; set; }

    public Guid PayrollID { get; set; }

    public Guid AllowanceTypeID { get; set; }

    public string Description { get; set; } = "";

    public decimal RateApplied { get; set; }

    public decimal Amount { get; set; }
}
