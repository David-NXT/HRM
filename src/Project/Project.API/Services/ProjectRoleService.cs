using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.Entities;

namespace Project.API.Services;

public class ProjectRoleService : IProjectRoleService
{
    private readonly ProjectManagementDbContext _context;

    public ProjectRoleService(ProjectManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectRole>> GetProjectRolesAsync()
    {
        return await _context.ProjectRoles
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProjectRole?> GetProjectRoleAsync(Guid id)
    {
        return await _context.ProjectRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.ProjectRoleId == id);
    }

    public async Task<ProjectRole> CreateProjectRoleAsync(ProjectRole role)
    {
        role.ProjectRoleId = Guid.NewGuid();

        _context.ProjectRoles.Add(role);
        await _context.SaveChangesAsync();

        return role;
    }

    public async Task<bool> UpdateProjectRoleAsync(Guid id, ProjectRole input)
    {
        var role = await _context.ProjectRoles.FindAsync(id);

        if (role == null)
            return false;

        role.RoleName = input.RoleName;
        role.Description = input.Description;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProjectRoleAsync(Guid id)
    {
        var role = await _context.ProjectRoles.FindAsync(id);

        if (role == null)
            return false;

        _context.ProjectRoles.Remove(role);
        await _context.SaveChangesAsync();

        return true;
    }
}