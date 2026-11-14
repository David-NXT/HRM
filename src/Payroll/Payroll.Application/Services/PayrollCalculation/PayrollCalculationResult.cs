namespace Payroll.Application.Services.PayrollCalculation;

public class PayrollCalculationResult
{
    public decimal ProratedSalary { get; set; }

    public decimal OTPay { get; set; }

    public decimal TotalAllowance { get; set; }

    public decimal TotalPayCut { get; set; }

    public decimal GrossSalary { get; set; }

    public decimal NetSalary { get; set; }
}