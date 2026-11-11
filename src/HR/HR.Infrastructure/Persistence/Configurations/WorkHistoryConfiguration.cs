using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class WorkHistoryConfiguration : IEntityTypeConfiguration<WorkHistory>
{
    public void Configure(EntityTypeBuilder<WorkHistory> builder)
    {
        builder.ToTable("work_history");
        builder.HasKey(history => history.WorkHistoryId);
        builder.Property(history => history.WorkHistoryId).HasColumnName("work_history_id").HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(history => history.EmployeeId).HasColumnName("employee_id").IsRequired();
        builder.Property(history => history.DepartmentId).HasColumnName("department_id").IsRequired();
        builder.Property(history => history.PositionId).HasColumnName("position_id").IsRequired();
        builder.Property(history => history.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(history => history.EndDate).HasColumnName("end_date");
        builder.Property(history => history.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(history => history.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.HasIndex(history => history.EmployeeId)
            .IsUnique()
            .HasFilter("end_date IS NULL");
        builder.HasIndex(history => new { history.EmployeeId, history.StartDate });
        builder.HasOne(history => history.Employee)
            .WithMany()
            .HasForeignKey(history => history.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(history => history.Department)
            .WithMany()
            .HasForeignKey(history => history.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(history => history.Position)
            .WithMany()
            .HasForeignKey(history => history.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
