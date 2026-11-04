namespace Attendance.DTOs.LeaveRequest;

public class UpdateLeaveRequestDto
{
    public Guid? EmployeeId { get; set; }

    public string? LeaveType { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
