using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddExamClassSectionGroupFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "StudentSubjectResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "Marks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Exams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppliedStudentGroupId",
                table: "Admissions",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Admissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "AppliedStudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClassId", "SectionId" },
                values: new object[] { 0, null });

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 1,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 2,
                column: "StudentGroupId",
                value: null);

            // Fix existing exam records: ClassId=0 (default) won't satisfy FK.
            // Assign the smallest valid ClassId so the FK constraint can be created.
            migrationBuilder.Sql(@"
                UPDATE e SET e.ClassId = (SELECT TOP 1 Id FROM Classes ORDER BY Id)
                FROM Exams e
                WHERE e.ClassId = 0 AND EXISTS (SELECT 1 FROM Classes)
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ClassId",
                table: "Exams",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_SectionId",
                table: "Exams",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Classes_ClassId",
                table: "Exams",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Sections_SectionId",
                table: "Exams",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Classes_ClassId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Sections_SectionId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_ClassId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_SectionId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "AppliedStudentGroupId",
                table: "Admissions");
        }
    }
}
