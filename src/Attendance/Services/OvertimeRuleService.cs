using Attendance.Data;
using Attendance.DTOs.OvertimeRule;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class OvertimeRuleService : IOvertimeRuleService
{
    private readonly AttendanceDbContext _dbContext;

    public OvertimeRuleService(AttendanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<OvertimeRuleDto>> GetAllAsync(bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.OvertimeRules.AsNoTracking();
        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        var list = await query.OrderBy(x => x.DayType).ToListAsync(cancellationToken);
        return list.Select(MapToDto).ToList();
    }

    public async Task<OvertimeRuleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rule = await _dbContext.OvertimeRules
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return rule == null ? null : MapToDto(rule);
    }

    public async Task<OvertimeRuleDto?> GetByDayTypeAsync(string dayType, CancellationToken cancellationToken = default)
    {
        var rule = await _dbContext.OvertimeRules
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DayType.ToLower() == dayType.ToLower() && x.IsActive, cancellationToken);

        return rule == null ? null : MapToDto(rule);
    }

    public async Task<OvertimeRuleDto> CreateAsync(CreateOvertimeRuleDto dto, CancellationToken cancellationToken = default)
    {
        var rule = new OvertimeRule
        {
            Id = Guid.NewGuid(),
            DayType = dto.DayType,
            Multiplier = dto.Multiplier,
            MaxHoursPerDay = dto.MaxHoursPerDay,
            MaxHoursPerMonth = dto.MaxHoursPerMonth,
            MaxHoursPerYear = dto.MaxHoursPerYear,
            EffectiveDate = dto.EffectiveDate,
            IsActive = dto.IsActive,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = dto.CreatedBy
        };

        _dbContext.OvertimeRules.Add(rule);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(rule);
    }

    public async Task<OvertimeRuleDto?> UpdateAsync(Guid id, UpdateOvertimeRuleDto dto, CancellationToken cancellationToken = default)
    {
        var rule = await _dbContext.OvertimeRules.FindAsync([id], cancellationToken);
        if (rule == null)
        {
            return null;
        }

        rule.DayType = dto.DayType;
        rule.Multiplier = dto.Multiplier;
        rule.MaxHoursPerDay = dto.MaxHoursPerDay;
        rule.MaxHoursPerMonth = dto.MaxHoursPerMonth;
        rule.MaxHoursPerYear = dto.MaxHoursPerYear;
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
        var rule = await _dbContext.OvertimeRules.FindAsync([id], cancellationToken);
        if (rule == null)
        {
            return false;
        }

        _dbContext.OvertimeRules.Remove(rule);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<decimal> GetOvertimeMultiplierAsync(
        string dayType,
        DateOnly workDate)
    {
        var rule = await _dbContext.OvertimeRules
            .Where(x =>
                x.DayType == dayType &&
                x.IsActive &&
                x.EffectiveDate <= workDate)
            .OrderByDescending(x => x.EffectiveDate)
            .FirstOrDefaultAsync();

        if (rule == null)
        {
            throw new InvalidOperationException(
                $"Không tìm thấy cấu hình hệ số OT cho loại ngày '{dayType}' " +
                $"tại ngày {workDate:dd/MM/yyyy}.");
        }

        return rule.Multiplier;
    }
    
    private static OvertimeRuleDto MapToDto(OvertimeRule model) => new()
    {
        Id = model.Id,
        DayType = model.DayType,
        Multiplier = model.Multiplier,
        MaxHoursPerDay = model.MaxHoursPerDay,
        MaxHoursPerMonth = model.MaxHoursPerMonth,
        MaxHoursPerYear = model.MaxHoursPerYear,
        EffectiveDate = model.EffectiveDate,
        IsActive = model.IsActive,
        Description = model.Description,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt,
        UpdatedBy = model.UpdatedBy
    };
}
