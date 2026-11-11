using HR.Application.DTOs.Organization;
using HR.Application.Services.Organization;
using HR.API.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("departments")]
[ServiceFilter(typeof(HrRoleFilter))]
public sealed class DepartmentsController : ControllerBase
{
    private readonly OrgStructureService _service;

    public DepartmentsController(OrgStructureService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> List(CancellationToken cancellationToken) =>
        Ok(await _service.ListDepartmentsAsync(cancellationToken));

    [HttpGet("tree")]
    public async Task<ActionResult<IReadOnlyList<DepartmentTreeDto>>> Tree(CancellationToken cancellationToken) =>
        Ok(await _service.GetDepartmentTreeAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken) =>
        StatusCode(StatusCodes.Status201Created, await _service.CreateDepartmentAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DepartmentDto>> Update(
        Guid id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _service.UpdateDepartmentAsync(id, request, cancellationToken));

    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<DepartmentDto>> Deactivate(Guid id, CancellationToken cancellationToken) =>
        Ok(await _service.DeactivateDepartmentAsync(id, cancellationToken));
}
