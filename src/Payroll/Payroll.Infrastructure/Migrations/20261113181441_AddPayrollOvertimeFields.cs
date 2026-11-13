using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payroll.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollOvertimeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmploymentType",
                table: "Payrolls",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "Payrolls",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OTHours",
                table: "Payrolls",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OTMultiplier",
                table: "Payrolls",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OTPay",
                table: "Payrolls",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalWorkingHours",
                table: "Payrolls",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmploymentType",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "OTHours",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "OTMultiplier",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "OTPay",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "TotalWorkingHours",
                table: "Payrolls");
        }
    }
}
