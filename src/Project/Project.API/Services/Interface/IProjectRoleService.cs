using Project.API.Entities;

namespace Project.API.Services;

public interface IProjectRoleService
{
    Task<IEnumerable<ProjectRole>> GetProjectRolesAsync();
    Task<ProjectRole?> GetProjectRoleAsync(Guid id);
    Task<ProjectRole> CreateProjectRoleAsync(ProjectRole role);
    Task<bool> UpdateProjectRoleAsync(Guid id, ProjectRole input);
    Task<bool> DeleteProjectRoleAsync(Guid id);
}