namespace Payroll.Domain.Entities;

public class PayrollPeriod
{
    public Guid PayrollPeriodID { get; set; } = Guid.NewGuid();
    public int Year { get; set; }
    public int Month { get; set; }
    public string Status { get; set; } = "draft"; // draft, calculated, approved, paid
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Payroll> Payrolls { get; set; } = new();

    public bool IsDraft()
    {
        return string.Equals(Status, "draft", StringComparison.OrdinalIgnoreCase);
    }

    public void UpdateStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be empty.", nameof(status));

        string normalized = status.Trim().ToLowerInvariant();
        if (normalized is not ("draft" or "calculated" or "approved" or "paid"))
            throw new ArgumentException($"Unsupported payroll period status: {status}", nameof(status));

        Status = normalized;
    }

    public string GetPeriodName()
    {
        return $"{Year}-{Month:D2}";
    }
}
