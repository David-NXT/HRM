
using Microsoft.AspNetCore.Mvc;
using Project.API.DTOs;
using Project.API.Services;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectMembersController : ControllerBase
{
    private readonly IProjectMemberService _memberService;
    private readonly IHrEmployeeDirectoryClient _employees;

    public ProjectMembersController(
        IProjectMemberService memberService,
        IHrEmployeeDirectoryClient employees)
    {
        _memberService = memberService;
        _employees = employees;
    }

    // GET: api/ProjectMembers?projectId={projectId}
    [HttpGet]
    public async Task<IActionResult> GetProjectMembers(
        [FromQuery] Guid? projectId = null)
    {
        var members = await _memberService.GetProjectMembersAsync(projectId);

        var result = members.Select(m => new
        {
            m.ProjectMemberId,
            m.ProjectId,
            m.EmployeeId,
            m.ProjectRoleId,
            RoleName = m.ProjectRole.RoleName,
            m.JoinDate,
            m.LeaveDate,
            m.Status
        });

        return Ok(result);
    }

    // GET: api/ProjectMembers/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProjectMember(Guid id)
    {
        var member = await _memberService.GetProjectMemberAsync(id);

        if (member == null)
            return NotFound();

        return Ok(new
        {
            member.ProjectMemberId,
            member.ProjectId,
            member.EmployeeId,
            member.ProjectRoleId,
            RoleName = member.ProjectRole.RoleName,
            member.JoinDate,
            member.LeaveDate,
            member.Status
        });
    }

    // POST: api/ProjectMembers
    [HttpPost]
    public async Task<IActionResult> CreateProjectMember(
        [FromBody] CreateProjectMemberDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employees.GetActiveEmployeeAsync(
                dto.EmployeeId,
                cancellationToken);

            if (employee is null)
                return BadRequest(
                    "Employee does not exist or is not active in HR.");

            var member = await _memberService.CreateProjectMemberAsync(dto);

            return CreatedAtAction(
                nameof(GetProjectMember),
                new { id = member.ProjectMemberId },
                new { member.ProjectMemberId });
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "HR employee directory is unavailable.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT: api/ProjectMembers/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProjectMember(
        Guid id,
        [FromBody] CreateProjectMemberDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await _memberService.GetProjectMemberAsync(id);

            if (existing == null)
                return NotFound();

            if (existing.EmployeeId != dto.EmployeeId)
            {
                var employee = await _employees.GetActiveEmployeeAsync(
                    dto.EmployeeId,
                    cancellationToken);

                if (employee is null)
                    return BadRequest(
                        "Employee does not exist or is not active in HR.");
            }

            var result = await _memberService.UpdateProjectMemberAsync(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "HR employee directory is unavailable.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE: api/ProjectMembers/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProjectMember(Guid id)
    {
        try
        {
            var result = await _memberService.DeleteProjectMemberAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
