using Attendance.Data;
using Attendance.DTOs.AttendanceExplanation;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class AttendanceExplanationsService : IAttendanceExplanationsService
{
    private readonly AttendanceDbContext _context;
    private readonly IHrEmployeeDirectoryClient _employees;

    public AttendanceExplanationsService(AttendanceDbContext context, IHrEmployeeDirectoryClient employees)
    {
        _context = context;
        _employees = employees;
    }

    public async Task<List<AttendanceExplanationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AttendanceExplanations
            .AsNoTracking()
            .Include(x => x.Attachments)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AttendanceExplanationDto
            {
                Id = x.Id,
                AttendanceId = x.AttendanceId,
                EmployeeId = x.EmployeeId,
                Date = x.Date,
                ExplanationType = x.ExplanationType,
                Reason = x.Reason,
                RequestedTimeIn = x.RequestedTimeIn,
                RequestedTimeOut = x.RequestedTimeOut,
                AttachmentUrl = x.AttachmentUrl,
                Status = x.Status,
                ApprovedBy = x.ApprovedBy,
                ApprovedAt = x.ApprovedAt,
                RejectionReason = x.RejectionReason,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Attachments = x.Attachments != null
                    ? x.Attachments.Select(a => new AttendanceExplanationAttachmentDto
                    {
                        Id = a.Id,
                        ExplanationId = a.ExplanationId,
                        FileName = a.FileName,
                        FileUrl = a.FileUrl,
                        FileType = a.FileType,
                        FileSize = a.FileSize,
                        UploadedAt = a.UploadedAt
                    }).ToList()
                    : null
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AttendanceExplanationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        AttendanceExplanation? entity = await _context.AttendanceExplanations
            .AsNoTracking()
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return MapToDto(entity);
    }

    public async Task<AttendanceExplanationDto> CreateAsync(CreateAttendanceExplanationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.EmployeeId == Guid.Empty)
        {
            throw new ArgumentException("EmployeeId is required.", nameof(dto));
        }

        await EnsureActiveEmployeeAsync(dto.EmployeeId, cancellationToken);

        string? reason = dto.Reason?.Trim();
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason is required.", nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.ExplanationType))
        {
            throw new ArgumentException("ExplanationType is required.", nameof(dto));
        }

        Guid explanationId = Guid.NewGuid();
        AttendanceExplanation entity = new AttendanceExplanation
        {
            Id = explanationId,
            AttendanceId = dto.AttendanceId,
            EmployeeId = dto.EmployeeId,
            Date = dto.Date,
            ExplanationType = dto.ExplanationType.Trim(),
            Reason = reason,
            RequestedTimeIn = dto.RequestedTimeIn,
            RequestedTimeOut = dto.RequestedTimeOut,
            AttachmentUrl = dto.AttachmentUrl,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Pending" : dto.Status,
            ApprovedBy = string.IsNullOrWhiteSpace(dto.ApprovedBy) ? null : dto.ApprovedBy.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Attachments = dto.Attachments != null && dto.Attachments.Count > 0
                ? dto.Attachments.Select(a => new AttendanceExplanationAttachment
                {
                    Id = Guid.NewGuid(),
                    ExplanationId = explanationId,
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    FileType = a.FileType,
                    FileSize = a.FileSize,
                    UploadedAt = DateTime.UtcNow
                }).ToList()
                : new List<AttendanceExplanationAttachment>()
        };

        _context.AttendanceExplanations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<AttendanceExplanationDto?> UpdateAsync(Guid id, UpdateAttendanceExplanationDto dto, CancellationToken cancellationToken = default)
    {
        AttendanceExplanation? entity = await _context.AttendanceExplanations
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        if (dto.AttendanceId.HasValue)
        {
            entity.AttendanceId = dto.AttendanceId.Value;
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

        if (!string.IsNullOrWhiteSpace(dto.ExplanationType))
        {
            entity.ExplanationType = dto.ExplanationType.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.Reason))
        {
            entity.Reason = dto.Reason.Trim();
        }

        if (dto.RequestedTimeIn.HasValue)
        {
            entity.RequestedTimeIn = dto.RequestedTimeIn.Value;
        }

        if (dto.RequestedTimeOut.HasValue)
        {
            entity.RequestedTimeOut = dto.RequestedTimeOut.Value;
        }

        if (dto.AttachmentUrl != null)
        {
            entity.AttachmentUrl = dto.AttachmentUrl;
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            entity.Status = dto.Status;
        }

        if (dto.ApprovedBy != null)
        {
            entity.ApprovedBy = string.IsNullOrWhiteSpace(dto.ApprovedBy) ? null : dto.ApprovedBy.Trim();
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
        AttendanceExplanation? entity = await _context.AttendanceExplanations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _context.AttendanceExplanations.Remove(entity);
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

    public static AttendanceExplanationDto MapToDto(AttendanceExplanation entity)
    {
        return new AttendanceExplanationDto
        {
            Id = entity.Id,
            AttendanceId = entity.AttendanceId,
            EmployeeId = entity.EmployeeId,
            Date = entity.Date,
            ExplanationType = entity.ExplanationType,
            Reason = entity.Reason,
            RequestedTimeIn = entity.RequestedTimeIn,
            RequestedTimeOut = entity.RequestedTimeOut,
            AttachmentUrl = entity.AttachmentUrl,
            Status = entity.Status,
            ApprovedBy = entity.ApprovedBy,
            ApprovedAt = entity.ApprovedAt,
            RejectionReason = entity.RejectionReason,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Attachments = entity.Attachments?.Select(a => new AttendanceExplanationAttachmentDto
            {
                Id = a.Id,
                ExplanationId = a.ExplanationId,
                FileName = a.FileName,
                FileUrl = a.FileUrl,
                FileType = a.FileType,
                FileSize = a.FileSize,
                UploadedAt = a.UploadedAt
            }).ToList()
        };
    }
}
