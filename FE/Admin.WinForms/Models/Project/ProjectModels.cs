namespace Admin.WinForms.Models.Project;

internal sealed class ProjectDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Budget { get; set; }
}

internal sealed class ApprovedPayrollDataDto
{
    public Guid TimesheetId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid TaskId { get; set; }
    public DateTime WorkDate { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal RoleCoefficient { get; set; }
    public decimal TotalProjectIncome { get; set; }
}

internal sealed class TaskDto
{
    public Guid TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
}

internal sealed class ProjectMemberDto
{
    public Guid ProjectMemberId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ProjectRoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public DateTime? LeaveDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

internal sealed class ProjectRoleDto
{
    public Guid ProjectRoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal RoleCoefficient { get; set; }
}

internal sealed class TimesheetDto
{
    public Guid TimesheetId { get; set; }
    public Guid ProjectMemberId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid TaskId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public decimal HoursWorked { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectReason { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedBy { get; set; }
}

internal sealed class EmployeeDirectoryDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

internal sealed class EmployeeDirectoryPageDto
{
    public List<EmployeeDirectoryDto> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}

internal sealed class CreateTimesheetRequest
{
    public Guid ProjectMemberId { get; set; }
    public Guid TaskId { get; set; }
    public DateTime WorkDate { get; set; }
    public decimal HoursWorked { get; set; }
    public string? Description { get; set; }
}

internal sealed class UpdateTimesheetRequest
{
    public decimal HoursWorked { get; set; }
    public string? Description { get; set; }
}

internal sealed class ProjectMemberCreateResultDto
{
    public Guid ProjectMemberId { get; set; }
}
