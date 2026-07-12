using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class PhaseXX_55_AddExamAggregateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubjects_Exams_ExamId",
                table: "ExamSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubjects_Teachers_TeacherId",
                table: "ExamSubjects");

            migrationBuilder.DropTable(
                name: "ExamGroupSections");

            migrationBuilder.DropTable(
                name: "ExamGroupSubjectComponents");

            migrationBuilder.DropTable(
                name: "ExamGroupSubjects");

            migrationBuilder.DropTable(
                name: "ExamGroupClasses");

            migrationBuilder.DropTable(
                name: "ExamGroups");

            migrationBuilder.AddColumn<int>(
                name: "ExamSubjectId",
                table: "StudentSubjectResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExamId",
                table: "ExamTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ExamClassId",
                table: "ExamSubjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReligionSubject",
                table: "ExamSubjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NCTBCode",
                table: "ExamSubjects",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PracticalMarks",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SubjectCode",
                table: "ExamSubjects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectGroup",
                table: "ExamSubjects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectName",
                table: "ExamSubjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectType",
                table: "ExamSubjects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeacherEmployeeCode",
                table: "ExamSubjects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherName",
                table: "ExamSubjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TheoryMarks",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BuildingName",
                table: "ExamSchedules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "ExamSchedules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShiftName",
                table: "ExamSchedules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArchiveReason",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "Exams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArchivedByUserId",
                table: "Exams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ExamClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamClasses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamClasses_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamSubjectComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamSubjectId = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    MaxMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PassMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ComponentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ComponentCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSubjectComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSubjectComponents_ExamComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "ExamComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSubjectComponents_ExamSubjects_ExamSubjectId",
                        column: x => x.ExamSubjectId,
                        principalTable: "ExamSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSections_ExamClasses_ExamClassId",
                        column: x => x.ExamClassId,
                        principalTable: "ExamClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamSections_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentComponentMarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ExamSubjectId = table.Column<int>(type: "int", nullable: false),
                    ExamSubjectComponentId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    StudentGroupId = table.Column<int>(type: "int", nullable: true),
                    ObtainedMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentComponentMarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentComponentMarks_ExamSubjectComponents_ExamSubjectComponentId",
                        column: x => x.ExamSubjectComponentId,
                        principalTable: "ExamSubjectComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentComponentMarks_ExamSubjects_ExamSubjectId",
                        column: x => x.ExamSubjectId,
                        principalTable: "ExamSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentComponentMarks_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentComponentMarks_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ArchiveReason", "ArchivedAt", "ArchivedByUserId", "IsArchived", "IsPublished" },
                values: new object[] { null, null, null, false, false });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectResults_ExamSubjectId",
                table: "StudentSubjectResults",
                column: "ExamSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTemplates_ExamId",
                table: "ExamTemplates",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubjects_ExamClassId",
                table: "ExamSubjects",
                column: "ExamClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubjects_ExamId_ClassId_SubjectId",
                table: "ExamSubjects",
                columns: new[] { "ExamId", "ClassId", "SubjectId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamClasses_ClassId",
                table: "ExamClasses",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamClasses_ExamId_ClassId",
                table: "ExamClasses",
                columns: new[] { "ExamId", "ClassId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSections_ExamClassId_SectionId",
                table: "ExamSections",
                columns: new[] { "ExamClassId", "SectionId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSections_SectionId",
                table: "ExamSections",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubjectComponents_ComponentId",
                table: "ExamSubjectComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubjectComponents_ExamSubjectId_ComponentId",
                table: "ExamSubjectComponents",
                columns: new[] { "ExamSubjectId", "ComponentId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_StudentComponentMarks_ExamId",
                table: "StudentComponentMarks",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentComponentMarks_ExamSubjectComponentId",
                table: "StudentComponentMarks",
                column: "ExamSubjectComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentComponentMarks_ExamSubjectId",
                table: "StudentComponentMarks",
                column: "ExamSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentComponentMarks_StudentId",
                table: "StudentComponentMarks",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubjects_ExamClasses_ExamClassId",
                table: "ExamSubjects",
                column: "ExamClassId",
                principalTable: "ExamClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubjects_Exams_ExamId",
                table: "ExamSubjects",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubjects_Teachers_TeacherId",
                table: "ExamSubjects",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamTemplates_Exams_ExamId",
                table: "ExamTemplates",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjectResults_ExamSubjects_ExamSubjectId",
                table: "StudentSubjectResults",
                column: "ExamSubjectId",
                principalTable: "ExamSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubjects_ExamClasses_ExamClassId",
                table: "ExamSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubjects_Exams_ExamId",
                table: "ExamSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubjects_Teachers_TeacherId",
                table: "ExamSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamTemplates_Exams_ExamId",
                table: "ExamTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjectResults_ExamSubjects_ExamSubjectId",
                table: "StudentSubjectResults");

            migrationBuilder.DropTable(
                name: "ExamSections");

            migrationBuilder.DropTable(
                name: "StudentComponentMarks");

            migrationBuilder.DropTable(
                name: "ExamClasses");

            migrationBuilder.DropTable(
                name: "ExamSubjectComponents");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubjectResults_ExamSubjectId",
                table: "StudentSubjectResults");

            migrationBuilder.DropIndex(
                name: "IX_ExamTemplates_ExamId",
                table: "ExamTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ExamSubjects_ExamClassId",
                table: "ExamSubjects");

            migrationBuilder.DropIndex(
                name: "IX_ExamSubjects_ExamId_ClassId_SubjectId",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "ExamSubjectId",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "ExamTemplates");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "ExamClassId",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "IsReligionSubject",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "NCTBCode",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "PracticalMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "SubjectCode",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "SubjectGroup",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "SubjectName",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "SubjectType",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "TeacherEmployeeCode",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "TeacherName",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "TheoryMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "BuildingName",
                table: "ExamSchedules");

            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "ExamSchedules");

            migrationBuilder.DropColumn(
                name: "ShiftName",
                table: "ExamSchedules");

            migrationBuilder.DropColumn(
                name: "ArchiveReason",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ArchivedByUserId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Exams");

            migrationBuilder.CreateTable(
                name: "ExamGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExamType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedByUserId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Term = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamGroups_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamGroupClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    ExamGroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamGroupClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamGroupClasses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamGroupClasses_ExamGroups_ExamGroupId",
                        column: x => x.ExamGroupId,
                        principalTable: "ExamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamGroupSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamGroupClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamGroupSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamGroupSections_ExamGroupClasses_ExamGroupClassId",
                        column: x => x.ExamGroupClassId,
                        principalTable: "ExamGroupClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamGroupSections_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamGroupSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamGroupClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FullMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    IsReligionSubject = table.Column<bool>(type: "bit", nullable: false),
                    PassMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamGroupSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamGroupSubjects_ExamGroupClasses_ExamGroupClassId",
                        column: x => x.ExamGroupClassId,
                        principalTable: "ExamGroupClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamGroupSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamGroupSubjects_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ExamGroupSubjectComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    ExamGroupSubjectId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MaxMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PassMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamGroupSubjectComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamGroupSubjectComponents_ExamComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "ExamComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamGroupSubjectComponents_ExamGroupSubjects_ExamGroupSubjectId",
                        column: x => x.ExamGroupSubjectId,
                        principalTable: "ExamGroupSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupClasses_ClassId",
                table: "ExamGroupClasses",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupClasses_ExamGroupId_ClassId",
                table: "ExamGroupClasses",
                columns: new[] { "ExamGroupId", "ClassId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroups_AcademicYearId_Name",
                table: "ExamGroups",
                columns: new[] { "AcademicYearId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSections_ExamGroupClassId_SectionId",
                table: "ExamGroupSections",
                columns: new[] { "ExamGroupClassId", "SectionId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSections_SectionId",
                table: "ExamGroupSections",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSubjectComponents_ComponentId",
                table: "ExamGroupSubjectComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSubjectComponents_ExamGroupSubjectId_ComponentId",
                table: "ExamGroupSubjectComponents",
                columns: new[] { "ExamGroupSubjectId", "ComponentId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSubjects_ExamGroupClassId_SubjectId",
                table: "ExamGroupSubjects",
                columns: new[] { "ExamGroupClassId", "SubjectId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSubjects_SubjectId",
                table: "ExamGroupSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSubjects_TeacherId",
                table: "ExamGroupSubjects",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubjects_Exams_ExamId",
                table: "ExamSubjects",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubjects_Teachers_TeacherId",
                table: "ExamSubjects",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
