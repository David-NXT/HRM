namespace Admin.WinForms.BUS.DTOs.Attendance;

public class AttendanceOverviewStatsDto
{
    public DateOnly Date { get; set; }
    public int TotalEmployees { get; set; }
    public int PresentCount { get; set; }
    public int LateCount { get; set; }
    public int AbsentCount { get; set; }
    public int OnLeaveCount { get; set; }
}

public class AttendanceWeeklyTrendDto
{
    public string Name { get; set; } = string.Empty;
    public int Present { get; set; }
    public int Absent { get; set; }
}

public class AttendanceDepartmentTrendDto
{
    public string Name { get; set; } = string.Empty;
    public int Percentage { get; set; }
    public int TotalEmployees { get; set; }
    public int PresentCount { get; set; }
}
