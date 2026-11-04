using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.Entities;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectRolesController : ControllerBase
{
    private readonly ProjectManagementDbContext _context;

    public ProjectRolesController(ProjectManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectRoles()
    {
        var roles = await _context.ProjectRoles
            .Select(r => new
            {
                r.ProjectRoleId,
                r.RoleName,
                r.Description
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProjectRole(Guid id)
    {
        var role = await _context.ProjectRoles
            .Where(r => r.ProjectRoleId == id)
            .Select(r => new
            {
                r.ProjectRoleId,
                r.RoleName,
                r.Description
            })
            .FirstOrDefaultAsync();

        if (role == null)
            return NotFound();

        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProjectRole(
        [FromBody] ProjectRole dto)
    {
        var role = new ProjectRole
        {
            ProjectRoleId = Guid.NewGuid(),
            RoleName = dto.RoleName,
            Description = dto.Description
        };

        _context.ProjectRoles.Add(role);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProjectRole),
            new { id = role.ProjectRoleId },
            new
            {
                role.ProjectRoleId,
                role.RoleName,
                role.Description
            });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProjectRole(
        Guid id, [FromBody] ProjectRole dto)
    {
        var role = await _context.ProjectRoles.FindAsync(id);

        if (role == null)
            return NotFound();

        role.RoleName = dto.RoleName;
        role.Description = dto.Description;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProjectRole(Guid id)
    {
        var role = await _context.ProjectRoles.FindAsync(id);

        if (role == null)
            return NotFound();

        _context.ProjectRoles.Remove(role);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}