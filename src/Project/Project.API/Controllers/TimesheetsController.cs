
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
        GetTimesheets(
            [FromQuery] Guid? projectMemberId = null,
            [FromQuery] Guid? projectId = null,
            [FromQuery] Guid? employeeId = null,
            [FromQuery] DateOnly? fromDate = null,
            [FromQuery] DateOnly? toDate = null,
            [FromQuery] string? status = null)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
            return BadRequest("fromDate cannot be later than toDate.");

        if (fromDate == DateOnly.MinValue || toDate == DateOnly.MaxValue)
            return BadRequest("The requested date range is outside the supported range.");

        if (!TryNormalizeStatus(status, out var normalizedStatus))
            return BadRequest("status must be Pending/SUBMITTED, Approved, Rejected, or All.");

        var fromUtc = fromDate.HasValue
            ? ToVietnamDayStartUtc(fromDate.Value)
            : (DateTime?)null;
        var toExclusiveUtc = toDate.HasValue
            ? ToVietnamDayStartUtc(toDate.Value.AddDays(1))
            : (DateTime?)null;

        var timesheets =
            await _timesheetService.GetTimesheetsAsync(
                projectMemberId,
                projectId,
                employeeId,
                fromUtc,
                toExclusiveUtc,
                normalizedStatus);

        return Ok(timesheets);
    }

    [HttpPut("review/bulk-approve")]
    public async Task<ActionResult<BulkApproveTimesheetsResponse>> BulkApproveTimesheets(
        [FromBody] BulkApproveTimesheetsRequest request)
    {
        if (request.ApprovedBy == Guid.Empty)
            return BadRequest("Invalid approver ID.");

        if (request.TimesheetIds is null || request.TimesheetIds.Count is < 1 or > 100)
            return BadRequest("Provide between 1 and 100 timesheet IDs.");

        if (request.TimesheetIds.Any(id => id == Guid.Empty)
            || request.TimesheetIds.Distinct().Count() != request.TimesheetIds.Count)
        {
            return BadRequest("Timesheet IDs must be valid and unique.");
        }

        var response = await _timesheetService.ApproveTimesheetsAsync(
            request.TimesheetIds,
            request.ApprovedBy);
        return Ok(response);
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
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Không thể kiểm tra dữ liệu chấm công. Timesheet chưa được gửi.");
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

                if (reason.Trim().Length > 500)
                    return BadRequest(
                        "Rejection reason cannot exceed 500 characters.");

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
            return Conflict(new { message = ex.Message });
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

    private static bool TryNormalizeStatus(string? status, out string? normalizedStatus)
    {
        normalizedStatus = null;
        if (string.IsNullOrWhiteSpace(status)
            || string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Submitted", StringComparison.OrdinalIgnoreCase))
        {
            normalizedStatus = "Pending";
            return true;
        }

        if (string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase))
        {
            normalizedStatus = "Approved";
            return true;
        }

        if (string.Equals(status, "Rejected", StringComparison.OrdinalIgnoreCase))
        {
            normalizedStatus = "Rejected";
            return true;
        }

        return false;
    }

    private static DateTime ToVietnamDayStartUtc(DateOnly date)
    {
        return DateTime.SpecifyKind(
            date.ToDateTime(TimeOnly.MinValue).AddHours(-7),
            DateTimeKind.Utc);
    }
}
