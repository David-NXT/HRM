
using System.ComponentModel.DataAnnotations;

namespace Project.API.DTOs;

public class UpdateTimesheetDto
{
    [Range(0.01, 24)]
    public decimal HoursWorked { get; set; }

    public string? Description { get; set; }
}
