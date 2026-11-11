namespace Payroll.Application.DTOs.PayrollCalculation;

public class CalculatePayrollResponse
{
    public Guid PayrollID { get; set; }
    public Guid EmployeeID { get; set; }

    public decimal BaseSalary { get; set; }
    public decimal WorkingDays { get; set; }
    public decimal StandardWorkingDay { get; set; }

    public decimal ProratedSalary { get; set; }

    public decimal TotalAllowance { get; set; }
    public decimal TotalPayCut { get; set; }

    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }

    public string Status { get; set; } = "calculated";
    public DateTime? CalculatedAt { get; set; }
}
