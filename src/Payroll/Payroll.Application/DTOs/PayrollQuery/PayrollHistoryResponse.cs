namespace Payroll.Application.DTOs.PayrollQuery;

public class PayrollHistoryResponse
{
    public Guid PayrollID { get; set; }
    public Guid PayrollPeriodID { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public decimal BaseSalary { get; set; }
    public decimal TotalAllowance { get; set; }
    public decimal TotalPayCut { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }

    public string Status { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
}