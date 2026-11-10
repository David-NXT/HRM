using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class ContractAlertConfiguration : IEntityTypeConfiguration<ContractAlert>
{
    public void Configure(EntityTypeBuilder<ContractAlert> builder)
    {
        builder.ToTable("contract_alert");
        builder.HasKey(alert => alert.ContractAlertId);
        builder.Property(alert => alert.ContractAlertId).HasColumnName("contract_alert_id").HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(alert => alert.ContractId).HasColumnName("contract_id").IsRequired();
        builder.Property(alert => alert.ExpiryDate).HasColumnName("expiry_date").IsRequired();
        builder.Property(alert => alert.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(alert => alert.Version).HasColumnName("version").HasDefaultValue(1).IsConcurrencyToken();
        builder.Property(alert => alert.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(alert => alert.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.HasIndex(alert => new { alert.ContractId, alert.ExpiryDate }).IsUnique()
            .HasDatabaseName("UX_contract_alert_contract_expiry");
        builder.HasIndex(alert => new { alert.Status, alert.ExpiryDate });
        builder.HasOne(alert => alert.Contract).WithMany()
            .HasForeignKey(alert => alert.ContractId).OnDelete(DeleteBehavior.Restrict);
    }
}
