namespace Project.API.Entities;
public class Task
{
    public Guid TaskId { get; set; }

    public string TaskName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? DueDate { get; set; }

    public string Status { get; set; } = "ToDo";

    public string Priority { get; set; } = "Medium";

    public Guid ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public ICollection<Timesheet> Timesheets { get; set; }
        = new List<Timesheet>();
}