namespace Attendance.DTOs.AttendanceExplanation;

public class UpdateAttendanceExplanationDto
{
    public Guid? AttendanceId { get; set; }

    public Guid? EmployeeId { get; set; }

    public string? Reason { get; set; }

    public TimeOnly? RequestedTimeIn { get; set; }

    public TimeOnly? RequestedTimeOut { get; set; }

    public string? Status { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
