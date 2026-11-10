using Attendance.Data;
using Attendance.DTOs.AttendanceConfigAuditLog;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class AttendanceConfigAuditLogService : IAttendanceConfigAuditLogService
{
    private readonly AttendanceDbContext _dbContext;

    public AttendanceConfigAuditLogService(AttendanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AttendanceConfigAuditLogDto>> GetAllAsync(string? configType = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AttendanceConfigAuditLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(configType))
        {
            query = query.Where(x => x.ConfigType.ToLower() == configType.ToLower());
        }

        var list = await query.OrderByDescending(x => x.PerformedAt).ToListAsync(cancellationToken);
        return list.Select(MapToDto).ToList();
    }

    public async Task<AttendanceConfigAuditLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var log = await _dbContext.AttendanceConfigAuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return log == null ? null : MapToDto(log);
    }

    public async Task<AttendanceConfigAuditLogDto> LogAsync(CreateAttendanceConfigAuditLogDto dto, CancellationToken cancellationToken = default)
    {
        var log = new AttendanceConfigAuditLog
        {
            Id = Guid.NewGuid(),
            ConfigType = dto.ConfigType,
            EntityId = dto.EntityId,
            Action = dto.Action,
            OldValues = dto.OldValues,
            NewValues = dto.NewValues,
            PerformedBy = dto.PerformedBy,
            PerformedAt = DateTime.UtcNow
        };

        _dbContext.AttendanceConfigAuditLogs.Add(log);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(log);
    }

    private static AttendanceConfigAuditLogDto MapToDto(AttendanceConfigAuditLog model) => new()
    {
        Id = model.Id,
        ConfigType = model.ConfigType,
        EntityId = model.EntityId,
        Action = model.Action,
        OldValues = model.OldValues,
        NewValues = model.NewValues,
        PerformedBy = model.PerformedBy,
        PerformedAt = model.PerformedAt
    };
}
