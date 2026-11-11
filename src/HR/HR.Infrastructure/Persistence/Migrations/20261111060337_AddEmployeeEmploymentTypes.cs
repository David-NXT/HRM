using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeEmploymentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "employment_type",
                table: "employee",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "FULL_TIME");

            migrationBuilder.AddCheckConstraint(
                name: "CK_employee_employment_type",
                table: "employee",
                sql: "employment_type IN ('FULL_TIME', 'PART_TIME')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_employee_employment_type",
                table: "employee");

            migrationBuilder.DropColumn(
                name: "employment_type",
                table: "employee");
        }
    }
}
