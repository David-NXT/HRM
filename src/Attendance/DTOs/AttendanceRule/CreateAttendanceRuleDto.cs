namespace Attendance.DTOs.AttendanceRule;

public class CreateAttendanceRuleDto
{
    public string RuleName { get; set; } = string.Empty;

    public int AllowedLateMinutes { get; set; } = 15;

    public int AllowedEarlyLeaveMinutes { get; set; } = 0;

    public int MaxViolationsPerMonth { get; set; } = 3;

    public DateOnly EffectiveDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }
}
