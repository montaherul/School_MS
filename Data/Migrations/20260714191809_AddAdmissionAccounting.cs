using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmissionAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdmissionApplicationId",
                table: "OnlinePaymentRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentPurpose",
                table: "OnlinePaymentRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AdmissionReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    AdmissionApplicationId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApplicantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRefunded = table.Column<bool>(type: "bit", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RefundReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConvertedStudentId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionReceipts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePaymentRequests_AdmissionApplicationId",
                table: "OnlinePaymentRequests",
                column: "AdmissionApplicationId",
                filter: "[AdmissionApplicationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionReceipts_AdmissionApplicationId",
                table: "AdmissionReceipts",
                column: "AdmissionApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionReceipts_ReceiptNo",
                table: "AdmissionReceipts",
                column: "ReceiptNo",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionReceipts");

            migrationBuilder.DropIndex(
                name: "IX_OnlinePaymentRequests_AdmissionApplicationId",
                table: "OnlinePaymentRequests");

            migrationBuilder.DropColumn(
                name: "AdmissionApplicationId",
                table: "OnlinePaymentRequests");

            migrationBuilder.DropColumn(
                name: "PaymentPurpose",
                table: "OnlinePaymentRequests");
        }
    }
}
