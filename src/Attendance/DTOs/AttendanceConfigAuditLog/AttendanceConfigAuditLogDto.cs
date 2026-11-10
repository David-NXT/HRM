namespace Attendance.DTOs.AttendanceConfigAuditLog;

public class AttendanceConfigAuditLogDto
{
    public Guid Id { get; set; }

    public string ConfigType { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string PerformedBy { get; set; } = string.Empty;

    public DateTime PerformedAt { get; set; }
}
