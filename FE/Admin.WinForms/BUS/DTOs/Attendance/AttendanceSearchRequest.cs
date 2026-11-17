namespace Admin.WinForms.BUS.DTOs.Attendance;

public class AttendanceSearchRequest
{
    public DateOnly? Date { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
}