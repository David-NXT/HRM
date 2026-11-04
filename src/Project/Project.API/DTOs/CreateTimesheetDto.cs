using System.ComponentModel.DataAnnotations;

namespace Project.API.DTOs;

public class CreateTimesheetDto
{
    [Required]
    public Guid ProjectMemberId { get; set; }

    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public DateTime WorkDate { get; set; }

    [Range(0.01, 24)]
    public decimal HoursWorked { get; set; }

    public string? Description { get; set; }
}