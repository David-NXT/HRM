using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/employees")]
public sealed class EmployeesController(IHrEmployeeDirectoryClient employees) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<EmployeeDirectoryPage>> List(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            return BadRequest("page must be positive and pageSize must be between 1 and 100.");
        }

        try
        {
            return Ok(await employees.GetActiveEmployeesAsync(keyword, page, pageSize, cancellationToken));
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "HR employee directory is unavailable.");
        }
    }

    [HttpGet("{employeeId:guid}")]
    public async Task<ActionResult<EmployeeDirectoryEntry>> Get(Guid employeeId, CancellationToken cancellationToken)
    {
        try
        {
            EmployeeDirectoryEntry? employee = await employees.GetActiveEmployeeAsync(employeeId, cancellationToken);
            if (employee is null)
            {
                return NotFound();
            }

            return Ok(employee);
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "HR employee directory is unavailable.");
        }
    }
}
