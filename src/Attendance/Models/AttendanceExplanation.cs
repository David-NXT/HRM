namespace Attendance.Models;
public class AttendanceExplanation
{
    public Guid Id { get; set; }

    public Guid AttendanceId { get; set; }

    public Guid EmployeeId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public TimeOnly? RequestedTimeIn { get; set; }

    public TimeOnly? RequestedTimeOut { get; set; }

    public string Status { get; set; } = "Pending";

    public string? ApprovedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AttendanceRecord? Attendance { get; set; }

    public DateTime? UpdatedAt { get; set; }

}
