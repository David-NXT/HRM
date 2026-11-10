using HR.Application.DTOs.Employees;
using HR.Application.Interfaces.Services;
using HR.API.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("employees")]
[ServiceFilter(typeof(HrRoleFilter))]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employees;

    public EmployeesController(IEmployeeService employees)
    {
        _employees = employees;
    }

    [HttpGet]
    public async Task<ActionResult<EmployeeListDto>> List(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _employees.ListAsync(keyword, page, pageSize, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _employees.GetAsync(id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _employees.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.EmployeeId }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> Update(
        Guid id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _employees.UpdateAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/lock")]
    public async Task<ActionResult<EmployeeDto>> Lock(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _employees.LockAsync(id, cancellationToken));
    }
}
