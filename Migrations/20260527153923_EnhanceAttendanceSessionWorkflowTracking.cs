using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceAttendanceSessionWorkflowTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeAttendances_EmployeeId_AttendanceDate",
                table: "EmployeeAttendances");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "EmployeeAttendances",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldDefaultValue: "system");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "AttendanceSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "AttendanceSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevisedAt",
                table: "AttendanceSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisedBy",
                table: "AttendanceSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "AttendanceSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedBy",
                table: "AttendanceSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendances_EmployeeId_AttendanceDate",
                table: "EmployeeAttendances",
                columns: new[] { "EmployeeId", "AttendanceDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeAttendances_EmployeeId_AttendanceDate",
                table: "EmployeeAttendances");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "RevisedAt",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "RevisedBy",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "SubmittedBy",
                table: "AttendanceSessions");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "EmployeeAttendances",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "system",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendances_EmployeeId_AttendanceDate",
                table: "EmployeeAttendances",
                columns: new[] { "EmployeeId", "AttendanceDate" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
