
using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;
using Project.API.Entities;

namespace Project.API.Services;

public class ProjectMemberService : IProjectMemberService
{
    private readonly ProjectManagementDbContext _context;

    public ProjectMemberService(ProjectManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(
        Guid? projectId = null)
    {
        var query = _context.ProjectMembers
            .AsNoTracking()
            .Include(m => m.ProjectRole)
            .AsQueryable();

        if (projectId.HasValue)
            query = query.Where(m => m.ProjectId == projectId.Value);

        return await query
            .OrderBy(m => m.JoinDate)
            .ToListAsync();
    }

    public async Task<ProjectMember?> GetProjectMemberAsync(Guid id)
    {
        return await _context.ProjectMembers
            .AsNoTracking()
            .Include(m => m.ProjectRole)
            .FirstOrDefaultAsync(m => m.ProjectMemberId == id);
    }

    public async Task<ProjectMember> CreateProjectMemberAsync(
        CreateProjectMemberDto dto)
    {
        ValidateMemberDates(dto.JoinDate, dto.LeaveDate);

        var projectExists = await _context.Projects
            .AnyAsync(p => p.ProjectId == dto.ProjectId);

        if (!projectExists)
            throw new ArgumentException("Project does not exist.");

        var roleExists = await _context.ProjectRoles
            .AnyAsync(r => r.ProjectRoleId == dto.ProjectRoleId);

        if (!roleExists)
            throw new ArgumentException("Project role does not exist.");

        var hasOverlap = await HasOverlappingMembershipAsync(
            dto.ProjectId,
            dto.EmployeeId,
            dto.JoinDate,
            dto.LeaveDate);

        if (hasOverlap)
            throw new InvalidOperationException(
                "Employee already has an overlapping membership in this project.");

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

    public async Task<bool> UpdateProjectMemberAsync(
        Guid id,
        CreateProjectMemberDto dto)
    {
        var member = await _context.ProjectMembers.FindAsync(id);

        if (member == null)
            return false;

        ValidateMemberDates(dto.JoinDate, dto.LeaveDate);

        var projectExists = await _context.Projects
            .AnyAsync(p => p.ProjectId == dto.ProjectId);

        var roleExists = await _context.ProjectRoles
            .AnyAsync(r => r.ProjectRoleId == dto.ProjectRoleId);

        if (!projectExists)
            throw new ArgumentException("Project does not exist.");

        if (!roleExists)
            throw new ArgumentException("Project role does not exist.");

        var hasTimesheets = await _context.Timesheets
            .AnyAsync(t => t.ProjectMemberId == id);

        var hasHourlyRates = await _context.HourlyRates
            .AnyAsync(r => r.ProjectMemberId == id);

        // Không thay đổi thông tin định danh nếu đã có dữ liệu liên quan.
        if (hasTimesheets || hasHourlyRates)
        {
            if (member.ProjectId != dto.ProjectId ||
                member.EmployeeId != dto.EmployeeId ||
                member.ProjectRoleId != dto.ProjectRoleId ||
                member.JoinDate != dto.JoinDate)
            {
                throw new InvalidOperationException(
                    "Cannot change project, employee, role or join date because this member already has related records.");
            }
        }

        var hasOverlap = await HasOverlappingMembershipAsync(
            dto.ProjectId,
            dto.EmployeeId,
            dto.JoinDate,
            dto.LeaveDate,
            id);

        if (hasOverlap)
            throw new InvalidOperationException(
                "Employee already has an overlapping membership in this project.");

        if (dto.LeaveDate.HasValue)
        {
            var hasWorkAfterLeaveDate = await _context.Timesheets
                .AnyAsync(t =>
                    t.ProjectMemberId == id &&
                    t.WorkDate.Date > dto.LeaveDate.Value.Date);

            if (hasWorkAfterLeaveDate)
            {
                throw new InvalidOperationException(
                    "LeaveDate cannot be earlier than an existing timesheet work date.");
            }
        }

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

        var hasTimesheets = await _context.Timesheets
            .AnyAsync(t => t.ProjectMemberId == id);

        var hasHourlyRates = await _context.HourlyRates
            .AnyAsync(r => r.ProjectMemberId == id);

        if (hasTimesheets || hasHourlyRates)
        {
            throw new InvalidOperationException(
                "Cannot delete a project member that has timesheets or hourly rates.");
        }

        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateMemberDates(
        DateTime joinDate,
        DateTime? leaveDate)
    {
        if (leaveDate.HasValue &&
            leaveDate.Value.Date < joinDate.Date)
        {
            throw new ArgumentException(
                "LeaveDate cannot be earlier than JoinDate.");
        }
    }

    private async Task<bool> HasOverlappingMembershipAsync(
        Guid projectId,
        Guid employeeId,
        DateTime joinDate,
        DateTime? leaveDate,
        Guid? excludeMemberId = null)
    {
        var query = _context.ProjectMembers.Where(m =>
            m.ProjectId == projectId &&
            m.EmployeeId == employeeId);

        if (excludeMemberId.HasValue)
        {
            query = query.Where(m =>
                m.ProjectMemberId != excludeMemberId.Value);
        }

        return await query.AnyAsync(m =>
            (!leaveDate.HasValue ||
             m.JoinDate.Date <= leaveDate.Value.Date) &&
            (!m.LeaveDate.HasValue ||
             m.LeaveDate.Value.Date >= joinDate.Date));
    }
}
