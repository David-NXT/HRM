namespace Project.API.Entities;

public class Timesheet
{
    public Guid TimesheetId { get; set; }
    public Guid ProjectMemberId { get; set; }
    public Guid TaskId { get; set; }
    public DateTime WorkDate { get; set; }
    public decimal HoursWorked { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "SUBMITTED";
    public string? RejectReason { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedBy { get; set; }

    public ProjectMember ProjectMember { get; set; } = null!;
    public Task Task { get; set; } = null!;

    // 1. Gửi Timesheet
    public void Submit()
    {
        Status = "SUBMITTED";
    }

    // 2. Gửi lại Timesheet bị từ chối
    public void Resubmit(decimal hours, string? description)
    {
        if (Status != "REJECTED")
            throw new InvalidOperationException("Chỉ được gửi lại Timesheet bị từ chối.");

        HoursWorked = hours;
        Description = description;
        RejectReason = null;
        Status = "SUBMITTED";
    }

    // 3. Phê duyệt Timesheet
    public void Approve(Guid approverId)
    {
        Status = "APPROVED";
        ApprovedBy = approverId;
        ApprovedAt = DateTime.Now;
    }

    // 4. Từ chối Timesheet
    public void Reject(Guid approverId, string reason)
    {
        Status = "REJECTED";
        ApprovedBy = approverId;
        RejectReason = reason;
    }

    // 5. Kiểm tra đã được duyệt chưa
    public bool IsApproved()
    {
        return Status == "APPROVED";
    }

    // 6. Tính chi phí Timesheet
    public decimal CalculateCost(HourlyRate rate)
    {
        return HoursWorked * rate.Rate;
    }
}