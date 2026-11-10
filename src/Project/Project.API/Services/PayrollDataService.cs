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

    public async Task<IEnumerable<PayrollDataDto>> GetApprovedPayrollDataAsync(
        DateTime from,
        DateTime to)
    {
        var timesheets = await _context.Timesheets
            .AsNoTracking()
            .Include(t => t.ProjectMember)
            .Where(t => t.Status == "Approved"
                && t.WorkDate >= from
                && t.WorkDate <= to)
            .ToListAsync();

        var result = new List<PayrollDataDto>();

        foreach (var timesheet in timesheets)
        {
            var hourlyRate = await _context.HourlyRates
                .Where(r => r.ProjectMemberId == timesheet.ProjectMemberId
                    && r.EffectiveFrom <= timesheet.WorkDate
                    && (r.EffectiveTo == null
                        || r.EffectiveTo >= timesheet.WorkDate))
                .OrderByDescending(r => r.EffectiveFrom)
                .Select(r => (decimal?)r.Rate)
                .FirstOrDefaultAsync();

            if (hourlyRate == null)
                continue;

            result.Add(new PayrollDataDto
            {
                TimesheetId = timesheet.TimesheetId,
                EmployeeId = timesheet.ProjectMember.EmployeeId,
                ProjectId = timesheet.ProjectMember.ProjectId,
                TaskId = timesheet.TaskId,
                WorkDate = timesheet.WorkDate,
                HoursWorked = timesheet.HoursWorked,
                HourlyRate = hourlyRate.Value
            });
        }

        return result;
    }
}