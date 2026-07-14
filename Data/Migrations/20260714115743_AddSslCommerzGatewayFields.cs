using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSslCommerzGatewayFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GatewayResponse",
                table: "OnlinePaymentRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewaySessionKey",
                table: "OnlinePaymentRequests",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayTransactionId",
                table: "OnlinePaymentRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GatewayResponse",
                table: "OnlinePaymentRequests");

            migrationBuilder.DropColumn(
                name: "GatewaySessionKey",
                table: "OnlinePaymentRequests");

            migrationBuilder.DropColumn(
                name: "GatewayTransactionId",
                table: "OnlinePaymentRequests");
        }
    }
}
