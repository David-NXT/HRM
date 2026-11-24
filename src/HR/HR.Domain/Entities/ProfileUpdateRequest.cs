using HR.Domain.Constants;
namespace HR.Domain.Entities;

public sealed class ProfileUpdateRequest
{
    public Guid ProfileUpdateRequestId { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public string ChangesJson { get; set; } = string.Empty;
    public string Status { get; set; } = ProfileUpdateStatuses.Draft;
    public int Version { get; set; } = 1;
    // Phiên bản hồ sơ mà nhân viên đã đọc khi soạn yêu cầu; null dành cho dữ liệu cũ cần gửi lại.
    public int? BaseEmployeeVersion { get; set; }
    public string? ReviewReason { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Employee? Employee { get; set; }
    public List<ProfileUpdateAttachment> Attachments { get; set; } = [];
}
