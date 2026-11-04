using HR.Core.Employees;

namespace HR.Core.Entities;

public class Employee
{
    public int EmployeeId { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string CitizenId { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string Status { get; set; } = EmployeeStatuses.Active;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
