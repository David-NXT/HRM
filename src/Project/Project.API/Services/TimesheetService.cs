using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;
using Project.API.Entities;

namespace Project.API.Services;

public class TimesheetService : ITimesheetService
{
    private readonly ProjectManagementDbContext _context;

    public TimesheetService(ProjectManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TimesheetResponseDto>> GetTimesheetsAsync(Guid? projectMemberId = null)
    {
        var query = _context.Timesheets.AsNoTracking();

        if (projectMemberId.HasValue)
            query = query.Where(t => t.ProjectMemberId == projectMemberId.Value);

        return await query.Select(t => new TimesheetResponseDto
        {
            TimesheetId = t.TimesheetId,
            ProjectMemberId = t.ProjectMemberId,
            TaskId = t.TaskId,
            WorkDate = t.WorkDate,
            HoursWorked = t.HoursWorked,
            Description = t.Description,
            Status = t.Status,
            ApprovedAt = t.ApprovedAt,
            ApprovedBy = t.ApprovedBy
        }).ToListAsync();
    }

    public async Task<TimesheetResponseDto?> GetTimesheetAsync(Guid id)
    {
        return await _context.Timesheets
            .AsNoTracking()
            .Where(t => t.TimesheetId == id)
            .Select(t => new TimesheetResponseDto
            {
                TimesheetId = t.TimesheetId,
                ProjectMemberId = t.ProjectMemberId,
                TaskId = t.TaskId,
                WorkDate = t.WorkDate,
                HoursWorked = t.HoursWorked,
                Description = t.Description,
                Status = t.Status,
                ApprovedAt = t.ApprovedAt,
                ApprovedBy = t.ApprovedBy
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TimesheetResponseDto> CreateTimesheetAsync(CreateTimesheetDto dto)
    {
        var timesheet = new Timesheet
        {
            TimesheetId = Guid.NewGuid(),
            ProjectMemberId = dto.ProjectMemberId,
            TaskId = dto.TaskId,
            WorkDate = dto.WorkDate,
            HoursWorked = dto.HoursWorked,
            Description = dto.Description,
            Status = "Pending"
        };

        _context.Timesheets.Add(timesheet);
        await _context.SaveChangesAsync();

        return new TimesheetResponseDto
        {
            TimesheetId = timesheet.TimesheetId,
            ProjectMemberId = timesheet.ProjectMemberId,
            TaskId = timesheet.TaskId,
            WorkDate = timesheet.WorkDate,
            HoursWorked = timesheet.HoursWorked,
            Description = timesheet.Description,
            Status = timesheet.Status,
            ApprovedAt = timesheet.ApprovedAt,
            ApprovedBy = timesheet.ApprovedBy
        };
    }

    public async Task<bool> ApproveTimesheetAsync(Guid id, Guid approvedBy)
    {
        var timesheet = await _context.Timesheets.FindAsync(id);

        if (timesheet == null)
            return false;

        timesheet.Status = "Approved";
        timesheet.ApprovedAt = DateTime.UtcNow;
        timesheet.ApprovedBy = approvedBy;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectTimesheetAsync(Guid id)
    {
        var timesheet = await _context.Timesheets.FindAsync(id);

        if (timesheet == null)
            return false;

        timesheet.Status = "Rejected";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTimesheetAsync(Guid id)
    {
        var timesheet = await _context.Timesheets.FindAsync(id);

        if (timesheet == null)
            return false;

        _context.Timesheets.Remove(timesheet);
        await _context.SaveChangesAsync();

        return true;
    }
}