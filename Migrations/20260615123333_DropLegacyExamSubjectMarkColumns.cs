using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class DropLegacyExamSubjectMarkColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAssignmentMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "TotalMCQMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "TotalPracticalMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "TotalVivaMarks",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "TotalWrittenMarks",
                table: "ExamSubjects");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Id = 603)
    INSERT INTO Permissions (Id, Action, CanCreate, CanDelete, CanRead, CanUpdate, Code, CreatedAt, CreatedBy, IsDeleted, Module, ModuleName, UpdatedAt, UpdatedBy)
    VALUES (603, 'Regenerate', 1, 0, 1, 1, 'Calendar.Regenerate', '2026-01-01T00:00:00Z', 'system', 0, 'Calendar', 'Calendar', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Id = 604)
    INSERT INTO Permissions (Id, Action, CanCreate, CanDelete, CanRead, CanUpdate, Code, CreatedAt, CreatedBy, IsDeleted, Module, ModuleName, UpdatedAt, UpdatedBy)
    VALUES (604, 'Repair', 1, 0, 1, 1, 'Calendar.Repair', '2026-01-01T00:00:00Z', 'system', 0, 'Calendar', 'Calendar', NULL, NULL);
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 560 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (560, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 561 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (561, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 562 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (562, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 563 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (563, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 564 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (564, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 565 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (565, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 566 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (566, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 567 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (567, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 568 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (568, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 569 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (569, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 570 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (570, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 571 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (571, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 572 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (572, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 603 AND RoleId = 1)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (603, 1);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 604 AND RoleId = 1)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (604, 1);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 603 AND RoleId = 2)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (603, 2);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 604 AND RoleId = 2)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (604, 2);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 603 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (603, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 604 AND RoleId = 3)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (604, 3);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 603 AND RoleId = 26)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (603, 26);
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE PermissionId = 604 AND RoleId = 26)
    INSERT INTO RolePermissions (PermissionId, RoleId) VALUES (604, 26);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 603, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 604, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 603, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 604, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 560, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 561, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 562, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 563, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 564, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 565, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 566, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 567, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 568, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 569, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 570, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 571, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 572, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 603, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 604, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 603, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 604, 26 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 603);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 604);

            migrationBuilder.AddColumn<int>(
                name: "TotalAssignmentMarks",
                table: "ExamSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalMCQMarks",
                table: "ExamSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalPracticalMarks",
                table: "ExamSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalVivaMarks",
                table: "ExamSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalWrittenMarks",
                table: "ExamSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
