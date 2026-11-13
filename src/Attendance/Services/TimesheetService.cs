using Attendance.Data;
using Attendance.DTOs.TimesheetPeriod;
using Attendance.DTOs.TimesheetSummary;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class TimesheetService : ITimesheetService
{
    private readonly AttendanceDbContext _dbContext;

    public TimesheetService(AttendanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TimesheetPeriodDto>> GetPeriodsAsync(int? year = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.TimesheetPeriods.AsNoTracking();

        if (year.HasValue)
        {
            query = query.Where(p => p.Year == year.Value);
        }

        var periods = await query
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync(cancellationToken);

        return periods.Select(MapToPeriodDto).ToList();
    }

    public async Task<TimesheetPeriodDto?> GetPeriodByIdAsync(Guid periodId, CancellationToken cancellationToken = default)
    {
        var period = await _dbContext.TimesheetPeriods
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == periodId, cancellationToken);

        return period == null ? null : MapToPeriodDto(period);
    }

    public async Task<TimesheetPeriodDto> CreatePeriodAsync(CreateTimesheetPeriodDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.TimesheetPeriods
            .AnyAsync(p => p.Month == dto.Month && p.Year == dto.Year, cancellationToken);

        if (existing)
        {
            throw new ArgumentException($"Kỳ công cho tháng {dto.Month}/{dto.Year} đã tồn tại.");
        }

        var period = new TimesheetPeriod
        {
            Id = Guid.NewGuid(),
            Name = string.IsNullOrWhiteSpace(dto.Name) ? $"Kỳ công Tháng {dto.Month:D2}/{dto.Year}" : dto.Name,
            Month = dto.Month,
            Year = dto.Year,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = "OPEN",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.TimesheetPeriods.Add(period);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToPeriodDto(period);
    }

    public async Task<TimesheetPeriodDto?> LockPeriodAsync(Guid periodId, string lockedBy, CancellationToken cancellationToken = default)
    {
        var period = await _dbContext.TimesheetPeriods.FindAsync([periodId], cancellationToken);
        if (period == null)
        {
            return null;
        }

        period.Status = "LOCKED";
        period.LockedAt = DateTime.UtcNow;
        period.LockedBy = string.IsNullOrWhiteSpace(lockedBy) ? "System" : lockedBy;
        period.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToPeriodDto(period);
    }

    public async Task<List<TimesheetSummaryDto>> GetSummariesByPeriodAsync(Guid periodId, CancellationToken cancellationToken = default)
    {
        var summaries = await _dbContext.TimesheetSummaries
            .AsNoTracking()
            .Where(s => s.PeriodId == periodId)
            .ToListAsync(cancellationToken);

        return summaries.Select(MapToSummaryDto).ToList();
    }

    public async Task<EmployeeMonthlyTimesheetDto?> GetEmployeeSummaryAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
    {
        var summary = await _dbContext.TimesheetSummaries
            .Include(s => s.Period)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.Period != null && s.Period.Month == month && s.Period.Year == year, cancellationToken);

        if (summary == null)
        {
            return null;
        }

        return MapToEmployeeMonthlyDto(summary);
    }

    public async Task<EmployeeMonthlyTimesheetDto?> GetEmployeeSummaryByPeriodAsync(Guid periodId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        var summary = await _dbContext.TimesheetSummaries
            .Include(s => s.Period)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.PeriodId == periodId && s.EmployeeId == employeeId, cancellationToken);

        if (summary == null)
        {
            return null;
        }

        return MapToEmployeeMonthlyDto(summary);
    }

    public async Task<TimesheetSummaryDto> GenerateOrRecalculateSummaryAsync(Guid periodId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        var period = await _dbContext.TimesheetPeriods.FindAsync([periodId], cancellationToken);
        if (period == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy kỳ công với Id: {periodId}");
        }

        if (string.Equals(period.Status, "LOCKED", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Kỳ công {period.Name} đã bị khóa, không thể tổng hợp lại.");
        }

        // 1. Quét dữ liệu quẹt thẻ thực tế (AttendanceRecords)
        var records = await _dbContext.AttendanceRecords
            .AsNoTracking()
            .Where(r => r.EmployeeId == employeeId && r.Date >= period.StartDate && r.Date <= period.EndDate)
            .ToListAsync(cancellationToken);

        var actualWorkingDays = records.Count(r => r.WorkHour > 0);
        var totalWorkingHours = records.Sum(r => r.WorkHour);
        var lateOccurrences = records.Count(r => r.IsLate);
        var totalLateMinutes = records.Sum(r => r.LateMins);
        var earlyLeaveOccurrences = records.Count(r => r.IsEarlyLeave);
        var totalEarlyLeaveMinutes = records.Sum(r => r.EarlyLeaveMins);

        // 2. Quét đơn nghỉ phép đã duyệt (LeaveRequests)
        var leaves = await _dbContext.LeaveRequests
            .AsNoTracking()
            .Where(l => l.EmployeeId == employeeId 
                        && l.Status == "Approved" 
                        && l.StartDate <= period.EndDate 
                        && l.EndDate >= period.StartDate)
            .ToListAsync(cancellationToken);

        decimal paidLeaveDays = 0;
        decimal unpaidLeaveDays = 0;

        foreach (var leave in leaves)
        {
            var type = leave.LeaveType?.ToLowerInvariant() ?? string.Empty;
            if (type.Contains("unpaid") || type.Contains("không lương"))
            {
                unpaidLeaveDays += leave.TotalDays;
            }
            else
            {
                paidLeaveDays += leave.TotalDays;
            }
        }

        // 3. Quét đơn làm thêm đã duyệt (OTRequests)
        var ots = await _dbContext.OTRequests
            .AsNoTracking()
            .Where(o => o.EmployeeId == employeeId
                        && o.Status == "Approved"
                        && o.Date >= period.StartDate
                        && o.Date <= period.EndDate)
            .ToListAsync(cancellationToken);

        decimal totalOTHours = ots.Sum(o => o.ActualHours ?? o.PlannedHours);

        // 4. Tính số ngày công chuẩn trong tháng (loại trừ Thứ 7, Chủ Nhật)
        decimal standardWorkingDays = 0;
        for (var date = period.StartDate; date <= period.EndDate; date = date.AddDays(1))
        {
            var dow = date.DayOfWeek;
            if (dow != DayOfWeek.Saturday && dow != DayOfWeek.Sunday)
            {
                standardWorkingDays++;
            }
        }

        // 5. Tìm hoặc tạo mới TimesheetSummary
        var summary = await _dbContext.TimesheetSummaries
            .FirstOrDefaultAsync(s => s.PeriodId == periodId && s.EmployeeId == employeeId, cancellationToken);

        if (summary == null)
        {
            summary = new TimesheetSummary
            {
                Id = Guid.NewGuid(),
                PeriodId = periodId,
                EmployeeId = employeeId,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.TimesheetSummaries.Add(summary);
        }
        else
        {
            summary.UpdatedAt = DateTime.UtcNow;
        }

        summary.StandardWorkingDays = standardWorkingDays;
        summary.ActualWorkingDays = actualWorkingDays;
        summary.TotalWorkingHours = totalWorkingHours;
        summary.PaidLeaveDays = paidLeaveDays;
        summary.UnpaidLeaveDays = unpaidLeaveDays;
        summary.LateOccurrences = lateOccurrences;
        summary.TotalLateMinutes = totalLateMinutes;
        summary.EarlyLeaveOccurrences = earlyLeaveOccurrences;
        summary.TotalEarlyLeaveMinutes = totalEarlyLeaveMinutes;
        summary.OTHours = totalOTHours;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToSummaryDto(summary);
    }

    private static TimesheetPeriodDto MapToPeriodDto(TimesheetPeriod period) => new()
    {
        Id = period.Id,
        Name = period.Name,
        Month = period.Month,
        Year = period.Year,
        StartDate = period.StartDate,
        EndDate = period.EndDate,
        Status = period.Status,
        LockedAt = period.LockedAt,
        LockedBy = period.LockedBy,
        CreatedAt = period.CreatedAt,
        UpdatedAt = period.UpdatedAt
    };

    private static TimesheetSummaryDto MapToSummaryDto(TimesheetSummary summary) => new()
    {
        Id = summary.Id,
        PeriodId = summary.PeriodId,
        EmployeeId = summary.EmployeeId,
        StandardWorkingDays = summary.StandardWorkingDays,
        ActualWorkingDays = summary.ActualWorkingDays,
        TotalWorkingHours = summary.TotalWorkingHours,
        PaidLeaveDays = summary.PaidLeaveDays,
        UnpaidLeaveDays = summary.UnpaidLeaveDays,
        TotalLateMinutes = summary.TotalLateMinutes,
        LateOccurrences = summary.LateOccurrences,
        TotalEarlyLeaveMinutes = summary.TotalEarlyLeaveMinutes,
        EarlyLeaveOccurrences = summary.EarlyLeaveOccurrences,
        OTHours = summary.OTHours,
        CreatedAt = summary.CreatedAt,
        UpdatedAt = summary.UpdatedAt
    };

    private static EmployeeMonthlyTimesheetDto MapToEmployeeMonthlyDto(TimesheetSummary summary) => new()
    {
        SummaryId = summary.Id,
        EmployeeId = summary.EmployeeId,
        PeriodId = summary.PeriodId,
        PeriodName = summary.Period?.Name ?? string.Empty,
        Month = summary.Period?.Month ?? 0,
        Year = summary.Period?.Year ?? 0,
        StartDate = summary.Period?.StartDate ?? DateOnly.MinValue,
        EndDate = summary.Period?.EndDate ?? DateOnly.MinValue,
        PeriodStatus = summary.Period?.Status ?? string.Empty,
        StandardWorkingDays = summary.StandardWorkingDays,
        ActualWorkingDays = summary.ActualWorkingDays,
        TotalWorkingHours = summary.TotalWorkingHours,
        PaidLeaveDays = summary.PaidLeaveDays,
        UnpaidLeaveDays = summary.UnpaidLeaveDays,
        TotalLateMinutes = summary.TotalLateMinutes,
        LateOccurrences = summary.LateOccurrences,
        TotalEarlyLeaveMinutes = summary.TotalEarlyLeaveMinutes,
        EarlyLeaveOccurrences = summary.EarlyLeaveOccurrences,
        OTHours = summary.OTHours,
        CreatedAt = summary.CreatedAt,
        UpdatedAt = summary.UpdatedAt
    };
}
