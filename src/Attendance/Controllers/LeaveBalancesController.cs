using Attendance.DTOs.LeaveBalance;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveBalancesController : ControllerBase
{
    private readonly ILeaveBalanceService _leaveBalanceService;

    public LeaveBalancesController(ILeaveBalanceService leaveBalanceService)
    {
        _leaveBalanceService = leaveBalanceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaveBalanceDto>>> GetAll([FromQuery] int? year, CancellationToken cancellationToken)
    {
        var result = await _leaveBalanceService.GetAllAsync(year, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeaveBalanceDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _leaveBalanceService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<LeaveBalanceDto>> GetByEmployee(Guid employeeId, [FromQuery] int year, CancellationToken cancellationToken)
    {
        var result = await _leaveBalanceService.GetByEmployeeAsync(employeeId, year, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveBalanceDto>> Create([FromBody] CreateLeaveBalanceDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _leaveBalanceService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LeaveBalanceDto>> Update(Guid id, [FromBody] UpdateLeaveBalanceDto dto, CancellationToken cancellationToken)
    {
        var result = await _leaveBalanceService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _leaveBalanceService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
