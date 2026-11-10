namespace Attendance.DTOs.AttendanceRule;

public class UpdateAttendanceRuleDto
{
    public string RuleName { get; set; } = string.Empty;

    public int AllowedLateMinutes { get; set; }

    public int AllowedEarlyLeaveMinutes { get; set; }

    public int MaxViolationsPerMonth { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public bool IsActive { get; set; }

    public string? Description { get; set; }

    public string? UpdatedBy { get; set; }
}
