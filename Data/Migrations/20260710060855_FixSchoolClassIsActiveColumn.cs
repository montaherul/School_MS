using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSchoolClassIsActiveColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plan",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "LessonPlans");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "WebsitePages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TransferCertificates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NewSchoolName",
                table: "TransferCertificates",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OldClassId",
                table: "TransferCertificates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OldSectionId",
                table: "TransferCertificates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "Syllabi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Syllabi",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Syllabi",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Syllabi",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Syllabi",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Syllabi",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Syllabi",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "Syllabi",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "Syllabi",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "StudyMaterials",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "StudyMaterials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StudyMaterials",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalUrl",
                table: "StudyMaterials",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "StudyMaterials",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "StudyMaterials",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "StudyMaterials",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "StudyMaterials",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StudyMaterials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MaterialType",
                table: "StudyMaterials",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "LessonPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AssessmentMethod",
                table: "LessonPlans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "LessonPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "LessonPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LessonPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Materials",
                table: "LessonPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Objectives",
                table: "LessonPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Procedure",
                table: "LessonPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchoolClassId",
                table: "LessonPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "LessonPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "LessonPlans",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "LessonPlans",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "EmailTemplates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CurriculumVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurriculumVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurriculumVersions_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurriculumSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurriculumVersionId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    SubjectCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalHours = table.Column<int>(type: "int", nullable: false),
                    IsCompulsory = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurriculumSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurriculumSubjects_CurriculumVersions_CurriculumVersionId",
                        column: x => x.CurriculumVersionId,
                        principalTable: "CurriculumVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurriculumSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Syllabi_AcademicYearId",
                table: "Syllabi",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Syllabi_SchoolClassId",
                table: "Syllabi",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Syllabi_SubjectId",
                table: "Syllabi",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_AcademicYearId",
                table: "StudyMaterials",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_SchoolClassId",
                table: "StudyMaterials",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_SubjectId",
                table: "StudyMaterials",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlans_AcademicYearId",
                table: "LessonPlans",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlans_SchoolClassId",
                table: "LessonPlans",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlans_SubjectId",
                table: "LessonPlans",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumSubjects_CurriculumVersionId",
                table: "CurriculumSubjects",
                column: "CurriculumVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumSubjects_SubjectId",
                table: "CurriculumSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumVersions_AcademicYearId",
                table: "CurriculumVersions",
                column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPlans_AcademicYears_AcademicYearId",
                table: "LessonPlans",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPlans_Classes_SchoolClassId",
                table: "LessonPlans",
                column: "SchoolClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPlans_Subjects_SubjectId",
                table: "LessonPlans",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyMaterials_AcademicYears_AcademicYearId",
                table: "StudyMaterials",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyMaterials_Classes_SchoolClassId",
                table: "StudyMaterials",
                column: "SchoolClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyMaterials_Subjects_SubjectId",
                table: "StudyMaterials",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabi_AcademicYears_AcademicYearId",
                table: "Syllabi",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabi_Classes_SchoolClassId",
                table: "Syllabi",
                column: "SchoolClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabi_Subjects_SubjectId",
                table: "Syllabi",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonPlans_AcademicYears_AcademicYearId",
                table: "LessonPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonPlans_Classes_SchoolClassId",
                table: "LessonPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonPlans_Subjects_SubjectId",
                table: "LessonPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyMaterials_AcademicYears_AcademicYearId",
                table: "StudyMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyMaterials_Classes_SchoolClassId",
                table: "StudyMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyMaterials_Subjects_SubjectId",
                table: "StudyMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_Syllabi_AcademicYears_AcademicYearId",
                table: "Syllabi");

            migrationBuilder.DropForeignKey(
                name: "FK_Syllabi_Classes_SchoolClassId",
                table: "Syllabi");

            migrationBuilder.DropForeignKey(
                name: "FK_Syllabi_Subjects_SubjectId",
                table: "Syllabi");

            migrationBuilder.DropTable(
                name: "CurriculumSubjects");

            migrationBuilder.DropTable(
                name: "CurriculumVersions");

            migrationBuilder.DropIndex(
                name: "IX_Syllabi_AcademicYearId",
                table: "Syllabi");

            migrationBuilder.DropIndex(
                name: "IX_Syllabi_SchoolClassId",
                table: "Syllabi");

            migrationBuilder.DropIndex(
                name: "IX_Syllabi_SubjectId",
                table: "Syllabi");

            migrationBuilder.DropIndex(
                name: "IX_StudyMaterials_AcademicYearId",
                table: "StudyMaterials");

            migrationBuilder.DropIndex(
                name: "IX_StudyMaterials_SchoolClassId",
                table: "StudyMaterials");

            migrationBuilder.DropIndex(
                name: "IX_StudyMaterials_SubjectId",
                table: "StudyMaterials");

            migrationBuilder.DropIndex(
                name: "IX_LessonPlans_AcademicYearId",
                table: "LessonPlans");

            migrationBuilder.DropIndex(
                name: "IX_LessonPlans_SchoolClassId",
                table: "LessonPlans");

            migrationBuilder.DropIndex(
                name: "IX_LessonPlans_SubjectId",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TransferCertificates");

            migrationBuilder.DropColumn(
                name: "NewSchoolName",
                table: "TransferCertificates");

            migrationBuilder.DropColumn(
                name: "OldClassId",
                table: "TransferCertificates");

            migrationBuilder.DropColumn(
                name: "OldSectionId",
                table: "TransferCertificates");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "Syllabi");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Syllabi");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Syllabi");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Syllabi");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Syllabi");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Syllabi");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Syllabi");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "Syllabi");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "Syllabi");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "ExternalUrl",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "MaterialType",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "AssessmentMethod",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "Materials",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "Objectives",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "Procedure",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "SchoolClassId",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LessonPlans");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "LessonPlans");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "WebsitePages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "StudyMaterials",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "Plan",
                table: "LessonPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "LessonPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "EmailTemplates",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
