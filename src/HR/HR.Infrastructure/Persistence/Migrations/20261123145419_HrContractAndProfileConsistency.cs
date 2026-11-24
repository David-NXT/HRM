using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HrContractAndProfileConsistency : Migration
    {
        // Thêm ngày chấm dứt và baseline; đồng bộ hợp đồng của các hồ sơ nghỉ việc đã hoàn tất trước migration.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "base_employee_version",
                table: "profile_update_request",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "actual_end_date",
                table: "employment_contract",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "termination_reason",
                table: "employment_contract",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            // Không đoán baseline cho yêu cầu cũ; HR yêu cầu bổ sung/gửi lại trước khi chấp thuận.
            // Chỉ dùng hồ sơ nghỉ việc COMPLETED của nhân viên RESIGNED để xác định ngày chấm dứt cũ.
            migrationBuilder.Sql("""
                UPDATE employment_contract AS c
                SET status = CASE WHEN c.start_date > d.last_day THEN 'CANCELLED' ELSE 'TERMINATED' END,
                    actual_end_date = CASE WHEN c.start_date <= d.last_day THEN d.last_day ELSE NULL END,
                    termination_reason = 'Đồng bộ từ hồ sơ nghỉ việc đã hoàn tất.',
                    updated_at = CURRENT_TIMESTAMP
                FROM (
                    SELECT o.employee_id, MAX(o.last_working_date) AS last_day
                    FROM offboarding_record AS o
                    JOIN employee AS e ON e.employee_id = o.employee_id
                    WHERE o.status = 'COMPLETED' AND e.status = 'RESIGNED'
                    GROUP BY o.employee_id
                ) AS d
                WHERE c.employee_id = d.employee_id AND c.status IN ('ACTIVE', 'RENEWED')
                    AND (c.start_date > d.last_day OR c.end_date IS NULL OR c.end_date >= d.last_day);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "base_employee_version",
                table: "profile_update_request");

            migrationBuilder.DropColumn(
                name: "actual_end_date",
                table: "employment_contract");

            migrationBuilder.DropColumn(
                name: "termination_reason",
                table: "employment_contract");
        }
    }
}
