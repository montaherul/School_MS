using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentSectionId",
                table: "Sections",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Capacity", "Name", "ParentSectionId" },
                values: new object[] { 50, "Science", null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Capacity", "Name", "ParentSectionId" },
                values: new object[] { 50, "Science A", 17 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Capacity", "Name", "ParentSectionId" },
                values: new object[] { 50, "Science B", 17 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Capacity", "ParentSectionId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Capacity", "Name", "ParentSectionId" },
                values: new object[] { 50, "Business Studies A", 20 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Capacity", "Name", "ParentSectionId", "SchoolClassId" },
                values: new object[] { 50, "Business Studies B", 20, 9 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Capacity", "Name", "ParentSectionId", "SchoolClassId" },
                values: new object[] { 50, "Humanities", null, 9 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Capacity", "Name", "ParentSectionId", "SchoolClassId" },
                values: new object[] { 50, "Humanities A", 23, 9 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Capacity", "Name", "ParentSectionId", "SchoolClassId" },
                values: new object[] { 50, "Humanities B", 23, 9 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Capacity", "Name", "ParentSectionId" },
                values: new object[] { 50, "Science", null });

            migrationBuilder.InsertData(
                table: "Sections",
                columns: new[] { "Id", "Capacity", "CreatedAt", "CreatedBy", "IsDeleted", "Name", "ParentSectionId", "SchoolClassId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 27, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Science A", 26, 10, null, null },
                    { 28, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Science B", 26, 10, null, null },
                    { 29, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies", null, 10, null, null },
                    { 32, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities", null, 10, null, null },
                    { 30, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies A", 29, 10, null, null },
                    { 31, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies B", 29, 10, null, null },
                    { 33, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities A", 32, 10, null, null },
                    { 34, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities B", 32, 10, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_ParentSectionId",
                table: "Sections",
                column: "ParentSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Sections_ParentSectionId",
                table: "Sections",
                column: "ParentSectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Sections_ParentSectionId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_ParentSectionId",
                table: "Sections");

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ParentSectionId",
                table: "Sections");

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 17,
                column: "Name",
                value: "A");

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 18,
                column: "Name",
                value: "B");

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 19,
                column: "Name",
                value: "Science");

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "Humanities");

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Name", "SchoolClassId" },
                values: new object[] { "A", 10 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Name", "SchoolClassId" },
                values: new object[] { "B", 10 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Name", "SchoolClassId" },
                values: new object[] { "Science", 10 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Name", "SchoolClassId" },
                values: new object[] { "Business Studies", 10 });

            migrationBuilder.UpdateData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 26,
                column: "Name",
                value: "Humanities");
        }
    }
}
