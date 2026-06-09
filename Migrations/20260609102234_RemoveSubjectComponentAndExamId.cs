using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSubjectComponentAndExamId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectMarkStructures_Exams_ExamId",
                table: "SubjectMarkStructures");

            migrationBuilder.DropTable(
                name: "SubjectComponents");

            migrationBuilder.DropIndex(
                name: "IX_SubjectMarkStructures_ComponentId_ExamId_SubjectId_StudentGroupId",
                table: "SubjectMarkStructures");

            migrationBuilder.DropIndex(
                name: "IX_SubjectMarkStructures_ExamId",
                table: "SubjectMarkStructures");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "SubjectMarkStructures");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Students]') AND name = 'OptionalSubjectId')
                BEGIN
                    ALTER TABLE [Students] ADD [OptionalSubjectId] int NULL;
                END
            ");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AssignedReligionSubjectId", "OptionalSubjectId" },
                values: new object[] { 30, null });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AssignedReligionSubjectId", "OptionalSubjectId" },
                values: new object[] { 30, null });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "IsReligionSubject", "ReligionType", "SubjectGroup" },
                values: new object[] { true, "Islam", "Religion" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "IsReligionSubject", "ReligionType", "SubjectGroup" },
                values: new object[] { true, "Hindu", "Religion" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "IsReligionSubject", "ReligionType", "SubjectGroup" },
                values: new object[] { true, "Buddhist", "Religion" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "IsReligionSubject", "ReligionType", "SubjectGroup" },
                values: new object[] { true, "Christian", "Religion" });

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [Subjects] WHERE [Id] = 34)
                BEGIN
                    INSERT INTO [Subjects] ([Id], [Code], [CreatedAt], [CreatedBy], [DefaultFullMarks], [DefaultPassMarks], [DisplayOrder], [HasAssignment], [HasCQ], [HasContinuousAssessment], [HasLab], [HasMCQ], [HasOral], [HasPractical], [HasViva], [HasWritten], [IsActive], [IsDeleted], [IsMandatory], [IsOptional], [IsPractical], [IsReligionSubject], [Name], [NameBn], [ReligionType], [SubjectGroup], [UpdatedAt], [UpdatedBy])
                    VALUES (34, N'MUS', '2026-01-01T00:00:00.0000000Z', N'system', 100, 33, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 0, N'Music', N'সঙ্গীত', NULL, N'Common', NULL, NULL);
                END
            ");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectMarkStructures_ComponentId_SubjectId_StudentGroupId",
                table: "SubjectMarkStructures",
                columns: new[] { "ComponentId", "SubjectId", "StudentGroupId" },
                unique: true,
                filter: "[SubjectId] IS NOT NULL AND [StudentGroupId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Students_OptionalSubjectId",
                table: "Students",
                column: "OptionalSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Subjects_OptionalSubjectId",
                table: "Students",
                column: "OptionalSubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Subjects_OptionalSubjectId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_SubjectMarkStructures_ComponentId_SubjectId_StudentGroupId",
                table: "SubjectMarkStructures");

            migrationBuilder.DropIndex(
                name: "IX_Students_OptionalSubjectId",
                table: "Students");

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DropColumn(
                name: "OptionalSubjectId",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "ExamId",
                table: "SubjectMarkStructures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubjectComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassSubjectId = table.Column<int>(type: "int", nullable: false),
                    ComponentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    MaxMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectComponents_ClassSubjects_ClassSubjectId",
                        column: x => x.ClassSubjectId,
                        principalTable: "ClassSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedReligionSubjectId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                column: "AssignedReligionSubjectId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "IsReligionSubject", "ReligionType", "SubjectGroup" },
                values: new object[] { false, null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "IsReligionSubject", "ReligionType", "SubjectGroup" },
                values: new object[] { false, null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "IsReligionSubject", "ReligionType", "SubjectGroup" },
                values: new object[] { false, null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "IsReligionSubject", "ReligionType", "SubjectGroup" },
                values: new object[] { false, null, "" });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectMarkStructures_ComponentId_ExamId_SubjectId_StudentGroupId",
                table: "SubjectMarkStructures",
                columns: new[] { "ComponentId", "ExamId", "SubjectId", "StudentGroupId" },
                unique: true,
                filter: "[ExamId] IS NOT NULL AND [SubjectId] IS NOT NULL AND [StudentGroupId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectMarkStructures_ExamId",
                table: "SubjectMarkStructures",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComponents_ClassSubjectId_ComponentName",
                table: "SubjectComponents",
                columns: new[] { "ClassSubjectId", "ComponentName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectMarkStructures_Exams_ExamId",
                table: "SubjectMarkStructures",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
