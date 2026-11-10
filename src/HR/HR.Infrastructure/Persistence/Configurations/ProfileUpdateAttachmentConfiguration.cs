using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class ProfileUpdateAttachmentConfiguration : IEntityTypeConfiguration<ProfileUpdateAttachment>
{
    public void Configure(EntityTypeBuilder<ProfileUpdateAttachment> builder)
    {
        builder.ToTable("profile_update_attachment");
        builder.HasKey(attachment => attachment.ProfileUpdateAttachmentId);
        builder.Property(attachment => attachment.ProfileUpdateAttachmentId).HasColumnName("profile_update_attachment_id").HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(attachment => attachment.ProfileUpdateRequestId).HasColumnName("profile_update_request_id").IsRequired();
        builder.Property(attachment => attachment.StorageKey).HasColumnName("storage_key").HasMaxLength(80).IsRequired();
        builder.HasIndex(attachment => attachment.StorageKey).IsUnique();
        builder.Property(attachment => attachment.OriginalFileName).HasColumnName("original_file_name").HasMaxLength(255).IsRequired();
        builder.Property(attachment => attachment.ContentType).HasColumnName("content_type").HasMaxLength(100).IsRequired();
        builder.Property(attachment => attachment.Length).HasColumnName("length").IsRequired();
        builder.Property(attachment => attachment.UploadedAt).HasColumnName("uploaded_at").IsRequired();
    }
}
