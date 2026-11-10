using Attendance.Data;
using Attendance.DTOs.AttendanceRecord;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class AttendanceService : IAttendanceService
{
    private readonly AttendanceDbContext _context;
    private readonly IHrEmployeeDirectoryClient _employees;

    public AttendanceService(AttendanceDbContext context, IHrEmployeeDirectoryClient employees)
    {
        _context = context;
        _employees = employees;
    }

    public async Task<List<AttendanceRecordDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AttendanceRecords
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .Select(x => new AttendanceRecordDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                ShiftId = x.ShiftId,
                Date = x.Date,
                TimeIn = x.TimeIn,
                TimeOut = x.TimeOut,
                Status = x.Status,
                WorkHour = x.WorkHour,
                LateMins = x.LateMins,
                EarlyLeaveMins = x.EarlyLeaveMins,
                OTMins = x.OTMins,
                Note = x.Note,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AttendanceRecordDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.AttendanceRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return MapToDto(entity);
    }

    public async Task<AttendanceRecordDto> CreateAsync(CreateAttendanceRecordDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.EmployeeId == Guid.Empty)
        {
            throw new ArgumentException("EmployeeId is required.", nameof(dto));
        }

        await EnsureActiveEmployeeAsync(dto.EmployeeId, cancellationToken);

        if (dto.ShiftId == Guid.Empty)
        {
            throw new ArgumentException("ShiftId is required.", nameof(dto));
        }

        var entity = new AttendanceRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            ShiftId = dto.ShiftId,
            Date = dto.Date,
            TimeIn = dto.TimeIn,
            TimeOut = dto.TimeOut,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Pending" : dto.Status,
            WorkHour = dto.WorkHour,
            LateMins = dto.LateMins,
            EarlyLeaveMins = dto.EarlyLeaveMins,
            OTMins = dto.OTMins,
            Note = dto.Note,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AttendanceRecords.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<AttendanceRecordDto?> UpdateAsync(Guid id, UpdateAttendanceRecordDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _context.AttendanceRecords
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        if (dto.EmployeeId.HasValue)
        {
            await EnsureActiveEmployeeAsync(dto.EmployeeId.Value, cancellationToken);
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

        if (dto.TimeIn.HasValue)
        {
            entity.TimeIn = dto.TimeIn.Value;
        }

        if (dto.TimeOut.HasValue)
        {
            entity.TimeOut = dto.TimeOut.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            entity.Status = dto.Status;
        }

        if (dto.WorkHour.HasValue)
        {
            entity.WorkHour = dto.WorkHour.Value;
        }

        if (dto.LateMins.HasValue)
        {
            entity.LateMins = dto.LateMins.Value;
        }

        if (dto.EarlyLeaveMins.HasValue)
        {
            entity.EarlyLeaveMins = dto.EarlyLeaveMins.Value;
        }

        if (dto.OTMins.HasValue)
        {
            entity.OTMins = dto.OTMins.Value;
        }

        if (dto.Note != null)
        {
            entity.Note = dto.Note;
        }

        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.AttendanceRecords
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _context.AttendanceRecords.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureActiveEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        if (await _employees.GetActiveEmployeeAsync(employeeId, cancellationToken) is null)
        {
            throw new EmployeeNotAvailableException(employeeId);
        }
    }

    private static AttendanceRecordDto MapToDto(AttendanceRecord entity)
    {
        return new AttendanceRecordDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            ShiftId = entity.ShiftId,
            Date = entity.Date,
            TimeIn = entity.TimeIn,
            TimeOut = entity.TimeOut,
            Status = entity.Status,
            WorkHour = entity.WorkHour,
            LateMins = entity.LateMins,
            EarlyLeaveMins = entity.EarlyLeaveMins,
            OTMins = entity.OTMins,
            Note = entity.Note,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
