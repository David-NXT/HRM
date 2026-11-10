namespace Attendance.DTOs.AttendanceExplanation;

public class AttendanceExplanationDto
{
    public Guid Id { get; set; }

    public Guid? AttendanceId { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly Date { get; set; }

    public string ExplanationType { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public TimeOnly? RequestedTimeIn { get; set; }

    public TimeOnly? RequestedTimeOut { get; set; }

    public string? AttachmentUrl { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<AttendanceExplanationAttachmentDto>? Attachments { get; set; }
}
