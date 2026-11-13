using HR.Domain.Constants;

namespace HR.Domain.Entities;

public abstract class Employee
{
    protected Employee(string employmentType) => EmploymentType = employmentType;

    public string EmploymentType { get; private set; }

    // One identifier is used in HR and by other services.
    public Guid EmployeeId { get; set; } = Guid.NewGuid();

    public string EmployeeCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string CitizenId { get; set; } = string.Empty;

    public string? Address { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? PositionId { get; set; }

    public string Status { get; set; } = EmployeeStatuses.Active;

    public int Version { get; set; } = 1;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Department? Department { get; set; }

    public Position? Position { get; set; }
}
