namespace Payroll.Application.DTOs.PayrollAdjustment;

public class CreatePayCutResponse
{
    public int PayCutDetailID { get; set; }

    public int PayrollID { get; set; }

    public int PayCutReasonID { get; set; }

    public string Description { get; set; } = "";

    public decimal RateApplied { get; set; }

    public decimal Amount { get; set; }
}