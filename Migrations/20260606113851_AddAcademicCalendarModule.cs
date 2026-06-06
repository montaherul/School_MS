using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicCalendarModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId",
                table: "TeacherSubjectAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceNotificationLogs_StudentId_AttendanceDate_NotificationType_NotificationChannel",
                table: "AttendanceNotificationLogs");

          


            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "AttendanceNotificationLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AcademicCalendars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicCalendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicCalendars_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcademicCalendarEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicCalendarId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRecurringWeekly = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicCalendarEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicCalendarEvents_AcademicCalendars_AcademicCalendarId",
                        column: x => x.AcademicCalendarId,
                        principalTable: "AcademicCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_GroupId_AcademicYearId",
                table: "TeacherSubjectAssignments",
                columns: new[] { "TeacherId", "SubjectId", "ClassId", "SectionId", "GroupId", "AcademicYearId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_ClassId_SectionId_GroupId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "ClassId", "SectionId", "GroupId", "AcademicYearId" },
                unique: true,
                filter: "[IsActive] = 1 AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_GroupId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "TeacherId", "ClassId", "SectionId", "GroupId", "AcademicYearId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceNotificationLogs_EmployeeId_AttendanceDate_NotificationType_NotificationChannel",
                table: "AttendanceNotificationLogs",
                columns: new[] { "EmployeeId", "AttendanceDate", "NotificationType", "NotificationChannel" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [EmployeeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceNotificationLogs_StudentId_AttendanceDate_NotificationType_NotificationChannel",
                table: "AttendanceNotificationLogs",
                columns: new[] { "StudentId", "AttendanceDate", "NotificationType", "NotificationChannel" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [EmployeeId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendarEvents_AcademicCalendarId",
                table: "AcademicCalendarEvents",
                column: "AcademicCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_AcademicYearId",
                table: "AcademicCalendars",
                column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceNotificationLogs_Employees_EmployeeId",
                table: "AttendanceNotificationLogs",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceNotificationLogs_Employees_EmployeeId",
                table: "AttendanceNotificationLogs");

            migrationBuilder.DropTable(
                name: "AcademicCalendarEvents");

            migrationBuilder.DropTable(
                name: "AcademicCalendars");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_GroupId_AcademicYearId",
                table: "TeacherSubjectAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_ClassId_SectionId_GroupId_AcademicYearId",
                table: "TeacherClassAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_GroupId_AcademicYearId",
                table: "TeacherClassAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceNotificationLogs_EmployeeId_AttendanceDate_NotificationType_NotificationChannel",
                table: "AttendanceNotificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceNotificationLogs_StudentId_AttendanceDate_NotificationType_NotificationChannel",
                table: "AttendanceNotificationLogs");

            


            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "AttendanceNotificationLogs");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId",
                table: "TeacherSubjectAssignments",
                columns: new[] { "TeacherId", "SubjectId", "ClassId", "SectionId", "AcademicYearId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "ClassId", "SectionId", "AcademicYearId" },
                unique: true,
                filter: "[IsActive] = 1 AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "TeacherId", "ClassId", "SectionId", "AcademicYearId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceNotificationLogs_StudentId_AttendanceDate_NotificationType_NotificationChannel",
                table: "AttendanceNotificationLogs",
                columns: new[] { "StudentId", "AttendanceDate", "NotificationType", "NotificationChannel" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
