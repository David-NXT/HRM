
using Microsoft.AspNetCore.Mvc;
using Project.API.DTOs;
using Project.API.Services;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimesheetsController : ControllerBase
{
    private readonly ITimesheetService _timesheetService;

    public TimesheetsController(ITimesheetService timesheetService)
    {
        _timesheetService = timesheetService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimesheetResponseDto>>>
        GetTimesheets([FromQuery] Guid? projectMemberId = null)
    {
        var timesheets =
            await _timesheetService.GetTimesheetsAsync(projectMemberId);

        return Ok(timesheets);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TimesheetResponseDto>>
        GetTimesheet(Guid id)
    {
        var timesheet =
            await _timesheetService.GetTimesheetAsync(id);

        if (timesheet == null)
            return NotFound();

        return Ok(timesheet);
    }

    [HttpPost]
    public async Task<ActionResult<TimesheetResponseDto>>
        CreateTimesheet([FromBody] CreateTimesheetDto dto)
    {
        
        try
        {
            
            var result =
                await _timesheetService.CreateTimesheetAsync(dto);

            return CreatedAtAction(
                nameof(GetTimesheet),
                new { id = result.TimesheetId },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
[HttpPut("{id:guid}")]
public async Task<IActionResult> UpdateTimesheet(
    Guid id,
    [FromBody] UpdateTimesheetDto dto)
{
    try
    {
        var result = await _timesheetService
            .UpdateTimesheetAsync(id, dto);

        if (!result)
            return NotFound("Timesheet not found.");

        return NoContent();
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ex.Message);
    }
}
    // Duyệt hoặc từ chối Timesheet
    [HttpPut("{id:guid}/review")]
    public async Task<IActionResult> ReviewTimesheet(
        Guid id,
        [FromQuery] bool approved,
        [FromQuery] Guid approvedBy,
        [FromQuery] string? reason = null)
    {
        if (approvedBy == Guid.Empty)
            return BadRequest("Invalid approver ID.");
        try
        {
            bool result;

            if (approved)
            {
                result = await _timesheetService
                    .ApproveTimesheetAsync(id, approvedBy);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(reason))
                    return BadRequest(
                        "Rejection reason is required.");

                result = await _timesheetService
                    .RejectTimesheetAsync(id, approvedBy, reason);
            }

            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Gửi lại Timesheet sau khi bị từ chối
    [HttpPut("{id:guid}/resubmit")]
    public async Task<IActionResult> ResubmitTimesheet(
        Guid id,
        [FromBody] UpdateTimesheetDto dto)
    {
        try
        {
            var result = await _timesheetService
                .ResubmitTimesheetAsync(
                    id,
                    dto.HoursWorked,
                    dto.Description);

            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTimesheet(Guid id)
    {
        try
        {
            var result =
                await _timesheetService.DeleteTimesheetAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
