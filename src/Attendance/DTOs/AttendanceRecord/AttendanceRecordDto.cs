namespace Attendance.DTOs.AttendanceRecord;

public class AttendanceRecordDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? EmployeeName { get; set; }

    public Guid ShiftId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly? TimeIn { get; set; }

    public TimeOnly? TimeOut { get; set; }

    public string Status { get; set; } = string.Empty;

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

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
