using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddClassSubjectTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SchoolClassId",
                table: "ClassSubjectTeachers");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "ClassSubjectTeachers");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "ClassSubjectTeachers",
                newName: "ClassSubjectId");

            migrationBuilder.CreateTable(
                name: "ClassSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSubjects_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassSubjects_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjectTeachers_ClassSubjectId",
                table: "ClassSubjectTeachers",
                column: "ClassSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_SchoolClassId",
                table: "ClassSubjects",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_SectionId",
                table: "ClassSubjects",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_SubjectId",
                table: "ClassSubjects",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSubjectTeachers_ClassSubjects_ClassSubjectId",
                table: "ClassSubjectTeachers",
                column: "ClassSubjectId",
                principalTable: "ClassSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSubjectTeachers_ClassSubjects_ClassSubjectId",
                table: "ClassSubjectTeachers");

            migrationBuilder.DropTable(
                name: "ClassSubjects");

            migrationBuilder.DropIndex(
                name: "IX_ClassSubjectTeachers_ClassSubjectId",
                table: "ClassSubjectTeachers");

            migrationBuilder.RenameColumn(
                name: "ClassSubjectId",
                table: "ClassSubjectTeachers",
                newName: "SubjectId");

            migrationBuilder.AddColumn<int>(
                name: "SchoolClassId",
                table: "ClassSubjectTeachers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "ClassSubjectTeachers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
