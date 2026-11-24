namespace Attendance.Models;

public class WorkShift
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int BreakMins { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Ngày bắt đầu có hiệu lực của ca làm việc
    /// </summary>
    public DateOnly? EffectiveDate { get; set; }

    public decimal CalculateDuration()
    {
        double minutes = (EndTime.ToTimeSpan() - StartTime.ToTimeSpan()).TotalMinutes;
        if (minutes < 0) minutes += 24 * 60;
        return Math.Max(0, (decimal)(minutes - BreakMins) / 60m);
    }

    public bool IsWithinShift(TimeOnly time) => StartTime <= EndTime
        ? time >= StartTime && time <= EndTime
        : time >= StartTime || time <= EndTime;

    public void Deactivate() => IsActive = false;
}
