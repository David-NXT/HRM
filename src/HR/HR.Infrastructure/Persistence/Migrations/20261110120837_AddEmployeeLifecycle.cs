using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HR.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "version",
                table: "employee",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "offboarding_record",
                columns: table => new
                {
                    offboarding_record_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    last_working_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offboarding_record", x => x.offboarding_record_id);
                    table.ForeignKey(
                        name: "FK_offboarding_record_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "offboarding_checklist_item",
                columns: table => new
                {
                    offboarding_checklist_item_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    offboarding_record_id = table.Column<int>(type: "integer", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completion_note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offboarding_checklist_item", x => x.offboarding_checklist_item_id);
                    table.ForeignKey(
                        name: "FK_offboarding_checklist_item_offboarding_record_offboarding_r~",
                        column: x => x.offboarding_record_id,
                        principalTable: "offboarding_record",
                        principalColumn: "offboarding_record_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_offboarding_checklist_item_offboarding_record_id_sort_order",
                table: "offboarding_checklist_item",
                columns: new[] { "offboarding_record_id", "sort_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_offboarding_record_status_last_working_date",
                table: "offboarding_record",
                columns: new[] { "status", "last_working_date" });

            migrationBuilder.CreateIndex(
                name: "UX_offboarding_employee_in_progress",
                table: "offboarding_record",
                column: "employee_id",
                unique: true,
                filter: "status = 'IN_PROGRESS'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "offboarding_checklist_item");

            migrationBuilder.DropTable(
                name: "offboarding_record");

            migrationBuilder.DropColumn(
                name: "version",
                table: "employee");
        }
    }
}
