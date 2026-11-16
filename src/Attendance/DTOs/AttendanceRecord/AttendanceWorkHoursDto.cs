namespace Attendance.DTOs.AttendanceRecord;

public sealed class AttendanceWorkHoursDto
{
    public Guid EmployeeId { get; set; }

    public DateOnly Date { get; set; }

    public decimal TotalWorkHours { get; set; }

    public int RecordCount { get; set; }
}
