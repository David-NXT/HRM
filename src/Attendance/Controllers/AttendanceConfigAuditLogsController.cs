using Attendance.DTOs.AttendanceConfigAuditLog;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceConfigAuditLogsController : ControllerBase
{
    private readonly IAttendanceConfigAuditLogService _auditLogService;

    public AttendanceConfigAuditLogsController(IAttendanceConfigAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AttendanceConfigAuditLogDto>>> GetAll([FromQuery] string? configType, CancellationToken cancellationToken)
    {
        var result = await _auditLogService.GetAllAsync(configType, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AttendanceConfigAuditLogDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _auditLogService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceConfigAuditLogDto>> Log([FromBody] CreateAttendanceConfigAuditLogDto dto, CancellationToken cancellationToken)
    {
        var result = await _auditLogService.LogAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
