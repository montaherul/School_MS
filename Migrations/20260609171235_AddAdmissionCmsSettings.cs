using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmissionCmsSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdmissionCircularPath",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionCloseDate",
                table: "SchoolSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionCtaText",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionCtaTitle",
                table: "SchoolSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionEligibility",
                table: "SchoolSettings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AdmissionEnabled",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionFeeNote",
                table: "SchoolSettings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionFormPath",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionGuidelines",
                table: "SchoolSettings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionMetaDescription",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionMetaKeywords",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionMetaTitle",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionOgDescription",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionOgImagePath",
                table: "SchoolSettings",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionOgTitle",
                table: "SchoolSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionOpenDate",
                table: "SchoolSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionProcess",
                table: "SchoolSettings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionRequirements",
                table: "SchoolSettings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionSubtitle",
                table: "SchoolSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionTitle",
                table: "SchoolSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnlineAdmissionEnabled",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowAdmissionDownloads",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowAdmissionFees",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowAdmissionGuidelines",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowAdmissionPage",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowAdmissionRequirements",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AdmissionFeeStructures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdmissionFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SessionFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExamFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OtherFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_AdmissionFeeStructures", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionFeeStructures");

            migrationBuilder.DropColumn(
                name: "AdmissionCircularPath",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionCloseDate",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionCtaText",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionCtaTitle",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionEligibility",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionEnabled",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionFeeNote",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionFormPath",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionGuidelines",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionMetaDescription",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionMetaKeywords",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionMetaTitle",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionOgDescription",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionOgImagePath",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionOgTitle",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionOpenDate",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionProcess",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionRequirements",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionSubtitle",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionTitle",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "OnlineAdmissionEnabled",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowAdmissionDownloads",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowAdmissionFees",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowAdmissionGuidelines",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowAdmissionPage",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ShowAdmissionRequirements",
                table: "SchoolSettings");
        }
    }
}
