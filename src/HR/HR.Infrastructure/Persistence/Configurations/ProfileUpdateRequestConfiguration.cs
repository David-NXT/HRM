using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class ProfileUpdateRequestConfiguration : IEntityTypeConfiguration<ProfileUpdateRequest>
{
    public void Configure(EntityTypeBuilder<ProfileUpdateRequest> builder)
    {
        builder.ToTable("profile_update_request");
        builder.HasKey(request => request.ProfileUpdateRequestId);
        builder.Property(request => request.ProfileUpdateRequestId).HasColumnName("profile_update_request_id").HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(request => request.EmployeeId).HasColumnName("employee_id").IsRequired();
        builder.Property(request => request.ChangesJson).HasColumnName("changes_json").HasColumnType("jsonb").IsRequired();
        builder.Property(request => request.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(request => request.Version).HasColumnName("version").HasDefaultValue(1).IsConcurrencyToken();
        builder.Property(request => request.ReviewReason).HasColumnName("review_reason").HasMaxLength(1000);
        builder.Property(request => request.SubmittedAt).HasColumnName("submitted_at");
        builder.Property(request => request.ReviewedAt).HasColumnName("reviewed_at");
        builder.Property(request => request.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(request => request.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.HasIndex(request => new { request.EmployeeId, request.CreatedAt });
        builder.HasIndex(request => new { request.Status, request.SubmittedAt });
        builder.HasOne(request => request.Employee)
            .WithMany()
            .HasForeignKey(request => request.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(request => request.Attachments)
            .WithOne(attachment => attachment.Request)
            .HasForeignKey(attachment => attachment.ProfileUpdateRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
