using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class PartTimeEmployeeConfiguration : IEntityTypeConfiguration<PartTimeEmployee>
{
    public void Configure(EntityTypeBuilder<PartTimeEmployee> builder)
    {
        builder.Property(employee => employee.PartTimeSalary)
            .HasColumnName("part_time_salary")
            .HasPrecision(18, 2);
    }
}
