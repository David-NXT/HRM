using System.ComponentModel.DataAnnotations;

namespace Project.API.DTOs;

public class UpdateProjectDto
{
    [Required]
    [MaxLength(200)]
    public string ProjectName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Budget { get; set; }
}