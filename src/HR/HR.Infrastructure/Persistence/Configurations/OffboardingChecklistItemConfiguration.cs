using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class OffboardingChecklistItemConfiguration : IEntityTypeConfiguration<OffboardingChecklistItem>
{
    public void Configure(EntityTypeBuilder<OffboardingChecklistItem> builder)
    {
        builder.ToTable("offboarding_checklist_item");
        builder.HasKey(item => item.OffboardingChecklistItemId);
        builder.Property(item => item.OffboardingChecklistItemId).HasColumnName("offboarding_checklist_item_id").HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(item => item.OffboardingRecordId).HasColumnName("offboarding_record_id").IsRequired();
        builder.Property(item => item.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(item => item.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(item => item.IsRequired).HasColumnName("is_required").IsRequired();
        builder.Property(item => item.IsCompleted).HasColumnName("is_completed").IsRequired();
        builder.Property(item => item.CompletionNote).HasColumnName("completion_note").HasMaxLength(1000);
        builder.Property(item => item.CompletedAt).HasColumnName("completed_at");
        builder.HasIndex(item => new { item.OffboardingRecordId, item.SortOrder }).IsUnique();
    }
}
