using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models;

public class AttendanceExplanationAttachment
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Explanation))]
    public Guid ExplanationId { get; set; }

    /// <summary>
    /// Tên file minh chứng (VD: don_xin_xac_nhan.pdf, anh_bang_chung.jpg)
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Đường dẫn file lưu trữ hoặc URL tải về
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// Định dạng file (image/png, application/pdf...)
    /// </summary>
    public string? FileType { get; set; }

    /// <summary>
    /// Kích thước file (bytes)
    /// </summary>
    public long? FileSize { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public AttendanceExplanation? Explanation { get; set; }

    public bool ValidateFileType(string[] allowedTypes) => FileType != null && allowedTypes.Contains(FileType, StringComparer.OrdinalIgnoreCase);
    public bool ValidateFileSize(long maxBytes) => FileSize.HasValue && FileSize.Value <= maxBytes;
}
