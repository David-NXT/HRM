namespace Attendance.Models;

public class AttendanceRule
{
    public Guid Id { get; set; }

    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// Số phút đi trễ được chấp nhận (hạn mức vi phạm)
    /// </summary>
    public int AllowedLateMinutes { get; set; }

    /// <summary>
    /// Số phút về sớm được chấp nhận (hạn mức vi phạm)
    /// </summary>
    public int AllowedEarlyLeaveMinutes { get; set; }

    /// <summary>
    /// Số lần vi phạm tối đa được chấp nhận trong tháng
    /// </summary>
    public int MaxViolationsPerMonth { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool ExceedsLateThreshold(int lateMinutes) => lateMinutes > AllowedLateMinutes;
    public bool ExceedsEarlyLeaveThreshold(int earlyMinutes) => earlyMinutes > AllowedEarlyLeaveMinutes;
}
