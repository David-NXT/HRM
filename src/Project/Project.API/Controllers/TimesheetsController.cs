using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;
using Project.API.Entities;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimesheetsController : ControllerBase
{
    private readonly ProjectManagementDbContext _context;

    public TimesheetsController(ProjectManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimesheetResponseDto>>>
        GetTimesheets()
    {
        var timesheets = await _context.Timesheets
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
            .ToListAsync();

        return Ok(timesheets);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TimesheetResponseDto>>
        GetTimesheet(Guid id)
    {
        var timesheet = await _context.Timesheets
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

        if (timesheet == null)
            return NotFound();

        return Ok(timesheet);
    }

    [HttpPost]
    public async Task<ActionResult<TimesheetResponseDto>>
        CreateTimesheet(CreateTimesheetDto dto)
    {
        var member = await _context.ProjectMembers
            .FirstOrDefaultAsync(m =>
                m.ProjectMemberId == dto.ProjectMemberId);

        if (member == null)
            return BadRequest("Project member does not exist.");

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.TaskId == dto.TaskId);

        if (task == null)
            return BadRequest("Task does not exist.");

        if (task.ProjectId != member.ProjectId)
            return BadRequest(
                "Task and project member must belong to the same project.");

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

        var response = new TimesheetResponseDto
        {
            TimesheetId = timesheet.TimesheetId,
            ProjectMemberId = timesheet.ProjectMemberId,
            TaskId = timesheet.TaskId,
            WorkDate = timesheet.WorkDate,
            HoursWorked = timesheet.HoursWorked,
            Description = timesheet.Description,
            Status = timesheet.Status
        };

        return CreatedAtAction(
            nameof(GetTimesheet),
            new { id = timesheet.TimesheetId },
            response);
    }

    // Duyệt hoặc từ chối timesheet
    [HttpPut("{id:guid}/review")]
    public async Task<IActionResult> ReviewTimesheet(
        Guid id,
        [FromQuery] bool approved,
        [FromQuery] Guid approvedBy)
    {
        var timesheet = await _context.Timesheets.FindAsync(id);

        if (timesheet == null)
            return NotFound();

        if (timesheet.Status != "Pending")
            return BadRequest("Timesheet has already been reviewed.");

        timesheet.Status = approved ? "Approved" : "Rejected";
        timesheet.ApprovedAt = DateTime.UtcNow;
        timesheet.ApprovedBy = approved ? approvedBy : null;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTimesheet(Guid id)
    {
        var timesheet = await _context.Timesheets.FindAsync(id);

        if (timesheet == null)
            return NotFound();

        if (timesheet.Status == "Approved")
            return BadRequest(
                "Cannot delete an approved timesheet.");

        _context.Timesheets.Remove(timesheet);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}