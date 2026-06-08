using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIsGroupBasedToSchoolClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGroupBased",
                table: "Classes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsGroupBased",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsGroupBased",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsGroupBased",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsGroupBased",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsGroupBased",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsGroupBased",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsGroupBased",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsGroupBased",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsGroupBased",
                value: true);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 10,
                column: "IsGroupBased",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGroupBased",
                table: "Classes");
        }
    }
}
