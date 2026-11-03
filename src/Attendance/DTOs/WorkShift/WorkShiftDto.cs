namespace Attendance.DTOs.WorkShift;

public class WorkShiftDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int BreakMins { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
