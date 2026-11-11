namespace HR.Domain.Entities;

public sealed class ProfileUpdateAttachment
{
    public Guid ProfileUpdateAttachmentId { get; set; } = Guid.NewGuid();
    public Guid ProfileUpdateRequestId { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Length { get; set; }
    public DateTime UploadedAt { get; set; }

    public ProfileUpdateRequest? Request { get; set; }
}
