using Attendance.DTOs.AttendanceRecord;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AttendanceRecordDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _attendanceService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AttendanceRecordDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _attendanceService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceRecordDto>> Create([FromBody] CreateAttendanceRecordDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _attendanceService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AttendanceRecordDto>> Update(Guid id, [FromBody] UpdateAttendanceRecordDto dto, CancellationToken cancellationToken)
    {
        var result = await _attendanceService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _attendanceService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
