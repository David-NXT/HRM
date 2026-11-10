using HR.Application.DTOs.Organization;
using HR.Application.Services.Organization;
using HR.API.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("positions")]
[ServiceFilter(typeof(HrRoleFilter))]
public sealed class PositionsController : ControllerBase
{
    private readonly OrgStructureService _service;

    public PositionsController(OrgStructureService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PositionDto>>> List(CancellationToken cancellationToken) =>
        Ok(await _service.ListPositionsAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<PositionDto>> Create(
        CreatePositionRequest request,
        CancellationToken cancellationToken) =>
        StatusCode(StatusCodes.Status201Created, await _service.CreatePositionAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PositionDto>> Update(
        Guid id,
        UpdatePositionRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _service.UpdatePositionAsync(id, request, cancellationToken));

    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<PositionDto>> Deactivate(Guid id, CancellationToken cancellationToken) =>
        Ok(await _service.DeactivatePositionAsync(id, cancellationToken));
}
