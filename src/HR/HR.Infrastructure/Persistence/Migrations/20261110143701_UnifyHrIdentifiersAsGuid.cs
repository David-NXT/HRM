using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Infrastructure.Persistence.Migrations;

public partial class UnifyHrIdentifiersAsGuid : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => MigrateToGuids(migrationBuilder);
    protected override void Down(MigrationBuilder migrationBuilder) => MigrateToLegacyIntegers(migrationBuilder);
}
