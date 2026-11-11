
using ProjectTask = Project.API.Entities.Task;

namespace Project.API.Entities;

public class Timesheet
{
    public Guid TimesheetId { get; set; }

    public Guid ProjectMemberId { get; set; }

    public Guid TaskId { get; set; }

    public DateTime WorkDate { get; set; }

    public decimal HoursWorked { get; set; }

    public string? Description { get; set; }

    public string Status { get; private set; } = "Pending";

    public string? RejectReason { get; private set; }

    public DateTime? ApprovedAt { get; private set; }

    public Guid? ApprovedBy { get; private set; }

    public ProjectMember ProjectMember { get; set; } = null!;

    public ProjectTask Task { get; set; } = null!;

    // Duyệt Timesheet
    public void Approve(Guid approverId)
    {
        if (Status != "Pending")
            throw new InvalidOperationException(
                "Only pending timesheets can be approved.");

        if (approverId == Guid.Empty)
            throw new ArgumentException("Invalid approver ID.");

        Status = "Approved";
        ApprovedBy = approverId;
        ApprovedAt = DateTime.UtcNow;
        RejectReason = null;
    }

    // Từ chối Timesheet
    public void Reject(Guid approverId, string reason)
    {
        if (Status != "Pending")
            throw new InvalidOperationException(
                "Only pending timesheets can be rejected.");

        if (approverId == Guid.Empty)
            throw new ArgumentException("Invalid approver ID.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "Rejection reason is required.");

        Status = "Rejected";
        RejectReason = reason.Trim();
        ApprovedBy = null;
        ApprovedAt = null;
    }

    // Gửi lại Timesheet sau khi bị từ chối
    public void Resubmit(decimal hoursWorked, string? description)
    {
        if (Status != "Rejected")
            throw new InvalidOperationException(
                "Only rejected timesheets can be resubmitted.");

        if (hoursWorked <= 0 || hoursWorked > 24)
            throw new InvalidOperationException(
                "Hours worked must be greater than 0 and not exceed 24.");

        HoursWorked = hoursWorked;
        Description = description;
        Status = "Pending";

        RejectReason = null;
        ApprovedAt = null;
        ApprovedBy = null;
    }

    public bool IsApproved()
    {
        return Status == "Approved";
    }

    public decimal CalculateCost(HourlyRate rate)
    {
        if (!IsApproved())
            throw new InvalidOperationException(
                "Only approved timesheets can calculate cost.");

        if (!rate.IsEffectiveAt(WorkDate))
            throw new InvalidOperationException(
                "Hourly rate is not effective on work date.");

        return HoursWorked * rate.Rate;
    }
}
