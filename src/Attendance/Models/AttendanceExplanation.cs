using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models;

public class AttendanceExplanation
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Attendance))]
    public Guid? AttendanceId { get; set; }

    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Ngày cần giải trình
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Loại giải trình: Quên Check-in, Quên Check-out, Đi trễ, Về sớm
    /// </summary>
    public string ExplanationType { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public TimeOnly? RequestedTimeIn { get; set; }

    public TimeOnly? RequestedTimeOut { get; set; }

    /// <summary>
    /// Đường dẫn minh chứng chính (hoặc hỗ trợ 1 link đơn giản)
    /// </summary>
    public string? AttachmentUrl { get; set; }

    public string Status { get; set; } = "Pending";

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Lý do từ chối (bắt buộc khi Manager từ chối đơn)
    /// </summary>
    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public AttendanceRecord? Attendance { get; set; }

    /// <summary>
    /// Danh sách các file minh chứng đính kèm (cho phép nhiều file)
    /// </summary>
    public ICollection<AttendanceExplanationAttachment>? Attachments { get; set; }

    public void Submit() => Status = "Pending";
    public void Approve(string approvedBy) { Status = "Approved"; ApprovedBy = approvedBy; ApprovedAt = DateTime.UtcNow; RejectionReason = null; }
    public void Reject(string reason, string rejectedBy) { Status = "Rejected"; ApprovedBy = rejectedBy; ApprovedAt = DateTime.UtcNow; RejectionReason = reason; }
}
