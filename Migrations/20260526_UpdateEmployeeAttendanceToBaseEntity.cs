using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagementSystem.Migrations
{
    public partial class UpdateEmployeeAttendanceToBaseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add IsDeleted column for soft-delete support
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EmployeeAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Add UpdatedAt column
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EmployeeAttendances",
                type: "datetime2",
                nullable: true);

            // Add UpdatedBy column (for audit trail consistency)
            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "EmployeeAttendances",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            // Drop the old unique index
            migrationBuilder.DropIndex(
                name: "IX_EmployeeAttendances_EmployeeId_AttendanceDate",
                table: "EmployeeAttendances");

            // Recreate unique index with IsDeleted filter for soft-delete
            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendances_EmployeeId_AttendanceDate",
                table: "EmployeeAttendances",
                columns: new[] { "EmployeeId", "AttendanceDate" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the new unique index
            migrationBuilder.DropIndex(
                name: "IX_EmployeeAttendances_EmployeeId_AttendanceDate",
                table: "EmployeeAttendances");

            // Remove new columns
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EmployeeAttendances");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EmployeeAttendances");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "EmployeeAttendances");

            // Recreate old index without filter
            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendances_EmployeeId_AttendanceDate",
                table: "EmployeeAttendances",
                columns: new[] { "EmployeeId", "AttendanceDate" },
                unique: true);
        }
    }
}
