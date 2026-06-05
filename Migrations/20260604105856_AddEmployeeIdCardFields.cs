using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeIdCardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CardExpiryDate",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CardIssueDate",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CardPrintedAt",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CardVersion",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCardNumber",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRVerificationCode",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardExpiryDate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CardIssueDate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CardPrintedAt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CardVersion",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeeCardNumber",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "QRVerificationCode",
                table: "Employees");
        }
    }
}
