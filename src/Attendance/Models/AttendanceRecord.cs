using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models;

public class AttendanceRecord
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    [ForeignKey(nameof(WorkShift))]
    public Guid ShiftId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly? TimeIn { get; set; }

    public TimeOnly? TimeOut { get; set; }

    public string Status { get; set; } = "Pending";

    public decimal WorkHour { get; set; }

    public int LateMins { get; set; }

    public int EarlyLeaveMins { get; set; }

    public int OTMins { get; set; }

    /// <summary>
    /// Số phút OT thực tế đối chiếu với đơn OT đã duyệt
    /// </summary>
    public int ActualOTMins { get; set; }

    /// <summary>
    /// Cờ đánh dấu đi trễ
    /// </summary>
    public bool IsLate { get; set; }

    /// <summary>
    /// Cờ đánh dấu về sớm
    /// </summary>
    public bool IsEarlyLeave { get; set; }

    /// <summary>
    /// Nguồn chấm công khi Check-in: MobileApp, TimekeepingDevice, GPS, Web...
    /// </summary>
    public string? CheckInSource { get; set; }

    /// <summary>
    /// Nguồn chấm công khi Check-out
    /// </summary>
    public string? CheckOutSource { get; set; }

    public double? CheckInLatitude { get; set; }

    public double? CheckInLongitude { get; set; }

    public string? CheckInLocation { get; set; }

    public double? CheckOutLatitude { get; set; }

    public double? CheckOutLongitude { get; set; }

    public string? CheckOutLocation { get; set; }

    /// <summary>
    /// Mã thiết bị máy chấm công (quét vân tay / thẻ)
    /// </summary>
    public string? DeviceId { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public WorkShift? WorkShift { get; set; }

    public ICollection<AttendanceExplanation>? AttendanceExplanations { get; set; }
}
