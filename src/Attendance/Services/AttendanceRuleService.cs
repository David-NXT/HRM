using Attendance.Data;
using Attendance.DTOs.AttendanceRule;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class AttendanceRuleService : IAttendanceRuleService
{
    private readonly AttendanceDbContext _dbContext;

    public AttendanceRuleService(AttendanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AttendanceRuleDto>> GetAllAsync(bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AttendanceRules.AsNoTracking();
        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        var list = await query.OrderByDescending(x => x.EffectiveDate).ToListAsync(cancellationToken);
        return list.Select(MapToDto).ToList();
    }

    public async Task<AttendanceRuleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rule = await _dbContext.AttendanceRules
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return rule == null ? null : MapToDto(rule);
    }

    public async Task<AttendanceRuleDto?> GetActiveRuleAsync(CancellationToken cancellationToken = default)
    {
        var rule = await _dbContext.AttendanceRules
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.EffectiveDate)
            .FirstOrDefaultAsync(cancellationToken);

        return rule == null ? null : MapToDto(rule);
    }

    public async Task<AttendanceRuleDto> CreateAsync(CreateAttendanceRuleDto dto, CancellationToken cancellationToken = default)
    {
        var rule = new AttendanceRule
        {
            Id = Guid.NewGuid(),
            RuleName = dto.RuleName,
            AllowedLateMinutes = dto.AllowedLateMinutes,
            AllowedEarlyLeaveMinutes = dto.AllowedEarlyLeaveMinutes,
            MaxViolationsPerMonth = dto.MaxViolationsPerMonth,
            EffectiveDate = dto.EffectiveDate,
            IsActive = dto.IsActive,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = dto.CreatedBy
        };

        _dbContext.AttendanceRules.Add(rule);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(rule);
    }

    public async Task<AttendanceRuleDto?> UpdateAsync(Guid id, UpdateAttendanceRuleDto dto, CancellationToken cancellationToken = default)
    {
        var rule = await _dbContext.AttendanceRules.FindAsync([id], cancellationToken);
        if (rule == null)
        {
            return null;
        }

        rule.RuleName = dto.RuleName;
        rule.AllowedLateMinutes = dto.AllowedLateMinutes;
        rule.AllowedEarlyLeaveMinutes = dto.AllowedEarlyLeaveMinutes;
        rule.MaxViolationsPerMonth = dto.MaxViolationsPerMonth;
        rule.EffectiveDate = dto.EffectiveDate;
        rule.IsActive = dto.IsActive;
        rule.Description = dto.Description;
        rule.UpdatedAt = DateTime.UtcNow;
        rule.UpdatedBy = dto.UpdatedBy;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(rule);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rule = await _dbContext.AttendanceRules.FindAsync([id], cancellationToken);
        if (rule == null)
        {
            return false;
        }

        _dbContext.AttendanceRules.Remove(rule);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static AttendanceRuleDto MapToDto(AttendanceRule model) => new()
    {
        Id = model.Id,
        RuleName = model.RuleName,
        AllowedLateMinutes = model.AllowedLateMinutes,
        AllowedEarlyLeaveMinutes = model.AllowedEarlyLeaveMinutes,
        MaxViolationsPerMonth = model.MaxViolationsPerMonth,
        EffectiveDate = model.EffectiveDate,
        IsActive = model.IsActive,
        Description = model.Description,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt,
        UpdatedBy = model.UpdatedBy
    };
}
