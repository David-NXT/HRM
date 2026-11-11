using HR.Application.DTOs.Employees;
using HR.Application.DTOs.ProfileUpdates;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Application.Interfaces.Storage;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
namespace HR.Application.Services.ProfileUpdates;

public sealed class ProfileUpdateService
{
    private const int MaxAttachmentsPerRequest = 5;
    private const int MaxChangesJsonLength = 32_000;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IProfileUpdateRepository _requests;
    private readonly IEmployeeRepository _employees;
    private readonly IEmployeeService _employeeService;
    private readonly IEmployeeUpdateEvidenceStore _evidenceStore;
    private readonly ILogger<ProfileUpdateService> _logger;

    public ProfileUpdateService(
        IProfileUpdateRepository requests,
        IEmployeeRepository employees,
        IEmployeeService employeeService,
        IEmployeeUpdateEvidenceStore evidenceStore,
        ILogger<ProfileUpdateService> logger)
    {
        _requests = requests;
        _employees = employees;
        _employeeService = employeeService;
        _evidenceStore = evidenceStore;
        _logger = logger;
    }

    public async Task<ProfileUpdateRequestDto> CreateAsync(
        Guid employeeId,
        UpdateEmployeeRequest changes,
        bool submit,
        IReadOnlyList<EvidenceUpload> uploads,
        CancellationToken cancellationToken)
    {
        await EnsureActiveEmployeeAsync(employeeId, cancellationToken);
        _employeeService.ValidatePersonalInfoUpdate(changes);
        ValidateChangesSize(changes);
        ValidateAttachmentCount(uploads.Count);
        var savedFiles = await SaveEvidenceAsync(uploads, cancellationToken);
        try
        {
            var now = DateTime.UtcNow;
            var request = new ProfileUpdateRequest
            {
                EmployeeId = employeeId,
                ChangesJson = JsonSerializer.Serialize(changes, JsonOptions),
                Status = submit ? ProfileUpdateStatuses.Pending : ProfileUpdateStatuses.Draft,
                SubmittedAt = submit ? now : null,
                CreatedAt = now,
                UpdatedAt = now,
                Attachments = savedFiles.Select(ToEntity).ToList()
            };
            await _requests.AddRequestAsync(request, cancellationToken);
            await _requests.SaveChangesAsync(cancellationToken);
            return ToDto(request);
        }
        catch
        {
            await CleanupEvidenceAsync(savedFiles);
            throw;
        }
    }

    public async Task<ProfileUpdateRequestDto> UpdateAsync(
        Guid employeeId,
        Guid requestId,
        UpdateEmployeeRequest changes,
        bool submit,
        IReadOnlyList<EvidenceUpload> uploads,
        CancellationToken cancellationToken)
    {
        var request = await _requests.GetOwnedRequestAsync(requestId, employeeId, cancellationToken)
            ?? throw new HrException(404, "ProfileUpdateRequestNotFound", "Không tìm thấy yêu cầu cập nhật.");
        if (request.Status is not (ProfileUpdateStatuses.Draft or ProfileUpdateStatuses.NeedInfo))
        {
            throw new HrException(409, "InvalidRequestState", "Chỉ có thể sửa bản nháp hoặc yêu cầu cần bổ sung thông tin.");
        }
        _employeeService.ValidatePersonalInfoUpdate(changes);
        ValidateChangesSize(changes);
        ValidateAttachmentCount(request.Attachments.Count + uploads.Count);
        var savedFiles = await SaveEvidenceAsync(uploads, cancellationToken);
        try
        {
            var now = DateTime.UtcNow;
            request.ChangesJson = JsonSerializer.Serialize(changes, JsonOptions);
            request.Status = submit
                ? ProfileUpdateStatuses.Pending
                : request.Status == ProfileUpdateStatuses.NeedInfo ? ProfileUpdateStatuses.NeedInfo : ProfileUpdateStatuses.Draft;
            request.ReviewReason = null;
            request.SubmittedAt = submit ? now : request.SubmittedAt;
            request.UpdatedAt = now;
            foreach (var saved in savedFiles)
            {
                request.Attachments.Add(ToEntity(saved));
            }
            await _requests.SaveChangesAsync(cancellationToken);
            return ToDto(request);
        }
        catch
        {
            await CleanupEvidenceAsync(savedFiles);
            throw;
        }
    }

    public async Task<ProfileUpdateRequestDto> CancelAsync(Guid employeeId, Guid requestId, CancellationToken cancellationToken)
    {
        var request = await _requests.GetOwnedRequestAsync(requestId, employeeId, cancellationToken)
            ?? throw new HrException(404, "ProfileUpdateRequestNotFound", "Không tìm thấy yêu cầu cập nhật.");
        if (request.Status is not (ProfileUpdateStatuses.Draft or ProfileUpdateStatuses.Pending))
        {
            throw new HrException(409, "InvalidRequestState", "Chỉ có thể hủy bản nháp hoặc yêu cầu đang chờ xử lý.");
        }
        request.Status = ProfileUpdateStatuses.Cancelled;
        request.UpdatedAt = DateTime.UtcNow;
        await _requests.SaveChangesAsync(cancellationToken);
        return ToDto(request);
    }

    public async Task<IReadOnlyList<ProfileUpdateRequestDto>> ListOwnedAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var requests = await _requests.ListOwnedRequestsAsync(employeeId, cancellationToken);
        return requests.Select(ToDto).ToArray();
    }

    public async Task<IReadOnlyList<ProfileUpdateRequestDto>> ListForReviewAsync(string? status, CancellationToken cancellationToken)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? ProfileUpdateStatuses.Pending : status.Trim().ToUpperInvariant();
        if (normalizedStatus is not (ProfileUpdateStatuses.Draft or ProfileUpdateStatuses.Pending
            or ProfileUpdateStatuses.NeedInfo or ProfileUpdateStatuses.Approved
            or ProfileUpdateStatuses.Rejected or ProfileUpdateStatuses.Cancelled))
        {
            throw new HrException(400, "ValidationError", "Trạng thái yêu cầu không hợp lệ.");
        }
        var requests = await _requests.ListForReviewAsync(normalizedStatus, cancellationToken);
        return requests.Select(ToDto).ToArray();
    }

    public async Task<ProfileUpdateRequestDto> GetForReviewAsync(Guid requestId, CancellationToken cancellationToken)
    {
        var request = await _requests.GetRequestAsync(requestId, cancellationToken)
            ?? throw new HrException(404, "ProfileUpdateRequestNotFound", "Không tìm thấy yêu cầu cập nhật.");
        return ToDto(request);
    }

    public async Task<ProfileUpdateRequestDto> ReviewAsync(
        Guid requestId,
        ReviewProfileUpdateRequest decision,
        CancellationToken cancellationToken)
    {
        var request = await _requests.GetRequestAsync(requestId, cancellationToken)
            ?? throw new HrException(404, "ProfileUpdateRequestNotFound", "Không tìm thấy yêu cầu cập nhật.");
        if (request.Status != ProfileUpdateStatuses.Pending)
        {
            throw new HrException(409, "InvalidRequestState", "Yêu cầu này không còn ở trạng thái chờ xử lý.");
        }
        var action = (decision.Decision ?? string.Empty).Trim().ToUpperInvariant();
        if (action is not ("APPROVE" or "REJECT" or "REQUEST_MORE_INFO"))
        {
            throw new HrException(400, "ValidationError", "Quyết định phải là APPROVE, REJECT hoặc REQUEST_MORE_INFO.");
        }
        var reason = string.IsNullOrWhiteSpace(decision.Reason) ? null : decision.Reason.Trim();
        if (reason is { Length: > 1000 })
        {
            throw new HrException(400, "ValidationError", "Lý do xử lý tối đa 1000 ký tự.");
        }
        if ((action is "REJECT" or "REQUEST_MORE_INFO") && reason is null)
        {
            throw new HrException(400, "ValidationError", "Cần ghi lý do từ chối hoặc thông tin cần bổ sung.");
        }

        var employee = await _employees.GetByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ nhân viên.");
        if (action == "APPROVE")
        {
            var changes = DeserializeChanges(request.ChangesJson);
            _employeeService.ApplyPersonalInfoUpdate(employee, changes);
            request.Status = ProfileUpdateStatuses.Approved;
        }
        else if (action == "REJECT")
        {
            request.Status = ProfileUpdateStatuses.Rejected;
        }
        else
        {
            request.Status = ProfileUpdateStatuses.NeedInfo;
        }

        var now = DateTime.UtcNow;
        request.ReviewReason = reason;
        request.ReviewedAt = now;
        request.UpdatedAt = now;
        var notificationMessage = action == "APPROVE" ? "Yêu cầu cập nhật hồ sơ của bạn đã được chấp thuận."
            : action == "REJECT" ? $"Yêu cầu cập nhật hồ sơ bị từ chối: {reason}"
            : $"Vui lòng bổ sung thông tin hoặc minh chứng: {reason}";
        if (notificationMessage.Length > 1000)
        {
            notificationMessage = notificationMessage[..1000];
        }
        await _requests.AddNotificationAsync(new EmployeeNotification
        {
            EmployeeId = request.EmployeeId,
            ProfileUpdateRequestId = request.ProfileUpdateRequestId,
            Title = action == "APPROVE" ? "Yêu cầu cập nhật được chấp thuận"
                : action == "REJECT" ? "Yêu cầu cập nhật bị từ chối" : "Yêu cầu bổ sung thông tin",
            Message = notificationMessage,
            CreatedAt = now
        }, cancellationToken);
        await _requests.SaveChangesInTransactionAsync(cancellationToken);
        return ToDto(request);
    }

    public async Task<IReadOnlyList<EmployeeNotificationDto>> ListNotificationsAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var notifications = await _requests.ListNotificationsAsync(employeeId, cancellationToken);
        return notifications.Select(item => new EmployeeNotificationDto(item.EmployeeNotificationId,
            item.ProfileUpdateRequestId, item.Title, item.Message, item.CreatedAt, item.ReadAt)).ToArray();
    }

    public async Task<EvidenceDownload> GetOwnedEvidenceAsync(
        Guid employeeId,
        Guid requestId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var request = await _requests.GetOwnedRequestAsync(requestId, employeeId, cancellationToken)
            ?? throw new HrException(404, "ProfileUpdateRequestNotFound", "Không tìm thấy yêu cầu cập nhật.");
        return await GetEvidenceAsync(request, attachmentId, cancellationToken);
    }

    public async Task<EvidenceDownload> GetReviewEvidenceAsync(
        Guid requestId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var request = await _requests.GetRequestAsync(requestId, cancellationToken)
            ?? throw new HrException(404, "ProfileUpdateRequestNotFound", "Không tìm thấy yêu cầu cập nhật.");
        return await GetEvidenceAsync(request, attachmentId, cancellationToken);
    }

    private async Task<EvidenceDownload> GetEvidenceAsync(
        ProfileUpdateRequest request,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var attachment = request.Attachments.FirstOrDefault(item => item.ProfileUpdateAttachmentId == attachmentId)
            ?? throw new HrException(404, "EvidenceNotFound", "Không tìm thấy file minh chứng.");
        return await _evidenceStore.OpenReadAsync(attachment.StorageKey, attachment.OriginalFileName,
            attachment.ContentType, cancellationToken);
    }

    private async Task<Employee> EnsureActiveEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null || employee.Status != EmployeeStatuses.Active)
        {
            throw new HrException(404, "EmployeeNotFound", "Không tìm thấy nhân viên đang hoạt động.");
        }
        return employee;
    }

    private async Task<IReadOnlyList<StoredEvidence>> SaveEvidenceAsync(
        IReadOnlyList<EvidenceUpload> uploads,
        CancellationToken cancellationToken)
    {
        var saved = new List<StoredEvidence>();
        try
        {
            foreach (var upload in uploads)
            {
                saved.Add(await _evidenceStore.SaveAsync(upload, cancellationToken));
            }
            return saved;
        }
        catch
        {
            await CleanupEvidenceAsync(saved);
            throw;
        }
    }

    private async Task CleanupEvidenceAsync(IEnumerable<StoredEvidence> savedFiles)
    {
        foreach (var file in savedFiles)
        {
            try
            {
                await _evidenceStore.DeleteAsync(file.StorageKey, CancellationToken.None);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Không xóa được file minh chứng tạm {StorageKey}.", file.StorageKey);
            }
        }
    }

    private static void ValidateAttachmentCount(int count)
    {
        if (count > MaxAttachmentsPerRequest)
        {
            throw new HrException(400, "TooManyAttachments", "Mỗi yêu cầu tối đa 5 file minh chứng.");
        }
    }

    private static void ValidateChangesSize(UpdateEmployeeRequest changes)
    {
        if (JsonSerializer.Serialize(changes, JsonOptions).Length > MaxChangesJsonLength)
        {
            throw new HrException(400, "ValidationError", "Dữ liệu thay đổi vượt quá giới hạn cho phép.");
        }
    }

    private static UpdateEmployeeRequest DeserializeChanges(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<UpdateEmployeeRequest>(json, JsonOptions)
                ?? throw new JsonException("Empty request changes.");
        }
        catch (JsonException)
        {
            throw new HrException(500, "InvalidStoredChanges", "Dữ liệu thay đổi đã lưu không hợp lệ.");
        }
    }

    private static ProfileUpdateAttachment ToEntity(StoredEvidence file) => new()
    {
        StorageKey = file.StorageKey,
        OriginalFileName = file.OriginalFileName,
        ContentType = file.ContentType,
        Length = file.Length,
        UploadedAt = DateTime.UtcNow
    };

    private static ProfileUpdateRequestDto ToDto(ProfileUpdateRequest request) =>
        new(request.ProfileUpdateRequestId, request.EmployeeId, DeserializeChanges(request.ChangesJson),
            request.Status, request.ReviewReason, request.SubmittedAt, request.ReviewedAt,
            request.CreatedAt, request.UpdatedAt,
            request.Attachments.Select(attachment => new ProfileUpdateAttachmentDto(
                attachment.ProfileUpdateAttachmentId, attachment.OriginalFileName,
                attachment.ContentType, attachment.Length, attachment.UploadedAt)).ToArray());
}
