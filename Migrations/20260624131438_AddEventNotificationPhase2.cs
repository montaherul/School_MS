using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEventNotificationPhase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultReminderTiming",
                table: "SchoolSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultReminderUnit",
                table: "SchoolSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableEventApprovalWorkflow",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableEventReminders",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxRemindersPerEvent",
                table: "SchoolSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Events",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Events",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "EventNotifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "EventNotifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BounceCount",
                table: "EventNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClickCount",
                table: "EventNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ComplaintCount",
                table: "EventNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DuplicateHash",
                table: "EventNotifications",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BouncedAt",
                table: "EventNotificationRecipients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClickedAt",
                table: "EventNotificationRecipients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ComplaintAt",
                table: "EventNotificationRecipients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageId",
                table: "EventNotificationRecipients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentIds",
                table: "EventNotificationQueues",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "EventNotificationQueues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EventNotificationAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventNotificationId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    IsInline = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNotificationAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventNotificationAttachments_EventNotifications_EventNotificationId",
                        column: x => x.EventNotificationId,
                        principalTable: "EventNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuardainNotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuardianId = table.Column<int>(type: "int", nullable: false),
                    OptInEventNotifications = table.Column<bool>(type: "bit", nullable: false),
                    OptInSMS = table.Column<bool>(type: "bit", nullable: false),
                    OptInEmail = table.Column<bool>(type: "bit", nullable: false),
                    OptInWhatsApp = table.Column<bool>(type: "bit", nullable: false),
                    OptInInApp = table.Column<bool>(type: "bit", nullable: false),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedEmail = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    EmailVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscribedEventTypes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuietHoursStart = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuietHoursEnd = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AllowReminders = table.Column<bool>(type: "bit", nullable: false),
                    ReminderLeadMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuardainNotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuardainNotificationPreferences_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReminderConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ReminderValue = table.Column<int>(type: "int", nullable: false),
                    ReminderUnit = table.Column<int>(type: "int", nullable: false),
                    LastSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentCount = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReminderConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReminderConfigs_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventNotificationId = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledNotifications_EventNotifications_EventNotificationId",
                        column: x => x.EventNotificationId,
                        principalTable: "EventNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventNotificationAttachments_EventNotificationId",
                table: "EventNotificationAttachments",
                column: "EventNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_GuardainNotificationPreferences_GuardianId",
                table: "GuardainNotificationPreferences",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderConfigs_EventId",
                table: "ReminderConfigs",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledNotifications_EventNotificationId",
                table: "ScheduledNotifications",
                column: "EventNotificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventNotificationAttachments");

            migrationBuilder.DropTable(
                name: "GuardainNotificationPreferences");

            migrationBuilder.DropTable(
                name: "ReminderConfigs");

            migrationBuilder.DropTable(
                name: "ScheduledNotifications");

            migrationBuilder.DropColumn(
                name: "DefaultReminderTiming",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "DefaultReminderUnit",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "EnableEventApprovalWorkflow",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "EnableEventReminders",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "MaxRemindersPerEvent",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "BounceCount",
                table: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "ClickCount",
                table: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "ComplaintCount",
                table: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "DuplicateHash",
                table: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "BouncedAt",
                table: "EventNotificationRecipients");

            migrationBuilder.DropColumn(
                name: "ClickedAt",
                table: "EventNotificationRecipients");

            migrationBuilder.DropColumn(
                name: "ComplaintAt",
                table: "EventNotificationRecipients");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "EventNotificationRecipients");

            migrationBuilder.DropColumn(
                name: "AttachmentIds",
                table: "EventNotificationQueues");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "EventNotificationQueues");
        }
    }
}
