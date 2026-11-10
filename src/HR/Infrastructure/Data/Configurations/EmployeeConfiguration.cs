using HR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Data.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employee");
        builder.HasKey(employee => employee.EmployeeId);
        builder.Property(employee => employee.EmployeeId).HasColumnName("employee_id").ValueGeneratedOnAdd();
        builder.Property(employee => employee.EmployeeGuid).HasColumnName("employee_guid").HasDefaultValueSql("gen_random_uuid()");
        builder.HasIndex(employee => employee.EmployeeGuid).IsUnique();
        builder.Property(employee => employee.EmployeeCode).HasColumnName("employee_code").HasMaxLength(20).IsRequired();
        builder.HasIndex(employee => employee.EmployeeCode).IsUnique();
        builder.Property(employee => employee.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
        builder.Property(employee => employee.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
        builder.Property(employee => employee.Gender).HasColumnName("gender").HasMaxLength(20).IsRequired();
        builder.Property(employee => employee.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
        builder.Property(employee => employee.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
        builder.Property(employee => employee.CitizenId).HasColumnName("citizen_id").HasMaxLength(12).IsRequired();
        builder.Property(employee => employee.Address).HasColumnName("address").HasMaxLength(500);
        builder.Property(employee => employee.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(employee => employee.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(employee => employee.UpdatedAt).HasColumnName("updated_at").IsRequired();
    }
}
