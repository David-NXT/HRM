namespace Project.API.DTOs;

public class TaskResponseDto
{
    public Guid TaskId { get; set; }

    public string TaskName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? DueDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public Guid ProjectId { get; set; }
}