namespace Attendance.Models;
public class AttendanceRecord
{
    public Guid Id {get; set;}

    public Guid EmployeeId { get; set; }

    public Guid ShiftId {get; set;}

    public DateOnly Date {get; set;}

    public TimeOnly? TimeIn {get; set;}

    public TimeOnly? TimeOut {get; set;}

    public string Status {get; set;} = "Pending";

    public decimal WorkHour {get; set;}

    public int LateMins {get; set;}

    public int EarlyLeaveMins {get; set;}

    public int OTMins {get; set;}

    public string? Note {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime UpdatedAt {get; set;} = DateTime.UtcNow;

}