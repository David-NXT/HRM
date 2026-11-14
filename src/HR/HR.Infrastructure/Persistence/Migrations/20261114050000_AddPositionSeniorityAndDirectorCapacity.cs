using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Infrastructure.Persistence.Migrations
{
    public partial class AddPositionSeniorityAndDirectorCapacity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "seniority_level",
                table: "position",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_director",
                table: "work_history",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.DropIndex(
                name: "IX_work_history_department_id",
                table: "work_history");

            migrationBuilder.CreateIndex(
                name: "IX_work_history_director_department_open",
                table: "work_history",
                column: "department_id",
                unique: true,
                filter: "is_director = TRUE AND end_date IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_work_history_director_department_open",
                table: "work_history");

            migrationBuilder.CreateIndex(
                name: "IX_work_history_department_id",
                table: "work_history",
                column: "department_id");

            migrationBuilder.DropColumn(
                name: "seniority_level",
                table: "position");

            migrationBuilder.DropColumn(
                name: "is_director",
                table: "work_history");
        }
    }
}
