using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class EmployeeNotificationConfiguration : IEntityTypeConfiguration<EmployeeNotification>
{
    public void Configure(EntityTypeBuilder<EmployeeNotification> builder)
    {
        builder.ToTable("employee_notification");
        builder.HasKey(notification => notification.EmployeeNotificationId);
        builder.Property(notification => notification.EmployeeNotificationId).HasColumnName("employee_notification_id").HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(notification => notification.EmployeeId).HasColumnName("employee_id").IsRequired();
        builder.Property(notification => notification.ProfileUpdateRequestId).HasColumnName("profile_update_request_id");
        builder.Property(notification => notification.Title).HasColumnName("title").HasMaxLength(150).IsRequired();
        builder.Property(notification => notification.Message).HasColumnName("message").HasMaxLength(1000).IsRequired();
        builder.Property(notification => notification.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(notification => notification.ReadAt).HasColumnName("read_at");
        builder.HasIndex(notification => new { notification.EmployeeId, notification.CreatedAt });
        builder.HasOne(notification => notification.Employee)
            .WithMany()
            .HasForeignKey(notification => notification.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(notification => notification.ProfileUpdateRequest)
            .WithMany()
            .HasForeignKey(notification => notification.ProfileUpdateRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
