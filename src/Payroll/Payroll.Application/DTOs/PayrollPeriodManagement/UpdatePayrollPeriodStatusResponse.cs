namespace Payroll.Application.DTOs.PayrollPeriodManagement;

public class UpdatePayrollPeriodStatusResponse
{
    public Guid PayrollPeriodID { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Status { get; set; } = string.Empty;
    public int PayrollCount { get; set; }
    public DateTime UpdatedAt { get; set; }
}