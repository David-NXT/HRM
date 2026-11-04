namespace Project.API.DTOs;

public class TimesheetResponseDto
{
    public Guid TimesheetId { get; set; }

    public Guid ProjectMemberId { get; set; }

    public Guid TaskId { get; set; }

    public DateTime WorkDate { get; set; }

    public decimal HoursWorked { get; set; }

    public string? Description { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? ApprovedAt { get; set; }

    public Guid? ApprovedBy { get; set; }
}