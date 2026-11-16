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

    [HttpGet("statistics")]
    public async Task<ActionResult<EmployeeStatisticsDto>> Statistics(CancellationToken cancellationToken)
    {
        return Ok(await _employees.GetStatisticsAsync(cancellationToken));
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] string? keyword,
        [FromServices] IEmployeeExportService exporter,
        CancellationToken cancellationToken)
    {
        var file = await exporter.ExportAsync(keyword, cancellationToken);
        return File(file.Content, EmployeeExportFile.ContentType, file.FileName);
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _employees.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> Update(
        Guid id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _employees.UpdateAsync(id, request, cancellationToken));
    }

    [HttpPut("{id:guid}/salary")]
    public async Task<ActionResult<EmployeeDto>> UpdateSalary(
        Guid id,
        [FromBody] UpdateEmployeeSalaryRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _employees.UpdateSalaryAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/lock")]
    public async Task<ActionResult<EmployeeDto>> Lock(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _employees.LockAsync(id, cancellationToken));
    }
}
