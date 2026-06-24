using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEventNotificationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DailyDigestMode",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DefaultEventTemplateId",
                table: "SchoolSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnableEventEmailNotifications",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableStudentNotifications",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaximumEmailsPerBatch",
                table: "SchoolSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NotificationSenderEmail",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotificationSenderName",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SendImmediately",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendOnPublish",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EventNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: true),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    StudentIds = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    GuardianIds = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NotifyGuardians = table.Column<bool>(type: "bit", nullable: false),
                    NotifyStudents = table.Column<bool>(type: "bit", nullable: false),
                    PrimaryGuardianOnly = table.Column<bool>(type: "bit", nullable: false),
                    EmailTemplateId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalRecipients = table.Column<int>(type: "int", nullable: false),
                    SentCount = table.Column<int>(type: "int", nullable: false),
                    FailedCount = table.Column<int>(type: "int", nullable: false),
                    TriggeredByUserId = table.Column<int>(type: "int", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventNotifications_EmailTemplates_EmailTemplateId",
                        column: x => x.EmailTemplateId,
                        principalTable: "EmailTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventNotifications_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventNotificationRecipients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventNotificationId = table.Column<int>(type: "int", nullable: false),
                    GuardianId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    RecipientEmail = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    DeliveryStatus = table.Column<int>(type: "int", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNotificationRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventNotificationRecipients_EventNotifications_EventNotificationId",
                        column: x => x.EventNotificationId,
                        principalTable: "EventNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventNotificationRecipients_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventNotificationRecipients_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventNotificationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventNotificationId = table.Column<int>(type: "int", nullable: false),
                    RecipientId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNotificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventNotificationLogs_EventNotificationRecipients_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "EventNotificationRecipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventNotificationLogs_EventNotifications_EventNotificationId",
                        column: x => x.EventNotificationId,
                        principalTable: "EventNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventNotificationQueues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventNotificationId = table.Column<int>(type: "int", nullable: false),
                    RecipientId = table.Column<int>(type: "int", nullable: true),
                    RecipientEmail = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    MaxRetries = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextRetryAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNotificationQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventNotificationQueues_EventNotificationRecipients_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "EventNotificationRecipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventNotificationQueues_EventNotifications_EventNotificationId",
                        column: x => x.EventNotificationId,
                        principalTable: "EventNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventNotificationLogs_EventNotificationId",
                table: "EventNotificationLogs",
                column: "EventNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotificationLogs_RecipientId",
                table: "EventNotificationLogs",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotificationQueues_EventNotificationId",
                table: "EventNotificationQueues",
                column: "EventNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotificationQueues_RecipientId",
                table: "EventNotificationQueues",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotificationRecipients_EventNotificationId",
                table: "EventNotificationRecipients",
                column: "EventNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotificationRecipients_GuardianId",
                table: "EventNotificationRecipients",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotificationRecipients_StudentId",
                table: "EventNotificationRecipients",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotifications_EmailTemplateId",
                table: "EventNotifications",
                column: "EmailTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotifications_EventId",
                table: "EventNotifications",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventNotificationLogs");

            migrationBuilder.DropTable(
                name: "EventNotificationQueues");

            migrationBuilder.DropTable(
                name: "EventNotificationRecipients");

            migrationBuilder.DropTable(
                name: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "DailyDigestMode",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "DefaultEventTemplateId",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "EnableEventEmailNotifications",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "EnableStudentNotifications",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "MaximumEmailsPerBatch",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NotificationSenderEmail",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NotificationSenderName",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SendImmediately",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SendOnPublish",
                table: "SchoolSettings");
        }
    }
}
