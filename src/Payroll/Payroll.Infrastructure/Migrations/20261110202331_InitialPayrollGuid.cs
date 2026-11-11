using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payroll.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPayrollGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllowanceTypes",
                columns: table => new
                {
                    AllowanceTypeID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CalculationType = table.Column<string>(type: "text", nullable: false),
                    DefaultRate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowanceTypes", x => x.AllowanceTypeID);
                });

            migrationBuilder.CreateTable(
                name: "PayCutReasons",
                columns: table => new
                {
                    PayCutReasonID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CalculationType = table.Column<string>(type: "text", nullable: false),
                    DefaultRate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayCutReasons", x => x.PayCutReasonID);
                });

            migrationBuilder.CreateTable(
                name: "PayrollPeriods",
                columns: table => new
                {
                    PayrollPeriodID = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollPeriods", x => x.PayrollPeriodID);
                });

            migrationBuilder.CreateTable(
                name: "Payrolls",
                columns: table => new
                {
                    PayrollID = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollPeriodID = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    WorkingDays = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    StandardWorkingDay = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TotalAllowance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPayCut = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossSalary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NetSalary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payrolls", x => x.PayrollID);
                    table.ForeignKey(
                        name: "FK_Payrolls_PayrollPeriods_PayrollPeriodID",
                        column: x => x.PayrollPeriodID,
                        principalTable: "PayrollPeriods",
                        principalColumn: "PayrollPeriodID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Allowances",
                columns: table => new
                {
                    AllowanceID = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollID = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allowances", x => x.AllowanceID);
                    table.ForeignKey(
                        name: "FK_Allowances_Payrolls_PayrollID",
                        column: x => x.PayrollID,
                        principalTable: "Payrolls",
                        principalColumn: "PayrollID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayCuts",
                columns: table => new
                {
                    PayCutID = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollID = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayCuts", x => x.PayCutID);
                    table.ForeignKey(
                        name: "FK_PayCuts_Payrolls_PayrollID",
                        column: x => x.PayrollID,
                        principalTable: "Payrolls",
                        principalColumn: "PayrollID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AllowanceDetails",
                columns: table => new
                {
                    AllowanceDetailID = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowanceID = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowanceTypeID = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RateApplied = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowanceDetails", x => x.AllowanceDetailID);
                    table.ForeignKey(
                        name: "FK_AllowanceDetails_AllowanceTypes_AllowanceTypeID",
                        column: x => x.AllowanceTypeID,
                        principalTable: "AllowanceTypes",
                        principalColumn: "AllowanceTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllowanceDetails_Allowances_AllowanceID",
                        column: x => x.AllowanceID,
                        principalTable: "Allowances",
                        principalColumn: "AllowanceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayCutDetails",
                columns: table => new
                {
                    PayCutDetailID = table.Column<Guid>(type: "uuid", nullable: false),
                    PayCutID = table.Column<Guid>(type: "uuid", nullable: false),
                    PayCutReasonID = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RateApplied = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayCutDetails", x => x.PayCutDetailID);
                    table.ForeignKey(
                        name: "FK_PayCutDetails_PayCutReasons_PayCutReasonID",
                        column: x => x.PayCutReasonID,
                        principalTable: "PayCutReasons",
                        principalColumn: "PayCutReasonID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayCutDetails_PayCuts_PayCutID",
                        column: x => x.PayCutID,
                        principalTable: "PayCuts",
                        principalColumn: "PayCutID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceDetails_AllowanceID",
                table: "AllowanceDetails",
                column: "AllowanceID");

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceDetails_AllowanceTypeID",
                table: "AllowanceDetails",
                column: "AllowanceTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Allowances_PayrollID",
                table: "Allowances",
                column: "PayrollID");

            migrationBuilder.CreateIndex(
                name: "IX_PayCutDetails_PayCutID",
                table: "PayCutDetails",
                column: "PayCutID");

            migrationBuilder.CreateIndex(
                name: "IX_PayCutDetails_PayCutReasonID",
                table: "PayCutDetails",
                column: "PayCutReasonID");

            migrationBuilder.CreateIndex(
                name: "IX_PayCuts_PayrollID",
                table: "PayCuts",
                column: "PayrollID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPeriods_Year_Month",
                table: "PayrollPeriods",
                columns: new[] { "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_PayrollPeriodID_EmployeeID",
                table: "Payrolls",
                columns: new[] { "PayrollPeriodID", "EmployeeID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowanceDetails");

            migrationBuilder.DropTable(
                name: "PayCutDetails");

            migrationBuilder.DropTable(
                name: "AllowanceTypes");

            migrationBuilder.DropTable(
                name: "Allowances");

            migrationBuilder.DropTable(
                name: "PayCutReasons");

            migrationBuilder.DropTable(
                name: "PayCuts");

            migrationBuilder.DropTable(
                name: "Payrolls");

            migrationBuilder.DropTable(
                name: "PayrollPeriods");
        }
    }
}
