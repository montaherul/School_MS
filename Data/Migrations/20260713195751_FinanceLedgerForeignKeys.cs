using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinanceLedgerForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FeeDiscountId",
                table: "FeeLedgers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FeeRefundId",
                table: "FeeLedgers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FeeWaiverId",
                table: "FeeLedgers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeLedgers_FeeDiscountId",
                table: "FeeLedgers",
                column: "FeeDiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeLedgers_FeeInvoiceId",
                table: "FeeLedgers",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeLedgers_FeePaymentId",
                table: "FeeLedgers",
                column: "FeePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeLedgers_FeeRefundId",
                table: "FeeLedgers",
                column: "FeeRefundId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeLedgers_FeeWaiverId",
                table: "FeeLedgers",
                column: "FeeWaiverId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeLedgers_StudentId",
                table: "FeeLedgers",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeeLedgers_FeeDiscounts_FeeDiscountId",
                table: "FeeLedgers",
                column: "FeeDiscountId",
                principalTable: "FeeDiscounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeeLedgers_FeeInvoices_FeeInvoiceId",
                table: "FeeLedgers",
                column: "FeeInvoiceId",
                principalTable: "FeeInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeeLedgers_FeeRefunds_FeeRefundId",
                table: "FeeLedgers",
                column: "FeeRefundId",
                principalTable: "FeeRefunds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeeLedgers_FeeWaivers_FeeWaiverId",
                table: "FeeLedgers",
                column: "FeeWaiverId",
                principalTable: "FeeWaivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeeLedgers_Payments_FeePaymentId",
                table: "FeeLedgers",
                column: "FeePaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeeLedgers_FeeDiscounts_FeeDiscountId",
                table: "FeeLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_FeeLedgers_FeeInvoices_FeeInvoiceId",
                table: "FeeLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_FeeLedgers_FeeRefunds_FeeRefundId",
                table: "FeeLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_FeeLedgers_FeeWaivers_FeeWaiverId",
                table: "FeeLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_FeeLedgers_Payments_FeePaymentId",
                table: "FeeLedgers");

            migrationBuilder.DropIndex(
                name: "IX_FeeLedgers_FeeDiscountId",
                table: "FeeLedgers");

            migrationBuilder.DropIndex(
                name: "IX_FeeLedgers_FeeInvoiceId",
                table: "FeeLedgers");

            migrationBuilder.DropIndex(
                name: "IX_FeeLedgers_FeePaymentId",
                table: "FeeLedgers");

            migrationBuilder.DropIndex(
                name: "IX_FeeLedgers_FeeRefundId",
                table: "FeeLedgers");

            migrationBuilder.DropIndex(
                name: "IX_FeeLedgers_FeeWaiverId",
                table: "FeeLedgers");

            migrationBuilder.DropIndex(
                name: "IX_FeeLedgers_StudentId",
                table: "FeeLedgers");

            migrationBuilder.DropColumn(
                name: "FeeDiscountId",
                table: "FeeLedgers");

            migrationBuilder.DropColumn(
                name: "FeeRefundId",
                table: "FeeLedgers");

            migrationBuilder.DropColumn(
                name: "FeeWaiverId",
                table: "FeeLedgers");
        }
    }
}
