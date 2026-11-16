namespace Payroll.Application.DTOs.PayrollQuery;

public class PayrollListResponse
{
    public Guid PayrollID { get; set; }
    public Guid EmployeeID { get; set; }
    public Guid PayrollPeriodID { get; set; }

    public string? EmploymentType { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal WorkingDays { get; set; }
    public decimal StandardWorkingDay { get; set; }
    public decimal TotalWorkingHours { get; set; }

    public decimal TotalAllowance { get; set; }
    public decimal TotalProjectAllowance { get; set; }
    public decimal TotalPayCut { get; set; }

    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }

    public string Status { get; set; } = string.Empty;
    public DateTime? CalculatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}