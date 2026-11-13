using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class FullTimeEmployeeConfiguration : IEntityTypeConfiguration<FullTimeEmployee>
{
    public void Configure(EntityTypeBuilder<FullTimeEmployee> builder)
    {
        builder.Property(employee => employee.BaseSalary)
            .HasColumnName("base_salary")
            .HasPrecision(18, 2);
    }
}
