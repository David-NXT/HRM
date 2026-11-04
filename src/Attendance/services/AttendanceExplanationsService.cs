using Attendance.DTOs.AttendanceExplanation;
using Attendance.Data;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class AttendanceExplanationsService : IAttendanceExplanationsService
{
    private readonly AttendanceDbContext _context;

    public AttendanceExplanationsService (AttendanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<AttendanceExplanationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AttendanceExplanations
        .AsNoTracking()
        .Select(x => new AttendanceExplanationDto
        {
            Id=x.Id,
            AttendanceId=x.AttendanceId,
            EmployeeId=x.EmployeeId,
            Reason=x.Reason,
            RequestedTimeIn=x.RequestedTimeIn,
            RequestedTimeOut=x.RequestedTimeOut,
            Status=x.Status,
            ApprovedBy=x.ApprovedBy,
            CreatedAt=x.CreatedAt,
            UpdatedAt=x.UpdatedAt
        })
        .ToListAsync(cancellationToken);
    }

    public async Task<AttendanceExplanationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.AttendanceExplanations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id==id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return MapToDto(entity);
    }

    public async Task<AttendanceExplanationDto> CreateAsync(AttendanceExplanationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.AttendanceId == Guid.Empty)
        {
            throw new ArgumentException("AttendanceId is required.", nameof(dto));
        }
         if (dto.EmployeeId == Guid.Empty)
        {
            throw new ArgumentException("EmployeeId is required.", nameof(dto));
        }
        dto.Reason=dto.Reason.Trim();
        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            throw new ArgumentException("Reason is required", nameof(dto));
        }
        var entity = new AttendanceExplanation
        {
            Id=Guid.NewGuid(),
            AttendanceId=dto.AttendanceId,
            EmployeeId=dto.EmployeeId,
            Reason=dto.Reason,
            RequestedTimeIn=dto.RequestedTimeIn,
            RequestedTimeOut=dto.RequestedTimeOut,
            Status=string.IsNullOrWhiteSpace(dto.Status) ? "Pending":dto.Status,
            ApprovedBy=string.IsNullOrWhiteSpace(dto.ApprovedBy) ? null : dto.ApprovedBy.Trim(),
            CreatedAt=DateTime.UtcNow,
            UpdatedAt=DateTime.UtcNow
        };

        _context.AttendanceExplanations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<AttendanceExplanationDto?> UpdateAsync(AttendanceExplanationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Id == Guid.Empty)
        {
            throw new ArgumentException("Id is required.", nameof(dto));
        }

        var entity = await _context.AttendanceExplanations
            .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        if (dto.AttendanceId != Guid.Empty)
        {
            entity.AttendanceId = dto.AttendanceId;
        }

        if (dto.EmployeeId != Guid.Empty)
        {
            entity.EmployeeId = dto.EmployeeId;
        }

        if (!string.IsNullOrWhiteSpace(dto.Reason))
        {
            entity.Reason = dto.Reason.Trim();
        }

        entity.RequestedTimeIn = dto.RequestedTimeIn;
        entity.RequestedTimeOut = dto.RequestedTimeOut;

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            entity.Status = dto.Status;
        }

        entity.ApprovedBy = string.IsNullOrWhiteSpace(dto.ApprovedBy)
            ? null
            : dto.ApprovedBy.Trim();

        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(AttendanceExplanationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Id == Guid.Empty)
        {
            throw new ArgumentException("Id is required.", nameof(dto));
        }

        var entity = await _context.AttendanceExplanations
            .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _context.AttendanceExplanations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public static AttendanceExplanationDto MapToDto(AttendanceExplanation entity)
    {
        return new AttendanceExplanationDto
        {
            Id=entity.Id,
            AttendanceId=entity.AttendanceId,
            EmployeeId=entity.EmployeeId,
            Reason=entity.Reason,
            RequestedTimeIn=entity.RequestedTimeIn,
            RequestedTimeOut=entity.RequestedTimeOut,
            Status=entity.Status,
            ApprovedBy=entity.ApprovedBy,
            CreatedAt=entity.CreatedAt,
            UpdatedAt=entity.UpdatedAt
        };
    }
}