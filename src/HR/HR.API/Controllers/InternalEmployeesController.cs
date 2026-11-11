using HR.Application.DTOs.Employees;
using HR.Application.Interfaces.Services;
using HR.API.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("internal/employees")]
[ServiceFilter(typeof(InternalEmployeeDirectoryFilter))]
public sealed class InternalEmployeesController : ControllerBase
{
    private readonly IEmployeeService _employees;

    public InternalEmployeesController(IEmployeeService employees)
    {
        _employees = employees;
    }

    [HttpGet]
    public async Task<ActionResult<EmployeeDirectoryListDto>> ListActive(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _employees.ListActiveDirectoryAsync(keyword, page, pageSize, cancellationToken));
    }

    [HttpGet("{employeeId:guid}")]
    public async Task<ActionResult<EmployeeDirectoryEntry>> GetActive(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        return Ok(await _employees.GetActiveDirectoryEntryAsync(employeeId, cancellationToken));
    }
}
