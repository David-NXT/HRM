
using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;

namespace Project.API.Services;

public class PayrollDataService : IPayrollDataService
{
    private readonly ProjectManagementDbContext _context;

    public PayrollDataService(ProjectManagementDbContext context)
    {
        _context = context;
    }

    // Lấy chi tiết Timesheet đã duyệt và tính phụ cấp
    public async Task<IEnumerable<PayrollDataDto>>
        GetApprovedPayrollDataAsync(
            DateTime fromUtc,
            DateTime toExclusiveUtc,
            Guid? employeeId = null)
    {
        var query = _context.Timesheets
            .AsNoTracking()
            .Include(t => t.ProjectMember)
                .ThenInclude(pm => pm.ProjectRole)
            .Include(t => t.ProjectMember)
                .ThenInclude(pm => pm.HourlyRates)
            .Where(t =>
                t.Status == "Approved" &&
                t.WorkDate >= fromUtc &&
                t.WorkDate < toExclusiveUtc);

        if (employeeId.HasValue)
        {
            query = query.Where(t =>
                t.ProjectMember.EmployeeId == employeeId.Value);
        }

        var timesheets = await query.ToListAsync();

        var result = new List<PayrollDataDto>();

        foreach (var timesheet in timesheets)
        {
            // Tìm HourlyRate có hiệu lực tại ngày làm việc
            var applicableRates = timesheet.ProjectMember.HourlyRates
    .Where(r =>
        r.EffectiveFrom <= timesheet.WorkDate &&
        (r.EffectiveTo == null ||
         r.EffectiveTo >= timesheet.WorkDate))
    .OrderByDescending(r => r.EffectiveFrom)
    .ToList();

// Kiểm tra có mức lương phù hợp không
        if (applicableRates.Count == 0)
        {
            throw new InvalidOperationException(
                $"Timesheet {timesheet.TimesheetId} does not have an applicable hourly rate.");
        }

        // Không cho phép nhiều mức lương cùng hiệu lực
        if (applicableRates.Count > 1)
        {
            throw new InvalidOperationException(
                $"Multiple applicable hourly rates found for Timesheet {timesheet.TimesheetId}.");
        }

        var hourlyRate = applicableRates[0];

        // Kiểm tra mức lương hợp lệ
        if (hourlyRate.Rate <= 0)
        {
            throw new InvalidOperationException(
                $"Invalid hourly rate for Timesheet {timesheet.TimesheetId}.");
        }
            // Lấy hệ số vai trò
            var roleCoefficient =
                timesheet.ProjectMember.ProjectRole.RoleCoefficient;

            if (roleCoefficient < 0)
            {
                throw new InvalidOperationException(
                    $"Invalid RoleCoefficient for Timesheet " +
                    $"{timesheet.TimesheetId}.");
            }

            result.Add(new PayrollDataDto
            {
                TimesheetId = timesheet.TimesheetId,
                EmployeeId = timesheet.ProjectMember.EmployeeId,
                ProjectId = timesheet.ProjectMember.ProjectId,
                TaskId = timesheet.TaskId,
                WorkDate = timesheet.WorkDate,
                HoursWorked = timesheet.HoursWorked,
                HourlyRate = hourlyRate.Rate,
                RoleCoefficient = roleCoefficient
            });
        }

        return result;
    }

    // Tổng hợp phụ cấp theo nhân viên và dự án
    public async Task<IEnumerable<PayrollProjectSummaryDto>>
        GetProjectAllowanceSummaryAsync(
            DateTime fromUtc,
            DateTime toExclusiveUtc,
            DateTime fromDate,
            DateTime toDate,
            Guid? employeeId = null)
    {
        var details = await GetApprovedPayrollDataAsync(
            fromUtc,
            toExclusiveUtc,
            employeeId);

        
        var summaries = details
            .GroupBy(x => new
            {
                x.EmployeeId,
                x.ProjectId
            })
            .Select(group => new PayrollProjectSummaryDto
            {
                EmployeeId = group.Key.EmployeeId,
                ProjectId = group.Key.ProjectId,

                FromDate = fromDate,
                ToDate = toDate,

                TimesheetCount = group.Count(),

                TotalApprovedHours = group.Sum(x => x.HoursWorked),

                TotalProjectIncome =
                    group.Sum(x => x.TotalProjectIncome)
            })
            .ToList();


        return summaries;
    }
}
