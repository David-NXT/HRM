namespace Attendance.Models;

public class TimesheetPeriod
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Month { get; set; }

    public int Year { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Trạng thái kỳ công: OPEN hoặc LOCKED
    /// </summary>
    public string Status { get; set; } = "OPEN";

    public DateTime? LockedAt { get; set; }

    public string? LockedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<TimesheetSummary>? Summaries { get; set; }
}
