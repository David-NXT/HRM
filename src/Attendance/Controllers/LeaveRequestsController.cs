using Attendance.DTOs.LeaveRequest;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestsController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _leaveRequestService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeaveRequestDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _leaveRequestService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> Create([FromBody] CreateLeaveRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _leaveRequestService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LeaveRequestDto>> Update(Guid id, [FromBody] UpdateLeaveRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _leaveRequestService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _leaveRequestService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
