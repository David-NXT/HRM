using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;
using Project.API.Entities;

namespace Project.API.Services;

public class ProjectMemberService : IProjectMemberService
{
    private readonly ProjectManagementDbContext _context;
    private readonly IHrEmployeeDirectoryClient _employees;

    public ProjectMemberService(ProjectManagementDbContext context, IHrEmployeeDirectoryClient employees)
    {
        _context = context;
        _employees = employees;
    }

    public async Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(Guid? projectId = null)
    {
        var query = _context.ProjectMembers
            .AsNoTracking()
            .Include(m => m.ProjectRole)
            .AsQueryable();

        if (projectId.HasValue)
            query = query.Where(m => m.ProjectId == projectId.Value);

        return await query.ToListAsync();
    }

    public async Task<ProjectMember?> GetProjectMemberAsync(Guid id)
    {
        return await _context.ProjectMembers
            .AsNoTracking()
            .Include(m => m.ProjectRole)
            .FirstOrDefaultAsync(m => m.ProjectMemberId == id);
    }

    public async Task<ProjectMember> CreateProjectMemberAsync(CreateProjectMemberDto dto)
    {
        await EnsureAssignableEmployeeAsync(dto.EmployeeId);
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

        return member;
    }

    public async Task<bool> UpdateProjectMemberAsync(Guid id, CreateProjectMemberDto dto)
    {
        var member = await _context.ProjectMembers.FindAsync(id);

        if (member == null)
            return false;

        await EnsureAssignableEmployeeAsync(dto.EmployeeId);
        member.ProjectId = dto.ProjectId;
        member.EmployeeId = dto.EmployeeId;
        member.ProjectRoleId = dto.ProjectRoleId;
        member.JoinDate = dto.JoinDate;
        member.LeaveDate = dto.LeaveDate;
        member.Status = dto.Status;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProjectMemberAsync(Guid id)
    {
        var member = await _context.ProjectMembers.FindAsync(id);

        if (member == null)
            return false;

        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();

        return true;
    }

    private async System.Threading.Tasks.Task EnsureAssignableEmployeeAsync(Guid employeeId)
    {
        if (await _employees.GetActiveEmployeeAsync(employeeId, CancellationToken.None) is null)
            throw new ArgumentException("Employee does not exist or is not active in HR.", nameof(employeeId));
    }
}
