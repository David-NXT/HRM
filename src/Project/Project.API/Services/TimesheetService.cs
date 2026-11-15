
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
        Guid? projectMemberId = null,
        Guid? projectId = null,
        Guid? employeeId = null,
        DateTime? fromUtc = null,
        DateTime? toExclusiveUtc = null,
        string? status = null)
    {
        var query = _context.Timesheets.AsNoTracking();

        if (projectMemberId.HasValue)
        {
            query = query.Where(t =>
                t.ProjectMemberId == projectMemberId.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(t =>
                t.ProjectMember.ProjectId == projectId.Value);
        }

        if (employeeId.HasValue)
        {
            query = query.Where(t =>
                t.ProjectMember.EmployeeId == employeeId.Value);
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(t => t.WorkDate >= fromUtc.Value);
        }

        if (toExclusiveUtc.HasValue)
        {
            query = query.Where(t => t.WorkDate < toExclusiveUtc.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(t => t.Status == status);
        }

        return await query
            .OrderByDescending(t => t.WorkDate)
            .Select(t => new TimesheetResponseDto
            {
                TimesheetId = t.TimesheetId,
                ProjectMemberId = t.ProjectMemberId,
                ProjectId = t.ProjectMember.ProjectId,
                EmployeeId = t.ProjectMember.EmployeeId,
                TaskId = t.TaskId,
                ProjectName = t.ProjectMember.Project.ProjectName,
                TaskName = t.Task.TaskName,
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
                ProjectId = t.ProjectMember.ProjectId,
                EmployeeId = t.ProjectMember.EmployeeId,
                TaskId = t.TaskId,
                ProjectName = t.ProjectMember.Project.ProjectName,
                TaskName = t.Task.TaskName,
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
        var workDate = DateTimeNormalizer.ToUtc(dto.WorkDate);

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
            WorkDate = workDate,
            HoursWorked = dto.HoursWorked,
            Description = dto.Description
        };

        _context.Timesheets.Add(timesheet);
        await _context.SaveChangesAsync();

        return await GetTimesheetAsync(timesheet.TimesheetId)
            ?? throw new InvalidOperationException("The created timesheet could not be loaded.");
    }
public async Task<bool> UpdateTimesheetAsync(
    Guid id,
    UpdateTimesheetDto dto)
{
    var timesheet = await _context.Timesheets.FindAsync(id);

    if (timesheet == null)
        return false;

    // Chỉ cho phép chỉnh sửa Timesheet Pending
    if (timesheet.Status != "Pending")
        throw new InvalidOperationException(
            "Only pending timesheets can be updated.");

    // Kiểm tra số giờ
    if (dto.HoursWorked <= 0 || dto.HoursWorked > 24)
        throw new ArgumentException(
            "Hours worked must be greater than 0 and not exceed 24.");

    // Cập nhật thông tin
    timesheet.HoursWorked = dto.HoursWorked;
    timesheet.Description = dto.Description;

    await _context.SaveChangesAsync();

    return true;
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

    public async Task<BulkApproveTimesheetsResponse> ApproveTimesheetsAsync(
        IReadOnlyCollection<Guid> timesheetIds,
        Guid approvedBy)
    {
        var response = new BulkApproveTimesheetsResponse();
        foreach (var timesheetId in timesheetIds)
        {
            try
            {
                var approved = await ApproveTimesheetAsync(timesheetId, approvedBy);
                var current = approved
                    ? null
                    : await GetTimesheetAsync(timesheetId);

                response.Results.Add(new TimesheetReviewResultDto
                {
                    TimesheetId = timesheetId,
                    Succeeded = approved,
                    Status = approved ? "Approved" : current?.Status,
                    Message = approved
                        ? null
                        : current is null
                            ? "Timesheet not found."
                            : "Timesheet is no longer pending."
                });
            }
            catch (InvalidOperationException ex)
            {
                var current = await GetTimesheetAsync(timesheetId);
                response.Results.Add(new TimesheetReviewResultDto
                {
                    TimesheetId = timesheetId,
                    Status = current?.Status,
                    Message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                response.Results.Add(new TimesheetReviewResultDto
                {
                    TimesheetId = timesheetId,
                    Message = ex.Message
                });
            }
        }

        return response;
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
