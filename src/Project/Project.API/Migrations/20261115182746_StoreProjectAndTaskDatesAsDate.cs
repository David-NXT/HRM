using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.API.Data;

#nullable disable

namespace Project.Migrations;

[DbContext(typeof(ProjectManagementDbContext))]
[Migration("20261115182746_StoreProjectAndTaskDatesAsDate")]
public partial class StoreProjectAndTaskDatesAsDate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE "Projects"
                ALTER COLUMN "StartDate" TYPE date
                    USING ("StartDate" AT TIME ZONE 'UTC')::date,
                ALTER COLUMN "EndDate" TYPE date
                    USING ("EndDate" AT TIME ZONE 'UTC')::date;

            ALTER TABLE "Tasks"
                ALTER COLUMN "StartDate" TYPE date
                    USING ("StartDate" AT TIME ZONE 'UTC')::date,
                ALTER COLUMN "DueDate" TYPE date
                    USING ("DueDate" AT TIME ZONE 'UTC')::date;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE "Tasks"
                ALTER COLUMN "StartDate" TYPE timestamp with time zone
                    USING ("StartDate"::timestamp AT TIME ZONE 'UTC'),
                ALTER COLUMN "DueDate" TYPE timestamp with time zone
                    USING ("DueDate"::timestamp AT TIME ZONE 'UTC');

            ALTER TABLE "Projects"
                ALTER COLUMN "StartDate" TYPE timestamp with time zone
                    USING ("StartDate"::timestamp AT TIME ZONE 'UTC'),
                ALTER COLUMN "EndDate" TYPE timestamp with time zone
                    USING ("EndDate"::timestamp AT TIME ZONE 'UTC');
            """);
    }
}
