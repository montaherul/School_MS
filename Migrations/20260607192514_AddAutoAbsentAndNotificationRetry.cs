using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    public partial class AddAutoAbsentAndNotificationRetry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "AttendanceNotificationLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRetryAt",
                table: "AttendanceNotificationLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AutoAbsentExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentsProcessed = table.Column<int>(type: "int", nullable: false),
                    StudentsMarkedAbsent = table.Column<int>(type: "int", nullable: false),
                    EmployeesProcessed = table.Column<int>(type: "int", nullable: false),
                    EmployeesMarkedAbsent = table.Column<int>(type: "int", nullable: false),
                    HolidaysSkipped = table.Column<int>(type: "int", nullable: false),
                    WeeklyOffsSkipped = table.Column<int>(type: "int", nullable: false),
                    WorkingDaysEvaluated = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false, defaultValue: "Success"),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DurationMs = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, defaultValue: "system"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoAbsentExecutionLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoAbsentExecutionLogs");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "AttendanceNotificationLogs");

            migrationBuilder.DropColumn(
                name: "NextRetryAt",
                table: "AttendanceNotificationLogs");
        }
    }
}
