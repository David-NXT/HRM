namespace Project.API.DTOs;

public class ProjectResponseDto
{
    public Guid ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal Budget { get; set; }
}