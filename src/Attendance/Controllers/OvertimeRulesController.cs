using Attendance.DTOs.OvertimeRule;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OvertimeRulesController : ControllerBase
{
    private readonly IOvertimeRuleService _overtimeRuleService;

    public OvertimeRulesController(IOvertimeRuleService overtimeRuleService)
    {
        _overtimeRuleService = overtimeRuleService;
    }

    [HttpGet]
    public async Task<ActionResult<List<OvertimeRuleDto>>> GetAll([FromQuery] bool? isActive, CancellationToken cancellationToken)
    {
        var result = await _overtimeRuleService.GetAllAsync(isActive, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OvertimeRuleDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _overtimeRuleService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }


    [HttpGet("multiplier")]
    public async Task<IActionResult> GetOvertimeMultiplier(
        CancellationToken cancellationToken)
    {
        try
        {
            var multiplier = await _overtimeRuleService
                .GetOvertimeMultiplierAsync(cancellationToken);
    
            return Ok(new
            {
                multiplier
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<OvertimeRuleDto>> Create([FromBody] CreateOvertimeRuleDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _overtimeRuleService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OvertimeRuleDto>> Update(Guid id, [FromBody] UpdateOvertimeRuleDto dto, CancellationToken cancellationToken)
    {
        var result = await _overtimeRuleService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _overtimeRuleService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
