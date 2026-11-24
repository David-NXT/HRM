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
        IQueryable<AttendanceConfigAuditLog> query = _dbContext.AttendanceConfigAuditLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(configType))
        {
            query = query.Where(x => x.ConfigType.ToLower() == configType.ToLower());
        }

        List<AttendanceConfigAuditLog> list = await query.OrderByDescending(x => x.PerformedAt).ToListAsync(cancellationToken);
        return list.Select(MapToDto).ToList();
    }

    public async Task<AttendanceConfigAuditLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        AttendanceConfigAuditLog? log = await _dbContext.AttendanceConfigAuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return log == null ? null : MapToDto(log);
    }

    public async Task<AttendanceConfigAuditLogDto> LogAsync(CreateAttendanceConfigAuditLogDto dto, CancellationToken cancellationToken = default)
    {
        AttendanceConfigAuditLog log = new AttendanceConfigAuditLog
        {
            Id = Guid.NewGuid(),
            EntityId = dto.EntityId,
            OldValues = dto.OldValues,
            NewValues = dto.NewValues,
        };
        log.RecordAction(dto.ConfigType, dto.Action, dto.PerformedBy);

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
