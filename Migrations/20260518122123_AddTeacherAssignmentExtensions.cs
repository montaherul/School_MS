using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherAssignmentExtensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_ClassId",
                table: "TeacherClassAssignments");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TeacherSubjectAssignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "TeacherSubjectAssignments",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TeacherClassAssignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "TeacherClassAssignments",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TeacherAcademicProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    SubjectSpecialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TeachingLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsExamController = table.Column<bool>(type: "bit", nullable: false),
                    IsRoutineCoordinator = table.Column<bool>(type: "bit", nullable: false),
                    IsClassTeacherEligible = table.Column<bool>(type: "bit", nullable: false),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAcademicProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAcademicProfiles_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherAssignmentLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAssignmentLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAssignmentLogs_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "ClassId", "SectionId", "AcademicYearId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAcademicProfiles_TeacherId",
                table: "TeacherAcademicProfiles",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignmentLogs_TeacherId",
                table: "TeacherAssignmentLogs",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeacherAcademicProfiles");

            migrationBuilder.DropTable(
                name: "TeacherAssignmentLogs");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TeacherSubjectAssignments");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "TeacherSubjectAssignments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TeacherClassAssignments");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "TeacherClassAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_ClassId",
                table: "TeacherClassAssignments",
                column: "ClassId");
        }
    }
}
