using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolSettingCMSFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassLabel",
                table: "SchoolSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeLabel",
                table: "SchoolSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficeHours",
                table: "SchoolSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolHistory",
                table: "SchoolSettings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentLabel",
                table: "SchoolSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherLabel",
                table: "SchoolSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WelcomeHeading",
                table: "SchoolSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WelcomeTagline",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WelcomeText",
                table: "SchoolSettings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassLabel",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "EmployeeLabel",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "OfficeHours",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SchoolHistory",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "StudentLabel",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "TeacherLabel",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "WelcomeHeading",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "WelcomeTagline",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "WelcomeText",
                table: "SchoolSettings");
        }
    }
}
