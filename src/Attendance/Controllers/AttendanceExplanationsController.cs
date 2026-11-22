using System.Runtime.InteropServices;
using Attendance.DTOs.AttendanceExplanation;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceExplanationsController : ControllerBase
{
    private readonly IAttendanceExplanationsService _attendanceExplanationsService;

    public AttendanceExplanationsController(IAttendanceExplanationsService attendanceExplanationsService)
    {
        _attendanceExplanationsService = attendanceExplanationsService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AttendanceExplanationDto>>> GetAll(CancellationToken cancellationToken)
    {
        List<AttendanceExplanationDto> result = await _attendanceExplanationsService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AttendanceExplanationDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        AttendanceExplanationDto? result = await _attendanceExplanationsService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceExplanationDto>> Create([FromBody] CreateAttendanceExplanationDto dto, CancellationToken cancellationToken)
    {
        try
        {
            AttendanceExplanationDto result = await _attendanceExplanationsService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AttendanceExplanationDto>> Update(Guid id, [FromBody] UpdateAttendanceExplanationDto dto, CancellationToken cancellationToken)
    {
        AttendanceExplanationDto? result = await _attendanceExplanationsService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        bool deleted = await _attendanceExplanationsService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}