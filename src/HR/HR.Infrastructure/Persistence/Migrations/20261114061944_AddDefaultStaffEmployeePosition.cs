using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultStaffEmployeePosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO position (
                    position_id, code, name, description, seniority_level, status, created_at, updated_at
                )
                VALUES (
                    gen_random_uuid(),
                    'SYS_STAFF',
                    'Nhân viên',
                    'Chức vụ mặc định cấp STAFF cho nhân viên part-time.',
                    'STAFF',
                    'ACTIVE',
                    now(),
                    now()
                )
                ON CONFLICT (code) DO UPDATE
                SET name = EXCLUDED.name,
                    description = EXCLUDED.description,
                    seniority_level = EXCLUDED.seniority_level,
                    status = EXCLUDED.status,
                    updated_at = now();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM position
                WHERE code = 'SYS_STAFF'
                  AND NOT EXISTS (
                      SELECT 1 FROM employee WHERE employee.position_id = position.position_id
                  )
                  AND NOT EXISTS (
                      SELECT 1 FROM work_history WHERE work_history.position_id = position.position_id
                  );
                """);
        }
    }
}
