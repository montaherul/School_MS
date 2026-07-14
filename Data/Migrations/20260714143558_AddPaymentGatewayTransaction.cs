using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentGatewayTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentExpiryAt",
                table: "OnlinePaymentRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentGatewayTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OnlinePaymentRequestId = table.Column<int>(type: "int", nullable: false),
                    GatewayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ValidationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CardType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GatewayAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    GatewayStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RiskLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    InitRequestPayload = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    InitResponsePayload = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    IpnPayload = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    ValidationPayload = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    InitiatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentGatewayTransactions_OnlinePaymentRequests_OnlinePaymentRequestId",
                        column: x => x.OnlinePaymentRequestId,
                        principalTable: "OnlinePaymentRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayTransactions_OnlinePaymentRequestId",
                table: "PaymentGatewayTransactions",
                column: "OnlinePaymentRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentGatewayTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentExpiryAt",
                table: "OnlinePaymentRequests");
        }
    }
}
