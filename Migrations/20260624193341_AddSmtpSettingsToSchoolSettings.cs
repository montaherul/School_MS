using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSmtpSettingsToSchoolSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseUrl",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SmtpEnableSsl",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "SmtpFromEmail",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmtpHost",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmtpPassword",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SmtpPort",
                table: "SchoolSettings",
                type: "int",
                nullable: false,
                defaultValue: 587);

            migrationBuilder.AddColumn<string>(
                name: "SmtpUserName",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseUrl",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SmtpEnableSsl",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SmtpFromEmail",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SmtpHost",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SmtpPassword",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SmtpPort",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SmtpUserName",
                table: "SchoolSettings");
        }
    }
}
