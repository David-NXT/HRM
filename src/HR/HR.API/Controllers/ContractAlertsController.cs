using HR.Application.DTOs.ContractAlerts;
using HR.Application.Services.ContractAlerts;
using HR.API.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("contract-alerts")]
[ServiceFilter(typeof(HrRoleFilter))]
public sealed class ContractAlertsController(ContractAlertService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ContractAlertListDto>> List(
        [FromQuery] string? status, [FromQuery] Guid? employeeId,
        [FromQuery] DateOnly? expiresFrom, [FromQuery] DateOnly? expiresTo,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        Ok(await service.ListAsync(status, employeeId, expiresFrom, expiresTo, page, pageSize, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContractAlertDto>> Get(Guid id, CancellationToken cancellationToken) =>
        Ok(await service.GetAsync(id, cancellationToken));

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ContractAlertDto>> UpdateStatus(
        Guid id, UpdateContractAlertStatusRequest request, CancellationToken cancellationToken) =>
        Ok(await service.UpdateStatusAsync(id, request, cancellationToken));
}
