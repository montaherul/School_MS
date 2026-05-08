using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class exam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DefaultFullMarks",
                table: "Subjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultPassMarks",
                table: "Subjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasAssignment",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasCQ",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasContinuousAssessment",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasLab",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasMCQ",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasOral",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasPractical",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasViva",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasWritten",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPractical",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReligionSubject",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NameBn",
                table: "Subjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReligionType",
                table: "Subjects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectGroup",
                table: "Subjects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CalculatedAt",
                table: "StudentSubjectResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "FullMarks",
                table: "StudentSubjectResults",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PassMarks",
                table: "StudentSubjectResults",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "StudentSubjectResults",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedReligionSubjectId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "StudentGroups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "StudentGroups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                WITH CTE AS (
                    SELECT Id, Code,
                           ROW_NUMBER() OVER (PARTITION BY Code ORDER BY Id) AS rn
                    FROM StudentGroups
                    WHERE Code IS NULL OR LTRIM(RTRIM(Code)) = ''
                )
                UPDATE CTE
                SET Code = CONCAT('GRP-', Id)
                WHERE rn >= 1;
            ");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StudentGroups",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "StudentGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StudentGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxClass",
                table: "StudentGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinClass",
                table: "StudentGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CalculatedAt",
                table: "StudentExamResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ClassPosition",
                table: "StudentExamResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FailedSubjectCount",
                table: "StudentExamResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupPosition",
                table: "StudentExamResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassedSubjectCount",
                table: "StudentExamResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "StudentExamResults",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFullMarks",
                table: "StudentExamResults",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BlockNo",
                table: "SeatingPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HallNo",
                table: "SeatingPlans",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RowNo",
                table: "SeatingPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedAt",
                table: "ResultPublications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicationNotes",
                table: "ResultPublications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeType",
                table: "ResultAuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangedAt",
                table: "ResultAuditLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NewGpa",
                table: "ResultAuditLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldGpa",
                table: "ResultAuditLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "ReEvaluationRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "ReEvaluationRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestReason",
                table: "ReEvaluationRequests",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AssignmentMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BehaviourMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CQMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CompetencyMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ContinuousAssessmentMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LabMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedAt",
                table: "Marks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MCQMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OralMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ParticipationMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PracticalMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Marks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VivaMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WrittenMarks",
                table: "Marks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "GradingRules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "GradingRules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "GradingRules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CalculatedAt",
                table: "FinalResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FinalClassPosition",
                table: "FinalResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FinalGrade",
                table: "FinalResults",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromotionRemarks",
                table: "FinalResults",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchoolClassId",
                table: "FinalResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalFailedSubjects",
                table: "FinalResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AssignmentMarks",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ContinuousAssessmentMarks",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "ExamSubjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LabMarks",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MCQMarks",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OralMarks",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PracticalMarks",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VivaMarks",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WrittenMarks",
                table: "ExamSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "ExamSchedules",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedAt",
                table: "Exams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LockedByUserId",
                table: "Exams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AssignmentMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BehaviourMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CQMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CompetencyMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ContinuousAssessmentMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "ClassSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "FullMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "ClassSubjects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ClassSubjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroupSubject",
                table: "ClassSubjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                table: "ClassSubjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "ClassSubjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReligionSubject",
                table: "ClassSubjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LabMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MCQMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OralMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ParticipationMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PassMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PracticalMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VivaMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WrittenMarks",
                table: "ClassSubjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "AdmitCards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrintedAt",
                table: "AdmitCards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExamTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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
                    table.PrimaryKey("PK_ExamTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GpaConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MinMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GradePoint = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_GpaConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarkAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkEntryId = table.Column<int>(type: "int", nullable: false),
                    OldMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NewMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarkAuditLogs_Marks_MarkEntryId",
                        column: x => x.MarkEntryId,
                        principalTable: "Marks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarkEntryDrafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    WrittenMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MCQMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CQMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PracticalMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    VivaMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LabMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OralMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    CreatedByTeacherId = table.Column<int>(type: "int", nullable: false),
                    DraftSavedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkEntryDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarkEntryDrafts_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarkEntryDrafts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarkEntryDrafts_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeritResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    ClassPosition = table.Column<int>(type: "int", nullable: false),
                    GroupPosition = table.Column<int>(type: "int", nullable: true),
                    TotalMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Gpa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeritResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeritResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeritResults_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeritResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromotionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FromClassId = table.Column<int>(type: "int", nullable: false),
                    ToClassId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PromotedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PromotedByUserId = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionHistories_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionHistories_Classes_FromClassId",
                        column: x => x.FromClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionHistories_Classes_ToClassId",
                        column: x => x.ToClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionHistories_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultLocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    LockedByUserId = table.Column<int>(type: "int", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    CanUnlock = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultLocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultLocks_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroupAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentGroupId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroupAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentGroupAssignments_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroupAssignments_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroupAssignments_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroupAssignments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassSubjectId = table.Column<int>(type: "int", nullable: false),
                    ComponentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ExamConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamTypeId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExamWeightage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_ExamConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamConfigurations_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamConfigurations_ExamTypes_ExamTypeId",
                        column: x => x.ExamTypeId,
                        principalTable: "ExamTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsLocked", "LockedAt", "LockedByUserId", "Term" },
                values: new object[] { false, null, null, 8 });

            migrationBuilder.UpdateData(
                table: "GradingRules",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "DisplayOrder", "IsActive" },
                values: new object[] { "", 0, true });

            migrationBuilder.UpdateData(
                table: "GradingRules",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "DisplayOrder", "IsActive" },
                values: new object[] { "", 0, true });

            migrationBuilder.UpdateData(
                table: "GradingRules",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "DisplayOrder", "IsActive" },
                values: new object[] { "", 0, true });

            migrationBuilder.UpdateData(
                table: "GradingRules",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "DisplayOrder", "IsActive" },
                values: new object[] { "", 0, true });

            migrationBuilder.UpdateData(
                table: "GradingRules",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "DisplayOrder", "IsActive" },
                values: new object[] { "", 0, true });

            migrationBuilder.UpdateData(
                table: "GradingRules",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "DisplayOrder", "IsActive" },
                values: new object[] { "", 0, true });

            migrationBuilder.UpdateData(
                table: "GradingRules",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "DisplayOrder", "IsActive" },
                values: new object[] { "", 0, true });

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AssignmentMarks", "BehaviourMarks", "CQMarks", "CompetencyMarks", "ContinuousAssessmentMarks", "LabMarks", "LockedAt", "MCQMarks", "OralMarks", "ParticipationMarks", "PracticalMarks", "SubmittedAt", "VivaMarks", "WrittenMarks" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Marks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AssignmentMarks", "BehaviourMarks", "CQMarks", "CompetencyMarks", "ContinuousAssessmentMarks", "LabMarks", "LockedAt", "MCQMarks", "OralMarks", "ParticipationMarks", "PracticalMarks", "SubmittedAt", "VivaMarks", "WrittenMarks" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AssignedReligionSubjectId", "StudentGroupId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AssignedReligionSubjectId", "StudentGroupId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "NameBn", "ReligionType", "SubjectGroup" },
                values: new object[] { 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, true, false, false, false, "", null, "" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_AssignedReligionSubjectId",
                table: "Students",
                column: "AssignedReligionSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentGroupId",
                table: "Students",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroups_Code",
                table: "StudentGroups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinalResults_SchoolClassId",
                table: "FinalResults",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubjects_ExamId",
                table: "ExamSubjects",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubjects_SubjectId",
                table: "ExamSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSchedules_ExamId",
                table: "ExamSchedules",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSchedules_SubjectId",
                table: "ExamSchedules",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamConfigurations_ClassId",
                table: "ExamConfigurations",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamConfigurations_ExamTypeId_ClassId",
                table: "ExamConfigurations",
                columns: new[] { "ExamTypeId", "ClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamTypes_Code",
                table: "ExamTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GpaConfigurations_Grade",
                table: "GpaConfigurations",
                column: "Grade",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GpaConfigurations_MinMarks_MaxMarks",
                table: "GpaConfigurations",
                columns: new[] { "MinMarks", "MaxMarks" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarkAuditLogs_MarkEntryId",
                table: "MarkAuditLogs",
                column: "MarkEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkEntryDrafts_ExamId_StudentId_SubjectId",
                table: "MarkEntryDrafts",
                columns: new[] { "ExamId", "StudentId", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarkEntryDrafts_StudentId",
                table: "MarkEntryDrafts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkEntryDrafts_SubjectId",
                table: "MarkEntryDrafts",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MeritResults_ExamId_SectionId_Position",
                table: "MeritResults",
                columns: new[] { "ExamId", "SectionId", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_MeritResults_ExamId_StudentId",
                table: "MeritResults",
                columns: new[] { "ExamId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeritResults_SectionId",
                table: "MeritResults",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeritResults_StudentId",
                table: "MeritResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionHistories_AcademicYearId",
                table: "PromotionHistories",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionHistories_FromClassId",
                table: "PromotionHistories",
                column: "FromClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionHistories_StudentId_AcademicYearId",
                table: "PromotionHistories",
                columns: new[] { "StudentId", "AcademicYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromotionHistories_ToClassId",
                table: "PromotionHistories",
                column: "ToClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultLocks_ExamId",
                table: "ResultLocks",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupAssignments_AcademicYearId",
                table: "StudentGroupAssignments",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupAssignments_SchoolClassId",
                table: "StudentGroupAssignments",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupAssignments_StudentGroupId",
                table: "StudentGroupAssignments",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupAssignments_StudentId_SchoolClassId_AcademicYearId",
                table: "StudentGroupAssignments",
                columns: new[] { "StudentId", "SchoolClassId", "AcademicYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComponents_ClassSubjectId_ComponentName",
                table: "SubjectComponents",
                columns: new[] { "ClassSubjectId", "ComponentName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSchedules_Exams_ExamId",
                table: "ExamSchedules",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSchedules_Subjects_SubjectId",
                table: "ExamSchedules",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubjects_Exams_ExamId",
                table: "ExamSubjects",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubjects_Subjects_SubjectId",
                table: "ExamSubjects",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinalResults_Classes_SchoolClassId",
                table: "FinalResults",
                column: "SchoolClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentGroups_StudentGroupId",
                table: "Students",
                column: "StudentGroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Subjects_AssignedReligionSubjectId",
                table: "Students",
                column: "AssignedReligionSubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamSchedules_Exams_ExamId",
                table: "ExamSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamSchedules_Subjects_SubjectId",
                table: "ExamSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubjects_Exams_ExamId",
                table: "ExamSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubjects_Subjects_SubjectId",
                table: "ExamSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_FinalResults_Classes_SchoolClassId",
                table: "FinalResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentGroups_StudentGroupId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Subjects_AssignedReligionSubjectId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "ExamConfigurations");

            migrationBuilder.DropTable(
                name: "GpaConfigurations");

            migrationBuilder.DropTable(
                name: "MarkAuditLogs");

            migrationBuilder.DropTable(
                name: "MarkEntryDrafts");

            migrationBuilder.DropTable(
                name: "MeritResults");

            migrationBuilder.DropTable(
                name: "PromotionHistories");

            migrationBuilder.DropTable(
                name: "ResultLocks");

            migrationBuilder.DropTable(
                name: "StudentGroupAssignments");

            migrationBuilder.DropTable(
                name: "SubjectComponents");

            migrationBuilder.DropTable(
                name: "ExamTypes");

            migrationBuilder.DropIndex(
                name: "IX_Students_AssignedReligionSubjectId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_StudentGroupId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_StudentGroups_Code",
                table: "StudentGroups");

            migrationBuilder.DropIndex(
                name: "IX_FinalResults_SchoolClassId",
                table: "FinalResults");

            migrationBuilder.DropIndex(
                name: "IX_ExamSubjects_ExamId",
                table: "ExamSubjects");

            migrationBuilder.DropIndex(
                name: "IX_ExamSubjects_SubjectId",
                table: "ExamSubjects");

            migrationBuilder.DropIndex(
                name: "IX_ExamSchedules_ExamId",
                table: "ExamSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ExamSchedules_SubjectId",
                table: "ExamSchedules");

            migrationBuilder.DropColumn(
                name: "DefaultFullMarks",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "DefaultPassMarks",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "HasAssignment",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "HasCQ",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "HasContinuousAssessment",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "HasLab",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "HasMCQ",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "HasOral",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "HasPractical",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "HasViva",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "HasWritten",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "IsMandatory",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "IsPractical",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "IsReligionSubject",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "NameBn",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "ReligionType",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SubjectGroup",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "CalculatedAt",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "FullMarks",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "PassMarks",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "StudentSubjectResults");

            migrationBuilder.DropColumn(
                name: "AssignedReligionSubjectId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "MaxClass",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "MinClass",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "CalculatedAt",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "ClassPosition",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "FailedSubjectCount",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "GroupPosition",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "PassedSubjectCount",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "TotalFullMarks",
                table: "StudentExamResults");

            migrationBuilder.DropColumn(
                name: "BlockNo",
                table: "SeatingPlans");

            migrationBuilder.DropColumn(
                name: "HallNo",
                table: "SeatingPlans");

            migrationBuilder.DropColumn(
                name: "RowNo",
                table: "SeatingPlans");

            migrationBuilder.DropColumn(
                name: "LockedAt",
                table: "ResultPublications");

            migrationBuilder.DropColumn(
                name: "PublicationNotes",
                table: "ResultPublications");

            migrationBuilder.DropColumn(
                name: "ChangeType",
                table: "ResultAuditLogs");

            migrationBuilder.DropColumn(
                name: "ChangedAt",
                table: "ResultAuditLogs");

            migrationBuilder.DropColumn(
                name: "NewGpa",
                table: "ResultAuditLogs");

            migrationBuilder.DropColumn(
                name: "OldGpa",
                table: "ResultAuditLogs");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "ReEvaluationRequests");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "ReEvaluationRequests");

            migrationBuilder.DropColumn(
                name: "RequestReason",
                table: "ReEvaluationRequests");

            migrationBuilder.DropColumn(
                name: "AssignmentMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "BehaviourMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "CQMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "CompetencyMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "ContinuousAssessmentMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "LabMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "LockedAt",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "MCQMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "OralMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "ParticipationMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "PracticalMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "VivaMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "WrittenMarks",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "GradingRules");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "GradingRules");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GradingRules");

            migrationBuilder.DropColumn(
                name: "CalculatedAt",
                table: "FinalResults");

            migrationBuilder.DropColumn(
                name: "FinalClassPosition",
                table: "FinalResults");

            migrationBuilder.DropColumn(
                name: "FinalGrade",
                table: "FinalResults");

            migrationBuilder.DropColumn(
                name: "PromotionRemarks",
                table: "FinalResults");

            migrationBuilder.DropColumn(
                name: "SchoolClassId",
                table: "FinalResults");

            migrationBuilder.DropColumn(
                name: "TotalFailedSubjects",
                table: "FinalResults");

            migrationBuilder.DropColumn(
                name: "AssignmentMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "ContinuousAssessmentMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "LabMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "MCQMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "OralMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "PracticalMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "VivaMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "WrittenMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "ExamSchedules");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "LockedAt",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "LockedByUserId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "AssignmentMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "BehaviourMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "CQMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "CompetencyMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "ContinuousAssessmentMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "FullMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "IsGroupSubject",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "IsMandatory",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "IsReligionSubject",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "LabMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "MCQMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "OralMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "ParticipationMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "PassMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "PracticalMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "VivaMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "WrittenMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "AdmitCards");

            migrationBuilder.DropColumn(
                name: "PrintedAt",
                table: "AdmitCards");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "StudentGroups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: 1,
                column: "Term",
                value: 3);
        }
    }
}
