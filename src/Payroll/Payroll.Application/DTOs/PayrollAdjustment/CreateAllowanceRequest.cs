namespace Payroll.Application.DTOs.PayrollAdjustment;

public class CreateAllowanceRequest
{
    public int AllowanceTypeID { get; set; }

    public string Description { get; set; } = "";

    public decimal RateApplied { get; set; }

    public decimal Amount { get; set; }
}