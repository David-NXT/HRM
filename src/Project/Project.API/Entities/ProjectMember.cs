namespace Project.API.Entities;
public class ProjectMember
{
    public Guid ProjectMemberId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid ProjectRoleId { get; set; }

    public DateTime JoinDate { get; set; }

    public DateTime? LeaveDate { get; set; }

    public string Status { get; set; } = "Active";

    public Project Project { get; set; } = null!;

    public ProjectRole ProjectRole { get; set; } = null!;

    public ICollection<HourlyRate> HourlyRates { get; set; }
        = new List<HourlyRate>();

    public ICollection<Timesheet> Timesheets { get; set; }
        = new List<Timesheet>();
}