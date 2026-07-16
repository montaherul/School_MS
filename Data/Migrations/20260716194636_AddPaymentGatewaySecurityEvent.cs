using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentGatewaySecurityEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentGatewaySecurityEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentProviderId = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Severity = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    EventSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MachineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewaySecurityEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewaySecurityEvents_EventType",
                table: "PaymentGatewaySecurityEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewaySecurityEvents_PaymentProviderId",
                table: "PaymentGatewaySecurityEvents",
                column: "PaymentProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewaySecurityEvents_PaymentProviderId_PerformedAt",
                table: "PaymentGatewaySecurityEvents",
                columns: new[] { "PaymentProviderId", "PerformedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewaySecurityEvents_PerformedAt",
                table: "PaymentGatewaySecurityEvents",
                column: "PerformedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewaySecurityEvents_PerformedBy",
                table: "PaymentGatewaySecurityEvents",
                column: "PerformedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentGatewaySecurityEvents");
        }
    }
}
