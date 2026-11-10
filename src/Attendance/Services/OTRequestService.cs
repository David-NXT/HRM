using Attendance.Data;
using Attendance.DTOs.OTRequest;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class OTRequestService : IOTRequestService
{
    private readonly AttendanceDbContext _context;
    private readonly IHrEmployeeDirectoryClient _employees;

    public OTRequestService(AttendanceDbContext context, IHrEmployeeDirectoryClient employees)
    {
        _context = context;
        _employees = employees;
    }

    public async Task<List<OTRequestDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OTRequests
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .Select(x => new OTRequestDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                Date = x.Date,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                PlannedHours = x.PlannedHours,
                ActualHours = x.ActualHours,
                DayType = x.DayType,
                Multiplier = x.Multiplier,
                IsUnplanned = x.IsUnplanned,
                Reason = x.Reason,
                Status = x.Status,
                ApprovedBy = x.ApprovedBy,
                ApprovedAt = x.ApprovedAt,
                RejectionReason = x.RejectionReason,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<OTRequestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.OTRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return MapToDto(entity);
    }

    public async Task<OTRequestDto> CreateAsync(CreateOTRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.EmployeeId == Guid.Empty)
        {
            throw new ArgumentException("EmployeeId is required.", nameof(dto));
        }

        await EnsureActiveEmployeeAsync(dto.EmployeeId, cancellationToken);
        var plannedHours = dto.PlannedHours;
        if (plannedHours <= 0 && dto.EndTime > dto.StartTime)
        {
            var span = dto.EndTime - dto.StartTime;
            plannedHours = (decimal)span.TotalHours;
        }

        var entity = new OTRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            Date = dto.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            PlannedHours = plannedHours,
            DayType = dto.DayType,
            Multiplier = dto.Multiplier > 0 ? dto.Multiplier : 1.5m,
            IsUnplanned = dto.IsUnplanned,
            Reason = dto.Reason,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Pending" : dto.Status,
            ApprovedBy = dto.ApprovedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.OTRequests.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<OTRequestDto?> UpdateAsync(Guid id, UpdateOTRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _context.OTRequests
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

        if (dto.Date.HasValue)
        {
            entity.Date = dto.Date.Value;
        }

        if (dto.StartTime.HasValue)
        {
            entity.StartTime = dto.StartTime.Value;
        }

        if (dto.EndTime.HasValue)
        {
            entity.EndTime = dto.EndTime.Value;
        }

        if (dto.PlannedHours.HasValue)
        {
            entity.PlannedHours = dto.PlannedHours.Value;
        }

        if (dto.ActualHours.HasValue)
        {
            entity.ActualHours = dto.ActualHours.Value;
        }

        if (dto.DayType != null)
        {
            entity.DayType = dto.DayType;
        }

        if (dto.Multiplier.HasValue)
        {
            entity.Multiplier = dto.Multiplier.Value;
        }

        if (dto.IsUnplanned.HasValue)
        {
            entity.IsUnplanned = dto.IsUnplanned.Value;
        }

        if (dto.Reason != null)
        {
            entity.Reason = dto.Reason;
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            entity.Status = dto.Status;
        }

        if (dto.ApprovedBy != null)
        {
            entity.ApprovedBy = dto.ApprovedBy;
        }

        if (dto.ApprovedAt.HasValue)
        {
            entity.ApprovedAt = dto.ApprovedAt.Value;
        }

        if (dto.RejectionReason != null)
        {
            entity.RejectionReason = dto.RejectionReason;
        }

        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.OTRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _context.OTRequests.Remove(entity);
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

    private static OTRequestDto MapToDto(OTRequest entity)
    {
        return new OTRequestDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            Date = entity.Date,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            PlannedHours = entity.PlannedHours,
            ActualHours = entity.ActualHours,
            DayType = entity.DayType,
            Multiplier = entity.Multiplier,
            IsUnplanned = entity.IsUnplanned,
            Reason = entity.Reason,
            Status = entity.Status,
            ApprovedBy = entity.ApprovedBy,
            ApprovedAt = entity.ApprovedAt,
            RejectionReason = entity.RejectionReason,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
