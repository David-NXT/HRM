namespace Project.API.Entities;
public class Project
{
    public Guid ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Status { get; set; } = "Planning";

    public decimal Budget { get; set; }

    public ICollection<Task> Tasks { get; set; }
        = new List<Task>();

    public ICollection<ProjectMember> Members { get; set; }
        = new List<ProjectMember>();
}