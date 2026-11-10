using System.ComponentModel.DataAnnotations;

namespace Project.API.DTOs;

public class CreateTaskDto
{
    [Required]
    [MaxLength(200)]
    public string TaskName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? DueDate { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    [MaxLength(50)]
    public string Priority { get; set; } = "Medium";

    [Required]
    public Guid ProjectId { get; set; }
}