using HR.Application.DTOs.Lifecycle;
using HR.Application.Services.Lifecycle;
using HR.API.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("")]
[ServiceFilter(typeof(HrRoleFilter))]
public sealed class LifecycleController(LifecycleService service) : ControllerBase
{
    [HttpGet("lifecycle/options")]
    public async Task<ActionResult<LifecycleOptionsDto>> Options(CancellationToken cancellationToken) =>
        Ok(await service.GetOptionsAsync(cancellationToken));

    [HttpPost("onboarding")]
    public async Task<ActionResult<OnboardingResultDto>> Onboard(
        StartOnboardingRequest request, CancellationToken cancellationToken) =>
        StatusCode(StatusCodes.Status201Created, await service.StartOnboardingAsync(request, cancellationToken));

    [HttpPost("offboarding")]
    public async Task<ActionResult<OffboardingDto>> StartOffboarding(
        StartOffboardingRequest request, CancellationToken cancellationToken) =>
        Ok(await service.StartOffboardingAsync(request, cancellationToken));

    [HttpGet("offboarding")]
    public async Task<ActionResult<IReadOnlyList<OffboardingDto>>> ListOffboardings(
        [FromQuery] Guid? employeeId, [FromQuery] string? status, CancellationToken cancellationToken) =>
        Ok(await service.ListOffboardingsAsync(employeeId, status, cancellationToken));

    [HttpGet("offboarding/{id:guid}")]
    public async Task<ActionResult<OffboardingDto>> GetOffboarding(Guid id, CancellationToken cancellationToken) =>
        Ok(await service.GetOffboardingAsync(id, cancellationToken));

    [HttpPut("offboarding/{id:guid}")]
    public async Task<ActionResult<OffboardingDto>> UpdateOffboarding(
        Guid id, UpdateOffboardingRequest request, CancellationToken cancellationToken) =>
        Ok(await service.UpdateOffboardingAsync(id, request, cancellationToken));

    [HttpPut("offboarding/{id:guid}/checklist/{itemId:guid}")]
    public async Task<ActionResult<OffboardingDto>> UpdateChecklist(
        Guid id, Guid itemId, UpdateOffboardingChecklistRequest request, CancellationToken cancellationToken) =>
        Ok(await service.UpdateChecklistAsync(id, itemId, request, cancellationToken));

    [HttpPost("offboarding/{id:guid}/complete")]
    public async Task<ActionResult<OffboardingDto>> CompleteOffboarding(
        Guid id, CompleteOffboardingRequest request, CancellationToken cancellationToken) =>
        Ok(await service.CompleteOffboardingAsync(id, request, cancellationToken));
}
