using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherClassAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Evaluator",
                table: "TeacherPerformances");

            migrationBuilder.AddColumn<int>(
                name: "EvaluatorUserId",
                table: "TeacherPerformances",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApproverRemarks",
                table: "TeacherLeaves",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "TeacherLeaves",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "TeacherLeaves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Marks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Marks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Attendance",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Attendance",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Assignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Assignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Module = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherClassAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    IsClassTeacher = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherClassAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherClassAssignments_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherClassAssignments_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherClassAssignments_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherClassAssignments_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSubjectAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSubjectAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignments_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignments_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignments_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignments_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignments_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherTimetables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RoomNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherTimetables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherTimetables_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherTimetables_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherTimetables_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherTimetables_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Attendance",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedByUserId", "UpdatedByUserId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Attendance",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedByUserId", "UpdatedByUserId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedByUserId", "UpdatedByUserId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedByUserId", "UpdatedByUserId" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPerformances_EvaluatorUserId",
                table: "TeacherPerformances",
                column: "EvaluatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLeaves_ApprovedByUserId",
                table: "TeacherLeaves",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_AcademicYearId",
                table: "TeacherClassAssignments",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_ClassId",
                table: "TeacherClassAssignments",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_SectionId",
                table: "TeacherClassAssignments",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "TeacherId", "ClassId", "SectionId", "AcademicYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_AcademicYearId",
                table: "TeacherSubjectAssignments",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_ClassId",
                table: "TeacherSubjectAssignments",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_SectionId",
                table: "TeacherSubjectAssignments",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_SubjectId",
                table: "TeacherSubjectAssignments",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId",
                table: "TeacherSubjectAssignments",
                columns: new[] { "TeacherId", "SubjectId", "ClassId", "SectionId", "AcademicYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTimetables_ClassId",
                table: "TeacherTimetables",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTimetables_SectionId",
                table: "TeacherTimetables",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTimetables_SubjectId",
                table: "TeacherTimetables",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTimetables_TeacherId_DayOfWeek_StartTime",
                table: "TeacherTimetables",
                columns: new[] { "TeacherId", "DayOfWeek", "StartTime" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherLeaves_Users_ApprovedByUserId",
                table: "TeacherLeaves",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherPerformances_Users_EvaluatorUserId",
                table: "TeacherPerformances",
                column: "EvaluatorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherLeaves_Users_ApprovedByUserId",
                table: "TeacherLeaves");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherPerformances_Users_EvaluatorUserId",
                table: "TeacherPerformances");

            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "TeacherClassAssignments");

            migrationBuilder.DropTable(
                name: "TeacherSubjectAssignments");

            migrationBuilder.DropTable(
                name: "TeacherTimetables");

            migrationBuilder.DropIndex(
                name: "IX_TeacherPerformances_EvaluatorUserId",
                table: "TeacherPerformances");

            migrationBuilder.DropIndex(
                name: "IX_TeacherLeaves_ApprovedByUserId",
                table: "TeacherLeaves");

            migrationBuilder.DropColumn(
                name: "EvaluatorUserId",
                table: "TeacherPerformances");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "TeacherLeaves");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "TeacherLeaves");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Assignments");

            migrationBuilder.AddColumn<string>(
                name: "Evaluator",
                table: "TeacherPerformances",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ApproverRemarks",
                table: "TeacherLeaves",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
