using Attendance.DTOs.ShiftAssignment;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftAssignmentsController : ControllerBase
{
    private readonly IShiftAssignmentService _shiftAssignmentService;

    public ShiftAssignmentsController(IShiftAssignmentService shiftAssignmentService)
    {
        _shiftAssignmentService = shiftAssignmentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShiftAssignmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _shiftAssignmentService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShiftAssignmentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _shiftAssignmentService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ShiftAssignmentDto>> Create([FromBody] CreateShiftAssignmentDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _shiftAssignmentService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ShiftAssignmentDto>> Update(Guid id, [FromBody] UpdateShiftAssignmentDto dto, CancellationToken cancellationToken)
    {
        var result = await _shiftAssignmentService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _shiftAssignmentService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
