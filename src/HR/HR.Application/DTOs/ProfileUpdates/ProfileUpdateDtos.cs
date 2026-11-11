using HR.Application.DTOs.Employees;
namespace HR.Application.DTOs.ProfileUpdates;

public sealed record ProfileUpdateAttachmentDto(
    Guid ProfileUpdateAttachmentId,
    string OriginalFileName,
    string ContentType,
    long Length,
    DateTime UploadedAt);

public sealed record ProfileUpdateRequestDto(
    Guid ProfileUpdateRequestId,
    Guid EmployeeId,
    UpdateEmployeeRequest Changes,
    string Status,
    string? ReviewReason,
    DateTime? SubmittedAt,
    DateTime? ReviewedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<ProfileUpdateAttachmentDto> Attachments);

public sealed record ReviewProfileUpdateRequest(string Decision, string? Reason);

public sealed record EmployeeNotificationDto(
    Guid EmployeeNotificationId,
    Guid? ProfileUpdateRequestId,
    string Title,
    string Message,
    DateTime CreatedAt,
    DateTime? ReadAt);

public sealed record StoredEvidence(
    string StorageKey,
    string OriginalFileName,
    string ContentType,
    long Length);

public sealed record EvidenceUpload(
    string FileName,
    string ContentType,
    long Length,
    Stream Content);

public sealed record EvidenceDownload(Stream Content, string FileName, string ContentType);
