namespace Attendance.DTOs.AttendanceRecord;

public class AttendanceSearchRequestDto
{
    public DateOnly? Date { get; set; }

    public Guid? DepartmentId { get; set; }

    public string? Status { get; set; }

    public string? Keyword { get; set; }
}