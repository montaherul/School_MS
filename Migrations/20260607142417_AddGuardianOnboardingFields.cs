using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddGuardianOnboardingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextRetryAt",
                table: "AttendanceNotificationLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "AttendanceNotificationLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GuardianAddress",
                table: "Admissions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianEmail",
                table: "Admissions",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianMobileNumber",
                table: "Admissions",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianNationalId",
                table: "Admissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianPhoto",
                table: "Admissions",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianRelationship",
                table: "Admissions",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianRemarks",
                table: "Admissions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianAddress",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianEmail",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianMobileNumber",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianNationalId",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianPhoto",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianRelationship",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianRemarks",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LinkedGuardianId",
                table: "AdmissionListResults",
                type: "int",
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
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DurationMs = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoAbsentExecutionLogs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Admissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GuardianAddress", "GuardianEmail", "GuardianMobileNumber", "GuardianNationalId", "GuardianPhoto", "GuardianRelationship", "GuardianRemarks" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSettings_IsActive",
                table: "AttendanceSettings",
                column: "IsActive",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_Date",
                table: "AcademicCalendars",
                column: "Date",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoAbsentExecutionLogs");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSettings_IsActive",
                table: "AttendanceSettings");

            migrationBuilder.DropIndex(
                name: "IX_AcademicCalendars_Date",
                table: "AcademicCalendars");

            migrationBuilder.DropColumn(
                name: "NextRetryAt",
                table: "AttendanceNotificationLogs");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "AttendanceNotificationLogs");

            migrationBuilder.DropColumn(
                name: "GuardianAddress",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "GuardianEmail",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "GuardianMobileNumber",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "GuardianNationalId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "GuardianPhoto",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "GuardianRelationship",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "GuardianRemarks",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "GuardianAddress",
                table: "AdmissionListResults");

            migrationBuilder.DropColumn(
                name: "GuardianEmail",
                table: "AdmissionListResults");

            migrationBuilder.DropColumn(
                name: "GuardianMobileNumber",
                table: "AdmissionListResults");

            migrationBuilder.DropColumn(
                name: "GuardianNationalId",
                table: "AdmissionListResults");

            migrationBuilder.DropColumn(
                name: "GuardianPhoto",
                table: "AdmissionListResults");

            migrationBuilder.DropColumn(
                name: "GuardianRelationship",
                table: "AdmissionListResults");

            migrationBuilder.DropColumn(
                name: "GuardianRemarks",
                table: "AdmissionListResults");

            migrationBuilder.DropColumn(
                name: "LinkedGuardianId",
                table: "AdmissionListResults");
        }
    }
}
