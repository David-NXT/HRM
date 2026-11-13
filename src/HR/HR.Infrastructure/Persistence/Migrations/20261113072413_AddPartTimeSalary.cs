using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPartTimeSalary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "base_salary",
                table: "employee",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "part_time_salary",
                table: "employee",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE employee SET part_time_salary = COALESCE(base_salary, 0), base_salary = NULL WHERE employment_type = 'PART_TIME';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE employee SET base_salary = COALESCE(part_time_salary, 0) WHERE employment_type = 'PART_TIME';");

            migrationBuilder.DropColumn(
                name: "part_time_salary",
                table: "employee");

            migrationBuilder.AlterColumn<decimal>(
                name: "base_salary",
                table: "employee",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
