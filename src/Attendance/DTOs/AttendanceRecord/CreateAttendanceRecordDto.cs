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

    public int ActualOTMins { get; set; }

    public bool IsLate { get; set; }

    public bool IsEarlyLeave { get; set; }

    public string? CheckInSource { get; set; }

    public string? CheckOutSource { get; set; }

    public double? CheckInLatitude { get; set; }

    public double? CheckInLongitude { get; set; }

    public string? CheckInLocation { get; set; }

    public double? CheckOutLatitude { get; set; }

    public double? CheckOutLongitude { get; set; }

    public string? CheckOutLocation { get; set; }

    public string? DeviceId { get; set; }

    public string? Note { get; set; }
}
