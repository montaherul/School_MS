using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTeacherToEmployeeExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Users_UserId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_TeacherNo",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_UserId",
                table: "Teachers");



            migrationBuilder.DropColumn(
                name: "AlternativeNumber",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "FullNameBangla",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "JoiningDate",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "MotherName",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "NationalIdNo",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "NationalIdPath",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PassportNo",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PassportPath",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PermanentDistrict",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PermanentPostOffice",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PermanentThana",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PermanentVillage",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PresentPostOffice",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PresentThana",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PresentVillage",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "ProfilePicturePath",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Qualification",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Religion",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "SpouseName",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "TeacherNo",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PresentDistrict",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "Teachers");

            migrationBuilder.AddColumn<int>(
                name: "TeachingExperienceYears",
                table: "Teachers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubjectSpecialization",
                table: "Teachers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingLevel",
                table: "Teachers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherCode",
                table: "Teachers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Dynamically assign unique migration codes to existing teacher rows
            migrationBuilder.Sql("UPDATE Teachers SET TeacherCode = 'MIG-' + CAST(Id AS VARCHAR(10))");

            migrationBuilder.AlterColumn<int>(
              name: "EmployeeId",
              table: "Teachers",
              type: "int",
              nullable: true,
              oldClrType: typeof(int),
              oldType: "int",
              oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClassTeacher",
                table: "Teachers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsExamController",
                table: "Teachers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRoutineCoordinator",
                table: "Teachers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Teachers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TeacherCode",
                table: "Teachers",
                column: "TeacherCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teachers_TeacherCode",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "IsClassTeacher",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "IsExamController",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "IsRoutineCoordinator",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Teachers");

            migrationBuilder.RenameColumn(
                name: "TeachingLevel",
                table: "Teachers",
                newName: "PresentDistrict");

            migrationBuilder.RenameColumn(
                name: "TeachingExperienceYears",
                table: "Teachers",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TeacherCode",
                table: "Teachers",
                newName: "Nationality");

            migrationBuilder.RenameColumn(
                name: "SubjectSpecialization",
                table: "Teachers",
                newName: "Specialization");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "Teachers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AlternativeNumber",
                table: "Teachers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "Teachers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Teachers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Teachers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Teachers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Teachers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Teachers",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "Teachers",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Teachers",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullNameBangla",
                table: "Teachers",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Teachers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "JoiningDate",
                table: "Teachers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "Teachers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Teachers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MotherName",
                table: "Teachers",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalIdNo",
                table: "Teachers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalIdPath",
                table: "Teachers",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNo",
                table: "Teachers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportPath",
                table: "Teachers",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentDistrict",
                table: "Teachers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentPostOffice",
                table: "Teachers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentThana",
                table: "Teachers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentVillage",
                table: "Teachers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentPostOffice",
                table: "Teachers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentThana",
                table: "Teachers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentVillage",
                table: "Teachers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "Teachers",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Qualification",
                table: "Teachers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Religion",
                table: "Teachers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpouseName",
                table: "Teachers",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherNo",
                table: "Teachers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Teachers",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "AlternativeNumber", "BloodGroup", "Country", "CreatedAt", "CreatedBy", "DateOfBirth", "Department", "Designation", "EmailAddress", "EmployeeId", "FatherName", "FullName", "FullNameBangla", "Gender", "IsDeleted", "JoiningDate", "MaritalStatus", "MobileNumber", "MotherName", "NationalIdNo", "NationalIdPath", "Nationality", "PassportNo", "PassportPath", "PermanentDistrict", "PermanentPostOffice", "PermanentThana", "PermanentVillage", "PresentDistrict", "PresentPostOffice", "PresentThana", "PresentVillage", "ProfilePicturePath", "Qualification", "Religion", "Specialization", "SpouseName", "Status", "TeacherNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { 1, null, null, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Senior Lecturer", null, null, null, "Senior Lecturer", null, "", false, null, "", "01000000001", null, null, null, "Bangladeshi", null, null, null, null, null, null, null, null, null, null, null, null, "", null, null, 1, "T-0001", null, null, null },
                    { 2, null, null, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Lecturer", null, null, null, "Class Teacher", null, "", false, null, "", "01000000002", null, null, null, "Bangladeshi", null, null, null, null, null, null, null, null, null, null, null, null, "", null, null, 1, "T-0002", null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TeacherNo",
                table: "Teachers",
                column: "TeacherNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_UserId",
                table: "Teachers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Users_UserId",
                table: "Teachers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
