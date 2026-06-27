using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEventCategoryAndEmailTemplateCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventCategoryId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "EmailTemplates",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventCategoryId",
                table: "Events",
                column: "EventCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventCategories_EventCategoryId",
                table: "Events",
                column: "EventCategoryId",
                principalTable: "EventCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "EventCategories",
                columns: new[] { "Name", "Slug", "IsActive", "CreatedBy", "CreatedAt", "IsDeleted" },
                values: new object[,]
                {
                    { "General Event", "general-event", true, "seeder", now, false },
                    { "Parent-Teacher Meeting", "parent-meeting", true, "seeder", now, false },
                    { "Sports Event", "sports-event", true, "seeder", now, false },
                    { "Academic Event", "academic-event", true, "seeder", now, false },
                    { "Exam Announcement", "exam-announcement", true, "seeder", now, false },
                    { "Holiday Notice", "holiday-notice", true, "seeder", now, false },
                    { "Emergency Notice", "emergency-notice", true, "seeder", now, false },
                    { "Admission Event", "admission-event", true, "seeder", now, false }
                });

            migrationBuilder.Sql(@"
                UPDATE e SET e.EventCategoryId = ec.Id
                FROM Events e
                INNER JOIN EventCategories ec ON ec.Slug = CASE e.Category
                    WHEN 'ParentMeeting' THEN 'parent-meeting'
                    WHEN 'SportsEvent' THEN 'sports-event'
                    WHEN 'AcademicEvent' THEN 'academic-event'
                    WHEN 'ExamAnnouncement' THEN 'exam-announcement'
                    WHEN 'HolidayNotice' THEN 'holiday-notice'
                    WHEN 'EmergencyNotice' THEN 'emergency-notice'
                    WHEN 'AdmissionEvent' THEN 'admission-event'
                    ELSE 'general-event'
                END
                WHERE e.Category IS NOT NULL AND e.Category != ''
            ");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventCategories_EventCategoryId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventCategories");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventCategoryId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventCategoryId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "EmailTemplates");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Events",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "EventPublished");
        }
    }
}
