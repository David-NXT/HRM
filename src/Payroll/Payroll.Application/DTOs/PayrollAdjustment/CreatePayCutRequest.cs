namespace Payroll.Application.DTOs.PayrollAdjustment;

public class CreatePayCutRequest
{
    public Guid PayCutReasonID { get; set; }

    public string Description { get; set; } = "";

    public decimal RateApplied { get; set; }

    public decimal Amount { get; set; }
}
