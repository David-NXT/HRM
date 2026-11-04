using Microsoft.EntityFrameworkCore;
using Project.API.Entities;

using ProjectEntity = Project.API.Entities.Project;
using ProjectTask = Project.API.Entities.Task;

namespace Project.API.Data;

public class ProjectManagementDbContext : DbContext
{
    public ProjectManagementDbContext(
        DbContextOptions<ProjectManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProjectEntity> Projects { get; set; }

    public DbSet<ProjectTask> Tasks { get; set; }

    public DbSet<ProjectMember> ProjectMembers { get; set; }

    public DbSet<ProjectRole> ProjectRoles { get; set; }

    public DbSet<HourlyRate> HourlyRates { get; set; }

    public DbSet<Timesheet> Timesheets { get; set; }
}