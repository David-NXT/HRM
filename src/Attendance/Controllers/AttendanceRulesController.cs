using Attendance.DTOs.AttendanceRule;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceRulesController : ControllerBase
{
    private readonly IAttendanceRuleService _attendanceRuleService;

    public AttendanceRulesController(IAttendanceRuleService attendanceRuleService)
    {
        _attendanceRuleService = attendanceRuleService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AttendanceRuleDto>>> GetAll([FromQuery] bool? isActive, CancellationToken cancellationToken)
    {
        List<AttendanceRuleDto> result = await _attendanceRuleService.GetAllAsync(isActive, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AttendanceRuleDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        AttendanceRuleDto? result = await _attendanceRuleService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<AttendanceRuleDto>> GetActiveRule(CancellationToken cancellationToken)
    {
        AttendanceRuleDto? result = await _attendanceRuleService.GetActiveRuleAsync(cancellationToken);
        if (result == null)
        {
            return NotFound(new { message = "Chưa có quy định chấm công nào đang hoạt động." });
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceRuleDto>> Create([FromBody] CreateAttendanceRuleDto dto, CancellationToken cancellationToken)
    {
        try
        {
            AttendanceRuleDto result = await _attendanceRuleService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AttendanceRuleDto>> Update(Guid id, [FromBody] UpdateAttendanceRuleDto dto, CancellationToken cancellationToken)
    {
        AttendanceRuleDto? result = await _attendanceRuleService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        bool deleted = await _attendanceRuleService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
