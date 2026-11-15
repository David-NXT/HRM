
namespace Project.API.DTOs;

public class TimesheetResponseDto
{
    public Guid TimesheetId { get; set; }

    public Guid ProjectMemberId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid TaskId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string TaskName { get; set; } = string.Empty;

    public DateTime WorkDate { get; set; }

    public decimal HoursWorked { get; set; }

    public string? Description { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? RejectReason { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid? ApprovedBy { get; set; }
}
