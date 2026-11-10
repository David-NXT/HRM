using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("position");
        builder.HasKey(position => position.PositionId);
        builder.Property(position => position.PositionId).HasColumnName("position_id").HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(position => position.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        builder.HasIndex(position => position.Code).IsUnique();
        builder.Property(position => position.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(position => position.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(position => position.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(position => position.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(position => position.UpdatedAt).HasColumnName("updated_at").IsRequired();
    }
}
