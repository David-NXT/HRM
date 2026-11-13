
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

    // GET: api/ProjectRoles
    [HttpGet]
    public async Task<IActionResult> GetProjectRoles()
    {
        var roles = await _context.ProjectRoles
            .AsNoTracking()
            .Select(r => new
            {
                r.ProjectRoleId,
                r.RoleName,
                r.Description,
                r.RoleCoefficient
            })
            .ToListAsync();

        return Ok(roles);
    }

    // GET: api/ProjectRoles/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProjectRole(Guid id)
    {
        var role = await _context.ProjectRoles
            .AsNoTracking()
            .Where(r => r.ProjectRoleId == id)
            .Select(r => new
            {
                r.ProjectRoleId,
                r.RoleName,
                r.Description,
                r.RoleCoefficient
            })
            .FirstOrDefaultAsync();

        if (role == null)
            return NotFound();

        return Ok(role);
    }

    // POST: api/ProjectRoles
    [HttpPost]
    public async Task<IActionResult> CreateProjectRole(
        [FromBody] ProjectRole dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RoleName))
            return BadRequest("RoleName is required.");

        if (dto.RoleCoefficient < 0)
            return BadRequest(
                "RoleCoefficient cannot be negative.");

        var roleName = dto.RoleName.Trim();

        var exists = await _context.ProjectRoles
            .AnyAsync(r => r.RoleName.ToLower() == roleName.ToLower());

        if (exists)
            return Conflict("RoleName already exists.");

        var role = new ProjectRole
        {
            ProjectRoleId = Guid.NewGuid(),
            RoleName = roleName,
            Description = dto.Description,
            RoleCoefficient = dto.RoleCoefficient
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
                role.Description,
                role.RoleCoefficient
            });
    }

    // PUT: api/ProjectRoles/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProjectRole(
        Guid id,
        [FromBody] ProjectRole dto)
    {
        var role = await _context.ProjectRoles.FindAsync(id);

        if (role == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.RoleName))
            return BadRequest("RoleName is required.");

        if (dto.RoleCoefficient < 0)
            return BadRequest(
                "RoleCoefficient cannot be negative.");

        var roleName = dto.RoleName.Trim();

        var exists = await _context.ProjectRoles
            .AnyAsync(r =>
                r.ProjectRoleId != id &&
                r.RoleName.ToLower() == roleName.ToLower());

        if (exists)
            return Conflict("RoleName already exists.");

        role.RoleName = roleName;
        role.Description = dto.Description;
        role.RoleCoefficient = dto.RoleCoefficient;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/ProjectRoles/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProjectRole(Guid id)
    {
        var role = await _context.ProjectRoles.FindAsync(id);

        if (role == null)
            return NotFound();

        var isBeingUsed = await _context.ProjectMembers
            .AnyAsync(m => m.ProjectRoleId == id);

        if (isBeingUsed)
        {
            return Conflict(
                "Cannot delete a project role that is being used by project members.");
        }

        _context.ProjectRoles.Remove(role);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
