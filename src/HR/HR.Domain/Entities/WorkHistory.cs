namespace HR.Domain.Entities;

public sealed class WorkHistory
{
    public Guid WorkHistoryId { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid PositionId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public Employee? Employee { get; set; }
    public Department? Department { get; set; }
    public Position? Position { get; set; }
}
