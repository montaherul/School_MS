using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDenormalizedFieldsToResultEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "StudentSubjectResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "StudentSubjectResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "StudentSubjectResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "StudentExamResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "StudentExamResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "StudentExamResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "StudentExamResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "ResultPublications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "MeritResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "MeritResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "MeritResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "Marks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Marks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Marks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "MarkEntryDrafts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "MarkEntryDrafts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "MarkEntryDrafts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "FinalResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "FinalResults",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AcademicYearId", "ClassId", "SectionId" },
                values: new object[] { 0, 0, 0 });

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AcademicYearId", "ClassId", "SectionId" },
                values: new object[] { 0, 0, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "ResultPublications");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "MeritResults");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "MeritResults");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "MeritResults");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "MarkEntryDrafts");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "MarkEntryDrafts");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "MarkEntryDrafts");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "FinalResults");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "FinalResults");
        }
    }
}
