namespace Payroll.Application.DTOs.PayrollPayment;

public class MarkPayrollAsPaidResponse
{
    public Guid PayrollID { get; set; }
    public Guid EmployeeID { get; set; }
    public decimal NetSalary { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
}