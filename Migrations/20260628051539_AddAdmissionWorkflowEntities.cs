using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmissionWorkflowEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowDirectAdmissionToClass10",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GroupStartsFromClassId",
                table: "SchoolSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "EmailTemplates",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllDocumentsVerified",
                table: "Admissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DocumentsVerifiedAt",
                table: "Admissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentsVerifiedBy",
                table: "Admissions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppliedStudentGroupId",
                table: "AdmissionListResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppliedStudentGroupName",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "AdmissionDocuments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "AdmissionDocuments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "AdmissionDocuments",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreviousVersionId",
                table: "AdmissionDocuments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationRemarks",
                table: "AdmissionDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                table: "AdmissionDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "AdmissionDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedBy",
                table: "AdmissionDocuments",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VersionNumber",
                table: "AdmissionDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionApplicationId = table.Column<int>(type: "int", nullable: false),
                    WorkflowDefinitionId = table.Column<int>(type: "int", nullable: false),
                    CurrentState = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_Admissions_AdmissionApplicationId",
                        column: x => x.AdmissionApplicationId,
                        principalTable: "Admissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_WorkflowDefinitions_WorkflowDefinitionId",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTransitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefinitionId = table.Column<int>(type: "int", nullable: false),
                    FromState = table.Column<int>(type: "int", nullable: false),
                    ToState = table.Column<int>(type: "int", nullable: false),
                    TransitionType = table.Column<int>(type: "int", nullable: false),
                    RequiredPermission = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ConditionExpression = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    RequiredApprovalCount = table.Column<int>(type: "int", nullable: true),
                    RequiredRole = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_WorkflowDefinitions_WorkflowDefinitionId",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowHistoryEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: false),
                    FromState = table.Column<int>(type: "int", nullable: false),
                    ToState = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsApproval = table.Column<bool>(type: "bit", nullable: false),
                    ActionedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ActionedByRole = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActionedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRolledBack = table.Column<bool>(type: "bit", nullable: false),
                    RolledBackAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RolledBackBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowHistoryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowHistoryEntries_WorkflowInstances_WorkflowInstanceId",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Admissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AllDocumentsVerified", "DocumentsVerifiedAt", "DocumentsVerifiedBy" },
                values: new object[] { false, null, null });

            migrationBuilder.InsertData(
                table: "WorkflowDefinitions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDeleted", "Name", "SortOrder", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Default workflow for student admissions (17 states)", true, false, "Standard Admission Workflow", 1, null, null });

            migrationBuilder.InsertData(
                table: "WorkflowInstances",
                columns: new[] { "Id", "AdmissionApplicationId", "CompletedAt", "CreatedAt", "CreatedBy", "CurrentState", "IsCompleted", "IsDeleted", "UpdatedAt", "UpdatedBy", "WorkflowDefinitionId" },
                values: new object[] { 1, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 1, false, false, null, null, 1 });

            migrationBuilder.InsertData(
                table: "WorkflowTransitions",
                columns: new[] { "Id", "ConditionExpression", "CreatedAt", "CreatedBy", "FromState", "IsActive", "IsDeleted", "RequiredApprovalCount", "RequiredPermission", "RequiredRole", "RequiresApproval", "SortOrder", "ToState", "TransitionType", "UpdatedAt", "UpdatedBy", "WorkflowDefinitionId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 1, true, false, null, null, null, false, 1, 2, 1, null, null, 1 },
                    { 2, "AllDocumentsVerified", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 2, true, false, null, null, null, false, 2, 3, 1, null, null, 1 },
                    { 3, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 3, true, false, null, null, null, true, 3, 4, 2, null, null, 1 },
                    { 4, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 4, true, false, null, null, null, false, 4, 5, 1, null, null, 1 },
                    { 5, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 5, true, false, null, null, null, false, 5, 6, 1, null, null, 1 },
                    { 6, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 6, true, false, null, null, null, true, 6, 7, 2, null, null, 1 },
                    { 7, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 7, true, false, null, null, null, false, 7, 8, 1, null, null, 1 },
                    { 8, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 8, true, false, null, null, null, false, 8, 9, 1, null, null, 1 },
                    { 9, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 9, true, false, null, null, null, false, 9, 10, 1, null, null, 1 },
                    { 10, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 10, true, false, null, null, null, false, 10, 11, 1, null, null, 1 },
                    { 11, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 11, true, false, null, null, null, false, 11, 12, 1, null, null, 1 },
                    { 12, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 12, true, false, null, null, null, false, 12, 13, 1, null, null, 1 },
                    { 13, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 13, true, false, null, null, null, false, 13, 14, 1, null, null, 1 },
                    { 14, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 1, true, false, null, null, null, true, 14, 17, 2, null, null, 1 },
                    { 15, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 17, true, false, null, null, null, true, 15, 2, 2, null, null, 1 },
                    { 16, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 2, true, false, null, null, null, true, 16, 15, 2, null, null, 1 },
                    { 17, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 3, true, false, null, null, null, true, 17, 15, 2, null, null, 1 },
                    { 18, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 5, true, false, null, null, null, true, 18, 15, 2, null, null, 1 },
                    { 19, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 6, true, false, null, null, null, true, 19, 15, 2, null, null, 1 },
                    { 20, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 7, true, false, null, null, null, true, 20, 15, 2, null, null, 1 }
                });

            migrationBuilder.InsertData(
                table: "WorkflowHistoryEntries",
                columns: new[] { "Id", "ActionedAt", "ActionedBy", "ActionedByRole", "CreatedAt", "CreatedBy", "FromState", "IsApproval", "IsDeleted", "IsRolledBack", "Remarks", "RolledBackAt", "RolledBackBy", "ToState", "UpdatedAt", "UpdatedBy", "WorkflowInstanceId" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "applicant", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 1, false, false, false, "Application submitted by applicant", null, null, 1, null, null, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionDocuments_PreviousVersionId",
                table: "AdmissionDocuments",
                column: "PreviousVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_Name",
                table: "WorkflowDefinitions",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowHistoryEntries_WorkflowInstanceId_ActionedAt",
                table: "WorkflowHistoryEntries",
                columns: new[] { "WorkflowInstanceId", "ActionedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_AdmissionApplicationId",
                table: "WorkflowInstances",
                column: "AdmissionApplicationId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_WorkflowDefinitionId",
                table: "WorkflowInstances",
                column: "WorkflowDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_WorkflowDefinitionId_FromState_ToState",
                table: "WorkflowTransitions",
                columns: new[] { "WorkflowDefinitionId", "FromState", "ToState" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionDocuments_AdmissionDocuments_PreviousVersionId",
                table: "AdmissionDocuments",
                column: "PreviousVersionId",
                principalTable: "AdmissionDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdmissionDocuments_AdmissionDocuments_PreviousVersionId",
                table: "AdmissionDocuments");

            migrationBuilder.DropTable(
                name: "WorkflowHistoryEntries");

            migrationBuilder.DropTable(
                name: "WorkflowTransitions");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_AdmissionDocuments_PreviousVersionId",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "AllowDirectAdmissionToClass10",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "GroupStartsFromClassId",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AllDocumentsVerified",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "DocumentsVerifiedAt",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "DocumentsVerifiedBy",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "AppliedStudentGroupId",
                table: "AdmissionListResults");

            migrationBuilder.DropColumn(
                name: "AppliedStudentGroupName",
                table: "AdmissionListResults");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "PreviousVersionId",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "VerificationRemarks",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "VersionNumber",
                table: "AdmissionDocuments");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "EmailTemplates",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);
        }
    }
}
