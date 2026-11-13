using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendance.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttendanceOvertimeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HolidayOTHours",
                table: "TimesheetSummaries");

            migrationBuilder.DropColumn(
                name: "NormalOTHours",
                table: "TimesheetSummaries");

            migrationBuilder.RenameColumn(
                name: "WeekendOTHours",
                table: "TimesheetSummaries",
                newName: "OTHours");

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftAssignmentId",
                table: "AttendanceRecords",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_ShiftAssignmentId",
                table: "AttendanceRecords",
                column: "ShiftAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_ShiftAssignments_ShiftAssignmentId",
                table: "AttendanceRecords",
                column: "ShiftAssignmentId",
                principalTable: "ShiftAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_ShiftAssignments_ShiftAssignmentId",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_ShiftAssignmentId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "ShiftAssignmentId",
                table: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "OTHours",
                table: "TimesheetSummaries",
                newName: "WeekendOTHours");

            migrationBuilder.AddColumn<decimal>(
                name: "HolidayOTHours",
                table: "TimesheetSummaries",
                type: "numeric(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NormalOTHours",
                table: "TimesheetSummaries",
                type: "numeric(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
