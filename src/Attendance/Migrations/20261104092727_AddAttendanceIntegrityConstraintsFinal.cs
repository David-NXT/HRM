using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendance.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceIntegrityConstraintsFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceExplanations_AttendanceRecords_AttendanceId",
                table: "AttendanceExplanations");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_ShiftId",
                table: "ShiftAssignments",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_ShiftId",
                table: "AttendanceRecords",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceExplanations_AttendanceRecords_AttendanceId",
                table: "AttendanceExplanations",
                column: "AttendanceId",
                principalTable: "AttendanceRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_WorkShifts_ShiftId",
                table: "AttendanceRecords",
                column: "ShiftId",
                principalTable: "WorkShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_WorkShifts_ShiftId",
                table: "ShiftAssignments",
                column: "ShiftId",
                principalTable: "WorkShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceExplanations_AttendanceRecords_AttendanceId",
                table: "AttendanceExplanations");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_WorkShifts_ShiftId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_WorkShifts_ShiftId",
                table: "ShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ShiftAssignments_ShiftId",
                table: "ShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_ShiftId",
                table: "AttendanceRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceExplanations_AttendanceRecords_AttendanceId",
                table: "AttendanceExplanations",
                column: "AttendanceId",
                principalTable: "AttendanceRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
