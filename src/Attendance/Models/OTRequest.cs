namespace Attendance.Models;

public class OTRequest
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Số giờ làm thêm đăng ký dự kiến
    /// </summary>
    public decimal PlannedHours { get; set; }

    /// <summary>
    /// Số giờ làm thêm thực tế sau khi đối chiếu với dữ liệu Check-out
    /// </summary>
    public decimal? ActualHours { get; set; }

    /// <summary>
    /// Loại ngày làm thêm: Ngày thường (Weekday), Ngày nghỉ (Weekend), Ngày lễ (Holiday)
    /// </summary>
    public string? DayType { get; set; }

    /// <summary>
    /// Hệ số làm thêm giờ theo loại ngày (1.5, 2.0, 3.0)
    /// </summary>
    public decimal Multiplier { get; set; } = 1.0m;

    /// <summary>
    /// Đăng ký bổ sung OT ngoài kế hoạch (khi đã làm ngoài giờ nhưng chưa đăng ký trước)
    /// </summary>
    public bool IsUnplanned { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = "Pending";

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Lý do từ chối (bắt buộc khi Manager từ chối)
    /// </summary>
    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public void Submit()
    {
        if (PlannedHours <= 0 && EndTime > StartTime) PlannedHours = (decimal)(EndTime - StartTime).TotalHours;
        DayType = "GENERAL";
        Status = "Pending";
    }

    public void Approve(string approvedBy)
    {
        Status = "Approved"; ApprovedBy = approvedBy; ApprovedAt = DateTime.UtcNow; RejectionReason = null;
    }

    public void Reject(string reason, string rejectedBy)
    {
        Status = "Rejected"; ApprovedBy = rejectedBy; ApprovedAt = DateTime.UtcNow; RejectionReason = reason;
    }

    public void ReconcileWithRecord(AttendanceRecord record)
    {
        ActualHours = Math.Round((decimal)record.ActualOTMins / 60m, 2);
    }
}
