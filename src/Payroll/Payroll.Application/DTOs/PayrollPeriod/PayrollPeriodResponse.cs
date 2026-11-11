namespace Payroll.Application.DTOs.PayrollPeriod;

public class PayrollPeriodResponse
{
    public Guid PayrollPeriodID { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
