using System.ComponentModel.DataAnnotations;

namespace Attendance.DTOs.AttendanceExplanation;

public class CreateAttendanceExplanationDto
{
    [Required]
    public Guid AttendanceId { get; set; }

    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public string Reason { get; set; } = string.Empty;

    public TimeOnly? RequestedTimeIn { get; set; }

    public TimeOnly? RequestedTimeOut { get; set; }

    public string Status { get; set; } = "Pending";

    public string? ApprovedBy { get; set; }
}
