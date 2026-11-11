namespace Payroll.Application.DTOs.PayrollAdjustment;

public class CreatePayCutResponse
{
    public Guid PayCutDetailID { get; set; }

    public Guid PayrollID { get; set; }

    public Guid PayCutReasonID { get; set; }

    public string Description { get; set; } = "";

    public decimal RateApplied { get; set; }

    public decimal Amount { get; set; }
}
