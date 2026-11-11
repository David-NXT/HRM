
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

    public async Task<IEnumerable<TimesheetResponseDto>> GetTimesheetsAsync(
        Guid? projectMemberId = null)
    {
        var query = _context.Timesheets.AsNoTracking();

        if (projectMemberId.HasValue)
        {
            query = query.Where(t =>
                t.ProjectMemberId == projectMemberId.Value);
        }

        return await query
            .OrderByDescending(t => t.WorkDate)
            .Select(t => new TimesheetResponseDto
            {
                TimesheetId = t.TimesheetId,
                ProjectMemberId = t.ProjectMemberId,
                TaskId = t.TaskId,
                WorkDate = t.WorkDate,
                HoursWorked = t.HoursWorked,
                Description = t.Description,
                Status = t.Status,
                RejectReason = t.RejectReason,
                ApprovedAt = t.ApprovedAt,
                ApprovedBy = t.ApprovedBy
            })
            .ToListAsync();
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
                RejectReason = t.RejectReason,
                ApprovedAt = t.ApprovedAt,
                ApprovedBy = t.ApprovedBy
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TimesheetResponseDto> CreateTimesheetAsync(
        CreateTimesheetDto dto)
    {
        if (dto.HoursWorked <= 0 || dto.HoursWorked > 24)
            throw new ArgumentException(
                "Hours worked must be greater than 0 and not exceed 24.");

        var member = await _context.ProjectMembers
            .FirstOrDefaultAsync(m =>
                m.ProjectMemberId == dto.ProjectMemberId);

        if (member == null)
            throw new ArgumentException(
                "Project member does not exist.");
        if (member.Status != "Active")
    throw new ArgumentException(
        "Project member is not active.");

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.TaskId == dto.TaskId);

        if (task == null)
            throw new ArgumentException("Task does not exist.");

        if (task.ProjectId != member.ProjectId)
            throw new ArgumentException(
                "Task and project member must belong to the same project.");

        var timesheet = new Timesheet
        {
            TimesheetId = Guid.NewGuid(),
            ProjectMemberId = dto.ProjectMemberId,
            TaskId = dto.TaskId,
            WorkDate = dto.WorkDate,
            HoursWorked = dto.HoursWorked,
            Description = dto.Description
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
            RejectReason = timesheet.RejectReason,
            ApprovedAt = timesheet.ApprovedAt,
            ApprovedBy = timesheet.ApprovedBy
        };
    }

    public async Task<bool> ApproveTimesheetAsync(
        Guid id,
        Guid approvedBy)
    {
        var timesheet = await _context.Timesheets.FindAsync(id);

        if (timesheet == null)
            return false;

        timesheet.Approve(approvedBy);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectTimesheetAsync(
        Guid id,
        Guid approvedBy,
        string reason)
    {
        var timesheet = await _context.Timesheets.FindAsync(id);

        if (timesheet == null)
            return false;

        timesheet.Reject(approvedBy, reason);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResubmitTimesheetAsync(
        Guid id,
        decimal hoursWorked,
        string? description)
    {
        var timesheet = await _context.Timesheets.FindAsync(id);

        if (timesheet == null)
            return false;

        timesheet.Resubmit(hoursWorked, description);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTimesheetAsync(Guid id)
    {
        var timesheet = await _context.Timesheets.FindAsync(id);

        if (timesheet == null)
            return false;

        if (timesheet.Status == "Approved")
            throw new InvalidOperationException(
                "Cannot delete an approved timesheet.");

        _context.Timesheets.Remove(timesheet);

        await _context.SaveChangesAsync();
        return true;
    }
}
