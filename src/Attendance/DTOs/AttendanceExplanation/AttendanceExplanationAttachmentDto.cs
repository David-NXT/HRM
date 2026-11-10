namespace Attendance.DTOs.AttendanceExplanation;

public class AttendanceExplanationAttachmentDto
{
    public Guid Id { get; set; }

    public Guid ExplanationId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    public DateTime UploadedAt { get; set; }
}
