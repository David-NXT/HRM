namespace Attendance.DTOs.AttendanceConfigAuditLog;

public class CreateAttendanceConfigAuditLogDto
{
    public string ConfigType { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string PerformedBy { get; set; } = string.Empty;
}
