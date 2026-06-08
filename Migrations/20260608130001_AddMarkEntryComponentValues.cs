using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMarkEntryComponentValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComponentValues",
                table: "Marks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExamComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    DefaultFullMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DefaultPassMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPractical = table.Column<bool>(type: "bit", nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectMarkStructures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    ExamId = table.Column<int>(type: "int", nullable: true),
                    ClassId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    StudentGroupId = table.Column<int>(type: "int", nullable: true),
                    FullMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PassMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectMarkStructures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectMarkStructures_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectMarkStructures_ExamComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "ExamComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectMarkStructures_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectMarkStructures_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectMarkStructures_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 1,
                column: "ComponentValues",
                value: null);

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 2,
                column: "ComponentValues",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_ExamComponents_Code",
                table: "ExamComponents",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectMarkStructures_ClassId",
                table: "SubjectMarkStructures",
                column: "ClassId");

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
                name: "IX_SubjectMarkStructures_StudentGroupId",
                table: "SubjectMarkStructures",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectMarkStructures_SubjectId",
                table: "SubjectMarkStructures",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectMarkStructures");

            migrationBuilder.DropTable(
                name: "ExamComponents");

            migrationBuilder.DropColumn(
                name: "ComponentValues",
                table: "Marks");
        }
    }
}
