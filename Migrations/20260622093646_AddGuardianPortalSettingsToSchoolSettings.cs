using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddGuardianPortalSettingsToSchoolSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnableGuardianActivation",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableGuardianNotifications",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableGuardianPortal",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireGuardianForAdmission",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableGuardianActivation",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "EnableGuardianNotifications",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "EnableGuardianPortal",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RequireGuardianForAdmission",
                table: "SchoolSettings");
        }
    }
}
