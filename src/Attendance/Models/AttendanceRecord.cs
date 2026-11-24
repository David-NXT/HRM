
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models;

public class AttendanceRecord
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    [ForeignKey(nameof(WorkShift))]
    public Guid ShiftId { get; set; }

    // Khóa ngoại liên kết với lần phân ca của nhân viên
    [ForeignKey(nameof(ShiftAssignment))]
    public Guid ShiftAssignmentId { get; set; }

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

    // Navigation property liên kết với lần phân ca
    public ShiftAssignment? ShiftAssignment { get; set; }

    public ICollection<AttendanceExplanation>? AttendanceExplanations { get; set; }

    public void CheckIn(TimeOnly time, string? location, string? source)
    {
        TimeIn = time;
        CheckInLocation = location;
        CheckInSource = source;
    }

    public void CheckOut(TimeOnly time, string? location, string? source)
    {
        TimeOut = time;
        CheckOutLocation = location;
        CheckOutSource = source;
    }

    public decimal CalculateWorkHours(WorkShift shift)
    {
        if (!TimeIn.HasValue || !TimeOut.HasValue) return WorkHour;
        double minutes = (TimeOut.Value.ToTimeSpan() - TimeIn.Value.ToTimeSpan()).TotalMinutes;
        if (minutes < 0) minutes += 24 * 60;
        if (minutes > 240) minutes -= shift.BreakMins;
        WorkHour = Math.Round(Math.Max(0, (decimal)minutes / 60m), 2);
        return WorkHour;
    }

    public void EvaluateLateAndEarlyLeave(WorkShift shift, AttendanceRule rule)
    {
        int lateDifference = TimeIn.HasValue && TimeIn.Value > shift.StartTime
            ? (int)(TimeIn.Value.ToTimeSpan() - shift.StartTime.ToTimeSpan()).TotalMinutes : 0;
        LateMins = rule.ExceedsLateThreshold(lateDifference) ? lateDifference : 0;
        IsLate = LateMins > 0;
        int earlyDifference = TimeOut.HasValue && TimeOut.Value < shift.EndTime
            ? (int)(shift.EndTime.ToTimeSpan() - TimeOut.Value.ToTimeSpan()).TotalMinutes : 0;
        EarlyLeaveMins = rule.ExceedsEarlyLeaveThreshold(earlyDifference) ? earlyDifference : 0;
        IsEarlyLeave = EarlyLeaveMins > 0;
    }
}
