namespace Attendance.Models;

public class LeaveBalance
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public int Year { get; set; }

    public decimal TotalDays { get; set; }

    public decimal UsedDays { get; set; }

    public decimal RemainingDays { get; set; }

    public decimal CarriedOverDays { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool DeductDays(decimal days)
    {
        if (days < 0 || RemainingDays < days) return false;
        UsedDays += days; RemainingDays -= days; UpdatedAt = DateTime.UtcNow; return true;
    }

    public void AddCarriedOverDays(decimal days)
    {
        if (days <= 0) return;
        CarriedOverDays += days; TotalDays += days; RemainingDays += days; UpdatedAt = DateTime.UtcNow;
    }

    public decimal RecalculateRemaining()
    {
        RemainingDays = Math.Max(0, TotalDays - UsedDays); return RemainingDays;
    }
}
