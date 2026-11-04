using Attendance.Data;
using Attendance.DTOs.ShiftAssignment;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class ShiftAssignmentService : IShiftAssignmentService
{
    private readonly AttendanceDbContext _context;

    public ShiftAssignmentService(AttendanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShiftAssignmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ShiftAssignments
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .Select(x => new ShiftAssignmentDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                ShiftId = x.ShiftId,
                Date = x.Date,
                Status = x.Status,
                AssignAt = x.AssignAt,
                Note = x.Note,
                ApprovedBy = x.ApprovedBy,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ShiftAssignmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ShiftAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return MapToDto(entity);
    }

    public async Task<ShiftAssignmentDto> CreateAsync(CreateShiftAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.EmployeeId == Guid.Empty)
        {
            throw new ArgumentException("EmployeeId is required.", nameof(dto));
        }

        if (dto.ShiftId == Guid.Empty)
        {
            throw new ArgumentException("ShiftId is required.", nameof(dto));
        }

        var entity = new ShiftAssignment
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            ShiftId = dto.ShiftId,
            Date = dto.Date,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Pending" : dto.Status,
            AssignAt = dto.AssignAt,
            Note = dto.Note,
            ApprovedBy = dto.ApprovedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ShiftAssignments.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<ShiftAssignmentDto?> UpdateAsync(Guid id, UpdateShiftAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ShiftAssignments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        if (dto.EmployeeId.HasValue)
        {
            entity.EmployeeId = dto.EmployeeId.Value;
        }

        if (dto.ShiftId.HasValue)
        {
            entity.ShiftId = dto.ShiftId.Value;
        }

        if (dto.Date.HasValue)
        {
            entity.Date = dto.Date.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            entity.Status = dto.Status;
        }

        if (dto.AssignAt.HasValue)
        {
            entity.AssignAt = dto.AssignAt.Value;
        }

        if (dto.Note != null)
        {
            entity.Note = dto.Note;
        }

        if (dto.ApprovedBy != null)
        {
            entity.ApprovedBy = dto.ApprovedBy;
        }

        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ShiftAssignments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _context.ShiftAssignments.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ShiftAssignmentDto MapToDto(ShiftAssignment entity)
    {
        return new ShiftAssignmentDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            ShiftId = entity.ShiftId,
            Date = entity.Date,
            Status = entity.Status,
            AssignAt = entity.AssignAt,
            Note = entity.Note,
            ApprovedBy = entity.ApprovedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
