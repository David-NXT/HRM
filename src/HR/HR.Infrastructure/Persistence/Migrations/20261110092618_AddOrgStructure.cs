using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HR.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "employee",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "position_id",
                table: "employee",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    department_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    parent_department_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => x.department_id);
                    table.ForeignKey(
                        name: "FK_department_department_parent_department_id",
                        column: x => x.parent_department_id,
                        principalTable: "department",
                        principalColumn: "department_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "position",
                columns: table => new
                {
                    position_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_position", x => x.position_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employee_department_id",
                table: "employee",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_position_id",
                table: "employee",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_department_code",
                table: "department",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_department_parent_department_id",
                table: "department",
                column: "parent_department_id");

            migrationBuilder.CreateIndex(
                name: "IX_position_code",
                table: "position",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_employee_department_department_id",
                table: "employee",
                column: "department_id",
                principalTable: "department",
                principalColumn: "department_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_employee_position_position_id",
                table: "employee",
                column: "position_id",
                principalTable: "position",
                principalColumn: "position_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employee_department_department_id",
                table: "employee");

            migrationBuilder.DropForeignKey(
                name: "FK_employee_position_position_id",
                table: "employee");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "position");

            migrationBuilder.DropIndex(
                name: "IX_employee_department_id",
                table: "employee");

            migrationBuilder.DropIndex(
                name: "IX_employee_position_id",
                table: "employee");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "employee");

            migrationBuilder.DropColumn(
                name: "position_id",
                table: "employee");
        }
    }
}
