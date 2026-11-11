using HR.Application.DTOs.ProfileUpdates;
using HR.Application.Services.ProfileUpdates;
using HR.API.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("update-requests")]
[ServiceFilter(typeof(HrRoleFilter))]
public sealed class ProfileUpdateReviewController : ControllerBase
{
    private readonly ProfileUpdateService _updates;

    public ProfileUpdateReviewController(ProfileUpdateService updates) => _updates = updates;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProfileUpdateRequestDto>>> List(
        [FromQuery] string? status,
        CancellationToken cancellationToken) =>
        Ok(await _updates.ListForReviewAsync(status, cancellationToken));

    [HttpGet("{requestId:guid}")]
    public async Task<ActionResult<ProfileUpdateRequestDto>> Get(Guid requestId, CancellationToken cancellationToken) =>
        Ok(await _updates.GetForReviewAsync(requestId, cancellationToken));

    [HttpPost("{requestId:guid}/review")]
    public async Task<ActionResult<ProfileUpdateRequestDto>> Review(
        Guid requestId,
        ReviewProfileUpdateRequest decision,
        CancellationToken cancellationToken) =>
        Ok(await _updates.ReviewAsync(requestId, decision, cancellationToken));

    [HttpGet("{requestId:guid}/attachments/{attachmentId:guid}")]
    public async Task<IActionResult> DownloadEvidence(
        Guid requestId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var download = await _updates.GetReviewEvidenceAsync(requestId, attachmentId, cancellationToken);
        return File(download.Content, download.ContentType, download.FileName);
    }
}
