using HR.Domain.Constants;
namespace HR.Domain.Entities;

public sealed class Department
{
    public Guid DepartmentId { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentDepartmentId { get; set; }
    public string Status { get; set; } = OrgStatuses.Active;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Department? ParentDepartment { get; set; }
    public List<Department> ChildDepartments { get; set; } = [];
}
