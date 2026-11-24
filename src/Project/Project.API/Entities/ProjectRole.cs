namespace Project.API.Entities;

public class ProjectRole
{
    public Guid ProjectRoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal RoleCoefficient { get; set; }

    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();

    // 1. Cập nhật hệ số vai trò
    public void UpdateCoefficient(decimal value)
    {
        RoleCoefficient = value;
    }
}