using Attendance.Data;
using Attendance.DTOs.Holiday;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class HolidayService : IHolidayService
{
    private readonly AttendanceDbContext _context;

    public HolidayService(AttendanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<HolidayDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Holidays
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .Select(x => new HolidayDto
            {
                Id = x.Id,
                Date = x.Date,
                EndDate = x.EndDate,
                Name = x.Name,
                Description = x.Description,
                HolidayType = x.HolidayType,
                CreatedAt = x.CreatedAt,
                IsPaid = x.IsPaid
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<HolidayDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Holiday? entity = await _context.Holidays
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return MapToDto(entity);
    }

    public async Task<HolidayDto> CreateAsync(CreateHolidayDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException("Name is required.", nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.HolidayType))
        {
            throw new ArgumentException("HolidayType is required.", nameof(dto));
        }

        Holiday entity = new Holiday
        {
            Id = Guid.NewGuid(),
            Date = dto.Date,
            EndDate = dto.EndDate,
            Name = dto.Name.Trim(),
            Description = dto.Description,
            HolidayType = dto.HolidayType.Trim(),
            IsPaid = dto.IsPaid,
            CreatedAt = DateTime.UtcNow
        };

        _context.Holidays.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<HolidayDto?> UpdateAsync(Guid id, UpdateHolidayDto dto, CancellationToken cancellationToken = default)
    {
        Holiday? entity = await _context.Holidays
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        if (dto.Date.HasValue)
        {
            entity.Date = dto.Date.Value;
        }

        if (dto.EndDate.HasValue)
        {
            entity.EndDate = dto.EndDate.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            entity.Name = dto.Name.Trim();
        }

        if (dto.Description != null)
        {
            entity.Description = dto.Description;
        }

        if (!string.IsNullOrWhiteSpace(dto.HolidayType))
        {
            entity.HolidayType = dto.HolidayType.Trim();
        }

        if (dto.IsPaid.HasValue)
        {
            entity.IsPaid = dto.IsPaid.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Holiday? entity = await _context.Holidays
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _context.Holidays.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static HolidayDto MapToDto(Holiday entity)
    {
        return new HolidayDto
        {
            Id = entity.Id,
            Date = entity.Date,
            EndDate = entity.EndDate,
            Name = entity.Name,
            Description = entity.Description,
            HolidayType = entity.HolidayType,
            CreatedAt = entity.CreatedAt,
            IsPaid = entity.IsPaid
        };
    }
}
