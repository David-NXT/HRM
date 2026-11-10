using System.ComponentModel.DataAnnotations;

namespace Attendance.DTOs.LeaveRequest;

public class CreateLeaveRequestDto
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public string LeaveType { get; set; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    public decimal TotalDays { get; set; } = 1.0m;

    [Required]
    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public string? ApprovedBy { get; set; }
}
