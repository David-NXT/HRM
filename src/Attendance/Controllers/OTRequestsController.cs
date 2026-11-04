using Attendance.DTOs.OTRequest;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OTRequestsController : ControllerBase
{
    private readonly IOTRequestService _otRequestService;

    public OTRequestsController(IOTRequestService otRequestService)
    {
        _otRequestService = otRequestService;
    }

    [HttpGet]
    public async Task<ActionResult<List<OTRequestDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _otRequestService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OTRequestDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _otRequestService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OTRequestDto>> Create([FromBody] CreateOTRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _otRequestService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OTRequestDto>> Update(Guid id, [FromBody] UpdateOTRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _otRequestService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _otRequestService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
