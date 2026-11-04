using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;
using Project.API.Entities;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectMembersController : ControllerBase
{
    private readonly ProjectManagementDbContext _context;

    public ProjectMembersController(ProjectManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectMembers()
    {
        var members = await _context.ProjectMembers
            .Select(m => new
            {
                m.ProjectMemberId,
                m.ProjectId,
                m.EmployeeId,
                m.ProjectRoleId,
                RoleName = m.ProjectRole.RoleName,
                m.JoinDate,
                m.LeaveDate,
                m.Status
            })
            .ToListAsync();

        return Ok(members);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProjectMember(Guid id)
    {
        var member = await _context.ProjectMembers
            .Where(m => m.ProjectMemberId == id)
            .Select(m => new
            {
                m.ProjectMemberId,
                m.ProjectId,
                m.EmployeeId,
                m.ProjectRoleId,
                RoleName = m.ProjectRole.RoleName,
                m.JoinDate,
                m.LeaveDate,
                m.Status
            })
            .FirstOrDefaultAsync();

        if (member == null)
            return NotFound();

        return Ok(member);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProjectMember(
        CreateProjectMemberDto dto)
    {
        var projectExists = await _context.Projects
            .AnyAsync(p => p.ProjectId == dto.ProjectId);

        if (!projectExists)
            return BadRequest("Project does not exist.");

        var roleExists = await _context.ProjectRoles
            .AnyAsync(r => r.ProjectRoleId == dto.ProjectRoleId);

        if (!roleExists)
            return BadRequest("Project role does not exist.");

        var member = new ProjectMember
        {
            ProjectMemberId = Guid.NewGuid(),
            ProjectId = dto.ProjectId,
            EmployeeId = dto.EmployeeId,
            ProjectRoleId = dto.ProjectRoleId,
            JoinDate = dto.JoinDate,
            LeaveDate = dto.LeaveDate,
            Status = dto.Status
        };

        _context.ProjectMembers.Add(member);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProjectMember),
            new { id = member.ProjectMemberId },
            member.ProjectMemberId);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProjectMember(
        Guid id, CreateProjectMemberDto dto)
    {
        var member = await _context.ProjectMembers.FindAsync(id);

        if (member == null)
            return NotFound();

        var projectExists = await _context.Projects
            .AnyAsync(p => p.ProjectId == dto.ProjectId);

        var roleExists = await _context.ProjectRoles
            .AnyAsync(r => r.ProjectRoleId == dto.ProjectRoleId);

        if (!projectExists || !roleExists)
            return BadRequest("Project or project role does not exist.");

        member.ProjectId = dto.ProjectId;
        member.EmployeeId = dto.EmployeeId;
        member.ProjectRoleId = dto.ProjectRoleId;
        member.JoinDate = dto.JoinDate;
        member.LeaveDate = dto.LeaveDate;
        member.Status = dto.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProjectMember(Guid id)
    {
        var member = await _context.ProjectMembers.FindAsync(id);

        if (member == null)
            return NotFound();

        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}