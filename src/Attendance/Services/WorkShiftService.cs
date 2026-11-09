using Attendance.Data;
using Attendance.DTOs.WorkShift;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class WorkShiftService : IWorkShiftService
{
    private readonly AttendanceDbContext _context;

    public WorkShiftService(AttendanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkShiftDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WorkShifts
            .AsNoTracking()
            .OrderBy(x => x.StartTime)
            .Select(x => new WorkShiftDto
            {
                Id = x.Id,
                Name = x.Name,
                BreakMins = x.BreakMins,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Description = x.Description,
                IsActive = x.IsActive,
                EffectiveDate = x.EffectiveDate
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<WorkShiftDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkShifts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return MapToDto(entity);
    }

    public async Task<WorkShiftDto> CreateAsync(CreateWorkShiftDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException("Name is required.", nameof(dto));
        }

        if (dto.StartTime == default && dto.EndTime == default)
        {
            throw new ArgumentException("StartTime and EndTime are required.", nameof(dto));
        }

        var entity = new WorkShift
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            BreakMins = dto.BreakMins,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Description = dto.Description,
            IsActive = dto.IsActive,
            EffectiveDate = dto.EffectiveDate
        };

        _context.WorkShifts.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<WorkShiftDto?> UpdateAsync(Guid id, UpdateWorkShiftDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkShifts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            entity.Name = dto.Name.Trim();
        }

        if (dto.BreakMins.HasValue)
        {
            entity.BreakMins = dto.BreakMins.Value;
        }

        if (dto.StartTime.HasValue)
        {
            entity.StartTime = dto.StartTime.Value;
        }

        if (dto.EndTime.HasValue)
        {
            entity.EndTime = dto.EndTime.Value;
        }

        if (dto.Description != null)
        {
            entity.Description = dto.Description;
        }

        if (dto.IsActive.HasValue)
        {
            entity.IsActive = dto.IsActive.Value;
        }

        if (dto.EffectiveDate.HasValue)
        {
            entity.EffectiveDate = dto.EffectiveDate.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkShifts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _context.WorkShifts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static WorkShiftDto MapToDto(WorkShift entity)
    {
        return new WorkShiftDto
        {
            Id = entity.Id,
            Name = entity.Name,
            BreakMins = entity.BreakMins,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            Description = entity.Description,
            IsActive = entity.IsActive,
            EffectiveDate = entity.EffectiveDate
        };
    }
}
