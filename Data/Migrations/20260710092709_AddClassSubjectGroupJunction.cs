using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClassSubjectGroupJunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSubjects_StudentGroups_StudentGroupId",
                table: "ClassSubjects");

            migrationBuilder.DropIndex(
                name: "IX_Sections_SchoolClassId_StudentGroupId_Name",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_ClassSubjects_SchoolClassId_SubjectId_GroupName",
                table: "ClassSubjects");

            migrationBuilder.DropIndex(
                name: "IX_ClassSubjects_StudentGroupId",
                table: "ClassSubjects");

            // Migrate existing StudentGroupId data to the ClassSubjectGroups junction table
            // before dropping the legacy column
            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[ClassSubjectGroups] ([ClassSubjectId], [StudentGroupId], [CreatedBy], [CreatedAt], [IsDeleted])
                SELECT [Id], [StudentGroupId], 'migration', SYSUTCDATETIME(), 0
                FROM [dbo].[ClassSubjects]
                WHERE [StudentGroupId] IS NOT NULL
            ");

            migrationBuilder.DropColumn(
                name: "IsGroupSubject",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "ClassSubjects");

            migrationBuilder.CreateTable(
                name: "ClassSubjectGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassSubjectId = table.Column<int>(type: "int", nullable: false),
                    StudentGroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSubjectGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSubjectGroups_ClassSubjects_ClassSubjectId",
                        column: x => x.ClassSubjectId,
                        principalTable: "ClassSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassSubjectGroups_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_SchoolClassId_Name",
                table: "Sections",
                columns: new[] { "SchoolClassId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_SchoolClassId_SubjectId",
                table: "ClassSubjects",
                columns: new[] { "SchoolClassId", "SubjectId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjectGroups_ClassSubjectId_StudentGroupId",
                table: "ClassSubjectGroups",
                columns: new[] { "ClassSubjectId", "StudentGroupId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjectGroups_StudentGroupId",
                table: "ClassSubjectGroups",
                column: "StudentGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Drop new indexes first
            migrationBuilder.DropIndex(
                name: "IX_Sections_SchoolClassId_Name",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_ClassSubjects_SchoolClassId_SubjectId",
                table: "ClassSubjects");

            // 2. Re-add the legacy columns (they were dropped in Up)
            migrationBuilder.AddColumn<bool>(
                name: "IsGroupSubject",
                table: "ClassSubjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "ClassSubjects",
                type: "int",
                nullable: true);

            // 3. Restore first group's StudentGroupId per ClassSubject from the junction table
            migrationBuilder.Sql(@"
                UPDATE cs
                SET cs.[StudentGroupId] = g.[StudentGroupId],
                    cs.[IsGroupSubject] = 1
                FROM [dbo].[ClassSubjects] cs
                INNER JOIN (
                    SELECT [ClassSubjectId], MIN([StudentGroupId]) AS [StudentGroupId]
                    FROM [dbo].[ClassSubjectGroups]
                    WHERE [IsDeleted] = 0
                    GROUP BY [ClassSubjectId]
                ) g ON g.[ClassSubjectId] = cs.[Id]
            ");

            // 4. Drop the junction table (data already migrated back)
            migrationBuilder.DropTable(
                name: "ClassSubjectGroups");

            // 5. Restore old indexes and FK
            migrationBuilder.CreateIndex(
                name: "IX_Sections_SchoolClassId_StudentGroupId_Name",
                table: "Sections",
                columns: new[] { "SchoolClassId", "StudentGroupId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_SchoolClassId_SubjectId_GroupName",
                table: "ClassSubjects",
                columns: new[] { "SchoolClassId", "SubjectId", "GroupName" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_StudentGroupId",
                table: "ClassSubjects",
                column: "StudentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSubjects_StudentGroups_StudentGroupId",
                table: "ClassSubjects",
                column: "StudentGroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
