using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSettingsAndCalendarEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "AcademicCalendars");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "AutoAbsentTime",
                table: "AttendanceSettings",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AttendanceSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AttendanceSettings",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AttendanceSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AttendanceSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AttendanceSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AttendanceSettings",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "AcademicCalendars",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AcademicCalendars",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HolidayType",
                table: "AcademicCalendars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEventDay",
                table: "AcademicCalendars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsExamDay",
                table: "AcademicCalendars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHoliday",
                table: "AcademicCalendars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWorkingDay",
                table: "AcademicCalendars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "AcademicCalendars",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AcademicCalendars",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoAbsentTime",
                table: "AttendanceSettings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AttendanceSettings");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AttendanceSettings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AttendanceSettings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AttendanceSettings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AttendanceSettings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AttendanceSettings");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "AcademicCalendars");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AcademicCalendars");

            migrationBuilder.DropColumn(
                name: "HolidayType",
                table: "AcademicCalendars");

            migrationBuilder.DropColumn(
                name: "IsEventDay",
                table: "AcademicCalendars");

            migrationBuilder.DropColumn(
                name: "IsExamDay",
                table: "AcademicCalendars");

            migrationBuilder.DropColumn(
                name: "IsHoliday",
                table: "AcademicCalendars");

            migrationBuilder.DropColumn(
                name: "IsWorkingDay",
                table: "AcademicCalendars");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "AcademicCalendars");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AcademicCalendars");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AcademicCalendars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
