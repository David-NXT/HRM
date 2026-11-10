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
}
