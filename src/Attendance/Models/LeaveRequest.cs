namespace Attendance.Models;

public class LeaveRequest
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string LeaveType { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Số ngày xin nghỉ phép
    /// </summary>
    public decimal TotalDays { get; set; } = 1.0m;

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Lý do từ chối (bắt buộc khi trạng thái là Rejected)
    /// </summary>
    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public void Submit() => Status = "Pending";
    public void Approve(string approvedBy) { Status = "Approved"; ApprovedBy = approvedBy; ApprovedAt = DateTime.UtcNow; RejectionReason = null; }
    public void Reject(string reason, string rejectedBy) { Status = "Rejected"; ApprovedBy = rejectedBy; ApprovedAt = DateTime.UtcNow; RejectionReason = reason; }
    public decimal CalculateLeaveDays() => Math.Max(1, EndDate.DayNumber - StartDate.DayNumber + 1);
}
