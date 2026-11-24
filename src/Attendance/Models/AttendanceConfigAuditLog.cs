namespace Attendance.Models;

public class AttendanceConfigAuditLog
{
    public Guid Id { get; set; }

    /// <summary>
    /// Nhóm cấu hình: WorkShift, OvertimeRule, AttendanceRule
    /// </summary>
    public string ConfigType { get; set; } = string.Empty;

    /// <summary>
    /// Id của đối tượng cấu hình được thay đổi (WorkShiftId, OvertimeRuleId, AttendanceRuleId)
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// Hành động: Create, Update, Deactivate, Delete
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Giá trị cấu hình trước khi thay đổi (định dạng JSON)
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// Giá trị cấu hình sau khi thay đổi (định dạng JSON)
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// Tài khoản thực hiện (HR Specialist / System Admin)
    /// </summary>
    public string PerformedBy { get; set; } = string.Empty;

    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

    public void RecordAction(string configType, string action, string performedBy)
    {
        ConfigType = configType; Action = action; PerformedBy = performedBy; PerformedAt = DateTime.UtcNow;
    }
}
