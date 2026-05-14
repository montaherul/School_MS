using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermanentAddress",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PresentAddress",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "PermanentDistrict",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentPostOffice",
                table: "Employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentThana",
                table: "Employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentVillage",
                table: "Employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentDistrict",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentPostOffice",
                table: "Employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentThana",
                table: "Employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentVillage",
                table: "Employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermanentDistrict",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PermanentPostOffice",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PermanentThana",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PermanentVillage",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PresentDistrict",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PresentPostOffice",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PresentThana",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PresentVillage",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "PermanentAddress",
                table: "Employees",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PresentAddress",
                table: "Employees",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
