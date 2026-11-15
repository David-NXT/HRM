using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class FixTimesheetTaskRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_Tasks_TaskId1",
                table: "Timesheets");

            migrationBuilder.DropIndex(
                name: "IX_Timesheets_TaskId1",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "TaskId1",
                table: "Timesheets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TaskId1",
                table: "Timesheets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_TaskId1",
                table: "Timesheets",
                column: "TaskId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_Tasks_TaskId1",
                table: "Timesheets",
                column: "TaskId1",
                principalTable: "Tasks",
                principalColumn: "TaskId");
        }
    }
}
