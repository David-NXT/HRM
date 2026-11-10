using Attendance.DTOs.AttendanceConfigAuditLog;

namespace Attendance.Services;

public interface IAttendanceConfigAuditLogService
{
    Task<List<AttendanceConfigAuditLogDto>> GetAllAsync(string? configType = null, CancellationToken cancellationToken = default);
    Task<AttendanceConfigAuditLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AttendanceConfigAuditLogDto> LogAsync(CreateAttendanceConfigAuditLogDto dto, CancellationToken cancellationToken = default);
}
