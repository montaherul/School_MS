using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixTeacherAssignmentSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId",
                table: "TeacherSubjectAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId",
                table: "TeacherSubjectAssignments",
                columns: new[] { "TeacherId", "SubjectId", "ClassId", "SectionId", "AcademicYearId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "ClassId", "SectionId", "AcademicYearId" },
                unique: true,
                filter: "[IsActive] = 1 AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "TeacherId", "ClassId", "SectionId", "AcademicYearId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId",
                table: "TeacherSubjectAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId",
                table: "TeacherSubjectAssignments",
                columns: new[] { "TeacherId", "SubjectId", "ClassId", "SectionId", "AcademicYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "ClassId", "SectionId", "AcademicYearId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "TeacherId", "ClassId", "SectionId", "AcademicYearId" },
                unique: true);
        }
    }
}
