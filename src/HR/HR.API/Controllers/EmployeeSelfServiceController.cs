using HR.Application.DTOs.Employees;
using HR.Application.DTOs.ProfileUpdates;
using HR.Application.Interfaces.Services;
using HR.Application.Services.ProfileUpdates;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using HR.Application.Exceptions;
using HR.API.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("me")]
[ServiceFilter(typeof(EmployeeIdentityFilter))]
public sealed class EmployeeSelfServiceController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly IEmployeeService _employees;
    private readonly ProfileUpdateService _updates;

    public EmployeeSelfServiceController(IEmployeeService employees, ProfileUpdateService updates)
    {
        _employees = employees;
        _updates = updates;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<EmployeeDto>> GetProfile(CancellationToken cancellationToken) =>
        Ok(await _employees.GetAsync(CurrentEmployeeId(), cancellationToken));

    [HttpGet("update-requests")]
    public async Task<ActionResult<IReadOnlyList<ProfileUpdateRequestDto>>> ListRequests(CancellationToken cancellationToken) =>
        Ok(await _updates.ListOwnedAsync(CurrentEmployeeId(), cancellationToken));

    [HttpPost("update-requests")]
    [RequestSizeLimit(55_000_000)]
    public async Task<ActionResult<ProfileUpdateRequestDto>> CreateRequest(
        [FromForm] ProfileUpdateForm form,
        CancellationToken cancellationToken)
    {
        var changes = ParseChanges(form.ChangesJson);
        var (uploads, streams) = OpenUploads(form.Attachments);
        try
        {
            var created = await _updates.CreateAsync(CurrentEmployeeId(), changes, form.Submit, uploads, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, created);
        }
        finally
        {
            await DisposeStreamsAsync(streams);
        }
    }

    [HttpPut("update-requests/{requestId:guid}")]
    [RequestSizeLimit(55_000_000)]
    public async Task<ActionResult<ProfileUpdateRequestDto>> UpdateRequest(
        Guid requestId,
        [FromForm] ProfileUpdateForm form,
        CancellationToken cancellationToken)
    {
        var changes = ParseChanges(form.ChangesJson);
        var (uploads, streams) = OpenUploads(form.Attachments);
        try
        {
            return Ok(await _updates.UpdateAsync(CurrentEmployeeId(), requestId, changes, form.Submit, uploads, cancellationToken));
        }
        finally
        {
            await DisposeStreamsAsync(streams);
        }
    }

    [HttpPost("update-requests/{requestId:guid}/cancel")]
    public async Task<ActionResult<ProfileUpdateRequestDto>> CancelRequest(Guid requestId, CancellationToken cancellationToken) =>
        Ok(await _updates.CancelAsync(CurrentEmployeeId(), requestId, cancellationToken));

    [HttpGet("update-requests/{requestId:guid}/attachments/{attachmentId:guid}")]
    public async Task<IActionResult> DownloadOwnEvidence(
        Guid requestId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var download = await _updates.GetOwnedEvidenceAsync(CurrentEmployeeId(), requestId, attachmentId, cancellationToken);
        return File(download.Content, download.ContentType, download.FileName);
    }

    [HttpGet("notifications")]
    public async Task<ActionResult<IReadOnlyList<EmployeeNotificationDto>>> Notifications(CancellationToken cancellationToken) =>
        Ok(await _updates.ListNotificationsAsync(CurrentEmployeeId(), cancellationToken));

    private Guid CurrentEmployeeId() =>
        HttpContext.Items.TryGetValue(EmployeeIdentityFilter.EmployeeIdItem, out var value) && value is Guid employeeId
            ? employeeId
            : throw new HrException(401, "Unauthorized", "Không xác định được nhân viên hiện tại.");

    private static UpdateEmployeeRequest ParseChanges(string? json)
    {
        if (string.IsNullOrWhiteSpace(json) || json.Length > 32_000)
        {
            throw new HrException(400, "ValidationError", "Thiếu dữ liệu thay đổi hoặc dữ liệu vượt quá giới hạn.");
        }
        try
        {
            return JsonSerializer.Deserialize<UpdateEmployeeRequest>(json, JsonOptions)
                ?? throw new JsonException("Empty profile changes.");
        }
        catch (JsonException)
        {
            throw new HrException(400, "ValidationError", "Dữ liệu thay đổi hồ sơ không hợp lệ.");
        }
    }

    private static (IReadOnlyList<EvidenceUpload> Uploads, IReadOnlyList<Stream> Streams) OpenUploads(IReadOnlyList<IFormFile>? files)
    {
        if (files is null || files.Count == 0)
        {
            return ([], []);
        }
        var streams = new List<Stream>(files.Count);
        try
        {
            var uploads = new List<EvidenceUpload>(files.Count);
            foreach (var file in files)
            {
                var stream = file.OpenReadStream();
                streams.Add(stream);
                uploads.Add(new EvidenceUpload(file.FileName, file.ContentType, file.Length, stream));
            }
            return (uploads, streams);
        }
        catch
        {
            foreach (var stream in streams)
            {
                stream.Dispose();
            }
            throw;
        }
    }

    private static async Task DisposeStreamsAsync(IEnumerable<Stream> streams)
    {
        foreach (var stream in streams)
        {
            await stream.DisposeAsync();
        }
    }
}

public sealed class ProfileUpdateForm
{
    [Required]
    public string ChangesJson { get; set; } = string.Empty;

    public bool Submit { get; set; }

    public List<IFormFile> Attachments { get; set; } = [];
}
