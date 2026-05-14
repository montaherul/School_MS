using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRollNumberAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RollNumberAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FromClassId = table.Column<int>(type: "int", nullable: false),
                    ToClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    RollNumber = table.Column<int>(type: "int", nullable: false),
                    MeritPosition = table.Column<int>(type: "int", nullable: false),
                    MeritValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GeneratedByUserId = table.Column<int>(type: "int", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RollNumberAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RollNumberAssignments_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RollNumberAssignments_Classes_FromClassId",
                        column: x => x.FromClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RollNumberAssignments_Classes_ToClassId",
                        column: x => x.ToClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RollNumberAssignments_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RollNumberAssignments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RollNumberAssignments_AcademicYearId_StudentId_ToClassId",
                table: "RollNumberAssignments",
                columns: new[] { "AcademicYearId", "StudentId", "ToClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RollNumberAssignments_FromClassId",
                table: "RollNumberAssignments",
                column: "FromClassId");

            migrationBuilder.CreateIndex(
                name: "IX_RollNumberAssignments_SectionId",
                table: "RollNumberAssignments",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_RollNumberAssignments_StudentId",
                table: "RollNumberAssignments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_RollNumberAssignments_ToClassId",
                table: "RollNumberAssignments",
                column: "ToClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RollNumberAssignments");
        }
    }
}
