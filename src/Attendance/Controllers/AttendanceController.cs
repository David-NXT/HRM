using Attendance.DTOs.AttendanceRecord;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly IHrEmployeeDirectoryClient _employees;

    public AttendanceController(IAttendanceService attendanceService, IHrEmployeeDirectoryClient employees)
    {
        _attendanceService = attendanceService;
        _employees = employees;
    }

    [HttpGet("departments")]
    public async Task<ActionResult<IReadOnlyList<HrDepartmentDto>>> GetDepartments(CancellationToken cancellationToken)
    {
        IReadOnlyList<HrDepartmentDto> departments = await _employees.GetDepartmentsAsync(cancellationToken);
        return Ok(departments);
    }

    [HttpGet]
    public async Task<ActionResult<List<AttendanceRecordDto>>> GetAll(CancellationToken cancellationToken)
    {
        List<AttendanceRecordDto> result = await _attendanceService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("work-hours")]
    public async Task<ActionResult<AttendanceWorkHoursDto>> GetWorkHours(
        [FromQuery] Guid employeeId,
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken)
    {
        if (employeeId == Guid.Empty || !date.HasValue)
        {
            return BadRequest("A valid employeeId and date are required.");
        }

        AttendanceWorkHoursDto? result = await _attendanceService.GetWorkHoursAsync(employeeId, date.Value, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AttendanceRecordDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        AttendanceRecordDto? result = await _attendanceService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<AttendanceRecordDto>>> Search([FromBody] AttendanceSearchRequestDto request, CancellationToken cancellationToken)
    {
        List<AttendanceRecordDto> result = await _attendanceService.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<AttendanceRecordDto>>> SearchGet(
        [FromQuery] DateOnly? date,
        [FromQuery] Guid? departmentId,
        [FromQuery] string? status,
        [FromQuery] string? keyword,
        CancellationToken cancellationToken)
    {
        AttendanceSearchRequestDto request = new AttendanceSearchRequestDto
        {
            Date = date,
            DepartmentId = departmentId,
            Status = status,
            Keyword = keyword
        };
        List<AttendanceRecordDto> result = await _attendanceService.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceRecordDto>> Create([FromBody] CreateAttendanceRecordDto dto, CancellationToken cancellationToken)
    {
        try
        {
            AttendanceRecordDto result = await _attendanceService.CreateAsync(dto, cancellationToken);
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
        AttendanceRecordDto? result = await _attendanceService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        bool deleted = await _attendanceService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
