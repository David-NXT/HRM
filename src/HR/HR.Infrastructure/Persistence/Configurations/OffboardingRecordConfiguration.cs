using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class OffboardingRecordConfiguration : IEntityTypeConfiguration<OffboardingRecord>
{
    public void Configure(EntityTypeBuilder<OffboardingRecord> builder)
    {
        builder.ToTable("offboarding_record");
        builder.HasKey(record => record.OffboardingRecordId);
        builder.Property(record => record.OffboardingRecordId).HasColumnName("offboarding_record_id").HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(record => record.EmployeeId).HasColumnName("employee_id").IsRequired();
        builder.Property(record => record.LastWorkingDate).HasColumnName("last_working_date").IsRequired();
        builder.Property(record => record.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(record => record.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(record => record.Version).HasColumnName("version").HasDefaultValue(1).IsConcurrencyToken();
        builder.Property(record => record.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(record => record.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(record => record.CompletedAt).HasColumnName("completed_at");
        builder.HasIndex(record => record.EmployeeId).IsUnique()
            .HasFilter("status = 'IN_PROGRESS'")
            .HasDatabaseName("UX_offboarding_employee_in_progress");
        builder.HasIndex(record => new { record.Status, record.LastWorkingDate });
        builder.HasOne(record => record.Employee).WithMany()
            .HasForeignKey(record => record.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(record => record.Checklist).WithOne(item => item.OffboardingRecord)
            .HasForeignKey(item => item.OffboardingRecordId).OnDelete(DeleteBehavior.Cascade);
    }
}
