using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class EmploymentContractConfiguration : IEntityTypeConfiguration<EmploymentContract>
{
    public void Configure(EntityTypeBuilder<EmploymentContract> builder)
    {
        builder.ToTable("employment_contract");
        builder.HasKey(contract => contract.ContractId);
        builder.Property(contract => contract.ContractId).HasColumnName("contract_id").HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(contract => contract.EmployeeId).HasColumnName("employee_id").IsRequired();
        builder.Property(contract => contract.ContractNumber).HasColumnName("contract_number").HasMaxLength(50).IsRequired();
        builder.HasIndex(contract => contract.ContractNumber).IsUnique();
        builder.Property(contract => contract.ContractType).HasColumnName("contract_type").HasMaxLength(50).IsRequired();
        builder.Property(contract => contract.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(contract => contract.EndDate).HasColumnName("end_date");
        builder.Property(contract => contract.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(contract => contract.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(contract => contract.RenewalOfContractId).HasColumnName("renewal_of_contract_id");
        builder.Property(contract => contract.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(contract => contract.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.HasIndex(contract => contract.EmployeeId);
        builder.HasIndex(contract => contract.RenewalOfContractId).IsUnique();
        builder.HasOne(contract => contract.Employee)
            .WithMany()
            .HasForeignKey(contract => contract.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(contract => contract.RenewalOfContract)
            .WithOne(contract => contract.RenewedByContract)
            .HasForeignKey<EmploymentContract>(contract => contract.RenewalOfContractId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
