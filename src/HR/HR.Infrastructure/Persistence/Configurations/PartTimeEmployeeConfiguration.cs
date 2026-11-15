using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class PartTimeEmployeeConfiguration : IEntityTypeConfiguration<PartTimeEmployee>
{
    public void Configure(EntityTypeBuilder<PartTimeEmployee> builder)
    {
        // Keep the existing column name to avoid a schema change; the value is an hourly rate in VND/hour.
        builder.Property(employee => employee.PartTimeSalary)
            .HasColumnName("part_time_salary")
            .HasPrecision(18, 2);
    }
}
