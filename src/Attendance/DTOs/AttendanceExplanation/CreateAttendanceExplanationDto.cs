using System.ComponentModel.DataAnnotations;

namespace Attendance.DTOs.AttendanceExplanation;

public class CreateAttendanceExplanationDto
{
    public Guid? AttendanceId { get; set; }

    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public string ExplanationType { get; set; } = string.Empty;

    [Required]
    public string Reason { get; set; } = string.Empty;

    public TimeOnly? RequestedTimeIn { get; set; }

    public TimeOnly? RequestedTimeOut { get; set; }

    public string? AttachmentUrl { get; set; }

    public string Status { get; set; } = "Pending";

    public string? ApprovedBy { get; set; }

    public List<CreateAttendanceExplanationAttachmentDto>? Attachments { get; set; }
}

public class CreateAttendanceExplanationAttachmentDto
{
    [Required]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string FileUrl { get; set; } = string.Empty;

    public string? FileType { get; set; }

    public long? FileSize { get; set; }
}
