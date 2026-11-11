using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureTimesheetAndHourlyRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRates_ProjectMembers_ProjectMemberId",
                table: "HourlyRates");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_ProjectMembers_ProjectMemberId",
                table: "Timesheets");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_Tasks_TaskId",
                table: "Timesheets");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Timesheets",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RejectReason",
                table: "Timesheets",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "HoursWorked",
                table: "Timesheets",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Timesheets",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId1",
                table: "Timesheets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RoleCoefficient",
                table: "ProjectRoles",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "HourlyRates",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_TaskId1",
                table: "Timesheets",
                column: "TaskId1");

            migrationBuilder.AddForeignKey(
                name: "FK_HourlyRates_ProjectMembers_ProjectMemberId",
                table: "HourlyRates",
                column: "ProjectMemberId",
                principalTable: "ProjectMembers",
                principalColumn: "ProjectMemberId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_ProjectMembers_ProjectMemberId",
                table: "Timesheets",
                column: "ProjectMemberId",
                principalTable: "ProjectMembers",
                principalColumn: "ProjectMemberId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_Tasks_TaskId",
                table: "Timesheets",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_Tasks_TaskId1",
                table: "Timesheets",
                column: "TaskId1",
                principalTable: "Tasks",
                principalColumn: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRates_ProjectMembers_ProjectMemberId",
                table: "HourlyRates");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_ProjectMembers_ProjectMemberId",
                table: "Timesheets");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_Tasks_TaskId",
                table: "Timesheets");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_Tasks_TaskId1",
                table: "Timesheets");

            migrationBuilder.DropIndex(
                name: "IX_Timesheets_TaskId1",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "TaskId1",
                table: "Timesheets");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Timesheets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "RejectReason",
                table: "Timesheets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "HoursWorked",
                table: "Timesheets",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Timesheets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RoleCoefficient",
                table: "ProjectRoles",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,4)",
                oldPrecision: 5,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "HourlyRates",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddForeignKey(
                name: "FK_HourlyRates_ProjectMembers_ProjectMemberId",
                table: "HourlyRates",
                column: "ProjectMemberId",
                principalTable: "ProjectMembers",
                principalColumn: "ProjectMemberId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_ProjectMembers_ProjectMemberId",
                table: "Timesheets",
                column: "ProjectMemberId",
                principalTable: "ProjectMembers",
                principalColumn: "ProjectMemberId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_Tasks_TaskId",
                table: "Timesheets",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
