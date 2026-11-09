using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendance.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceModelsAndRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveDate",
                table: "WorkShifts",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualHours",
                table: "OTRequests",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "OTRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DayType",
                table: "OTRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnplanned",
                table: "OTRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Multiplier",
                table: "OTRequests",
                type: "numeric(4,2)",
                precision: 4,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PlannedHours",
                table: "OTRequests",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "OTRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "LeaveRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "LeaveRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDays",
                table: "LeaveRequests",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "Holidays",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActualOTMins",
                table: "AttendanceRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "CheckInLatitude",
                table: "AttendanceRecords",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckInLocation",
                table: "AttendanceRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckInLongitude",
                table: "AttendanceRecords",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckInSource",
                table: "AttendanceRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckOutLatitude",
                table: "AttendanceRecords",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckOutLocation",
                table: "AttendanceRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckOutLongitude",
                table: "AttendanceRecords",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckOutSource",
                table: "AttendanceRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "AttendanceRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEarlyLeave",
                table: "AttendanceRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLate",
                table: "AttendanceRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "AttendanceId",
                table: "AttendanceExplanations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "AttendanceExplanations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "AttendanceExplanations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "AttendanceExplanations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "ExplanationType",
                table: "AttendanceExplanations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "AttendanceExplanations",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttendanceConfigAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: true),
                    Action = table.Column<string>(type: "text", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    PerformedBy = table.Column<string>(type: "text", nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceConfigAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceExplanationAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExplanationId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceExplanationAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceExplanationAttachments_AttendanceExplanations_Exp~",
                        column: x => x.ExplanationId,
                        principalTable: "AttendanceExplanations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleName = table.Column<string>(type: "text", nullable: false),
                    AllowedLateMinutes = table.Column<int>(type: "integer", nullable: false),
                    AllowedEarlyLeaveMinutes = table.Column<int>(type: "integer", nullable: false),
                    MaxViolationsPerMonth = table.Column<int>(type: "integer", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    TotalDays = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    UsedDays = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    RemainingDays = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    CarriedOverDays = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveBalances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DayType = table.Column<string>(type: "text", nullable: false),
                    Multiplier = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    MaxHoursPerDay = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    MaxHoursPerMonth = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    MaxHoursPerYear = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimesheetPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LockedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimesheetSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StandardWorkingDays = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ActualWorkingDays = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TotalWorkingHours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    PaidLeaveDays = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    UnpaidLeaveDays = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TotalLateMinutes = table.Column<int>(type: "integer", nullable: false),
                    LateOccurrences = table.Column<int>(type: "integer", nullable: false),
                    TotalEarlyLeaveMinutes = table.Column<int>(type: "integer", nullable: false),
                    EarlyLeaveOccurrences = table.Column<int>(type: "integer", nullable: false),
                    NormalOTHours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    WeekendOTHours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    HolidayOTHours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimesheetSummaries_TimesheetPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "TimesheetPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceExplanationAttachments_ExplanationId",
                table: "AttendanceExplanationAttachments",
                column: "ExplanationId");

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetSummaries_PeriodId",
                table: "TimesheetSummaries",
                column: "PeriodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceConfigAuditLogs");

            migrationBuilder.DropTable(
                name: "AttendanceExplanationAttachments");

            migrationBuilder.DropTable(
                name: "AttendanceRules");

            migrationBuilder.DropTable(
                name: "LeaveBalances");

            migrationBuilder.DropTable(
                name: "OvertimeRules");

            migrationBuilder.DropTable(
                name: "TimesheetSummaries");

            migrationBuilder.DropTable(
                name: "TimesheetPeriods");

            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "WorkShifts");

            migrationBuilder.DropColumn(
                name: "ActualHours",
                table: "OTRequests");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "OTRequests");

            migrationBuilder.DropColumn(
                name: "DayType",
                table: "OTRequests");

            migrationBuilder.DropColumn(
                name: "IsUnplanned",
                table: "OTRequests");

            migrationBuilder.DropColumn(
                name: "Multiplier",
                table: "OTRequests");

            migrationBuilder.DropColumn(
                name: "PlannedHours",
                table: "OTRequests");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "OTRequests");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "TotalDays",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "ActualOTMins",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CheckInLatitude",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CheckInLocation",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CheckInLongitude",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CheckInSource",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CheckOutLatitude",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CheckOutLocation",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CheckOutLongitude",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CheckOutSource",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "IsEarlyLeave",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "IsLate",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "AttendanceExplanations");

            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "AttendanceExplanations");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "AttendanceExplanations");

            migrationBuilder.DropColumn(
                name: "ExplanationType",
                table: "AttendanceExplanations");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "AttendanceExplanations");

            migrationBuilder.AlterColumn<Guid>(
                name: "AttendanceId",
                table: "AttendanceExplanations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
