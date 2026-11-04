using Attendance.DTOs.WorkShift;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkShiftsController : ControllerBase
{
    private readonly IWorkShiftService _workShiftService;

    public WorkShiftsController(IWorkShiftService workShiftService)
    {
        _workShiftService = workShiftService;
    }

    [HttpGet]
    public async Task<ActionResult<List<WorkShiftDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _workShiftService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkShiftDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _workShiftService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<WorkShiftDto>> Create([FromBody] CreateWorkShiftDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workShiftService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<WorkShiftDto>> Update(Guid id, [FromBody] UpdateWorkShiftDto dto, CancellationToken cancellationToken)
    {
        var result = await _workShiftService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _workShiftService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
