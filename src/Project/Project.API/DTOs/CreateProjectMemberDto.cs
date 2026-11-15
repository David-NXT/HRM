using System.ComponentModel.DataAnnotations;

namespace Project.API.DTOs;

public class CreateProjectMemberDto
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public Guid ProjectRoleId { get; set; }

    public string? ProjectRoleName { get; set; }

    public decimal? NewRoleCoefficient { get; set; }

    public DateTime JoinDate { get; set; }

    public DateTime? LeaveDate { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Active";

    public decimal? InitialHourlyRate { get; set; }
}