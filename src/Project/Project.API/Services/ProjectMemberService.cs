
using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;
using Project.API.Entities;
using Task = System.Threading.Tasks.Task;
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
        // 1. Kiểm tra nhân viên từ HR Service trước
        var employee = await _employees.GetActiveEmployeeAsync(
            dto.EmployeeId,
            CancellationToken.None);

        if (employee == null)
        {
            throw new ArgumentException(
                "Employee does not exist or is not active.");
        }

        // 2. Kiểm tra ngày tham gia
        ValidateMemberDates(dto.JoinDate, dto.LeaveDate);

        // 3. Kiểm tra dự án tồn tại và được phép chỉnh sửa
        await EnsureProjectEditableAsync(dto.ProjectId);

        // 4. Kiểm tra ProjectRole
        var roleExists = await _context.ProjectRoles
            .AnyAsync(r => r.ProjectRoleId == dto.ProjectRoleId);

        if (!roleExists)
            throw new ArgumentException("Project role does not exist.");

        // 5. Kiểm tra thành viên bị trùng thời gian
        var hasOverlap = await HasOverlappingMembershipAsync(
            dto.ProjectId,
            dto.EmployeeId,
            dto.JoinDate,
            dto.LeaveDate);

        if (hasOverlap)
            throw new InvalidOperationException(
                "Employee already has an overlapping membership in this project.");

        // 6. Tạo ProjectMember
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
        
        // Kiểm tra dự án hiện tại của thành viên
await EnsureProjectEditableAsync(member.ProjectId);

// Kiểm tra dự án mới nếu chuyển thành viên sang dự án khác
if (member.ProjectId != dto.ProjectId)
{
    await EnsureProjectEditableAsync(dto.ProjectId);
}

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

        await EnsureProjectEditableAsync(member.ProjectId);
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
    private async Task EnsureProjectEditableAsync(Guid projectId)
{
    var project = await _context.Projects
        .FirstOrDefaultAsync(p => p.ProjectId == projectId);

    if (project == null)
        throw new ArgumentException("Project does not exist.");

    if (project.Status == "Completed" ||
        project.Status == "Cancelled")
    {
        throw new InvalidOperationException(
            "Không thể thay đổi thành viên của dự án đã hoàn thành hoặc đã hủy.");
    }
}
}
