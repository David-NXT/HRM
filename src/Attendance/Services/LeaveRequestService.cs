using Attendance.Data;
using Attendance.DTOs.LeaveRequest;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly AttendanceDbContext _context;
    private readonly IHrEmployeeDirectoryClient _employees;

    public LeaveRequestService(AttendanceDbContext context, IHrEmployeeDirectoryClient employees)
    {
        _context = context;
        _employees = employees;
    }

    public async Task<List<LeaveRequestDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .AsNoTracking()
            .OrderByDescending(x => x.StartDate)
            .Select(x => new LeaveRequestDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                LeaveType = x.LeaveType,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TotalDays = x.TotalDays,
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

    public async Task<LeaveRequestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        LeaveRequest? entity = await _context.LeaveRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return MapToDto(entity);
    }

    public async Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.EmployeeId == Guid.Empty)
        {
            throw new ArgumentException("EmployeeId is required.", nameof(dto));
        }

        await EnsureActiveEmployeeAsync(dto.EmployeeId, cancellationToken);

        if (string.IsNullOrWhiteSpace(dto.LeaveType))
        {
            throw new ArgumentException("LeaveType is required.", nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            throw new ArgumentException("Reason is required.", nameof(dto));
        }

        decimal totalDays = dto.TotalDays > 0 ? dto.TotalDays : 1.0m;

        // Kiểm tra số dư phép nếu là nghỉ phép năm
        if (dto.LeaveType.Contains("phép năm", StringComparison.OrdinalIgnoreCase) ||
            dto.LeaveType.Contains("annual", StringComparison.OrdinalIgnoreCase))
        {
            int year = dto.StartDate.Year;
            LeaveBalance? balance = await _context.LeaveBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == dto.EmployeeId && b.Year == year, cancellationToken);

            if (balance != null && balance.RemainingDays < totalDays)
            {
                throw new InvalidOperationException($"Số ngày phép năm còn lại không đủ (còn {balance.RemainingDays} ngày, yêu cầu {totalDays} ngày).");
            }
        }

        LeaveRequest entity = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            LeaveType = dto.LeaveType.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalDays = totalDays,
            Reason = dto.Reason.Trim(),
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Pending" : dto.Status,
            ApprovedBy = dto.ApprovedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.LeaveRequests.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<LeaveRequestDto?> UpdateAsync(Guid id, UpdateLeaveRequestDto dto, CancellationToken cancellationToken = default)
    {
        LeaveRequest? entity = await _context.LeaveRequests
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

        if (!string.IsNullOrWhiteSpace(dto.LeaveType))
        {
            entity.LeaveType = dto.LeaveType.Trim();
        }

        if (dto.StartDate.HasValue)
        {
            entity.StartDate = dto.StartDate.Value;
        }

        if (dto.EndDate.HasValue)
        {
            entity.EndDate = dto.EndDate.Value;
        }

        if (dto.TotalDays.HasValue)
        {
            entity.TotalDays = dto.TotalDays.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Reason))
        {
            entity.Reason = dto.Reason.Trim();
        }

        bool wasApproved = entity.Status == "Approved";

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            entity.Status = dto.Status;
        }

        bool isNowApproved = entity.Status == "Approved";

        // Khấu trừ / hoàn trả số ngày phép trong LeaveBalance khi trạng thái phê duyệt thay đổi
        if (!wasApproved && isNowApproved)
        {
            int year = entity.StartDate.Year;
            LeaveBalance? balance = await _context.LeaveBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == entity.EmployeeId && b.Year == year, cancellationToken);

            if (balance != null)
            {
                balance.UsedDays += entity.TotalDays;
                balance.RemainingDays = Math.Max(0, balance.RemainingDays - entity.TotalDays);
                balance.UpdatedAt = DateTime.UtcNow;
            }
        }
        else if (wasApproved && (entity.Status == "Rejected" || entity.Status == "Pending"))
        {
            int year = entity.StartDate.Year;
            LeaveBalance? balance = await _context.LeaveBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == entity.EmployeeId && b.Year == year, cancellationToken);

            if (balance != null)
            {
                balance.UsedDays = Math.Max(0, balance.UsedDays - entity.TotalDays);
                balance.RemainingDays += entity.TotalDays;
                balance.UpdatedAt = DateTime.UtcNow;
            }
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
        LeaveRequest? entity = await _context.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _context.LeaveRequests.Remove(entity);
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

    private static LeaveRequestDto MapToDto(LeaveRequest entity)
    {
        return new LeaveRequestDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            LeaveType = entity.LeaveType,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            TotalDays = entity.TotalDays,
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
