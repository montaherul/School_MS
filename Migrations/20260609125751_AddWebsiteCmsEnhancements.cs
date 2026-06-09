using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteCmsEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClassSubjects_SchoolClassId",
                table: "ClassSubjects");

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
                name: "AssignmentMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "ContinuousAssessmentMarks",
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
                name: "PracticalMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "VivaMarks",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "WrittenMarks",
                table: "ClassSubjects");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Subjects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Subjects",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BanglaName",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CopyrightText",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstablishedYear",
                table: "SchoolSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FooterLogoPath",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginLogoPath",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywords",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "SchoolSettings",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OgDescription",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OgImagePath",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OgTitle",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrincipalDesignation",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrincipalQualification",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrincipalSignaturePath",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolCode",
                table: "SchoolSettings",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolDescription",
                table: "SchoolSettings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolMotto",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowAdmissionCTA",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowEvents",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowGallery",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowNotices",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPrincipalMessage",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowSlider",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowStatistics",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowWelcomeSection",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterUrl",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteBannerPath",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "Classes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Classes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Classes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Classes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NameBn",
                table: "Classes",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Placeholders = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ArchivedAt", "Capacity", "Code", "Description", "IsActive", "NameBn" },
                values: new object[] { null, 0, "", null, true, "" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ArchivedAt", "Capacity", "Code", "Description", "IsActive", "NameBn" },
                values: new object[] { null, 0, "", null, true, "" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ArchivedAt", "Capacity", "Code", "Description", "IsActive", "NameBn" },
                values: new object[] { null, 0, "", null, true, "" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ArchivedAt", "Capacity", "Code", "Description", "IsActive", "NameBn" },
                values: new object[] { null, 0, "", null, true, "" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ArchivedAt", "Capacity", "Code", "Description", "IsActive", "NameBn" },
                values: new object[] { null, 0, "", null, true, "" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ArchivedAt", "Capacity", "Code", "Description", "IsActive", "NameBn" },
                values: new object[] { null, 0, "", null, true, "" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ArchivedAt", "Capacity", "Code", "Description", "IsActive", "NameBn" },
                values: new object[] { null, 0, "", null, true, "" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ArchivedAt", "Capacity", "Code", "Description", "IsActive", "NameBn" },
                values: new object[] { null, 0, "", null, true, "" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ArchivedAt", "Capacity", "Code", "Description", "IsActive", "NameBn" },
                values: new object[] { null, 0, "", null, true, "" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ArchivedAt", "Capacity", "Code", "Description", "IsActive", "NameBn" },
                values: new object[] { null, 0, "", null, true, "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Category", "ShortName" },
                values: new object[] { "", "" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_SchoolClassId_SubjectId_GroupName",
                table: "ClassSubjects",
                columns: new[] { "SchoolClassId", "SubjectId", "GroupName" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ClassSubjects_SchoolClassId_SubjectId_GroupName",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "BanglaName",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "CopyrightText",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "EstablishedYear",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "FooterLogoPath",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "LoginLogoPath",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "MetaKeywords",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "OgDescription",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "OgImagePath",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "OgTitle",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "PrincipalDesignation",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "PrincipalQualification",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "PrincipalSignaturePath",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SchoolCode",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SchoolDescription",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "SchoolMotto",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowAdmissionCTA",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowEvents",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowGallery",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowNotices",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowPrincipalMessage",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowSlider",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowStatistics",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowWelcomeSection",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "TwitterUrl",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "WebsiteBannerPath",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "NameBn",
                table: "Classes");

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

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten" },
                values: new object[] { false, false, false, false, false, false, false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_SchoolClassId",
                table: "ClassSubjects",
                column: "SchoolClassId");
        }
    }
}
