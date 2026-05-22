using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixSectionGroupConflict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_SchoolClassId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeAttendances_EmployeeId",
                table: "EmployeeAttendances");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 198, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 199, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 200, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 201, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 202, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 203, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 204, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 205, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 206, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 207, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 208, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 37, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 38, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 39, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 40, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 41, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 42, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 43, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 44, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 45, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 46, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 47, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 48, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 49, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 50, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 51, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 52, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 79, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 80, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 81, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 82, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 83, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 84, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 85, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 86, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 87, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 88, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 89, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 90, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 91, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 181, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 182, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 183, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 184, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 185, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 186, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 187, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 188, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 189, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 199, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 200, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 201, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 202, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 203, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 204, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 205, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 206, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 207, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 208, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 209, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 210, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 211, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 212, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 213, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 214, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 215, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 216, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 217, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 218, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 219, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 220, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 221, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 222, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 223, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 224, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 225, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 226, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 227, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 228, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 229, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 230, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 231, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 232, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 233, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 234, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 235, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 236, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 237, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 238, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 239, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 240, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 241, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 242, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 243, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 244, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 245, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 246, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 247, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 248, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 249, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 250, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 251, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 252, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 46, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 64, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 91, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 100, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 109, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 110, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 118, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 119, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 181, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 226, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 37, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 38, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 39, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 41, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 42, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 43, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 44, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 45, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 46, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 47, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 48, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 50, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 51, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 52, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 127, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 128, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 129, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 131, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 132, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 133, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 134, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 135, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 136, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 137, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 138, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 140, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 141, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 142, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 143, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 144, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 181, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 182, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 188, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 209, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 210, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 212, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 213, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 214, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 215, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 216, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 217, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 218, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 219, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 221, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 222, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 223, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 224, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 225, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 46, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 91, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 109, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 118, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 119, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 127, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 217, 7 });

            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "Sections",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                -- 1. Update Child sections' StudentGroupId, Name, and set ParentSectionId = NULL
                UPDATE Child
                SET 
                    Child.StudentGroupId = CASE 
                        WHEN Parent.Name = 'Science' THEN 1
                        WHEN Parent.Name = 'Business Studies' THEN 2
                        WHEN Parent.Name = 'Humanities' THEN 3
                        ELSE NULL
                    END,
                    Child.Name = LTRIM(RTRIM(REPLACE(Child.Name, Parent.Name, ''))),
                    Child.ParentSectionId = NULL
                FROM Sections Child
                INNER JOIN Sections Parent ON Child.ParentSectionId = Parent.Id
                WHERE Child.ParentSectionId IS NOT NULL;

                -- 2. Delete the parent sections from the Sections table
                DELETE FROM Sections
                WHERE ParentSectionId IS NULL 
                  AND Name IN ('Science', 'Business Studies', 'Humanities')
                  AND SchoolClassId IN (9, 10);

                -- 3. Sync Students.StudentGroupId with Students.Section.StudentGroupId for consistency
                UPDATE S
                SET S.StudentGroupId = SEC.StudentGroupId
                FROM Students S
                INNER JOIN Sections SEC ON S.SectionId = SEC.Id
                WHERE SEC.StudentGroupId IS NOT NULL;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionDetails",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePicturePath",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PresentVillage",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PresentThana",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PresentPostOffice",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PresentDistrict",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PermanentVillage",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PermanentThana",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PermanentPostOffice",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PermanentDistrict",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentSlipPath",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MotherOccupation",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GuardianOccupation",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GuardianName",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FatherOccupation",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BloodGroup",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BirthCertificatePath",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BirthCertificateNo",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicantEmail",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AlternativeNumber",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "AttendanceNotificationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NotificationChannel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    NotificationStatus = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceNotificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceNotificationLogs_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code" },
                values: new object[] { "Read", false, true, "Dashboard.Read" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code" },
                values: new object[] { "Create", true, false, "Dashboard.Create" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code" },
                values: new object[] { "Edit", false, true, "Dashboard.Edit" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Action", "Code" },
                values: new object[] { "Update", "Dashboard.Update" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code" },
                values: new object[] { "Delete", true, false, "Dashboard.Delete" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Action", "Code" },
                values: new object[] { "Approve", "Dashboard.Approve" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code" },
                values: new object[] { "Assign", false, true, "Dashboard.Assign" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code" },
                values: new object[] { "Publish", false, false, false, "Dashboard.Publish" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", "Dashboard.Export", "Dashboard", "Dashboard" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", false, true, "Dashboard.Print", "Dashboard", "Dashboard" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, true, false, "Dashboard.Generate", "Dashboard", "Dashboard" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Dashboard.Manage", "Dashboard", "Dashboard" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code" },
                values: new object[] { "View", true, false, "Users.View" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code" },
                values: new object[] { "Read", true, false, "Users.Read" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code" },
                values: new object[] { "Create", true, false, "Users.Create" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code" },
                values: new object[] { "Edit", false, true, "Users.Edit" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code" },
                values: new object[] { "Update", false, false, false, "Users.Update" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Users.Delete", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Users.Approve", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Users.Assign", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Users.Publish", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Users.Export", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", true, false, "Users.Print", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, true, false, "Users.Generate", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Users.Manage", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code" },
                values: new object[] { "View", false, false, false, "Roles.View" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", "Roles.Read", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Roles.Create", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Roles.Edit", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, true, "Roles.Update", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Roles.Delete", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Roles.Approve", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Roles.Assign", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Roles.Publish", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, false, false, "Roles.Export", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", "Roles.Print", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Action", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, "Roles.Generate", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Roles.Manage", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, true, "Permissions.View", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", true, false, "Permissions.Read", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Permissions.Create", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Permissions.Edit", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, true, "Permissions.Update", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, false, false, "Permissions.Delete", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Permissions.Approve", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Permissions.Assign", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Permissions.Publish", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, true, "Permissions.Export", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", true, false, "Permissions.Print", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, true, false, "Permissions.Generate", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Permissions.Manage", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "View", "Admissions.View", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", false, false, false, "Admissions.Read", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Admissions.Create", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Admissions.Edit", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", "Admissions.Update", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Admissions.Delete", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Admissions.Approve", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Admissions.Assign", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Admissions.Publish", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Admissions.Export", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", false, false, false, "Admissions.Print", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "Action", "CanCreate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, "Admissions.Generate", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "Action", "CanDelete", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Admissions.Manage", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Students.View", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", false, true, "Students.Read", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Students.Create", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Students.Edit", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", "Students.Update", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Students.Delete", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, false, false, "Students.Approve", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Students.Assign", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Students.Publish", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Students.Export", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", false, true, "Students.Print", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, true, false, "Students.Generate", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Students.Manage", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Teachers.View", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", "Teachers.Read", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "Action", "CanDelete", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", false, false, false, "Teachers.Create", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Teachers.Edit", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, true, "Teachers.Update", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Teachers.Delete", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Teachers.Approve", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Teachers.Assign", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Teachers.Publish", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Teachers.Export", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", "Teachers.Print", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", false, false, "Teachers.Generate", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Teachers.Manage", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, true, "Classes.View", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", true, false, "Classes.Read", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "Action", "CanCreate", "CanDelete", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Classes.Create", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Classes.Edit", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 96,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", "Classes.Update", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 97,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Classes.Delete", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Classes.Approve", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, false, false, "Classes.Assign", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Classes.Publish", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, true, "Classes.Export", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", true, false, "Classes.Print", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, false, true, "Classes.Generate", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Classes.Manage", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Sections.View", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", true, false, "Sections.Read", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Sections.Create", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, false, false, "Sections.Edit", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, true, "Sections.Update", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "Action", "CanCreate", "CanDelete", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, true, "Sections.Delete", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Sections.Approve", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Sections.Assign", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Sections.Publish", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 114,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Sections.Export", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 115,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", true, false, "Sections.Print", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 116,
                columns: new[] { "Action", "CanCreate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, "Sections.Generate", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 117,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Sections.Manage", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 118,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Subjects.View", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 119,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", false, true, "Subjects.Read", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Subjects.Create", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Subjects.Edit", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", "Subjects.Update", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Subjects.Delete", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 124,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Subjects.Approve", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 125,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Subjects.Assign", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 126,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, false, false, "Subjects.Publish", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 127,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", "Subjects.Export", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 128,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", false, true, "Subjects.Print", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 129,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, true, false, "Subjects.Generate", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Subjects.Manage", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 131,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Attendance.View", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 132,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", true, false, "Attendance.Read", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 133,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Attendance.Create", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 134,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Attendance.Edit", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 135,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, false, false, "Attendance.Update", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 136,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Attendance.Delete", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 137,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Attendance.Approve", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 138,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Attendance.Assign", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 139,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Attendance.Publish", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 140,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Attendance.Export", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 141,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", true, false, "Attendance.Print", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 142,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, true, false, "Attendance.Generate", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 143,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Attendance.Manage", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 144,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, false, false, "Exams.View", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 145,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", "Exams.Read", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 146,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Exams.Create", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 147,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Exams.Edit", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 148,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, true, "Exams.Update", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 149,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Exams.Delete", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 150,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Exams.Approve", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 151,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Exams.Assign", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 152,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Exams.Publish", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 153,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, false, false, "Exams.Export", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 154,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", "Exams.Print", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 155,
                columns: new[] { "Action", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, "Exams.Generate", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 156,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Exams.Manage", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 157,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, true, "Marks.View", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 158,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", true, false, "Marks.Read", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 159,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Marks.Create", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 160,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Marks.Edit", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 161,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, true, "Marks.Update", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 162,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, false, false, "Marks.Delete", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 163,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Marks.Approve", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 164,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Marks.Assign", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 165,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Marks.Publish", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 166,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, true, "Marks.Export", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 167,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", true, false, "Marks.Print", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 168,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, true, false, "Marks.Generate", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 169,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Marks.Manage", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 170,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "View", "Assignments.View", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 171,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", false, false, false, "Assignments.Read", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 172,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Assignments.Create", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 173,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Assignments.Edit", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 174,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", "Assignments.Update", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 175,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Assignments.Delete", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 176,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Assignments.Approve", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 177,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Assignments.Assign", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 178,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Assignments.Publish", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 179,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Assignments.Export", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 180,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", false, false, false, "Assignments.Print", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 181,
                columns: new[] { "Action", "CanCreate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, "Assignments.Generate", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 182,
                columns: new[] { "Action", "CanDelete", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Assignments.Manage", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 183,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Fees.View", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 184,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", false, true, "Fees.Read", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 185,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Fees.Create", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 186,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Fees.Edit", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 187,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", "Fees.Update", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 188,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Fees.Delete", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 189,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, false, false, "Fees.Approve", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 190,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Fees.Assign", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 191,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Fees.Publish", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 192,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Fees.Export", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 193,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", false, true, "Fees.Print", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 194,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, true, false, "Fees.Generate", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 195,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Fees.Manage", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 196,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Payments.View", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 197,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", "Payments.Read", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 198,
                columns: new[] { "Action", "CanDelete", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", false, false, false, "Payments.Create", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 199,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Payments.Edit", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, true, "Payments.Update", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 201,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Payments.Delete", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 202,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Payments.Approve", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 203,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Payments.Assign", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 204,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Payments.Publish", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 205,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Payments.Export", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 206,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", "Payments.Print", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 207,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", false, false, "Payments.Generate", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 208,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Payments.Manage", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 209,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, true, "Library.View", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 210,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", true, false, "Library.Read", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 211,
                columns: new[] { "Action", "CanCreate", "CanDelete", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Library.Create", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 212,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Library.Edit", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 213,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", "Library.Update", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 214,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Library.Delete", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 215,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Library.Approve", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 216,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, false, false, "Library.Assign", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 217,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Library.Publish", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 218,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, true, "Library.Export", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 219,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", true, false, "Library.Print", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 220,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, false, true, "Library.Generate", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 221,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Library.Manage", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 222,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Transport.View", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 223,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", true, false, "Transport.Read", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 224,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Transport.Create", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 225,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, false, false, "Transport.Edit", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 226,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, true, "Transport.Update", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 227,
                columns: new[] { "Action", "CanCreate", "CanDelete", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, true, "Transport.Delete", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 228,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Transport.Approve", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 229,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Transport.Assign", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 230,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Transport.Publish", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 231,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Transport.Export", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 232,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", true, false, "Transport.Print", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 233,
                columns: new[] { "Action", "CanCreate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, "Transport.Generate", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 234,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Transport.Manage", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 235,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Health.View", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 236,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", false, true, "Health.Read", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 237,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Health.Create", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 238,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Health.Edit", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 239,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", "Health.Update", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 240,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Health.Delete", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 241,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Health.Approve", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 242,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Health.Assign", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 243,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, false, false, "Health.Publish", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 244,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", "Health.Export", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 245,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", false, true, "Health.Print", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 246,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, true, false, "Health.Generate", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 247,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Health.Manage", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 248,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Notifications.View", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 249,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", true, false, "Notifications.Read", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 250,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Notifications.Create", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 251,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Notifications.Edit", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 252,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, false, false, "Notifications.Update", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 253,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Notifications.Delete", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 254,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Notifications.Approve", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 255,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Notifications.Assign", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 256,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Notifications.Publish", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 257,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Notifications.Export", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 258,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Print", true, false, "Notifications.Print", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 259,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Generate", true, true, false, "Notifications.Generate", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 260,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Notifications.Manage", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 261,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, false, false, "Reports.View", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 262,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", "Reports.Read", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 263,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Reports.Create", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 264,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Reports.Edit", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 265,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, true, "Reports.Update", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 266,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Reports.Delete", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 267,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Reports.Approve", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 268,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Reports.Assign", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 269,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Reports.Publish", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 270,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, false, false, "Reports.Export", "Reports", "Reports" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CanCreate", "CanDelete", "CanRead", "CanUpdate", "Code", "CreatedAt", "CreatedBy", "IsDeleted", "Module", "ModuleName", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 271, "Print", false, false, true, false, "Reports.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 272, "Generate", true, false, true, false, "Reports.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 273, "Manage", true, true, true, true, "Reports.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 274, "View", false, false, true, false, "Settings.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 275, "Read", false, false, true, false, "Settings.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 276, "Create", true, false, false, false, "Settings.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 277, "Edit", false, false, false, true, "Settings.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 278, "Update", false, false, false, true, "Settings.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 279, "Delete", false, true, false, false, "Settings.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 280, "Approve", false, false, false, true, "Settings.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 281, "Assign", false, false, false, true, "Settings.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 282, "Publish", false, false, false, true, "Settings.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 283, "Export", false, false, true, false, "Settings.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 284, "Print", false, false, true, false, "Settings.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 285, "Generate", true, false, true, false, "Settings.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 286, "Manage", true, true, true, true, "Settings.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 287, "View", false, false, true, false, "Academic.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 288, "Read", false, false, true, false, "Academic.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 289, "Create", true, false, false, false, "Academic.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 290, "Edit", false, false, false, true, "Academic.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 291, "Update", false, false, false, true, "Academic.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 292, "Delete", false, true, false, false, "Academic.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 293, "Approve", false, false, false, true, "Academic.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 294, "Assign", false, false, false, true, "Academic.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 295, "Publish", false, false, false, true, "Academic.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 296, "Export", false, false, true, false, "Academic.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 297, "Print", false, false, true, false, "Academic.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 298, "Generate", true, false, true, false, "Academic.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 299, "Manage", true, true, true, true, "Academic.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 300, "View", false, false, true, false, "Admission.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 301, "Read", false, false, true, false, "Admission.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 302, "Create", true, false, false, false, "Admission.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 303, "Edit", false, false, false, true, "Admission.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 304, "Update", false, false, false, true, "Admission.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 305, "Delete", false, true, false, false, "Admission.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 306, "Approve", false, false, false, true, "Admission.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 307, "Assign", false, false, false, true, "Admission.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 308, "Publish", false, false, false, true, "Admission.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 309, "Export", false, false, true, false, "Admission.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 310, "Print", false, false, true, false, "Admission.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 311, "Generate", true, false, true, false, "Admission.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 312, "Manage", true, true, true, true, "Admission.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 313, "View", false, false, true, false, "Student.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 314, "Read", false, false, true, false, "Student.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 315, "Create", true, false, false, false, "Student.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 316, "Edit", false, false, false, true, "Student.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 317, "Update", false, false, false, true, "Student.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 318, "Delete", false, true, false, false, "Student.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 319, "Approve", false, false, false, true, "Student.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 320, "Assign", false, false, false, true, "Student.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 321, "Publish", false, false, false, true, "Student.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 322, "Export", false, false, true, false, "Student.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 323, "Print", false, false, true, false, "Student.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 324, "Generate", true, false, true, false, "Student.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 325, "Manage", true, true, true, true, "Student.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 326, "View", false, false, true, false, "Exam.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 327, "Read", false, false, true, false, "Exam.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 328, "Create", true, false, false, false, "Exam.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 329, "Edit", false, false, false, true, "Exam.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 330, "Update", false, false, false, true, "Exam.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 331, "Delete", false, true, false, false, "Exam.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 332, "Approve", false, false, false, true, "Exam.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 333, "Assign", false, false, false, true, "Exam.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 334, "Publish", false, false, false, true, "Exam.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 335, "Export", false, false, true, false, "Exam.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 336, "Print", false, false, true, false, "Exam.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 337, "Generate", true, false, true, false, "Exam.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 338, "Manage", true, true, true, true, "Exam.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 339, "View", false, false, true, false, "Result.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 340, "Read", false, false, true, false, "Result.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 341, "Create", true, false, false, false, "Result.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 342, "Edit", false, false, false, true, "Result.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 343, "Update", false, false, false, true, "Result.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 344, "Delete", false, true, false, false, "Result.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 345, "Approve", false, false, false, true, "Result.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 346, "Assign", false, false, false, true, "Result.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 347, "Publish", false, false, false, true, "Result.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 348, "Export", false, false, true, false, "Result.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 349, "Print", false, false, true, false, "Result.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 350, "Generate", true, false, true, false, "Result.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 351, "Manage", true, true, true, true, "Result.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 352, "View", false, false, true, false, "Communication.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 353, "Read", false, false, true, false, "Communication.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 354, "Create", true, false, false, false, "Communication.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 355, "Edit", false, false, false, true, "Communication.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 356, "Update", false, false, false, true, "Communication.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 357, "Delete", false, true, false, false, "Communication.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 358, "Approve", false, false, false, true, "Communication.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 359, "Assign", false, false, false, true, "Communication.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 360, "Publish", false, false, false, true, "Communication.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 361, "Export", false, false, true, false, "Communication.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 362, "Print", false, false, true, false, "Communication.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 363, "Generate", true, false, true, false, "Communication.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 364, "Manage", true, true, true, true, "Communication.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 365, "View", false, false, true, false, "System.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 366, "Read", false, false, true, false, "System.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 367, "Create", true, false, false, false, "System.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 368, "Edit", false, false, false, true, "System.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 369, "Update", false, false, false, true, "System.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 370, "Delete", false, true, false, false, "System.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 371, "Approve", false, false, false, true, "System.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 372, "Assign", false, false, false, true, "System.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 373, "Publish", false, false, false, true, "System.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 374, "Export", false, false, true, false, "System.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 375, "Print", false, false, true, false, "System.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 376, "Generate", true, false, true, false, "System.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 377, "Manage", true, true, true, true, "System.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 378, "View", false, false, true, false, "AuditLogs.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 379, "Read", false, false, true, false, "AuditLogs.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 380, "Create", true, false, false, false, "AuditLogs.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 381, "Edit", false, false, false, true, "AuditLogs.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 382, "Update", false, false, false, true, "AuditLogs.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 383, "Delete", false, true, false, false, "AuditLogs.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 384, "Approve", false, false, false, true, "AuditLogs.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 385, "Assign", false, false, false, true, "AuditLogs.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 386, "Publish", false, false, false, true, "AuditLogs.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 387, "Export", false, false, true, false, "AuditLogs.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 388, "Print", false, false, true, false, "AuditLogs.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 389, "Generate", true, false, true, false, "AuditLogs.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 390, "Manage", true, true, true, true, "AuditLogs.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 391, "View", false, false, true, false, "FeeStructures.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 392, "Read", false, false, true, false, "FeeStructures.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 393, "Create", true, false, false, false, "FeeStructures.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 394, "Edit", false, false, false, true, "FeeStructures.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 395, "Update", false, false, false, true, "FeeStructures.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 396, "Delete", false, true, false, false, "FeeStructures.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 397, "Approve", false, false, false, true, "FeeStructures.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 398, "Assign", false, false, false, true, "FeeStructures.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 399, "Publish", false, false, false, true, "FeeStructures.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 400, "Export", false, false, true, false, "FeeStructures.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 401, "Print", false, false, true, false, "FeeStructures.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 402, "Generate", true, false, true, false, "FeeStructures.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 403, "Manage", true, true, true, true, "FeeStructures.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FeeStructures", "FeeStructures", null, null },
                    { 404, "View", false, false, true, false, "Invoices.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 405, "Read", false, false, true, false, "Invoices.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 406, "Create", true, false, false, false, "Invoices.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 407, "Edit", false, false, false, true, "Invoices.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 408, "Update", false, false, false, true, "Invoices.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 409, "Delete", false, true, false, false, "Invoices.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 410, "Approve", false, false, false, true, "Invoices.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 411, "Assign", false, false, false, true, "Invoices.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 412, "Publish", false, false, false, true, "Invoices.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 413, "Export", false, false, true, false, "Invoices.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 414, "Print", false, false, true, false, "Invoices.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 415, "Generate", true, false, true, false, "Invoices.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 416, "Manage", true, true, true, true, "Invoices.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Invoices", "Invoices", null, null },
                    { 417, "View", false, false, true, false, "Scholarships.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 418, "Read", false, false, true, false, "Scholarships.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 419, "Create", true, false, false, false, "Scholarships.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 420, "Edit", false, false, false, true, "Scholarships.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 421, "Update", false, false, false, true, "Scholarships.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 422, "Delete", false, true, false, false, "Scholarships.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 423, "Approve", false, false, false, true, "Scholarships.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 424, "Assign", false, false, false, true, "Scholarships.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 425, "Publish", false, false, false, true, "Scholarships.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 426, "Export", false, false, true, false, "Scholarships.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 427, "Print", false, false, true, false, "Scholarships.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 428, "Generate", true, false, true, false, "Scholarships.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 429, "Manage", true, true, true, true, "Scholarships.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Scholarships", "Scholarships", null, null },
                    { 430, "View", false, false, true, false, "Waivers.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 431, "Read", false, false, true, false, "Waivers.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 432, "Create", true, false, false, false, "Waivers.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 433, "Edit", false, false, false, true, "Waivers.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 434, "Update", false, false, false, true, "Waivers.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 435, "Delete", false, true, false, false, "Waivers.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 436, "Approve", false, false, false, true, "Waivers.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 437, "Assign", false, false, false, true, "Waivers.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 438, "Publish", false, false, false, true, "Waivers.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 439, "Export", false, false, true, false, "Waivers.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 440, "Print", false, false, true, false, "Waivers.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 441, "Generate", true, false, true, false, "Waivers.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 442, "Manage", true, true, true, true, "Waivers.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Waivers", "Waivers", null, null },
                    { 443, "View", false, false, true, false, "StudentDues.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 444, "Read", false, false, true, false, "StudentDues.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 445, "Create", true, false, false, false, "StudentDues.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 446, "Edit", false, false, false, true, "StudentDues.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 447, "Update", false, false, false, true, "StudentDues.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 448, "Delete", false, true, false, false, "StudentDues.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 449, "Approve", false, false, false, true, "StudentDues.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 450, "Assign", false, false, false, true, "StudentDues.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 451, "Publish", false, false, false, true, "StudentDues.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 452, "Export", false, false, true, false, "StudentDues.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 453, "Print", false, false, true, false, "StudentDues.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 454, "Generate", true, false, true, false, "StudentDues.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 455, "Manage", true, true, true, true, "StudentDues.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "StudentDues", "StudentDues", null, null },
                    { 456, "View", false, false, true, false, "FinancialTransactions.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 457, "Read", false, false, true, false, "FinancialTransactions.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 458, "Create", true, false, false, false, "FinancialTransactions.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 459, "Edit", false, false, false, true, "FinancialTransactions.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 460, "Update", false, false, false, true, "FinancialTransactions.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 461, "Delete", false, true, false, false, "FinancialTransactions.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 462, "Approve", false, false, false, true, "FinancialTransactions.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 463, "Assign", false, false, false, true, "FinancialTransactions.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 464, "Publish", false, false, false, true, "FinancialTransactions.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 465, "Export", false, false, true, false, "FinancialTransactions.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 466, "Print", false, false, true, false, "FinancialTransactions.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 467, "Generate", true, false, true, false, "FinancialTransactions.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 468, "Manage", true, true, true, true, "FinancialTransactions.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinancialTransactions", "FinancialTransactions", null, null },
                    { 469, "View", false, false, true, false, "FinanceReports.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 470, "Read", false, false, true, false, "FinanceReports.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 471, "Create", true, false, false, false, "FinanceReports.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 472, "Edit", false, false, false, true, "FinanceReports.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 473, "Update", false, false, false, true, "FinanceReports.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 474, "Delete", false, true, false, false, "FinanceReports.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 475, "Approve", false, false, false, true, "FinanceReports.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 476, "Assign", false, false, false, true, "FinanceReports.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 477, "Publish", false, false, false, true, "FinanceReports.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 478, "Export", false, false, true, false, "FinanceReports.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 479, "Print", false, false, true, false, "FinanceReports.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 480, "Generate", true, false, true, false, "FinanceReports.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 481, "Manage", true, true, true, true, "FinanceReports.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceReports", "FinanceReports", null, null },
                    { 482, "View", false, false, true, false, "FinanceConfiguration.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 483, "Read", false, false, true, false, "FinanceConfiguration.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 484, "Create", true, false, false, false, "FinanceConfiguration.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 485, "Edit", false, false, false, true, "FinanceConfiguration.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 486, "Update", false, false, false, true, "FinanceConfiguration.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 487, "Delete", false, true, false, false, "FinanceConfiguration.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 488, "Approve", false, false, false, true, "FinanceConfiguration.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 489, "Assign", false, false, false, true, "FinanceConfiguration.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 490, "Publish", false, false, false, true, "FinanceConfiguration.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 491, "Export", false, false, true, false, "FinanceConfiguration.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 492, "Print", false, false, true, false, "FinanceConfiguration.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 493, "Generate", true, false, true, false, "FinanceConfiguration.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 494, "Manage", true, true, true, true, "FinanceConfiguration.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceConfiguration", "FinanceConfiguration", null, null },
                    { 495, "View", false, false, true, false, "FinanceDashboard.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 496, "Read", false, false, true, false, "FinanceDashboard.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 497, "Create", true, false, false, false, "FinanceDashboard.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 498, "Edit", false, false, false, true, "FinanceDashboard.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 499, "Update", false, false, false, true, "FinanceDashboard.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 500, "Delete", false, true, false, false, "FinanceDashboard.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 501, "Approve", false, false, false, true, "FinanceDashboard.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 502, "Assign", false, false, false, true, "FinanceDashboard.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 503, "Publish", false, false, false, true, "FinanceDashboard.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 504, "Export", false, false, true, false, "FinanceDashboard.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 505, "Print", false, false, true, false, "FinanceDashboard.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 506, "Generate", true, false, true, false, "FinanceDashboard.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 507, "Manage", true, true, true, true, "FinanceDashboard.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "FinanceDashboard", "FinanceDashboard", null, null },
                    { 508, "View", false, false, true, false, "Receipts.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 509, "Read", false, false, true, false, "Receipts.Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 510, "Create", true, false, false, false, "Receipts.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 511, "Edit", false, false, false, true, "Receipts.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 512, "Update", false, false, false, true, "Receipts.Update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 513, "Delete", false, true, false, false, "Receipts.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 514, "Approve", false, false, false, true, "Receipts.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 515, "Assign", false, false, false, true, "Receipts.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 516, "Publish", false, false, false, true, "Receipts.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 517, "Export", false, false, true, false, "Receipts.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 518, "Print", false, false, true, false, "Receipts.Print", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 519, "Generate", true, false, true, false, "Receipts.Generate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null },
                    { 520, "Manage", true, true, true, true, "Receipts.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Receipts", "Receipts", null, null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 10, 3 },
                    { 11, 3 },
                    { 12, 3 },
                    { 13, 3 },
                    { 55, 3 },
                    { 56, 3 },
                    { 57, 3 },
                    { 58, 3 },
                    { 59, 3 },
                    { 60, 3 },
                    { 61, 3 },
                    { 62, 3 },
                    { 63, 3 },
                    { 118, 3 },
                    { 119, 3 },
                    { 120, 3 },
                    { 121, 3 },
                    { 122, 3 },
                    { 123, 3 },
                    { 124, 3 },
                    { 125, 3 },
                    { 126, 3 },
                    { 127, 3 },
                    { 128, 3 },
                    { 129, 3 },
                    { 130, 3 },
                    { 131, 3 },
                    { 132, 3 },
                    { 133, 3 },
                    { 134, 3 },
                    { 135, 3 },
                    { 136, 3 },
                    { 137, 3 },
                    { 138, 3 },
                    { 139, 3 },
                    { 140, 3 },
                    { 141, 3 },
                    { 142, 3 },
                    { 143, 3 },
                    { 144, 3 },
                    { 145, 3 },
                    { 146, 3 },
                    { 147, 3 },
                    { 148, 3 },
                    { 149, 3 },
                    { 150, 3 },
                    { 151, 3 },
                    { 152, 3 },
                    { 153, 3 },
                    { 154, 3 },
                    { 155, 3 },
                    { 156, 3 },
                    { 157, 3 },
                    { 158, 3 },
                    { 159, 3 },
                    { 160, 3 },
                    { 161, 3 },
                    { 162, 3 },
                    { 163, 3 },
                    { 164, 3 },
                    { 165, 3 },
                    { 166, 3 },
                    { 167, 3 },
                    { 168, 3 },
                    { 169, 3 },
                    { 261, 3 },
                    { 262, 3 },
                    { 263, 3 },
                    { 264, 3 },
                    { 265, 3 },
                    { 266, 3 },
                    { 267, 3 },
                    { 268, 3 },
                    { 269, 3 },
                    { 270, 3 },
                    { 66, 5 },
                    { 131, 5 },
                    { 133, 5 },
                    { 144, 5 },
                    { 157, 5 },
                    { 159, 5 },
                    { 170, 5 },
                    { 172, 5 },
                    { 261, 5 },
                    { 4, 6 },
                    { 10, 6 },
                    { 11, 6 },
                    { 12, 6 },
                    { 13, 6 },
                    { 55, 6 },
                    { 56, 6 },
                    { 57, 6 },
                    { 59, 6 },
                    { 60, 6 },
                    { 61, 6 },
                    { 62, 6 },
                    { 63, 6 },
                    { 64, 6 },
                    { 65, 6 },
                    { 66, 6 },
                    { 67, 6 },
                    { 68, 6 },
                    { 69, 6 },
                    { 70, 6 },
                    { 72, 6 },
                    { 73, 6 },
                    { 74, 6 },
                    { 75, 6 },
                    { 76, 6 },
                    { 77, 6 },
                    { 78, 6 },
                    { 184, 6 },
                    { 190, 6 },
                    { 191, 6 },
                    { 192, 6 },
                    { 193, 6 },
                    { 194, 6 },
                    { 195, 6 },
                    { 196, 6 },
                    { 197, 6 },
                    { 198, 6 },
                    { 199, 6 },
                    { 200, 6 },
                    { 202, 6 },
                    { 203, 6 },
                    { 204, 6 },
                    { 205, 6 },
                    { 206, 6 },
                    { 207, 6 },
                    { 261, 6 },
                    { 262, 6 },
                    { 263, 6 },
                    { 264, 6 },
                    { 265, 6 },
                    { 267, 6 },
                    { 268, 6 },
                    { 269, 6 },
                    { 270, 6 },
                    { 2, 7 },
                    { 66, 7 },
                    { 131, 7 },
                    { 157, 7 },
                    { 170, 7 },
                    { 183, 7 },
                    { 196, 7 },
                    { 197, 7 },
                    { 248, 7 },
                    { 1, 20 },
                    { 2, 20 },
                    { 196, 20 },
                    { 197, 20 },
                    { 198, 20 },
                    { 199, 20 },
                    { 200, 20 },
                    { 201, 20 },
                    { 202, 20 },
                    { 203, 20 },
                    { 204, 20 },
                    { 205, 20 },
                    { 206, 20 },
                    { 207, 20 },
                    { 208, 20 },
                    { 1, 26 },
                    { 2, 26 },
                    { 3, 26 },
                    { 4, 26 },
                    { 5, 26 },
                    { 6, 26 },
                    { 7, 26 },
                    { 8, 26 },
                    { 9, 26 },
                    { 10, 26 },
                    { 11, 26 },
                    { 12, 26 },
                    { 13, 26 },
                    { 14, 26 },
                    { 15, 26 },
                    { 16, 26 },
                    { 17, 26 },
                    { 18, 26 },
                    { 19, 26 },
                    { 20, 26 },
                    { 21, 26 },
                    { 22, 26 },
                    { 23, 26 },
                    { 24, 26 },
                    { 25, 26 },
                    { 26, 26 },
                    { 27, 26 },
                    { 28, 26 },
                    { 29, 26 },
                    { 30, 26 },
                    { 31, 26 },
                    { 32, 26 },
                    { 33, 26 },
                    { 34, 26 },
                    { 35, 26 },
                    { 36, 26 },
                    { 37, 26 },
                    { 38, 26 },
                    { 39, 26 },
                    { 40, 26 },
                    { 41, 26 },
                    { 42, 26 },
                    { 43, 26 },
                    { 44, 26 },
                    { 45, 26 },
                    { 46, 26 },
                    { 47, 26 },
                    { 48, 26 },
                    { 49, 26 },
                    { 50, 26 },
                    { 51, 26 },
                    { 52, 26 },
                    { 53, 26 },
                    { 54, 26 },
                    { 55, 26 },
                    { 56, 26 },
                    { 57, 26 },
                    { 58, 26 },
                    { 59, 26 },
                    { 60, 26 },
                    { 61, 26 },
                    { 62, 26 },
                    { 63, 26 },
                    { 64, 26 },
                    { 65, 26 },
                    { 66, 26 },
                    { 67, 26 },
                    { 68, 26 },
                    { 69, 26 },
                    { 70, 26 },
                    { 71, 26 },
                    { 72, 26 },
                    { 73, 26 },
                    { 74, 26 },
                    { 75, 26 },
                    { 76, 26 },
                    { 77, 26 },
                    { 78, 26 },
                    { 79, 26 },
                    { 80, 26 },
                    { 81, 26 },
                    { 82, 26 },
                    { 83, 26 },
                    { 84, 26 },
                    { 85, 26 },
                    { 86, 26 },
                    { 87, 26 },
                    { 88, 26 },
                    { 89, 26 },
                    { 90, 26 },
                    { 91, 26 },
                    { 92, 26 },
                    { 93, 26 },
                    { 94, 26 },
                    { 95, 26 },
                    { 96, 26 },
                    { 97, 26 },
                    { 98, 26 },
                    { 99, 26 },
                    { 100, 26 },
                    { 101, 26 },
                    { 102, 26 },
                    { 103, 26 },
                    { 104, 26 },
                    { 105, 26 },
                    { 106, 26 },
                    { 107, 26 },
                    { 108, 26 },
                    { 109, 26 },
                    { 110, 26 },
                    { 111, 26 },
                    { 112, 26 },
                    { 113, 26 },
                    { 114, 26 },
                    { 115, 26 },
                    { 116, 26 },
                    { 117, 26 },
                    { 118, 26 },
                    { 119, 26 },
                    { 120, 26 },
                    { 121, 26 },
                    { 122, 26 },
                    { 123, 26 },
                    { 124, 26 },
                    { 125, 26 },
                    { 126, 26 },
                    { 127, 26 },
                    { 128, 26 },
                    { 129, 26 },
                    { 130, 26 },
                    { 131, 26 },
                    { 132, 26 },
                    { 133, 26 },
                    { 134, 26 },
                    { 135, 26 },
                    { 136, 26 },
                    { 137, 26 },
                    { 138, 26 },
                    { 139, 26 },
                    { 140, 26 },
                    { 141, 26 },
                    { 142, 26 },
                    { 143, 26 },
                    { 144, 26 },
                    { 145, 26 },
                    { 146, 26 },
                    { 147, 26 },
                    { 148, 26 },
                    { 149, 26 },
                    { 150, 26 },
                    { 151, 26 },
                    { 152, 26 },
                    { 153, 26 },
                    { 154, 26 },
                    { 155, 26 },
                    { 156, 26 },
                    { 157, 26 },
                    { 158, 26 },
                    { 159, 26 },
                    { 160, 26 },
                    { 161, 26 },
                    { 162, 26 },
                    { 163, 26 },
                    { 164, 26 },
                    { 165, 26 },
                    { 166, 26 },
                    { 167, 26 },
                    { 168, 26 },
                    { 169, 26 },
                    { 170, 26 },
                    { 171, 26 },
                    { 172, 26 },
                    { 173, 26 },
                    { 174, 26 },
                    { 175, 26 },
                    { 176, 26 },
                    { 177, 26 },
                    { 178, 26 },
                    { 179, 26 },
                    { 180, 26 },
                    { 181, 26 },
                    { 182, 26 },
                    { 183, 26 },
                    { 184, 26 },
                    { 185, 26 },
                    { 186, 26 },
                    { 187, 26 },
                    { 188, 26 },
                    { 189, 26 },
                    { 190, 26 },
                    { 191, 26 },
                    { 192, 26 },
                    { 193, 26 },
                    { 194, 26 },
                    { 195, 26 },
                    { 196, 26 },
                    { 197, 26 },
                    { 198, 26 },
                    { 199, 26 },
                    { 200, 26 },
                    { 201, 26 },
                    { 202, 26 },
                    { 203, 26 },
                    { 204, 26 },
                    { 205, 26 },
                    { 206, 26 },
                    { 207, 26 },
                    { 208, 26 },
                    { 209, 26 },
                    { 210, 26 },
                    { 211, 26 },
                    { 212, 26 },
                    { 213, 26 },
                    { 214, 26 },
                    { 215, 26 },
                    { 216, 26 },
                    { 217, 26 },
                    { 218, 26 },
                    { 219, 26 },
                    { 220, 26 },
                    { 221, 26 },
                    { 222, 26 },
                    { 223, 26 },
                    { 224, 26 },
                    { 225, 26 },
                    { 226, 26 },
                    { 227, 26 },
                    { 228, 26 },
                    { 229, 26 },
                    { 230, 26 },
                    { 231, 26 },
                    { 232, 26 },
                    { 233, 26 },
                    { 234, 26 },
                    { 235, 26 },
                    { 236, 26 },
                    { 237, 26 },
                    { 238, 26 },
                    { 239, 26 },
                    { 240, 26 },
                    { 241, 26 },
                    { 242, 26 },
                    { 243, 26 },
                    { 244, 26 },
                    { 245, 26 },
                    { 246, 26 },
                    { 247, 26 },
                    { 248, 26 },
                    { 249, 26 },
                    { 250, 26 },
                    { 251, 26 },
                    { 252, 26 },
                    { 253, 26 },
                    { 254, 26 },
                    { 255, 26 },
                    { 256, 26 },
                    { 257, 26 },
                    { 258, 26 },
                    { 259, 26 },
                    { 260, 26 },
                    { 261, 26 },
                    { 262, 26 },
                    { 263, 26 },
                    { 264, 26 },
                    { 265, 26 },
                    { 266, 26 },
                    { 267, 26 },
                    { 268, 26 },
                    { 269, 26 },
                    { 270, 26 }
                });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 1,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 2,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 3,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 4,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 5,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 6,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 7,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 8,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 9,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 10,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 11,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 12,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 13,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 14,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 15,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 16,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 17,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 18,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 19,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 20,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 21,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 22,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 23,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 24,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 25,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 26,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 27,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 28,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 29,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 30,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 31,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 32,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 33,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 34,
                column: "StudentGroupId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                column: "NameBn",
                value: "বাংলা");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "English", "ইংরেজি" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Mathematics", "গণিত" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "General Science", "সাধারণ বিজ্ঞান" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Bangladesh and Global Studies", "বাংলাদেশ ও বিশ্ব পরিচয়" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Religion and Moral Education", "ধর্ম ও নৈতিক শিক্ষা" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Arts and Crafts", "চারুকলা" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Physical Education", "শারীরিক শিক্ষা" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Bangla 1st Paper", "বাংলা ১ম পত্র" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Bangla 2nd Paper", "বাংলা ২য় পত্র" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "English 1st Paper", "ইংরেজি ১ম পত্র" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "English 2nd Paper", "ইংরেজি ২য় পত্র" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Science", "বিজ্ঞান" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Information and Communication Technology", "তথ্য ও যোগাযোগ প্রযুক্তি" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Agriculture Studies", "কৃষি শিক্ষা" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "Physics", "পদার্থবিজ্ঞান", "Science" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "Chemistry", "রসায়ন", "Science" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "Biology", "জীববিজ্ঞান", "Science" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "Higher Mathematics", "উচ্চতর গণিত", "Science" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "Accounting", "হিসাববিজ্ঞান", "Business Studies" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "Finance and Banking", "ফাইন্যান্স", "Business Studies" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "Business Entrepreneurship", "ব্যবসায় উদ্যোগ", "Business Studies" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "History", "ইতিহাস", "Humanities" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "Geography and Environment", "ভূগোল ও পরিবেশ", "Humanities" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "Economics", "অর্থনীতি", "Humanities" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "Civics", "নাগরিকতা", "Humanities" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Career Education", "ক্যারিয়ার শিক্ষা" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Physical Education, Health and Sports", "শারীরিক শিক্ষা, স্বাস্থ্য ও খেলাধুলা" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Home Science", "গার্হস্থ্য বিজ্ঞান" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Islam and Moral Education", "ইসলাম ও নৈতিক শিক্ষা" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Hindu Religion and Moral Education", "হিন্দুধর্ম ও নৈতিক শিক্ষা" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Buddhist Religion and Moral Education", "বৌদ্ধধর্ম ও নৈতিক শিক্ষা" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "Christian Religion and Moral Education", "খ্রিস্টধর্ম ও নৈতিক শিক্ষা" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 271, 1 },
                    { 272, 1 },
                    { 273, 1 },
                    { 274, 1 },
                    { 275, 1 },
                    { 276, 1 },
                    { 277, 1 },
                    { 278, 1 },
                    { 279, 1 },
                    { 280, 1 },
                    { 281, 1 },
                    { 282, 1 },
                    { 283, 1 },
                    { 284, 1 },
                    { 285, 1 },
                    { 286, 1 },
                    { 287, 1 },
                    { 288, 1 },
                    { 289, 1 },
                    { 290, 1 },
                    { 291, 1 },
                    { 292, 1 },
                    { 293, 1 },
                    { 294, 1 },
                    { 295, 1 },
                    { 296, 1 },
                    { 297, 1 },
                    { 298, 1 },
                    { 299, 1 },
                    { 300, 1 },
                    { 301, 1 },
                    { 302, 1 },
                    { 303, 1 },
                    { 304, 1 },
                    { 305, 1 },
                    { 306, 1 },
                    { 307, 1 },
                    { 308, 1 },
                    { 309, 1 },
                    { 310, 1 },
                    { 311, 1 },
                    { 312, 1 },
                    { 313, 1 },
                    { 314, 1 },
                    { 315, 1 },
                    { 316, 1 },
                    { 317, 1 },
                    { 318, 1 },
                    { 319, 1 },
                    { 320, 1 },
                    { 321, 1 },
                    { 322, 1 },
                    { 323, 1 },
                    { 324, 1 },
                    { 325, 1 },
                    { 326, 1 },
                    { 327, 1 },
                    { 328, 1 },
                    { 329, 1 },
                    { 330, 1 },
                    { 331, 1 },
                    { 332, 1 },
                    { 333, 1 },
                    { 334, 1 },
                    { 335, 1 },
                    { 336, 1 },
                    { 337, 1 },
                    { 338, 1 },
                    { 339, 1 },
                    { 340, 1 },
                    { 341, 1 },
                    { 342, 1 },
                    { 343, 1 },
                    { 344, 1 },
                    { 345, 1 },
                    { 346, 1 },
                    { 347, 1 },
                    { 348, 1 },
                    { 349, 1 },
                    { 350, 1 },
                    { 351, 1 },
                    { 352, 1 },
                    { 353, 1 },
                    { 354, 1 },
                    { 355, 1 },
                    { 356, 1 },
                    { 357, 1 },
                    { 358, 1 },
                    { 359, 1 },
                    { 360, 1 },
                    { 361, 1 },
                    { 362, 1 },
                    { 363, 1 },
                    { 364, 1 },
                    { 365, 1 },
                    { 366, 1 },
                    { 367, 1 },
                    { 368, 1 },
                    { 369, 1 },
                    { 370, 1 },
                    { 371, 1 },
                    { 372, 1 },
                    { 373, 1 },
                    { 374, 1 },
                    { 375, 1 },
                    { 376, 1 },
                    { 377, 1 },
                    { 378, 1 },
                    { 379, 1 },
                    { 380, 1 },
                    { 381, 1 },
                    { 382, 1 },
                    { 383, 1 },
                    { 384, 1 },
                    { 385, 1 },
                    { 386, 1 },
                    { 387, 1 },
                    { 388, 1 },
                    { 389, 1 },
                    { 390, 1 },
                    { 391, 1 },
                    { 392, 1 },
                    { 393, 1 },
                    { 394, 1 },
                    { 395, 1 },
                    { 396, 1 },
                    { 397, 1 },
                    { 398, 1 },
                    { 399, 1 },
                    { 400, 1 },
                    { 401, 1 },
                    { 402, 1 },
                    { 403, 1 },
                    { 404, 1 },
                    { 405, 1 },
                    { 406, 1 },
                    { 407, 1 },
                    { 408, 1 },
                    { 409, 1 },
                    { 410, 1 },
                    { 411, 1 },
                    { 412, 1 },
                    { 413, 1 },
                    { 414, 1 },
                    { 415, 1 },
                    { 416, 1 },
                    { 417, 1 },
                    { 418, 1 },
                    { 419, 1 },
                    { 420, 1 },
                    { 421, 1 },
                    { 422, 1 },
                    { 423, 1 },
                    { 424, 1 },
                    { 425, 1 },
                    { 426, 1 },
                    { 427, 1 },
                    { 428, 1 },
                    { 429, 1 },
                    { 430, 1 },
                    { 431, 1 },
                    { 432, 1 },
                    { 433, 1 },
                    { 434, 1 },
                    { 435, 1 },
                    { 436, 1 },
                    { 437, 1 },
                    { 438, 1 },
                    { 439, 1 },
                    { 440, 1 },
                    { 441, 1 },
                    { 442, 1 },
                    { 443, 1 },
                    { 444, 1 },
                    { 445, 1 },
                    { 446, 1 },
                    { 447, 1 },
                    { 448, 1 },
                    { 449, 1 },
                    { 450, 1 },
                    { 451, 1 },
                    { 452, 1 },
                    { 453, 1 },
                    { 454, 1 },
                    { 455, 1 },
                    { 456, 1 },
                    { 457, 1 },
                    { 458, 1 },
                    { 459, 1 },
                    { 460, 1 },
                    { 461, 1 },
                    { 462, 1 },
                    { 463, 1 },
                    { 464, 1 },
                    { 465, 1 },
                    { 466, 1 },
                    { 467, 1 },
                    { 468, 1 },
                    { 469, 1 },
                    { 470, 1 },
                    { 471, 1 },
                    { 472, 1 },
                    { 473, 1 },
                    { 474, 1 },
                    { 475, 1 },
                    { 476, 1 },
                    { 477, 1 },
                    { 478, 1 },
                    { 479, 1 },
                    { 480, 1 },
                    { 481, 1 },
                    { 482, 1 },
                    { 483, 1 },
                    { 484, 1 },
                    { 485, 1 },
                    { 486, 1 },
                    { 487, 1 },
                    { 488, 1 },
                    { 489, 1 },
                    { 490, 1 },
                    { 491, 1 },
                    { 492, 1 },
                    { 493, 1 },
                    { 494, 1 },
                    { 495, 1 },
                    { 496, 1 },
                    { 497, 1 },
                    { 498, 1 },
                    { 499, 1 },
                    { 500, 1 },
                    { 501, 1 },
                    { 502, 1 },
                    { 503, 1 },
                    { 504, 1 },
                    { 505, 1 },
                    { 506, 1 },
                    { 507, 1 },
                    { 508, 1 },
                    { 509, 1 },
                    { 510, 1 },
                    { 511, 1 },
                    { 512, 1 },
                    { 513, 1 },
                    { 514, 1 },
                    { 515, 1 },
                    { 516, 1 },
                    { 517, 1 },
                    { 518, 1 },
                    { 519, 1 },
                    { 520, 1 },
                    { 271, 2 },
                    { 272, 2 },
                    { 273, 2 },
                    { 274, 2 },
                    { 275, 2 },
                    { 276, 2 },
                    { 277, 2 },
                    { 278, 2 },
                    { 279, 2 },
                    { 280, 2 },
                    { 281, 2 },
                    { 282, 2 },
                    { 283, 2 },
                    { 284, 2 },
                    { 285, 2 },
                    { 286, 2 },
                    { 287, 2 },
                    { 288, 2 },
                    { 289, 2 },
                    { 290, 2 },
                    { 291, 2 },
                    { 292, 2 },
                    { 293, 2 },
                    { 294, 2 },
                    { 295, 2 },
                    { 296, 2 },
                    { 297, 2 },
                    { 298, 2 },
                    { 299, 2 },
                    { 300, 2 },
                    { 301, 2 },
                    { 302, 2 },
                    { 303, 2 },
                    { 304, 2 },
                    { 305, 2 },
                    { 306, 2 },
                    { 307, 2 },
                    { 308, 2 },
                    { 309, 2 },
                    { 310, 2 },
                    { 311, 2 },
                    { 312, 2 },
                    { 313, 2 },
                    { 314, 2 },
                    { 315, 2 },
                    { 316, 2 },
                    { 317, 2 },
                    { 318, 2 },
                    { 319, 2 },
                    { 320, 2 },
                    { 321, 2 },
                    { 322, 2 },
                    { 323, 2 },
                    { 324, 2 },
                    { 325, 2 },
                    { 326, 2 },
                    { 327, 2 },
                    { 328, 2 },
                    { 329, 2 },
                    { 330, 2 },
                    { 331, 2 },
                    { 332, 2 },
                    { 333, 2 },
                    { 334, 2 },
                    { 335, 2 },
                    { 336, 2 },
                    { 337, 2 },
                    { 338, 2 },
                    { 339, 2 },
                    { 340, 2 },
                    { 341, 2 },
                    { 342, 2 },
                    { 343, 2 },
                    { 344, 2 },
                    { 345, 2 },
                    { 346, 2 },
                    { 347, 2 },
                    { 348, 2 },
                    { 349, 2 },
                    { 350, 2 },
                    { 351, 2 },
                    { 352, 2 },
                    { 353, 2 },
                    { 354, 2 },
                    { 355, 2 },
                    { 356, 2 },
                    { 357, 2 },
                    { 358, 2 },
                    { 359, 2 },
                    { 360, 2 },
                    { 361, 2 },
                    { 362, 2 },
                    { 363, 2 },
                    { 364, 2 },
                    { 365, 2 },
                    { 366, 2 },
                    { 367, 2 },
                    { 368, 2 },
                    { 369, 2 },
                    { 370, 2 },
                    { 371, 2 },
                    { 372, 2 },
                    { 373, 2 },
                    { 374, 2 },
                    { 375, 2 },
                    { 376, 2 },
                    { 377, 2 },
                    { 378, 2 },
                    { 379, 2 },
                    { 380, 2 },
                    { 381, 2 },
                    { 382, 2 },
                    { 383, 2 },
                    { 384, 2 },
                    { 385, 2 },
                    { 386, 2 },
                    { 387, 2 },
                    { 388, 2 },
                    { 389, 2 },
                    { 390, 2 },
                    { 404, 2 },
                    { 405, 2 },
                    { 417, 2 },
                    { 418, 2 },
                    { 423, 2 },
                    { 430, 2 },
                    { 431, 2 },
                    { 436, 2 },
                    { 443, 2 },
                    { 444, 2 },
                    { 469, 2 },
                    { 470, 2 },
                    { 478, 2 },
                    { 479, 2 },
                    { 495, 2 },
                    { 496, 2 },
                    { 271, 3 },
                    { 272, 3 },
                    { 273, 3 },
                    { 287, 3 },
                    { 288, 3 },
                    { 289, 3 },
                    { 290, 3 },
                    { 291, 3 },
                    { 292, 3 },
                    { 293, 3 },
                    { 294, 3 },
                    { 295, 3 },
                    { 296, 3 },
                    { 297, 3 },
                    { 298, 3 },
                    { 299, 3 },
                    { 300, 3 },
                    { 301, 3 },
                    { 302, 3 },
                    { 303, 3 },
                    { 304, 3 },
                    { 305, 3 },
                    { 306, 3 },
                    { 307, 3 },
                    { 308, 3 },
                    { 309, 3 },
                    { 310, 3 },
                    { 311, 3 },
                    { 312, 3 },
                    { 313, 3 },
                    { 314, 3 },
                    { 315, 3 },
                    { 316, 3 },
                    { 317, 3 },
                    { 318, 3 },
                    { 319, 3 },
                    { 320, 3 },
                    { 321, 3 },
                    { 322, 3 },
                    { 323, 3 },
                    { 324, 3 },
                    { 325, 3 },
                    { 326, 3 },
                    { 327, 3 },
                    { 328, 3 },
                    { 329, 3 },
                    { 330, 3 },
                    { 331, 3 },
                    { 332, 3 },
                    { 333, 3 },
                    { 334, 3 },
                    { 335, 3 },
                    { 336, 3 },
                    { 337, 3 },
                    { 338, 3 },
                    { 339, 3 },
                    { 340, 3 },
                    { 341, 3 },
                    { 342, 3 },
                    { 343, 3 },
                    { 344, 3 },
                    { 345, 3 },
                    { 346, 3 },
                    { 347, 3 },
                    { 348, 3 },
                    { 349, 3 },
                    { 350, 3 },
                    { 351, 3 },
                    { 352, 3 },
                    { 353, 3 },
                    { 354, 3 },
                    { 355, 3 },
                    { 356, 3 },
                    { 357, 3 },
                    { 358, 3 },
                    { 359, 3 },
                    { 360, 3 },
                    { 361, 3 },
                    { 362, 3 },
                    { 363, 3 },
                    { 364, 3 },
                    { 326, 5 },
                    { 271, 6 },
                    { 272, 6 },
                    { 273, 6 },
                    { 300, 6 },
                    { 301, 6 },
                    { 302, 6 },
                    { 303, 6 },
                    { 304, 6 },
                    { 306, 6 },
                    { 307, 6 },
                    { 308, 6 },
                    { 309, 6 },
                    { 310, 6 },
                    { 311, 6 },
                    { 312, 6 },
                    { 313, 6 },
                    { 314, 6 },
                    { 315, 6 },
                    { 316, 6 },
                    { 317, 6 },
                    { 319, 6 },
                    { 320, 6 },
                    { 321, 6 },
                    { 322, 6 },
                    { 323, 6 },
                    { 324, 6 },
                    { 325, 6 },
                    { 313, 7 },
                    { 404, 7 },
                    { 405, 7 },
                    { 443, 7 },
                    { 444, 7 },
                    { 508, 7 },
                    { 509, 7 },
                    { 517, 7 },
                    { 518, 7 },
                    { 391, 20 },
                    { 392, 20 },
                    { 393, 20 },
                    { 394, 20 },
                    { 395, 20 },
                    { 396, 20 },
                    { 397, 20 },
                    { 398, 20 },
                    { 399, 20 },
                    { 400, 20 },
                    { 401, 20 },
                    { 402, 20 },
                    { 403, 20 },
                    { 404, 20 },
                    { 405, 20 },
                    { 406, 20 },
                    { 407, 20 },
                    { 408, 20 },
                    { 409, 20 },
                    { 410, 20 },
                    { 411, 20 },
                    { 412, 20 },
                    { 413, 20 },
                    { 414, 20 },
                    { 415, 20 },
                    { 416, 20 },
                    { 417, 20 },
                    { 418, 20 },
                    { 419, 20 },
                    { 420, 20 },
                    { 421, 20 },
                    { 422, 20 },
                    { 423, 20 },
                    { 424, 20 },
                    { 425, 20 },
                    { 426, 20 },
                    { 427, 20 },
                    { 428, 20 },
                    { 429, 20 },
                    { 430, 20 },
                    { 431, 20 },
                    { 432, 20 },
                    { 433, 20 },
                    { 434, 20 },
                    { 435, 20 },
                    { 436, 20 },
                    { 437, 20 },
                    { 438, 20 },
                    { 439, 20 },
                    { 440, 20 },
                    { 441, 20 },
                    { 442, 20 },
                    { 443, 20 },
                    { 444, 20 },
                    { 445, 20 },
                    { 446, 20 },
                    { 447, 20 },
                    { 448, 20 },
                    { 449, 20 },
                    { 450, 20 },
                    { 451, 20 },
                    { 452, 20 },
                    { 453, 20 },
                    { 454, 20 },
                    { 455, 20 },
                    { 456, 20 },
                    { 457, 20 },
                    { 458, 20 },
                    { 459, 20 },
                    { 460, 20 },
                    { 461, 20 },
                    { 462, 20 },
                    { 463, 20 },
                    { 464, 20 },
                    { 465, 20 },
                    { 466, 20 },
                    { 467, 20 },
                    { 468, 20 },
                    { 469, 20 },
                    { 470, 20 },
                    { 471, 20 },
                    { 472, 20 },
                    { 473, 20 },
                    { 474, 20 },
                    { 475, 20 },
                    { 476, 20 },
                    { 477, 20 },
                    { 478, 20 },
                    { 479, 20 },
                    { 480, 20 },
                    { 481, 20 },
                    { 482, 20 },
                    { 483, 20 },
                    { 484, 20 },
                    { 485, 20 },
                    { 486, 20 },
                    { 487, 20 },
                    { 488, 20 },
                    { 489, 20 },
                    { 490, 20 },
                    { 491, 20 },
                    { 492, 20 },
                    { 493, 20 },
                    { 494, 20 },
                    { 495, 20 },
                    { 496, 20 },
                    { 497, 20 },
                    { 498, 20 },
                    { 499, 20 },
                    { 500, 20 },
                    { 501, 20 },
                    { 502, 20 },
                    { 503, 20 },
                    { 504, 20 },
                    { 505, 20 },
                    { 506, 20 },
                    { 507, 20 },
                    { 508, 20 },
                    { 509, 20 },
                    { 510, 20 },
                    { 511, 20 },
                    { 512, 20 },
                    { 513, 20 },
                    { 514, 20 },
                    { 515, 20 },
                    { 516, 20 },
                    { 517, 20 },
                    { 518, 20 },
                    { 519, 20 },
                    { 520, 20 },
                    { 271, 26 },
                    { 272, 26 },
                    { 273, 26 },
                    { 274, 26 },
                    { 275, 26 },
                    { 276, 26 },
                    { 277, 26 },
                    { 278, 26 },
                    { 279, 26 },
                    { 280, 26 },
                    { 281, 26 },
                    { 282, 26 },
                    { 283, 26 },
                    { 284, 26 },
                    { 285, 26 },
                    { 286, 26 },
                    { 287, 26 },
                    { 288, 26 },
                    { 289, 26 },
                    { 290, 26 },
                    { 291, 26 },
                    { 292, 26 },
                    { 293, 26 },
                    { 294, 26 },
                    { 295, 26 },
                    { 296, 26 },
                    { 297, 26 },
                    { 298, 26 },
                    { 299, 26 },
                    { 300, 26 },
                    { 301, 26 },
                    { 302, 26 },
                    { 303, 26 },
                    { 304, 26 },
                    { 305, 26 },
                    { 306, 26 },
                    { 307, 26 },
                    { 308, 26 },
                    { 309, 26 },
                    { 310, 26 },
                    { 311, 26 },
                    { 312, 26 },
                    { 313, 26 },
                    { 314, 26 },
                    { 315, 26 },
                    { 316, 26 },
                    { 317, 26 },
                    { 318, 26 },
                    { 319, 26 },
                    { 320, 26 },
                    { 321, 26 },
                    { 322, 26 },
                    { 323, 26 },
                    { 324, 26 },
                    { 325, 26 },
                    { 326, 26 },
                    { 327, 26 },
                    { 328, 26 },
                    { 329, 26 },
                    { 330, 26 },
                    { 331, 26 },
                    { 332, 26 },
                    { 333, 26 },
                    { 334, 26 },
                    { 335, 26 },
                    { 336, 26 },
                    { 337, 26 },
                    { 338, 26 },
                    { 339, 26 },
                    { 340, 26 },
                    { 341, 26 },
                    { 342, 26 },
                    { 343, 26 },
                    { 344, 26 },
                    { 345, 26 },
                    { 346, 26 },
                    { 347, 26 },
                    { 348, 26 },
                    { 349, 26 },
                    { 350, 26 },
                    { 351, 26 },
                    { 352, 26 },
                    { 353, 26 },
                    { 354, 26 },
                    { 355, 26 },
                    { 356, 26 },
                    { 357, 26 },
                    { 358, 26 },
                    { 359, 26 },
                    { 360, 26 },
                    { 361, 26 },
                    { 362, 26 },
                    { 363, 26 },
                    { 364, 26 },
                    { 365, 26 },
                    { 366, 26 },
                    { 367, 26 },
                    { 368, 26 },
                    { 369, 26 },
                    { 370, 26 },
                    { 371, 26 },
                    { 372, 26 },
                    { 373, 26 },
                    { 374, 26 },
                    { 375, 26 },
                    { 376, 26 },
                    { 377, 26 },
                    { 378, 26 },
                    { 379, 26 },
                    { 380, 26 },
                    { 381, 26 },
                    { 382, 26 },
                    { 383, 26 },
                    { 384, 26 },
                    { 385, 26 },
                    { 386, 26 },
                    { 387, 26 },
                    { 388, 26 },
                    { 389, 26 },
                    { 390, 26 },
                    { 391, 26 },
                    { 392, 26 },
                    { 393, 26 },
                    { 394, 26 },
                    { 395, 26 },
                    { 396, 26 },
                    { 397, 26 },
                    { 398, 26 },
                    { 399, 26 },
                    { 400, 26 },
                    { 401, 26 },
                    { 402, 26 },
                    { 403, 26 },
                    { 404, 26 },
                    { 405, 26 },
                    { 406, 26 },
                    { 407, 26 },
                    { 408, 26 },
                    { 409, 26 },
                    { 410, 26 },
                    { 411, 26 },
                    { 412, 26 },
                    { 413, 26 },
                    { 414, 26 },
                    { 415, 26 },
                    { 416, 26 },
                    { 417, 26 },
                    { 418, 26 },
                    { 419, 26 },
                    { 420, 26 },
                    { 421, 26 },
                    { 422, 26 },
                    { 423, 26 },
                    { 424, 26 },
                    { 425, 26 },
                    { 426, 26 },
                    { 427, 26 },
                    { 428, 26 },
                    { 429, 26 },
                    { 430, 26 },
                    { 431, 26 },
                    { 432, 26 },
                    { 433, 26 },
                    { 434, 26 },
                    { 435, 26 },
                    { 436, 26 },
                    { 437, 26 },
                    { 438, 26 },
                    { 439, 26 },
                    { 440, 26 },
                    { 441, 26 },
                    { 442, 26 },
                    { 443, 26 },
                    { 444, 26 },
                    { 445, 26 },
                    { 446, 26 },
                    { 447, 26 },
                    { 448, 26 },
                    { 449, 26 },
                    { 450, 26 },
                    { 451, 26 },
                    { 452, 26 },
                    { 453, 26 },
                    { 454, 26 },
                    { 455, 26 },
                    { 456, 26 },
                    { 457, 26 },
                    { 458, 26 },
                    { 459, 26 },
                    { 460, 26 },
                    { 461, 26 },
                    { 462, 26 },
                    { 463, 26 },
                    { 464, 26 },
                    { 465, 26 },
                    { 466, 26 },
                    { 467, 26 },
                    { 468, 26 },
                    { 469, 26 },
                    { 470, 26 },
                    { 471, 26 },
                    { 472, 26 },
                    { 473, 26 },
                    { 474, 26 },
                    { 475, 26 },
                    { 476, 26 },
                    { 477, 26 },
                    { 478, 26 },
                    { 479, 26 },
                    { 480, 26 },
                    { 481, 26 },
                    { 482, 26 },
                    { 483, 26 },
                    { 484, 26 },
                    { 485, 26 },
                    { 486, 26 },
                    { 487, 26 },
                    { 488, 26 },
                    { 489, 26 },
                    { 490, 26 },
                    { 497, 26 },
                    { 498, 26 },
                    { 499, 26 },
                    { 500, 26 },
                    { 501, 26 },
                    { 502, 26 },
                    { 503, 26 },
                    { 504, 26 },
                    { 505, 26 },
                    { 506, 26 },
                    { 507, 26 },
                    { 508, 26 },
                    { 509, 26 },
                    { 510, 26 },
                    { 511, 26 },
                    { 512, 26 },
                    { 513, 26 },
                    { 514, 26 },
                    { 515, 26 },
                    { 516, 26 },
                    { 517, 26 },
                    { 518, 26 },
                    { 519, 26 },
                    { 520, 26 }
                });

            // Removed duplicate index for Sections

            migrationBuilder.CreateIndex(
                name: "IX_Sections_StudentGroupId",
                table: "Sections",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendances_EmployeeId_AttendanceDate",
                table: "EmployeeAttendances",
                columns: new[] { "EmployeeId", "AttendanceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_StudentId_AttendanceDate",
                table: "Attendance",
                columns: new[] { "StudentId", "AttendanceDate" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceNotificationLogs_StudentId_AttendanceDate_NotificationType_NotificationChannel",
                table: "AttendanceNotificationLogs",
                columns: new[] { "StudentId", "AttendanceDate", "NotificationType", "NotificationChannel" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_Students_StudentId",
                table: "Attendance",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_StudentGroups_StudentGroupId",
                table: "Sections",
                column: "StudentGroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Restore section names by prefixing parent group name (best effort rollback)
                UPDATE SEC
                SET 
                    SEC.Name = CASE 
                        WHEN SEC.StudentGroupId = 1 THEN 'Science ' + SEC.Name
                        WHEN SEC.StudentGroupId = 2 THEN 'Business Studies ' + SEC.Name
                        WHEN SEC.StudentGroupId = 3 THEN 'Humanities ' + SEC.Name
                        ELSE SEC.Name
                    END,
                    SEC.StudentGroupId = NULL
                FROM Sections SEC
                WHERE SEC.StudentGroupId IS NOT NULL;
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_Students_StudentId",
                table: "Attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_StudentGroups_StudentGroupId",
                table: "Sections");

            migrationBuilder.DropTable(
                name: "AttendanceNotificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_Sections_SchoolClassId_StudentGroupId_Name",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_StudentGroupId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeAttendances_EmployeeId_AttendanceDate",
                table: "EmployeeAttendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_StudentId_AttendanceDate",
                table: "Attendance");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 271, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 272, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 273, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 274, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 275, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 276, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 277, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 278, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 279, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 280, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 281, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 282, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 283, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 284, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 285, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 286, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 287, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 288, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 289, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 290, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 291, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 292, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 293, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 294, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 295, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 296, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 297, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 298, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 299, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 300, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 301, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 302, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 303, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 304, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 305, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 306, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 307, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 308, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 309, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 310, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 311, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 312, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 313, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 314, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 315, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 316, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 317, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 318, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 319, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 320, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 321, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 322, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 323, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 324, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 325, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 326, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 327, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 328, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 329, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 330, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 331, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 332, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 333, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 334, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 335, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 336, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 337, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 338, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 339, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 340, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 341, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 342, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 343, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 344, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 345, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 346, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 347, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 348, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 349, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 350, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 351, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 352, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 353, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 354, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 355, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 356, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 357, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 358, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 359, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 360, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 361, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 362, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 363, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 364, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 365, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 366, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 367, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 368, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 369, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 370, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 371, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 372, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 373, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 374, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 375, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 376, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 377, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 378, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 379, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 380, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 381, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 382, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 383, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 384, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 385, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 386, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 387, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 388, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 389, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 390, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 391, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 392, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 393, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 394, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 395, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 396, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 397, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 398, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 399, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 400, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 401, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 402, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 403, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 404, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 405, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 406, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 407, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 408, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 409, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 410, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 411, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 412, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 413, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 414, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 415, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 416, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 417, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 418, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 419, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 420, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 421, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 422, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 423, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 424, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 425, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 426, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 427, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 428, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 429, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 430, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 431, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 432, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 433, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 434, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 435, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 436, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 437, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 438, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 439, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 440, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 441, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 442, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 443, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 444, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 445, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 446, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 447, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 448, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 449, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 450, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 451, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 452, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 453, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 454, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 455, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 456, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 457, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 458, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 459, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 460, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 461, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 462, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 463, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 464, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 465, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 466, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 467, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 468, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 469, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 470, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 471, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 472, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 473, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 474, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 475, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 476, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 477, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 478, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 479, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 480, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 481, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 482, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 483, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 484, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 485, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 486, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 487, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 488, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 489, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 490, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 491, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 492, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 493, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 494, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 495, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 496, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 497, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 498, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 499, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 500, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 501, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 502, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 503, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 504, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 505, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 506, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 507, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 508, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 509, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 510, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 511, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 512, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 513, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 514, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 515, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 516, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 517, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 518, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 519, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 520, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 271, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 272, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 273, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 274, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 275, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 276, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 277, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 278, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 279, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 280, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 281, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 282, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 283, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 284, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 285, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 286, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 287, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 288, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 289, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 290, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 291, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 292, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 293, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 294, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 295, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 296, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 297, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 298, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 299, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 300, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 301, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 302, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 303, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 304, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 305, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 306, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 307, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 308, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 309, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 310, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 311, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 312, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 313, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 314, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 315, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 316, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 317, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 318, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 319, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 320, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 321, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 322, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 323, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 324, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 325, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 326, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 327, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 328, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 329, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 330, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 331, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 332, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 333, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 334, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 335, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 336, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 337, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 338, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 339, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 340, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 341, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 342, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 343, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 344, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 345, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 346, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 347, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 348, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 349, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 350, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 351, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 352, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 353, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 354, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 355, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 356, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 357, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 358, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 359, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 360, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 361, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 362, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 363, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 364, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 365, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 366, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 367, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 368, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 369, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 370, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 371, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 372, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 373, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 374, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 375, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 376, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 377, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 378, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 379, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 380, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 381, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 382, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 383, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 384, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 385, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 386, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 387, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 388, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 389, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 390, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 404, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 405, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 417, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 418, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 423, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 430, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 431, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 436, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 443, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 444, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 469, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 470, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 478, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 479, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 495, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 496, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 10, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 12, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 55, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 56, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 57, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 58, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 59, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 60, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 61, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 62, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 63, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 118, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 119, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 120, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 121, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 122, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 123, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 124, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 125, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 126, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 127, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 128, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 129, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 130, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 131, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 132, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 133, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 134, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 135, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 136, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 137, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 138, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 139, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 140, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 141, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 142, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 143, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 144, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 145, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 146, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 147, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 148, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 149, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 150, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 151, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 152, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 153, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 154, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 155, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 156, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 157, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 158, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 159, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 160, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 161, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 162, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 163, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 164, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 165, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 166, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 167, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 168, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 169, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 261, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 262, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 263, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 264, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 265, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 266, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 267, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 268, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 269, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 270, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 271, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 272, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 273, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 287, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 288, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 289, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 290, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 291, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 292, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 293, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 294, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 295, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 296, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 297, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 298, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 299, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 300, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 301, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 302, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 303, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 304, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 305, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 306, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 307, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 308, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 309, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 310, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 311, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 312, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 313, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 314, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 315, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 316, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 317, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 318, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 319, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 320, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 321, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 322, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 323, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 324, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 325, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 326, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 327, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 328, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 329, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 330, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 331, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 332, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 333, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 334, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 335, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 336, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 337, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 338, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 339, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 340, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 341, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 342, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 343, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 344, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 345, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 346, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 347, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 348, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 349, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 350, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 351, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 352, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 353, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 354, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 355, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 356, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 357, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 358, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 359, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 360, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 361, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 362, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 363, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 364, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 66, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 131, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 133, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 144, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 157, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 159, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 170, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 172, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 261, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 326, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 4, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 10, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 12, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 55, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 56, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 57, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 59, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 60, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 61, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 62, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 63, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 64, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 65, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 66, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 67, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 68, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 69, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 70, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 72, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 73, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 74, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 75, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 76, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 77, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 78, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 184, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 190, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 191, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 192, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 193, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 194, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 195, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 196, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 197, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 198, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 199, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 200, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 202, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 203, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 204, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 205, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 206, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 207, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 261, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 262, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 263, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 264, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 265, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 267, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 268, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 269, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 270, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 271, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 272, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 273, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 300, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 301, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 302, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 303, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 304, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 306, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 307, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 308, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 309, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 310, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 311, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 312, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 313, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 314, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 315, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 316, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 317, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 319, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 320, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 321, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 322, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 323, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 324, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 325, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 2, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 66, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 131, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 157, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 170, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 183, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 196, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 197, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 248, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 313, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 404, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 405, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 443, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 444, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 508, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 509, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 517, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 518, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 1, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 2, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 196, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 197, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 198, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 199, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 200, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 201, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 202, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 203, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 204, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 205, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 206, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 207, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 208, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 391, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 392, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 393, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 394, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 395, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 396, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 397, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 398, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 399, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 400, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 401, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 402, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 403, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 404, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 405, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 406, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 407, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 408, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 409, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 410, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 411, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 412, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 413, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 414, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 415, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 416, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 417, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 418, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 419, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 420, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 421, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 422, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 423, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 424, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 425, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 426, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 427, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 428, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 429, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 430, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 431, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 432, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 433, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 434, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 435, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 436, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 437, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 438, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 439, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 440, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 441, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 442, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 443, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 444, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 445, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 446, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 447, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 448, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 449, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 450, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 451, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 452, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 453, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 454, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 455, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 456, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 457, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 458, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 459, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 460, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 461, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 462, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 463, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 464, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 465, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 466, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 467, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 468, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 469, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 470, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 471, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 472, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 473, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 474, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 475, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 476, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 477, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 478, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 479, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 480, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 481, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 482, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 483, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 484, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 485, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 486, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 487, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 488, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 489, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 490, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 491, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 492, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 493, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 494, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 495, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 496, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 497, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 498, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 499, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 500, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 501, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 502, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 503, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 504, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 505, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 506, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 507, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 508, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 509, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 510, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 511, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 512, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 513, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 514, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 515, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 516, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 517, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 518, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 519, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 520, 20 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 1, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 2, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 3, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 4, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 5, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 6, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 7, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 8, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 9, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 10, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 12, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 14, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 15, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 16, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 17, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 18, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 19, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 20, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 21, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 22, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 23, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 24, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 25, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 26, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 27, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 28, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 29, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 30, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 31, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 32, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 33, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 34, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 35, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 36, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 37, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 38, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 39, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 40, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 41, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 42, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 43, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 44, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 45, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 46, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 47, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 48, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 49, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 50, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 51, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 52, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 53, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 54, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 55, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 56, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 57, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 58, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 59, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 60, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 61, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 62, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 63, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 64, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 65, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 66, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 67, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 68, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 69, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 70, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 71, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 72, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 73, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 74, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 75, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 76, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 77, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 78, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 79, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 80, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 81, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 82, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 83, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 84, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 85, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 86, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 87, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 88, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 89, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 90, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 91, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 92, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 93, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 94, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 95, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 96, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 97, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 98, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 99, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 100, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 101, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 102, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 103, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 104, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 105, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 106, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 107, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 108, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 109, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 110, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 111, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 112, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 113, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 114, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 115, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 116, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 117, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 118, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 119, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 120, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 121, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 122, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 123, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 124, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 125, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 126, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 127, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 128, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 129, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 130, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 131, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 132, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 133, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 134, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 135, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 136, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 137, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 138, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 139, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 140, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 141, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 142, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 143, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 144, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 145, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 146, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 147, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 148, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 149, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 150, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 151, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 152, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 153, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 154, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 155, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 156, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 157, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 158, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 159, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 160, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 161, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 162, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 163, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 164, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 165, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 166, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 167, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 168, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 169, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 170, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 171, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 172, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 173, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 174, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 175, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 176, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 177, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 178, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 179, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 180, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 181, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 182, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 183, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 184, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 185, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 186, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 187, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 188, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 189, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 190, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 191, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 192, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 193, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 194, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 195, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 196, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 197, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 198, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 199, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 200, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 201, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 202, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 203, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 204, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 205, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 206, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 207, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 208, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 209, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 210, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 211, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 212, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 213, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 214, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 215, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 216, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 217, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 218, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 219, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 220, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 221, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 222, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 223, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 224, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 225, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 226, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 227, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 228, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 229, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 230, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 231, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 232, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 233, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 234, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 235, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 236, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 237, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 238, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 239, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 240, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 241, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 242, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 243, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 244, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 245, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 246, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 247, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 248, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 249, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 250, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 251, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 252, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 253, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 254, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 255, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 256, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 257, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 258, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 259, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 260, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 261, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 262, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 263, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 264, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 265, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 266, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 267, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 268, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 269, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 270, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 271, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 272, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 273, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 274, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 275, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 276, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 277, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 278, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 279, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 280, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 281, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 282, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 283, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 284, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 285, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 286, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 287, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 288, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 289, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 290, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 291, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 292, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 293, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 294, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 295, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 296, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 297, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 298, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 299, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 300, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 301, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 302, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 303, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 304, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 305, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 306, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 307, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 308, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 309, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 310, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 311, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 312, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 313, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 314, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 315, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 316, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 317, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 318, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 319, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 320, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 321, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 322, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 323, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 324, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 325, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 326, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 327, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 328, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 329, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 330, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 331, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 332, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 333, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 334, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 335, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 336, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 337, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 338, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 339, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 340, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 341, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 342, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 343, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 344, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 345, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 346, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 347, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 348, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 349, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 350, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 351, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 352, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 353, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 354, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 355, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 356, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 357, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 358, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 359, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 360, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 361, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 362, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 363, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 364, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 365, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 366, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 367, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 368, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 369, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 370, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 371, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 372, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 373, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 374, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 375, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 376, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 377, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 378, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 379, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 380, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 381, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 382, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 383, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 384, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 385, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 386, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 387, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 388, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 389, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 390, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 391, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 392, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 393, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 394, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 395, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 396, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 397, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 398, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 399, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 400, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 401, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 402, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 403, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 404, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 405, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 406, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 407, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 408, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 409, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 410, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 411, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 412, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 413, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 414, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 415, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 416, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 417, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 418, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 419, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 420, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 421, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 422, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 423, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 424, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 425, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 426, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 427, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 428, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 429, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 430, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 431, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 432, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 433, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 434, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 435, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 436, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 437, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 438, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 439, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 440, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 441, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 442, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 443, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 444, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 445, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 446, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 447, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 448, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 449, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 450, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 451, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 452, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 453, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 454, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 455, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 456, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 457, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 458, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 459, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 460, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 461, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 462, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 463, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 464, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 465, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 466, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 467, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 468, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 469, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 470, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 471, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 472, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 473, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 474, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 475, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 476, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 477, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 478, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 479, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 480, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 481, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 482, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 483, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 484, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 485, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 486, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 487, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 488, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 489, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 490, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 491, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 492, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 493, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 494, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 495, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 496, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 497, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 498, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 499, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 500, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 501, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 502, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 503, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 504, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 505, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 506, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 507, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 508, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 509, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 510, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 511, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 512, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 513, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 514, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 515, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 516, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 517, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 518, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 519, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 520, 26 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 271);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 272);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 273);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 274);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 275);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 276);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 277);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 278);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 279);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 280);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 281);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 282);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 283);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 284);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 285);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 286);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 287);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 288);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 289);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 290);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 291);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 292);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 293);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 294);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 295);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 296);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 297);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 298);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 299);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 300);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 301);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 302);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 303);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 304);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 305);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 306);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 307);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 308);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 309);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 310);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 311);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 312);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 313);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 314);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 315);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 316);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 317);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 318);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 319);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 320);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 321);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 322);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 323);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 324);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 325);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 326);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 327);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 328);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 329);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 330);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 331);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 332);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 333);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 334);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 335);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 336);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 337);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 338);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 339);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 340);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 341);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 342);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 343);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 344);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 345);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 346);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 347);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 348);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 349);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 350);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 351);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 352);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 353);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 354);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 355);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 356);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 357);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 358);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 359);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 360);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 361);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 362);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 363);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 364);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 365);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 366);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 367);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 368);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 369);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 370);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 371);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 372);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 373);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 374);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 375);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 376);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 377);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 378);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 379);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 380);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 381);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 382);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 383);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 384);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 385);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 386);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 387);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 388);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 389);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 390);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 391);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 392);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 393);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 394);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 395);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 396);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 397);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 398);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 399);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 400);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 401);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 402);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 403);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 404);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 405);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 406);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 407);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 408);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 409);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 410);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 411);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 412);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 413);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 414);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 415);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 416);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 417);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 418);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 419);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 420);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 421);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 422);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 423);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 424);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 425);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 426);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 427);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 428);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 429);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 430);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 431);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 432);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 433);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 434);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 435);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 436);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 437);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 438);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 439);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 440);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 441);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 442);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 443);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 444);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 445);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 446);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 447);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 448);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 449);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 450);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 451);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 452);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 453);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 454);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 455);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 456);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 457);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 458);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 459);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 460);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 461);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 462);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 463);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 464);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 465);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 466);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 467);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 468);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 469);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 470);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 471);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 472);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 473);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 474);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 475);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 476);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 477);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 478);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 479);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 480);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 481);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 482);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 483);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 484);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 485);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 486);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 487);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 488);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 489);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 490);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 491);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 492);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 493);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 494);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 495);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 496);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 497);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 498);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 499);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 500);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 501);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 502);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 503);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 504);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 505);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 506);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 507);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 508);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 509);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 510);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 511);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 512);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 513);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 514);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 515);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 516);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 517);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 518);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 519);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 520);

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "Sections");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionDetails",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePicturePath",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PresentVillage",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PresentThana",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PresentPostOffice",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PresentDistrict",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PermanentVillage",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PermanentThana",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PermanentPostOffice",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PermanentDistrict",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentSlipPath",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MotherOccupation",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GuardianOccupation",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GuardianName",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FatherOccupation",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BloodGroup",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BirthCertificatePath",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BirthCertificateNo",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicantEmail",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlternativeNumber",
                table: "AdmissionListResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code" },
                values: new object[] { "Create", true, false, "Dashboard.Create" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code" },
                values: new object[] { "Edit", false, true, "Dashboard.Edit" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code" },
                values: new object[] { "Delete", true, false, "Dashboard.Delete" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Action", "Code" },
                values: new object[] { "Approve", "Dashboard.Approve" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code" },
                values: new object[] { "Assign", false, true, "Dashboard.Assign" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Action", "Code" },
                values: new object[] { "Publish", "Dashboard.Publish" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code" },
                values: new object[] { "Export", true, false, "Dashboard.Export" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code" },
                values: new object[] { "Manage", true, true, true, "Dashboard.Manage" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "View", "Users.View", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Users.Create", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, false, true, "Users.Edit", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, false, false, "Users.Delete", "Users", "Users" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code" },
                values: new object[] { "Approve", false, true, "Users.Approve" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code" },
                values: new object[] { "Assign", false, true, "Users.Assign" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code" },
                values: new object[] { "Publish", false, true, "Users.Publish" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code" },
                values: new object[] { "Export", true, false, "Users.Export" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code" },
                values: new object[] { "Manage", true, true, true, "Users.Manage" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, true, "Roles.View", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Roles.Create", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Roles.Edit", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Roles.Delete", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Roles.Approve", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Roles.Assign", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, false, true, "Roles.Publish", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, false, false, "Roles.Export", "Roles", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code" },
                values: new object[] { "Manage", true, true, true, "Roles.Manage" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "View", "Permissions.View", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Permissions.Create", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Permissions.Edit", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Permissions.Delete", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Permissions.Approve", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Permissions.Assign", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Permissions.Publish", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Permissions.Export", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Permissions.Manage", "Permissions", "Permissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "View", "Admissions.View", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Action", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", false, "Admissions.Create", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, false, false, "Admissions.Edit", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Admissions.Delete", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Admissions.Approve", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Admissions.Assign", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Admissions.Publish", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Admissions.Export", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Admissions.Manage", "Admissions", "Admissions" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Students.View", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Students.Create", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Students.Edit", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Students.Delete", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Students.Approve", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, false, true, "Students.Assign", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, false, false, "Students.Publish", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", "Students.Export", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Students.Manage", "Students", "Students" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, true, "Teachers.View", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Teachers.Create", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Teachers.Edit", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Teachers.Delete", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Teachers.Approve", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Teachers.Assign", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Teachers.Publish", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Teachers.Export", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Teachers.Manage", "Teachers", "Teachers" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "Action", "CanCreate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, "Classes.View", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "Action", "CanDelete", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", false, false, false, "Classes.Create", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Classes.Edit", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Classes.Delete", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Classes.Approve", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Classes.Assign", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Classes.Publish", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, true, "Classes.Export", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Classes.Manage", "Classes", "Classes" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Sections.View", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Sections.Create", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Sections.Edit", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Sections.Delete", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, false, true, "Sections.Approve", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, false, false, "Sections.Assign", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Sections.Publish", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", "Sections.Export", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "Action", "CanDelete", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Sections.Manage", "Sections", "Sections" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Subjects.View", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Subjects.Create", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Subjects.Edit", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Subjects.Delete", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Subjects.Approve", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Subjects.Assign", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Subjects.Publish", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", "Subjects.Export", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, "Subjects.Manage", "Subjects", "Subjects" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, false, false, "Attendance.View", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Attendance.Create", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Attendance.Edit", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "Action", "CanCreate", "CanDelete", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, true, "Attendance.Delete", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Attendance.Approve", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 96,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Attendance.Assign", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 97,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Attendance.Publish", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Attendance.Export", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Attendance.Manage", "Attendance", "Attendance" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Exams.View", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Exams.Create", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Exams.Edit", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, true, false, "Exams.Delete", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, false, false, "Exams.Approve", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Exams.Assign", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Exams.Publish", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, true, "Exams.Export", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Exams.Manage", "Exams", "Exams" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Marks.View", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "Action", "CanCreate", "CanDelete", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Marks.Create", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Marks.Edit", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Marks.Delete", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Marks.Approve", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 114,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Marks.Assign", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 115,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Marks.Publish", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 116,
                columns: new[] { "Action", "CanCreate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, "Marks.Export", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 117,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Marks.Manage", "Marks", "Marks" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 118,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Assignments.View", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 119,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Assignments.Create", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Assignments.Edit", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Assignments.Delete", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Assignments.Approve", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Assignments.Assign", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 124,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Assignments.Publish", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 125,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Assignments.Export", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 126,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Assignments.Manage", "Assignments", "Assignments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 127,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "View", "Fees.View", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 128,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Fees.Create", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 129,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, false, true, "Fees.Edit", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, false, false, "Fees.Delete", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 131,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Fees.Approve", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 132,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Fees.Assign", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 133,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Fees.Publish", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 134,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Fees.Export", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 135,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Fees.Manage", "Fees", "Fees" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 136,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, true, "Payments.View", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 137,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Payments.Create", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 138,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Payments.Edit", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 139,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Payments.Delete", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 140,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Payments.Approve", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 141,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Payments.Assign", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 142,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, false, true, "Payments.Publish", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 143,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, false, false, "Payments.Export", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 144,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Payments.Manage", "Payments", "Payments" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 145,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "View", "Library.View", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 146,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Library.Create", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 147,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Library.Edit", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 148,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Library.Delete", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 149,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Library.Approve", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 150,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Library.Assign", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 151,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Library.Publish", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 152,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Library.Export", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 153,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Library.Manage", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 154,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "View", "Transport.View", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 155,
                columns: new[] { "Action", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", false, "Transport.Create", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 156,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, false, false, "Transport.Edit", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 157,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Transport.Delete", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 158,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Transport.Approve", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 159,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Transport.Assign", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 160,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Transport.Publish", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 161,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Transport.Export", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 162,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Transport.Manage", "Transport", "Transport" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 163,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Health.View", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 164,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Health.Create", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 165,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Health.Edit", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 166,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Health.Delete", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 167,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Health.Approve", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 168,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, false, true, "Health.Assign", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 169,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, false, false, "Health.Publish", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 170,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", "Health.Export", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 171,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Health.Manage", "Health", "Health" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 172,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, true, "Notifications.View", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 173,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Notifications.Create", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 174,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Notifications.Edit", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 175,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notifications.Delete", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 176,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notifications.Approve", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 177,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notifications.Assign", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 178,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notifications.Publish", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 179,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notifications.Export", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 180,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Notifications.Manage", "Notifications", "Notifications" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 181,
                columns: new[] { "Action", "CanCreate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, "Reports.View", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 182,
                columns: new[] { "Action", "CanDelete", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", false, false, false, "Reports.Create", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 183,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Reports.Edit", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 184,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Reports.Delete", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 185,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Reports.Approve", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 186,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Reports.Assign", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 187,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Reports.Publish", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 188,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, true, "Reports.Export", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 189,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Reports.Manage", "Reports", "Reports" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 190,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Settings.View", "Settings", "Settings" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 191,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Settings.Create", "Settings", "Settings" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 192,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Settings.Edit", "Settings", "Settings" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 193,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Settings.Delete", "Settings", "Settings" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 194,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, false, true, "Settings.Approve", "Settings", "Settings" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 195,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, false, false, "Settings.Assign", "Settings", "Settings" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 196,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Settings.Publish", "Settings", "Settings" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 197,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", "Settings.Export", "Settings", "Settings" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 198,
                columns: new[] { "Action", "CanDelete", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Settings.Manage", "Settings", "Settings" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 199,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Academic.View", "Academic", "Academic" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Academic.Create", "Academic", "Academic" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 201,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Academic.Edit", "Academic", "Academic" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 202,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Academic.Delete", "Academic", "Academic" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 203,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Academic.Approve", "Academic", "Academic" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 204,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Academic.Assign", "Academic", "Academic" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 205,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Academic.Publish", "Academic", "Academic" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 206,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", "Academic.Export", "Academic", "Academic" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 207,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, "Academic.Manage", "Academic", "Academic" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 208,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, false, false, "Admission.View", "Admission", "Admission" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 209,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Admission.Create", "Admission", "Admission" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 210,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Admission.Edit", "Admission", "Admission" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 211,
                columns: new[] { "Action", "CanCreate", "CanDelete", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, true, "Admission.Delete", "Admission", "Admission" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 212,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Admission.Approve", "Admission", "Admission" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 213,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "Admission.Assign", "Admission", "Admission" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 214,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Admission.Publish", "Admission", "Admission" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 215,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Admission.Export", "Admission", "Admission" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 216,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Admission.Manage", "Admission", "Admission" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 217,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Student.View", "Student", "Student" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 218,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Student.Create", "Student", "Student" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 219,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Student.Edit", "Student", "Student" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 220,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, true, false, "Student.Delete", "Student", "Student" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 221,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, false, false, "Student.Approve", "Student", "Student" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 222,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Student.Assign", "Student", "Student" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 223,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Student.Publish", "Student", "Student" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 224,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, true, "Student.Export", "Student", "Student" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 225,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Student.Manage", "Student", "Student" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 226,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", true, false, "Exam.View", "Exam", "Exam" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 227,
                columns: new[] { "Action", "CanCreate", "CanDelete", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Exam.Create", "Exam", "Exam" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 228,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "Exam.Edit", "Exam", "Exam" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 229,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Exam.Delete", "Exam", "Exam" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 230,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Exam.Approve", "Exam", "Exam" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 231,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Exam.Assign", "Exam", "Exam" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 232,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Exam.Publish", "Exam", "Exam" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 233,
                columns: new[] { "Action", "CanCreate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, "Exam.Export", "Exam", "Exam" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 234,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Exam.Manage", "Exam", "Exam" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 235,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Result.View", "Result", "Result" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 236,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Result.Create", "Result", "Result" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 237,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, true, "Result.Edit", "Result", "Result" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 238,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "Result.Delete", "Result", "Result" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 239,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", "Result.Approve", "Result", "Result" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 240,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Result.Assign", "Result", "Result" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 241,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "Result.Publish", "Result", "Result" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 242,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Result.Export", "Result", "Result" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 243,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Result.Manage", "Result", "Result" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 244,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "View", "Communication.View", "Communication", "Communication" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 245,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Communication.Create", "Communication", "Communication" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 246,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, false, true, "Communication.Edit", "Communication", "Communication" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 247,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, false, false, "Communication.Delete", "Communication", "Communication" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 248,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "Communication.Approve", "Communication", "Communication" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 249,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "Communication.Assign", "Communication", "Communication" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 250,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, true, "Communication.Publish", "Communication", "Communication" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 251,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "Communication.Export", "Communication", "Communication" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 252,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Communication.Manage", "Communication", "Communication" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 253,
                columns: new[] { "Action", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, true, "System.View", "System", "System" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 254,
                columns: new[] { "Action", "CanCreate", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "System.Create", "System", "System" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 255,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", "System.Edit", "System", "System" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 256,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "System.Delete", "System", "System" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 257,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "System.Approve", "System", "System" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 258,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", false, true, "System.Assign", "System", "System" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 259,
                columns: new[] { "Action", "CanCreate", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", false, false, true, "System.Publish", "System", "System" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 260,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", false, false, false, "System.Export", "System", "System" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 261,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "System.Manage", "System", "System" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 262,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "View", "AuditLogs.View", "AuditLogs", "AuditLogs" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 263,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "AuditLogs.Create", "AuditLogs", "AuditLogs" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 264,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "AuditLogs.Edit", "AuditLogs", "AuditLogs" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 265,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", true, false, "AuditLogs.Delete", "AuditLogs", "AuditLogs" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 266,
                columns: new[] { "Action", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Approve", false, true, "AuditLogs.Approve", "AuditLogs", "AuditLogs" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 267,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Assign", "AuditLogs.Assign", "AuditLogs", "AuditLogs" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 268,
                columns: new[] { "Action", "Code", "Module", "ModuleName" },
                values: new object[] { "Publish", "AuditLogs.Publish", "AuditLogs", "AuditLogs" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 269,
                columns: new[] { "Action", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Export", true, false, "AuditLogs.Export", "AuditLogs", "AuditLogs" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 270,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "AuditLogs.Manage", "AuditLogs", "AuditLogs" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 198, 2 },
                    { 199, 2 },
                    { 200, 2 },
                    { 201, 2 },
                    { 202, 2 },
                    { 203, 2 },
                    { 204, 2 },
                    { 205, 2 },
                    { 206, 2 },
                    { 207, 2 },
                    { 208, 2 },
                    { 37, 3 },
                    { 38, 3 },
                    { 39, 3 },
                    { 40, 3 },
                    { 41, 3 },
                    { 42, 3 },
                    { 43, 3 },
                    { 44, 3 },
                    { 45, 3 },
                    { 46, 3 },
                    { 47, 3 },
                    { 48, 3 },
                    { 49, 3 },
                    { 50, 3 },
                    { 51, 3 },
                    { 52, 3 },
                    { 79, 3 },
                    { 80, 3 },
                    { 81, 3 },
                    { 82, 3 },
                    { 83, 3 },
                    { 84, 3 },
                    { 85, 3 },
                    { 86, 3 },
                    { 87, 3 },
                    { 88, 3 },
                    { 89, 3 },
                    { 90, 3 },
                    { 91, 3 },
                    { 181, 3 },
                    { 182, 3 },
                    { 183, 3 },
                    { 184, 3 },
                    { 185, 3 },
                    { 186, 3 },
                    { 187, 3 },
                    { 188, 3 },
                    { 189, 3 },
                    { 199, 3 },
                    { 200, 3 },
                    { 201, 3 },
                    { 202, 3 },
                    { 203, 3 },
                    { 204, 3 },
                    { 205, 3 },
                    { 206, 3 },
                    { 207, 3 },
                    { 208, 3 },
                    { 209, 3 },
                    { 210, 3 },
                    { 211, 3 },
                    { 212, 3 },
                    { 213, 3 },
                    { 214, 3 },
                    { 215, 3 },
                    { 216, 3 },
                    { 217, 3 },
                    { 218, 3 },
                    { 219, 3 },
                    { 220, 3 },
                    { 221, 3 },
                    { 222, 3 },
                    { 223, 3 },
                    { 224, 3 },
                    { 225, 3 },
                    { 226, 3 },
                    { 227, 3 },
                    { 228, 3 },
                    { 229, 3 },
                    { 230, 3 },
                    { 231, 3 },
                    { 232, 3 },
                    { 233, 3 },
                    { 234, 3 },
                    { 235, 3 },
                    { 236, 3 },
                    { 237, 3 },
                    { 238, 3 },
                    { 239, 3 },
                    { 240, 3 },
                    { 241, 3 },
                    { 242, 3 },
                    { 243, 3 },
                    { 244, 3 },
                    { 245, 3 },
                    { 246, 3 },
                    { 247, 3 },
                    { 248, 3 },
                    { 249, 3 },
                    { 250, 3 },
                    { 251, 3 },
                    { 252, 3 },
                    { 46, 5 },
                    { 64, 5 },
                    { 91, 5 },
                    { 100, 5 },
                    { 109, 5 },
                    { 110, 5 },
                    { 118, 5 },
                    { 119, 5 },
                    { 181, 5 },
                    { 226, 5 },
                    { 6, 6 },
                    { 37, 6 },
                    { 38, 6 },
                    { 39, 6 },
                    { 41, 6 },
                    { 42, 6 },
                    { 43, 6 },
                    { 44, 6 },
                    { 45, 6 },
                    { 46, 6 },
                    { 47, 6 },
                    { 48, 6 },
                    { 50, 6 },
                    { 51, 6 },
                    { 52, 6 },
                    { 127, 6 },
                    { 128, 6 },
                    { 129, 6 },
                    { 131, 6 },
                    { 132, 6 },
                    { 133, 6 },
                    { 134, 6 },
                    { 135, 6 },
                    { 136, 6 },
                    { 137, 6 },
                    { 138, 6 },
                    { 140, 6 },
                    { 141, 6 },
                    { 142, 6 },
                    { 143, 6 },
                    { 144, 6 },
                    { 181, 6 },
                    { 182, 6 },
                    { 188, 6 },
                    { 209, 6 },
                    { 210, 6 },
                    { 212, 6 },
                    { 213, 6 },
                    { 214, 6 },
                    { 215, 6 },
                    { 216, 6 },
                    { 217, 6 },
                    { 218, 6 },
                    { 219, 6 },
                    { 221, 6 },
                    { 222, 6 },
                    { 223, 6 },
                    { 224, 6 },
                    { 225, 6 },
                    { 46, 7 },
                    { 91, 7 },
                    { 109, 7 },
                    { 118, 7 },
                    { 119, 7 },
                    { 127, 7 },
                    { 217, 7 }
                });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                column: "NameBn",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "ইংরেজি", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "গণিত", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "সাধারণ বিজ্ঞান", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "বাংলাদেশ ও বিশ্ব পরিচয়", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "ধর্ম ও নৈতিক শিক্ষা", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "চারুকলা", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "শারীরিক শিক্ষা", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "বাংলা ১ম পত্র", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "বাংলা ২য় পত্র", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "ইংরেজি ১ম পত্র", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "ইংরেজি ২য় পত্র", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "বিজ্ঞান", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "তথ্য ও যোগাযোগ প্রযুক্তি", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "কৃষি শিক্ষা", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "পদার্থবিজ্ঞান", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "রসায়ন", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "জীববিজ্ঞান", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "উচ্চতর গণিত", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "হিসাববিজ্ঞান", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "ফাইন্যান্স", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "ব্যবসায় উদ্যোগ", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "ইতিহাস", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "ভূগোল ও পরিবেশ", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "অর্থনীতি", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Name", "NameBn", "SubjectGroup" },
                values: new object[] { "নাগরিকতা", "", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "ক্যারিয়ার শিক্ষা", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "শারীরিক শিক্ষা, স্বাস্থ্য ও খেলাধুলা", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "গার্হস্থ্য বিজ্ঞান", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "ইসলাম ও নৈতিক শিক্ষা", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "হিন্দুধর্ম ও নৈতিক শিক্ষা", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "বৌদ্ধধর্ম ও নৈতিক শিক্ষা", "" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Name", "NameBn" },
                values: new object[] { "খ্রিস্টধর্ম ও নৈতিক শিক্ষা", "" });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_SchoolClassId",
                table: "Sections",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendances_EmployeeId",
                table: "EmployeeAttendances",
                column: "EmployeeId");
        }
    }
}
