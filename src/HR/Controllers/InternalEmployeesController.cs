using HR.Core.DTOs;
using HR.Core.Interfaces;
using HR.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HR.Controllers;

[ApiController]
[Route("internal/employees")]
[ServiceFilter(typeof(HrRoleFilter))]
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

    [HttpGet("{employeeGuid:guid}")]
    public async Task<ActionResult<EmployeeDirectoryEntry>> GetActive(
        Guid employeeGuid,
        CancellationToken cancellationToken)
    {
        return Ok(await _employees.GetActiveDirectoryEntryAsync(employeeGuid, cancellationToken));
    }
}
