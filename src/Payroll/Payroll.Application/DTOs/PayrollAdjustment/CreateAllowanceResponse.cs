namespace Payroll.Application.DTOs.PayrollAdjustment;

public class CreateAllowanceResponse
{
    public int AllowanceDetailID { get; set; }

    public int PayrollID { get; set; }

    public int AllowanceTypeID { get; set; }

    public string Description { get; set; } = "";

    public decimal RateApplied { get; set; }

    public decimal Amount { get; set; }
}