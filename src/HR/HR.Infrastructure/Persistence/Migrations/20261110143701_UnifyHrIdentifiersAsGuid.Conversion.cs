using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Infrastructure.Persistence.Migrations;

public partial class UnifyHrIdentifiersAsGuid
{
    private const string MapTable = "hr_legacy_id_map";

    private static readonly (string Table, string Key)[] KeyTables =
    [
        ("employee", "employee_id"), ("department", "department_id"), ("position", "position_id"),
        ("employment_contract", "contract_id"), ("work_history", "work_history_id"),
        ("profile_update_request", "profile_update_request_id"), ("profile_update_attachment", "profile_update_attachment_id"),
        ("employee_notification", "employee_notification_id"), ("offboarding_record", "offboarding_record_id"),
        ("offboarding_checklist_item", "offboarding_checklist_item_id"), ("contract_alert", "contract_alert_id")
    ];

    private sealed record Reference(string Table, string Column, string Parent, string Name, bool Nullable = false, bool Cascade = false);

    private static readonly Reference[] References =
    [
        new("employee", "department_id", "department", "FK_employee_department_department_id", true),
        new("employee", "position_id", "position", "FK_employee_position_position_id", true),
        new("department", "parent_department_id", "department", "FK_department_department_parent_department_id", true),
        new("employment_contract", "employee_id", "employee", "FK_employment_contract_employee_employee_id"),
        new("employment_contract", "renewal_of_contract_id", "employment_contract", "FK_employment_contract_employment_contract_renewal_of_contract~", true),
        new("work_history", "employee_id", "employee", "FK_work_history_employee_employee_id"),
        new("work_history", "department_id", "department", "FK_work_history_department_department_id"),
        new("work_history", "position_id", "position", "FK_work_history_position_position_id"),
        new("profile_update_request", "employee_id", "employee", "FK_profile_update_request_employee_employee_id"),
        new("profile_update_attachment", "profile_update_request_id", "profile_update_request", "FK_profile_update_attachment_profile_update_request_profile_up~", false, true),
        new("employee_notification", "employee_id", "employee", "FK_employee_notification_employee_employee_id"),
        new("employee_notification", "profile_update_request_id", "profile_update_request", "FK_employee_notification_profile_update_request_profile_update~", true),
        new("offboarding_record", "employee_id", "employee", "FK_offboarding_record_employee_employee_id"),
        new("offboarding_checklist_item", "offboarding_record_id", "offboarding_record", "FK_offboarding_checklist_item_offboarding_record_offboarding_r~", false, true),
        new("contract_alert", "contract_id", "employment_contract", "FK_contract_alert_employment_contract_contract_id")
    ];

    private static void PrepareMapAndLockTables(MigrationBuilder builder)
    {
        builder.Sql($"""
            CREATE TABLE IF NOT EXISTS {MapTable} (
                entity_table text NOT NULL,
                legacy_id integer NOT NULL,
                guid_id uuid NOT NULL,
                PRIMARY KEY (entity_table, legacy_id),
                UNIQUE (entity_table, guid_id)
            );
            """);
        // All commands remain inside the EF migration transaction; writes must not race the mapping.
        builder.Sql("LOCK TABLE " + string.Join(", ", KeyTables.Select(item => Quote(item.Table))) + " IN ACCESS EXCLUSIVE MODE;");
    }

    private static void MigrateToGuids(MigrationBuilder builder)
    {
        PrepareMapAndLockTables(builder);
        builder.Sql("""
            DO $$ BEGIN
                IF EXISTS (SELECT 1 FROM employee WHERE employee_guid IS NULL OR employee_guid = '00000000-0000-0000-0000-000000000000'::uuid) THEN
                    RAISE EXCEPTION 'Cannot migrate HR: employee_guid must be a non-empty unique UUID';
                END IF;
            END $$;
            """);
        foreach (var (table, key) in KeyTables)
        {
            // Employee already has the stable UUID used by Project and Attendance: reuse it.
            var value = table == "employee" ? "employee_guid" : "gen_random_uuid()";
            var conflict = table == "employee" ? "DO UPDATE SET guid_id = EXCLUDED.guid_id" : "DO NOTHING";
            builder.Sql($"""
                INSERT INTO {MapTable} (entity_table, legacy_id, guid_id)
                SELECT '{table}', {Quote(key)}, {value} FROM {Quote(table)}
                ON CONFLICT (entity_table, legacy_id) {conflict};
                """);
        }
        ConvertIdentifiers(builder, toGuid: true);
    }

    private static void MigrateToLegacyIntegers(MigrationBuilder builder)
    {
        PrepareMapAndLockTables(builder);
        foreach (var (table, key) in KeyTables)
        {
            // Rows created after Up also receive a legacy number if the schema is rolled back.
            builder.Sql($"""
                INSERT INTO {MapTable} (entity_table, legacy_id, guid_id)
                SELECT '{table}',
                    (COALESCE((SELECT MAX(legacy_id) FROM {MapTable} WHERE entity_table = '{table}'), 0)
                        + ROW_NUMBER() OVER (ORDER BY source.{Quote(key)}))::integer,
                    source.{Quote(key)}
                FROM {Quote(table)} source
                WHERE NOT EXISTS (
                    SELECT 1 FROM {MapTable} existing
                    WHERE existing.entity_table = '{table}' AND existing.guid_id = source.{Quote(key)}
                );
                """);
        }
        builder.AddColumn<Guid>(name: "employee_guid", table: "employee", type: "uuid", nullable: true);
        builder.Sql("UPDATE employee SET employee_guid = employee_id;");
        ConvertIdentifiers(builder, toGuid: false);
        builder.Sql("ALTER TABLE employee ALTER COLUMN employee_guid SET NOT NULL, ALTER COLUMN employee_guid SET DEFAULT gen_random_uuid();");
        builder.CreateIndex(name: "IX_employee_employee_guid", table: "employee", column: "employee_guid", unique: true);
        // Retain the map as rollback evidence and to preserve UUIDs on a subsequent upgrade.
    }

    private static void ConvertIdentifiers(MigrationBuilder builder, bool toGuid)
    {
        foreach (var reference in References) builder.DropForeignKey(name: reference.Name, table: reference.Table);
        foreach (var (table, _) in KeyTables) builder.DropPrimaryKey(name: "PK_" + table, table: table);

        foreach (var (table, key) in KeyTables) AddMappedColumn(builder, table, key, table, nullable: false, toGuid);
        foreach (var reference in References)
            AddMappedColumn(builder, reference.Table, reference.Column, reference.Parent, reference.Nullable, toGuid);

        if (toGuid) builder.DropColumn(name: "employee_guid", table: "employee");
        foreach (var (table, key) in KeyTables) ReplaceColumn(builder, table, key);
        foreach (var reference in References) ReplaceColumn(builder, reference.Table, reference.Column);

        foreach (var (table, key) in KeyTables)
        {
            builder.AddPrimaryKey(name: "PK_" + table, table: table, column: key);
            if (toGuid)
                builder.Sql($"ALTER TABLE {Quote(table)} ALTER COLUMN {Quote(key)} SET DEFAULT gen_random_uuid();");
            else
            {
                builder.Sql($"ALTER TABLE {Quote(table)} ALTER COLUMN {Quote(key)} ADD GENERATED BY DEFAULT AS IDENTITY;");
                builder.Sql($"SELECT setval(pg_get_serial_sequence('{table}', '{key}'), GREATEST(COALESCE(MAX({Quote(key)}), 0), 1), MAX({Quote(key)}) IS NOT NULL) FROM {Quote(table)};");
            }
        }
        foreach (var reference in References)
        {
            builder.AddForeignKey(name: reference.Name, table: reference.Table, column: reference.Column,
                principalTable: reference.Parent, principalColumn: KeyTables.Single(item => item.Table == reference.Parent).Key,
                onDelete: reference.Cascade ? ReferentialAction.Cascade : ReferentialAction.Restrict);
        }
        RecreateIdentifierIndexes(builder);
    }

    private static void AddMappedColumn(MigrationBuilder builder, string table, string column, string parent, bool nullable, bool toGuid)
    {
        var temporary = column + "_hr_new";
        if (toGuid) builder.AddColumn<Guid>(name: temporary, table: table, type: "uuid", nullable: true);
        else builder.AddColumn<int>(name: temporary, table: table, type: "integer", nullable: true);
        var source = toGuid ? "legacy_id" : "guid_id";
        var target = toGuid ? "guid_id" : "legacy_id";
        builder.Sql($"""
            UPDATE {Quote(table)} row_to_convert
            SET {Quote(temporary)} = mapping.{target}
            FROM {MapTable} mapping
            WHERE mapping.entity_table = '{parent}' AND mapping.{source} = row_to_convert.{Quote(column)};
            DO $$ BEGIN
                IF EXISTS (SELECT 1 FROM {Quote(table)} WHERE {Quote(column)} IS NOT NULL AND {Quote(temporary)} IS NULL) THEN
                    RAISE EXCEPTION 'Cannot map {table}.{column}; migration rolled back';
                END IF;
            END $$;
            """);
        if (!nullable) builder.Sql($"ALTER TABLE {Quote(table)} ALTER COLUMN {Quote(temporary)} SET NOT NULL;");
    }

    private static void ReplaceColumn(MigrationBuilder builder, string table, string column)
    {
        // PostgreSQL removes indexes depending on the old column; recreate them below.
        builder.DropColumn(name: column, table: table);
        builder.RenameColumn(name: column + "_hr_new", table: table, newName: column);
    }

    private static void RecreateIdentifierIndexes(MigrationBuilder builder)
    {
        Index(builder, "employee", ["department_id"]);
        Index(builder, "employee", ["position_id"]);
        Index(builder, "department", ["parent_department_id"]);
        Index(builder, "employment_contract", ["employee_id"]);
        Index(builder, "employment_contract", ["renewal_of_contract_id"], unique: true);
        Index(builder, "work_history", ["department_id"]);
        Index(builder, "work_history", ["position_id"]);
        Index(builder, "work_history", ["employee_id"], unique: true, filter: "end_date IS NULL");
        Index(builder, "work_history", ["employee_id", "start_date"]);
        Index(builder, "profile_update_request", ["employee_id", "created_at"]);
        Index(builder, "profile_update_attachment", ["profile_update_request_id"]);
        Index(builder, "employee_notification", ["employee_id", "created_at"]);
        Index(builder, "employee_notification", ["profile_update_request_id"]);
        Index(builder, "offboarding_record", ["employee_id"], unique: true, filter: "status = 'IN_PROGRESS'", name: "UX_offboarding_employee_in_progress");
        Index(builder, "offboarding_checklist_item", ["offboarding_record_id", "sort_order"], unique: true);
        Index(builder, "contract_alert", ["contract_id", "expiry_date"], unique: true, name: "UX_contract_alert_contract_expiry");
    }

    private static void Index(MigrationBuilder builder, string table, string[] columns, bool unique = false, string filter = null, string name = null) =>
        builder.CreateIndex(name: name ?? "IX_" + table + "_" + string.Join("_", columns), table: table,
            columns: columns, unique: unique, filter: filter);

    private static string Quote(string name) => "\"" + name.Replace("\"", "\"\"") + "\"";
}
