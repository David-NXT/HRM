using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollDataController : ControllerBase
{
    private readonly ProjectManagementDbContext _context;

    public PayrollDataController(ProjectManagementDbContext context)
    {
        _context = context;
    }

    // Lấy toàn bộ dữ liệu timesheet đã duyệt
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PayrollDataDto>>> GetPayrollData(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var query = _context.Timesheets
            .AsNoTracking()
            .Include(t => t.ProjectMember)
            .Where(t => t.Status == "Approved");

        if (fromDate.HasValue)
        {
            query = query.Where(t => t.WorkDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(t => t.WorkDate <= toDate.Value);
        }

        var timesheets = await query.ToListAsync();

        var result = new List<PayrollDataDto>();

        foreach (var timesheet in timesheets)
        {
            var hourlyRate = await _context.HourlyRates
                .Where(r =>
                    r.ProjectMemberId == timesheet.ProjectMemberId &&
                    r.EffectiveFrom <= timesheet.WorkDate &&
                    (r.EffectiveTo == null ||
                     r.EffectiveTo >= timesheet.WorkDate))
                .OrderByDescending(r => r.EffectiveFrom)
                .Select(r => (decimal?)r.Rate)
                .FirstOrDefaultAsync();

            // Không có mức lương áp dụng thì bỏ qua bản ghi
            if (!hourlyRate.HasValue)
            {
                continue;
            }

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

        return Ok(result);
    }

    // Lấy dữ liệu tính lương theo nhân viên
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<IEnumerable<PayrollDataDto>>> GetPayrollDataByEmployee(
        Guid employeeId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var query = _context.Timesheets
            .AsNoTracking()
            .Include(t => t.ProjectMember)
            .Where(t =>
                t.Status == "Approved" &&
                t.ProjectMember.EmployeeId == employeeId);

        if (fromDate.HasValue)
        {
            query = query.Where(t => t.WorkDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(t => t.WorkDate <= toDate.Value);
        }

        var timesheets = await query.ToListAsync();

        var result = new List<PayrollDataDto>();

        foreach (var timesheet in timesheets)
        {
            var hourlyRate = await _context.HourlyRates
                .Where(r =>
                    r.ProjectMemberId == timesheet.ProjectMemberId &&
                    r.EffectiveFrom <= timesheet.WorkDate &&
                    (r.EffectiveTo == null ||
                     r.EffectiveTo >= timesheet.WorkDate))
                .OrderByDescending(r => r.EffectiveFrom)
                .Select(r => (decimal?)r.Rate)
                .FirstOrDefaultAsync();

            if (!hourlyRate.HasValue)
            {
                continue;
            }

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

        return Ok(result);
    }
}