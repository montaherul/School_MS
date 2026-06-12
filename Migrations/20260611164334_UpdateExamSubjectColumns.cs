using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExamSubjectColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ExamDate",
                table: "ExamSubjects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExamDuration",
                table: "ExamSubjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ExamStartTime",
                table: "ExamSubjects",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ExamSubjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RoomNumber",
                table: "ExamSubjects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "ExamSubjects",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubjects_TeacherId",
                table: "ExamSubjects",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubjects_Teachers_TeacherId",
                table: "ExamSubjects",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubjects_Teachers_TeacherId",
                table: "ExamSubjects");

            migrationBuilder.DropIndex(
                name: "IX_ExamSubjects_TeacherId",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "ExamDate",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "ExamDuration",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "ExamStartTime",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "ExamSubjects");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "ExamSubjects");

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
        }
    }
}
