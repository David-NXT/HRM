using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HR.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileUpdateWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "profile_update_request",
                columns: table => new
                {
                    profile_update_request_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    changes_json = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    review_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_update_request", x => x.profile_update_request_id);
                    table.ForeignKey(
                        name: "FK_profile_update_request_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employee_notification",
                columns: table => new
                {
                    employee_notification_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    profile_update_request_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_notification", x => x.employee_notification_id);
                    table.ForeignKey(
                        name: "FK_employee_notification_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employee_notification_profile_update_request_profile_update~",
                        column: x => x.profile_update_request_id,
                        principalTable: "profile_update_request",
                        principalColumn: "profile_update_request_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "profile_update_attachment",
                columns: table => new
                {
                    profile_update_attachment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_update_request_id = table.Column<int>(type: "integer", nullable: false),
                    storage_key = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    length = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_update_attachment", x => x.profile_update_attachment_id);
                    table.ForeignKey(
                        name: "FK_profile_update_attachment_profile_update_request_profile_up~",
                        column: x => x.profile_update_request_id,
                        principalTable: "profile_update_request",
                        principalColumn: "profile_update_request_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employee_notification_employee_id_created_at",
                table: "employee_notification",
                columns: new[] { "employee_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_employee_notification_profile_update_request_id",
                table: "employee_notification",
                column: "profile_update_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_profile_update_attachment_profile_update_request_id",
                table: "profile_update_attachment",
                column: "profile_update_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_profile_update_attachment_storage_key",
                table: "profile_update_attachment",
                column: "storage_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_profile_update_request_employee_id_created_at",
                table: "profile_update_request",
                columns: new[] { "employee_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_profile_update_request_status_submitted_at",
                table: "profile_update_request",
                columns: new[] { "status", "submitted_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_notification");

            migrationBuilder.DropTable(
                name: "profile_update_attachment");

            migrationBuilder.DropTable(
                name: "profile_update_request");
        }
    }
}
