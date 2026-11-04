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

    public string Status { get; set; } = "Pending";

    public DateTime? ApprovedAt { get; set; }

    public Guid? ApprovedBy { get; set; }

    // Navigation Properties
    public ProjectMember ProjectMember { get; set; } = null!;

    public ProjectTask Task { get; set; } = null!;
}