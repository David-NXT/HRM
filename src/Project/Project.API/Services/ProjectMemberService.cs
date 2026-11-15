
using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;
using Project.API.Entities;
using ProjectEntity = Project.API.Entities.Project;
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
        if (dto.InitialHourlyRate is <= 0m)
        {
            throw new ArgumentException(
                "Hourly rate must be greater than 0.");
        }
        if (dto.NewRoleCoefficient is < 0m or > 9.9999m)
        {
            throw new ArgumentException(
                "Role coefficient must be between 0 and 9.9999.");
        }

        var joinDate = DateTimeNormalizer.ToUtc(dto.JoinDate);
        var leaveDate = dto.LeaveDate.HasValue
            ? DateTimeNormalizer.ToUtc(dto.LeaveDate.Value)
            : (DateTime?)null;

        // 1. Kiểm tra nhân viên từ HR Service trước
        var employee = await _employees.GetActiveEmployeeAsync(
            dto.EmployeeId,
            CancellationToken.None);

        if (employee == null)
        {
            throw new ArgumentException(
                "Employee does not exist or is not active.");
        }

        // 2. Kiểm tra dự án và ngày thành viên
        var project = await EnsureProjectEditableAsync(dto.ProjectId);
        ValidateMemberDates(joinDate, leaveDate, project.EndDate);

        // 4. Reuse an existing role name or prepare a new role.
        var projectRoleId = dto.ProjectRoleId;
        ProjectRole? newRole = null;
        if (!string.IsNullOrWhiteSpace(dto.ProjectRoleName))
        {
            var roleName = dto.ProjectRoleName.Trim();
            var existingRole = await _context.ProjectRoles
                .FirstOrDefaultAsync(role => role.RoleName.ToLower() == roleName.ToLower());
            if (existingRole is not null)
            {
                projectRoleId = existingRole.ProjectRoleId;
            }
            else
            {
                if (!dto.NewRoleCoefficient.HasValue)
                {
                    throw new ArgumentException(
                        "A role coefficient is required when creating a new project role.");
                }

                newRole = new ProjectRole
                {
                    ProjectRoleId = Guid.NewGuid(),
                    RoleName = roleName,
                    RoleCoefficient = dto.NewRoleCoefficient.Value
                };
                projectRoleId = newRole.ProjectRoleId;
            }
        }
        else if (!await _context.ProjectRoles
            .AnyAsync(role => role.ProjectRoleId == projectRoleId))
        {
            throw new ArgumentException("Project role does not exist.");
        }

        // 5. Kiểm tra thành viên bị trùng thời gian
        var hasOverlap = await HasOverlappingMembershipAsync(
            dto.ProjectId,
            dto.EmployeeId,
            joinDate,
            leaveDate);

        if (hasOverlap)
            throw new InvalidOperationException(
                "Employee already has an overlapping membership in this project.");

        if (newRole is not null)
        {
            _context.ProjectRoles.Add(newRole);
        }

        // 6. Tạo ProjectMember
        var member = new ProjectMember
        {
            ProjectMemberId = Guid.NewGuid(),
            ProjectId = dto.ProjectId,
            EmployeeId = dto.EmployeeId,
            ProjectRoleId = projectRoleId,
            JoinDate = joinDate,
            LeaveDate = leaveDate,
            Status = dto.Status
        };

        _context.ProjectMembers.Add(member);
        if (dto.InitialHourlyRate.HasValue)
        {
            _context.HourlyRates.Add(new HourlyRate
            {
                HourlyRateId = Guid.NewGuid(),
                ProjectMemberId = member.ProjectMemberId,
                Rate = dto.InitialHourlyRate.Value,
                EffectiveFrom = joinDate
            });
        }
        await _context.SaveChangesAsync();

        return member;
    }

    public async Task<bool> UpdateProjectMemberAsync(
        Guid id,
        CreateProjectMemberDto dto)
    {
        if (dto.InitialHourlyRate is <= 0m)
        {
            throw new ArgumentException(
                "Hourly rate must be greater than 0.");
        }
        if (dto.NewRoleCoefficient is < 0m or > 9.9999m)
        {
            throw new ArgumentException(
                "Role coefficient must be between 0 and 9.9999.");
        }

        var member = await _context.ProjectMembers.FindAsync(id);

        if (member == null)
            return false;

        var joinDate = DateTimeNormalizer.ToUtc(dto.JoinDate);
        var leaveDate = dto.LeaveDate.HasValue
            ? DateTimeNormalizer.ToUtc(dto.LeaveDate.Value)
            : (DateTime?)null;

        var project = await EnsureProjectEditableAsync(member.ProjectId);
        if (member.ProjectId != dto.ProjectId)
        {
            project = await EnsureProjectEditableAsync(dto.ProjectId);
        }

        ValidateMemberDates(joinDate, leaveDate, project.EndDate);

        var projectRoleId = dto.ProjectRoleId;
        ProjectRole? newRole = null;
        if (!string.IsNullOrWhiteSpace(dto.ProjectRoleName))
        {
            var roleName = dto.ProjectRoleName.Trim();
            var existingRole = await _context.ProjectRoles
                .FirstOrDefaultAsync(role => role.RoleName.ToLower() == roleName.ToLower());
            if (existingRole is not null)
            {
                projectRoleId = existingRole.ProjectRoleId;
            }
            else
            {
                if (!dto.NewRoleCoefficient.HasValue)
                {
                    throw new ArgumentException(
                        "A role coefficient is required when creating a new project role.");
                }

                newRole = new ProjectRole
                {
                    ProjectRoleId = Guid.NewGuid(),
                    RoleName = roleName,
                    RoleCoefficient = dto.NewRoleCoefficient.Value
                };
                projectRoleId = newRole.ProjectRoleId;
            }
        }
        else if (!await _context.ProjectRoles
            .AnyAsync(role => role.ProjectRoleId == projectRoleId))
        {
            throw new ArgumentException("Project role does not exist.");
        }

        var hasTimesheets = await _context.Timesheets
            .AnyAsync(t => t.ProjectMemberId == id);

        var hasHourlyRates = await _context.HourlyRates
            .AnyAsync(r => r.ProjectMemberId == id);

        // Project and employee identity remain fixed when history is attached.
        if (hasTimesheets || hasHourlyRates)
        {
            if (member.ProjectId != dto.ProjectId ||
                member.EmployeeId != dto.EmployeeId)
            {
                throw new InvalidOperationException(
                    "Cannot change project or employee because this member already has related records.");
            }
        }

        var hasOverlap = await HasOverlappingMembershipAsync(
            dto.ProjectId,
            dto.EmployeeId,
            joinDate,
            leaveDate,
            id);

        if (hasOverlap)
            throw new InvalidOperationException(
                "Employee already has an overlapping membership in this project.");

        if (leaveDate.HasValue)
        {
            var hasWorkAfterLeaveDate = await _context.Timesheets
                .AnyAsync(t =>
                    t.ProjectMemberId == id &&
                    t.WorkDate.Date > leaveDate.Value.Date);

            if (hasWorkAfterLeaveDate)
            {
                throw new InvalidOperationException(
                    "LeaveDate cannot be earlier than an existing timesheet work date.");
            }
        }

        DateTime? hourlyRateEffectiveFrom = null;
        if (dto.InitialHourlyRate.HasValue)
        {
            if (!dto.HourlyRateEffectiveFrom.HasValue)
            {
                throw new ArgumentException(
                    "An effective date is required when changing the hourly rate.");
            }

            hourlyRateEffectiveFrom = DateTimeNormalizer.ToUtc(
                dto.HourlyRateEffectiveFrom.Value);
            if (hourlyRateEffectiveFrom.Value.Date < joinDate.Date ||
                (leaveDate.HasValue &&
                 hourlyRateEffectiveFrom.Value.Date > leaveDate.Value.Date))
            {
                throw new ArgumentException(
                    "The hourly rate effective date must fall within the member's date range.");
            }
        }

        member.ProjectId = dto.ProjectId;
        member.EmployeeId = dto.EmployeeId;
        member.ProjectRoleId = projectRoleId;
        member.JoinDate = joinDate;
        member.LeaveDate = leaveDate;
        member.Status = dto.Status;

        if (newRole is not null)
        {
            _context.ProjectRoles.Add(newRole);
        }

        if (dto.InitialHourlyRate is { } hourlyRate &&
            hourlyRateEffectiveFrom is { } effectiveFrom)
        {
            await ApplyHourlyRateChangeAsync(
                id,
                hourlyRate,
                effectiveFrom);
        }

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task ApplyHourlyRateChangeAsync(
        Guid projectMemberId,
        decimal rate,
        DateTime effectiveFrom)
    {
        var rates = await _context.HourlyRates
            .Where(item => item.ProjectMemberId == projectMemberId)
            .OrderBy(item => item.EffectiveFrom)
            .ToListAsync();

        var sameStart = rates.FirstOrDefault(item =>
            item.EffectiveFrom.Date == effectiveFrom.Date);
        if (sameStart is not null)
        {
            sameStart.Rate = rate;
            return;
        }

        var coveringRate = rates.FirstOrDefault(item =>
            item.EffectiveFrom.Date < effectiveFrom.Date &&
            (!item.EffectiveTo.HasValue ||
             item.EffectiveTo.Value.Date >= effectiveFrom.Date));
        DateTime? effectiveTo;
        if (coveringRate is not null)
        {
            effectiveTo = coveringRate.EffectiveTo;
            coveringRate.EffectiveTo = effectiveFrom.AddDays(-1);
        }
        else
        {
            var nextRate = rates.FirstOrDefault(item =>
                item.EffectiveFrom.Date > effectiveFrom.Date);
            effectiveTo = nextRate?.EffectiveFrom.Date.AddDays(-1);
        }

        _context.HourlyRates.Add(new HourlyRate
        {
            HourlyRateId = Guid.NewGuid(),
            ProjectMemberId = projectMemberId,
            Rate = rate,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo
        });
    }

    public async Task<bool> DeleteProjectMemberAsync(Guid id)
    {
        var member = await _context.ProjectMembers.FindAsync(id);

        if (member == null)
            return false;

        await EnsureProjectEditableAsync(member.ProjectId);
        var hasTimesheets = await _context.Timesheets
            .AnyAsync(t => t.ProjectMemberId == id);

        if (hasTimesheets)
        {
            throw new InvalidOperationException(
                "Cannot delete a project member that has timesheets. Set the member status to Inactive instead.");
        }

        var hourlyRates = await _context.HourlyRates
            .Where(r => r.ProjectMemberId == id)
            .ToListAsync();
        _context.HourlyRates.RemoveRange(hourlyRates);
        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateMemberDates(
        DateTime joinDate,
        DateTime? leaveDate,
        DateOnly? projectEndDate)
    {
        if (!leaveDate.HasValue)
        {
            throw new ArgumentException("LeaveDate is required.");
        }

        if (!projectEndDate.HasValue)
        {
            throw new ArgumentException(
                "Project end date must be set before assigning members.");
        }

        if (leaveDate.Value.Date < joinDate.Date)
        {
            throw new ArgumentException(
                "LeaveDate cannot be earlier than JoinDate.");
        }

        var endDate = projectEndDate.Value;
        if (DateOnly.FromDateTime(joinDate) > endDate ||
            DateOnly.FromDateTime(leaveDate.Value) > endDate)
        {
            throw new ArgumentException(
                "Member dates cannot be later than the project end date.");
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
    private async Task<ProjectEntity> EnsureProjectEditableAsync(Guid projectId)
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

        return project;
    }
}
