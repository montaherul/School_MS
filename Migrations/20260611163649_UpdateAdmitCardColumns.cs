using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdmitCardColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdmitCardNumber",
                table: "AdmitCards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsIssued",
                table: "AdmitCards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssuedAt",
                table: "AdmitCards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RollNumber",
                table: "AdmitCards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeatNumber",
                table: "AdmitCards",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmitCards_AdmitCardNumber",
                table: "AdmitCards",
                column: "AdmitCardNumber",
                unique: true,
                filter: "[IsDeleted] = 0 AND [AdmitCardNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AdmitCards_ExamId",
                table: "AdmitCards",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmitCards_StudentId",
                table: "AdmitCards",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmitCards_Exams_ExamId",
                table: "AdmitCards",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdmitCards_Students_StudentId",
                table: "AdmitCards",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdmitCards_Exams_ExamId",
                table: "AdmitCards");

            migrationBuilder.DropForeignKey(
                name: "FK_AdmitCards_Students_StudentId",
                table: "AdmitCards");

            migrationBuilder.DropIndex(
                name: "IX_AdmitCards_AdmitCardNumber",
                table: "AdmitCards");

            migrationBuilder.DropIndex(
                name: "IX_AdmitCards_ExamId",
                table: "AdmitCards");

            migrationBuilder.DropIndex(
                name: "IX_AdmitCards_StudentId",
                table: "AdmitCards");

            migrationBuilder.DropColumn(
                name: "AdmitCardNumber",
                table: "AdmitCards");

            migrationBuilder.DropColumn(
                name: "IsIssued",
                table: "AdmitCards");

            migrationBuilder.DropColumn(
                name: "IssuedAt",
                table: "AdmitCards");

            migrationBuilder.DropColumn(
                name: "RollNumber",
                table: "AdmitCards");

            migrationBuilder.DropColumn(
                name: "SeatNumber",
                table: "AdmitCards");
        }
    }
}
