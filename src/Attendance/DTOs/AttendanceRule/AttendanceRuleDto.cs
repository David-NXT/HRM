namespace Attendance.DTOs.AttendanceRule;

public class AttendanceRuleDto
{
    public Guid Id { get; set; }

    public string RuleName { get; set; } = string.Empty;

    public int AllowedLateMinutes { get; set; }

    public int AllowedEarlyLeaveMinutes { get; set; }

    public int MaxViolationsPerMonth { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public bool IsActive { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
