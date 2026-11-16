namespace Project.DTOs;

internal sealed class EmployeeDirectoryPageDto
{
    public List<EmployeeDirectoryDto> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}

internal sealed class EmployeeDirectoryDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }

    public string DisplayName => $"{EmployeeCode} - {FullName}";
}

internal sealed class ProjectMemberDto
{
    public Guid ProjectMemberId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Status { get; set; } = string.Empty;
}

internal sealed class TaskDto
{
    public Guid TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string Status { get; set; } = string.Empty;
}

internal sealed class CreateTimesheetDto
{
    public Guid ProjectMemberId { get; set; }
    public Guid TaskId { get; set; }
    public DateTime WorkDate { get; set; }
    public decimal HoursWorked { get; set; }
    public string? Description { get; set; }
}

internal sealed class TimesheetResponseDto
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
}

internal sealed class AttendanceWorkHoursDto
{
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public decimal TotalWorkHours { get; set; }
    public int RecordCount { get; set; }
}
