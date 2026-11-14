using HR.Application.DTOs.Employees;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence;
using HR.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Xunit;

namespace HR.Tests;

public sealed class GuidIdentifierTests
{
    [Fact]
    public void Model_UsesNonEmptyGuidKeysAndGuidForeignKeysThroughoutHr()
    {
        using var db = Context();
        foreach (var entity in db.Model.GetEntityTypes())
        {
            var key = Assert.Single(entity.FindPrimaryKey()!.Properties);
            Assert.Equal(typeof(Guid), key.ClrType);
            Assert.Equal("uuid", key.GetColumnType());
            Assert.Equal("gen_random_uuid()", key.GetDefaultValueSql());
            if (!entity.ClrType.IsAbstract)
            {
                var first = key.PropertyInfo!.GetValue(Activator.CreateInstance(entity.ClrType));
                var second = key.PropertyInfo.GetValue(Activator.CreateInstance(entity.ClrType));
                Assert.NotEqual(Guid.Empty, first);
                Assert.NotEqual(first, second);
            }

            foreach (var property in entity.GetForeignKeys().SelectMany(foreignKey => foreignKey.Properties))
                Assert.Equal(typeof(Guid), Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType);
        }
        Assert.Null(db.Model.FindEntityType(typeof(Employee))!.FindProperty("EmployeeGuid"));
    }

    [Fact]
    public void PublicDtos_UseTheSameGuidIdentifierForManagementAndDirectory()
    {
        foreach (var type in typeof(EmployeeDto).Assembly.GetTypes().Where(type => type.Namespace?.StartsWith("HR.Application.DTOs.", StringComparison.Ordinal) == true))
        foreach (var property in type.GetProperties().Where(property => property.Name.EndsWith("Id", StringComparison.Ordinal)))
        {
            // CitizenId is a national identity document number, not a database identifier.
            if (property.Name == "CitizenId") continue;
            Assert.Equal(typeof(Guid), Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }
        Assert.Equal(typeof(Guid), typeof(EmployeeDto).GetProperty("EmployeeId")!.PropertyType);
        Assert.Equal(typeof(Guid), typeof(EmployeeDirectoryEntry).GetProperty("EmployeeId")!.PropertyType);
        Assert.Null(typeof(EmployeeDto).GetProperty("EmployeeGuid"));
    }

    [Fact]
    public void Conversion_RecreatesAllAffectedConstraintsAndIndexesFromTheMigrationTargetModel()
    {
        var migration = new UnifyHrIdentifiersAsGuid();
        // Later migrations may change indexes; validate this conversion against its own schema.
        var model = migration.TargetModel;
        foreach (var operations in new[] { migration.UpOperations, migration.DownOperations })
        {
            var droppedKeys = operations.OfType<DropPrimaryKeyOperation>().ToArray();
            var addedKeys = operations.OfType<AddPrimaryKeyOperation>().ToArray();
            var droppedForeignKeys = operations.OfType<DropForeignKeyOperation>().ToArray();
            var addedForeignKeys = operations.OfType<AddForeignKeyOperation>().ToArray();
            Assert.Equal(model.GetEntityTypes().Count(entity => entity.BaseType is null), addedKeys.Length);
            Assert.Equal(addedKeys.Select(key => key.Name).Order(), droppedKeys.Select(key => key.Name).Order());
            Assert.Equal(addedForeignKeys.Select(key => key.Name).Order(), droppedForeignKeys.Select(key => key.Name).Order());
            Assert.Equal(model.GetEntityTypes().Where(entity => entity.BaseType is null).Sum(entity => entity.GetForeignKeys().Count()), addedForeignKeys.Length);

            foreach (var entity in model.GetEntityTypes().Where(entity => entity.BaseType is null))
            {
                var store = StoreObjectIdentifier.Table(entity.GetTableName()!, entity.GetSchema());
                foreach (var foreignKey in entity.GetForeignKeys())
                {
                    var recreated = Assert.Single(addedForeignKeys, key => key.Name == foreignKey.GetConstraintName());
                    Assert.Equal(entity.GetTableName(), recreated.Table);
                    Assert.Equal(foreignKey.Properties.Select(property => property.GetColumnName(store)), recreated.Columns);
                    Assert.Equal(foreignKey.PrincipalEntityType.GetTableName(), recreated.PrincipalTable);
                    Assert.Equal(foreignKey.DeleteBehavior == DeleteBehavior.Cascade ? ReferentialAction.Cascade : ReferentialAction.Restrict, recreated.OnDelete);
                }
                foreach (var index in entity.GetIndexes().Where(index => index.Properties.Any(property => property.ClrType == typeof(Guid) || property.ClrType == typeof(Guid?))))
                {
                    var recreated = Assert.Single(operations.OfType<CreateIndexOperation>(), operation => operation.Name == index.GetDatabaseName());
                    Assert.Equal(index.Properties.Select(property => property.GetColumnName(store)), recreated.Columns);
                    Assert.Equal(index.IsUnique, recreated.IsUnique);
                    Assert.Equal(index.GetFilter(), recreated.Filter);
                }
            }
            // An int -> uuid cast cannot preserve links. Populate new columns through a mapping instead.
            Assert.Empty(operations.OfType<AlterColumnOperation>());
            Assert.Empty(operations.OfType<DropTableOperation>());
            Assert.All(operations.OfType<SqlOperation>(), operation => Assert.False(operation.SuppressTransaction));
        }
    }

    [Fact]
    public void MigrationScript_PreservesExistingEmployeeGuidAndSupportsRollback()
    {
        using var db = Context();
        var migrator = db.GetService<IMigrator>();
        const string previous = "20261110134410_AddContractExpiryAlerts";
        const string current = "20261110143701_UnifyHrIdentifiersAsGuid";
        var upgrade = migrator.GenerateScript(previous, current);
        var downgrade = migrator.GenerateScript(current, previous);
        Assert.Contains("SELECT 'employee', \"employee_id\", employee_guid FROM \"employee\"", upgrade);
        Assert.Contains("IN ACCESS EXCLUSIVE MODE", upgrade);
        Assert.Contains("migration rolled back", upgrade);
        Assert.Contains("UPDATE employee SET employee_guid = employee_id", downgrade);
        Assert.Contains("ROW_NUMBER()", downgrade);
        Assert.DoesNotContain("DROP TABLE hr_legacy_id_map", downgrade);
    }

    private static HrDbContext Context() => new(new DbContextOptionsBuilder<HrDbContext>()
        .UseNpgsql("Host=localhost;Database=not_used").Options);
}
