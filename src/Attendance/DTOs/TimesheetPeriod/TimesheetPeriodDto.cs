namespace Attendance.DTOs.TimesheetPeriod;

public class TimesheetPeriodDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Month { get; set; }

    public int Year { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? LockedAt { get; set; }

    public string? LockedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
