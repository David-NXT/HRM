using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HR.Tests;

public sealed class EmployeeInheritanceTests
{
    [Fact]
    public void Model_MapsBothSubtypesToEmployeeWithAStableDiscriminator()
    {
        using var db = new HrDbContext(new DbContextOptionsBuilder<HrDbContext>()
            .UseNpgsql("Host=localhost;Database=not_used").Options);
        var common = db.Model.FindEntityType(typeof(Employee))!;
        var fullTime = db.Model.FindEntityType(typeof(FullTimeEmployee))!;
        var partTime = db.Model.FindEntityType(typeof(PartTimeEmployee))!;

        Assert.True(typeof(Employee).IsAbstract);
        Assert.Equal(nameof(Employee.EmploymentType), common.GetDiscriminatorPropertyName());
        Assert.Equal("employment_type", common.FindProperty(nameof(Employee.EmploymentType))!.GetColumnName());
        Assert.Equal(EmploymentTypes.FullTime, fullTime.GetDiscriminatorValue());
        Assert.Equal(EmploymentTypes.PartTime, partTime.GetDiscriminatorValue());
        Assert.Same(common, fullTime.BaseType);
        Assert.Same(common, partTime.BaseType);
        Assert.Equal("employee", fullTime.GetTableName());
        Assert.Equal("employee", partTime.GetTableName());
        Assert.Same(common.FindPrimaryKey(), partTime.FindPrimaryKey());
    }

    [Fact]
    public void PartTimeQueriesFilterTheSharedTableWithoutJoiningAnotherEmployeeTable()
    {
        using var db = new HrDbContext(new DbContextOptionsBuilder<HrDbContext>()
            .UseNpgsql("Host=localhost;Database=not_used").Options);
        var sql = db.Employees.OfType<PartTimeEmployee>().ToQueryString();
        Assert.Contains("employment_type", sql);
        Assert.Contains("'PART_TIME'", sql);
        Assert.DoesNotContain("JOIN", sql);
    }
}
