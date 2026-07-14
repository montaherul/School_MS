using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexGatewayTransactionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OnlinePaymentRequests_GatewayTransactionId",
                table: "OnlinePaymentRequests",
                column: "GatewayTransactionId",
                unique: true,
                filter: "[GatewayTransactionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OnlinePaymentRequests_GatewayTransactionId",
                table: "OnlinePaymentRequests");
        }
    }
}
