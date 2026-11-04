using System.ComponentModel.DataAnnotations;

namespace Attendance.DTOs.AttendanceRecord;

public class CreateAttendanceRecordDto
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public Guid ShiftId { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    public TimeOnly? TimeIn { get; set; }

    public TimeOnly? TimeOut { get; set; }

    public string Status { get; set; } = "Pending";

    public decimal WorkHour { get; set; }

    public int LateMins { get; set; }

    public int EarlyLeaveMins { get; set; }

    public int OTMins { get; set; }

    public string? Note { get; set; }
}
