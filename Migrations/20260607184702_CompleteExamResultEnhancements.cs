using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class CompleteExamResultEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOptionalSubject",
                table: "StudentSubjectResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReligionSubject",
                table: "StudentSubjectResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "Exams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "ExamConfigurations",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: 1,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: 5);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_StudentGroupId",
                table: "Exams",
                column: "StudentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_StudentGroups_StudentGroupId",
                table: "Exams",
                column: "StudentGroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_StudentGroups_StudentGroupId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_StudentGroupId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "IsOptionalSubject",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "IsReligionSubject",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "ExamConfigurations");

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: 4);
        }
    }
}
