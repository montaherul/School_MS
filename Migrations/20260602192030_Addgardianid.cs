using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Addgardianid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guardians_Students_StudentId",
                table: "Guardians");

            migrationBuilder.DropIndex(
                name: "IX_Guardians_StudentId",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "Relation",
                table: "Guardians");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Guardians",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Guardians",
                newName: "MobileNumber");

            migrationBuilder.AddColumn<string>(
                name: "AlternativeMobileNumber",
                table: "Guardians",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Guardians",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "Guardians",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactNumber",
                table: "Guardians",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployerName",
                table: "Guardians",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Guardians",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Guardians",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Guardians",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GuardianCode",
                table: "Guardians",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: null);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimaryGuardian",
                table: "Guardians",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Guardians",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyIncome",
                table: "Guardians",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "Guardians",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "Guardians",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentAddress",
                table: "Guardians",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Guardians",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PortalAccessEnabled",
                table: "Guardians",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PresentAddress",
                table: "Guardians",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RelationType",
                table: "Guardians",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Guardians",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Guardians",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GuardianId",
                table: "AttendanceNotificationLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LinkedGuardianId",
                table: "Admissions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GuardianNotificationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuardianId = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuardianNotificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuardianNotificationLogs_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuardianNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuardianId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuardianNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuardianNotifications_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentGuardians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    GuardianId = table.Column<int>(type: "int", nullable: false),
                    Relationship = table.Column<int>(type: "int", nullable: false),
                    IsPrimaryGuardian = table.Column<bool>(type: "bit", nullable: false),
                    ReceivesAttendanceNotifications = table.Column<bool>(type: "bit", nullable: false),
                    ReceivesResultNotifications = table.Column<bool>(type: "bit", nullable: false),
                    ReceivesFeeNotifications = table.Column<bool>(type: "bit", nullable: false),
                    ReceivesSMS = table.Column<bool>(type: "bit", nullable: false),
                    ReceivesEmail = table.Column<bool>(type: "bit", nullable: false),
                    ReceivesWhatsApp = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGuardians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentGuardians_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGuardians_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentLeaveApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    GuardianId = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLeaveApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentLeaveApplications_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLeaveApplications_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLeaveApplications_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Admissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LinkedGuardianId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Guardians",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AlternativeMobileNumber", "DateOfBirth", "EmergencyContactName", "EmergencyContactNumber", "EmployerName", "FirstName", "FullName", "Gender", "GuardianCode", "IsPrimaryGuardian", "LastName", "MonthlyIncome", "NationalId", "PassportNumber", "PermanentAddress", "PhotoPath", "PortalAccessEnabled", "PresentAddress", "RelationType", "Remarks", "UserId" },
                values: new object[] { null, null, null, null, null, "Guardian", "Guardian One", "Male", "GRD-00001", false, "One", null, null, null, null, null, false, null, 1, null, null });

            migrationBuilder.UpdateData(
                table: "Guardians",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AlternativeMobileNumber", "DateOfBirth", "EmergencyContactName", "EmergencyContactNumber", "EmployerName", "FirstName", "FullName", "Gender", "GuardianCode", "IsPrimaryGuardian", "LastName", "MonthlyIncome", "NationalId", "PassportNumber", "PermanentAddress", "PhotoPath", "PortalAccessEnabled", "PresentAddress", "RelationType", "Remarks", "Status", "UserId" },
                values: new object[] { null, null, null, null, null, "Guardian", "Guardian Two", "Female", "GRD-00002", false, "Two", null, null, null, null, null, false, null, 2, null, 1, null });

            migrationBuilder.InsertData(
                table: "StudentGuardians",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "GuardianId", "IsDeleted", "IsPrimaryGuardian", "ReceivesAttendanceNotifications", "ReceivesEmail", "ReceivesFeeNotifications", "ReceivesResultNotifications", "ReceivesSMS", "ReceivesWhatsApp", "Relationship", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 1, false, true, true, true, true, true, true, false, 1, 1, null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 2, false, true, true, true, true, true, true, false, 2, 2, null, null }
                });

            // Ensure GuardianCode is populated and unique
            migrationBuilder.Sql("UPDATE Guardians SET GuardianCode = 'GRD-' + RIGHT('00000' + CAST(Id AS varchar(5)),5) WHERE GuardianCode IS NULL OR GuardianCode = ''");
            migrationBuilder.AlterColumn<string>(
                name: "GuardianCode",
                table: "Guardians",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);
            migrationBuilder.CreateIndex(
                name: "IX_Guardians_GuardianCode",
                table: "Guardians",
                column: "GuardianCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_MobileNumber",
                table: "Guardians",
                column: "MobileNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceNotificationLogs_GuardianId",
                table: "AttendanceNotificationLogs",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_LinkedGuardianId",
                table: "Admissions",
                column: "LinkedGuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_GuardianNotificationLogs_GuardianId",
                table: "GuardianNotificationLogs",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_GuardianNotifications_GuardianId",
                table: "GuardianNotifications",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGuardians_GuardianId",
                table: "StudentGuardians",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGuardians_StudentId_GuardianId",
                table: "StudentGuardians",
                columns: new[] { "StudentId", "GuardianId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentLeaveApplications_GuardianId",
                table: "StudentLeaveApplications",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLeaveApplications_LeaveTypeId",
                table: "StudentLeaveApplications",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLeaveApplications_StudentId",
                table: "StudentLeaveApplications",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Guardians_LinkedGuardianId",
                table: "Admissions",
                column: "LinkedGuardianId",
                principalTable: "Guardians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceNotificationLogs_Guardians_GuardianId",
                table: "AttendanceNotificationLogs",
                column: "GuardianId",
                principalTable: "Guardians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Guardians_LinkedGuardianId",
                table: "Admissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceNotificationLogs_Guardians_GuardianId",
                table: "AttendanceNotificationLogs");

            migrationBuilder.DropTable(
                name: "GuardianNotificationLogs");

            migrationBuilder.DropTable(
                name: "GuardianNotifications");

            migrationBuilder.DropTable(
                name: "StudentGuardians");

            migrationBuilder.DropTable(
                name: "StudentLeaveApplications");

            migrationBuilder.DropIndex(
                name: "IX_Guardians_GuardianCode",
                table: "Guardians");

            migrationBuilder.DropIndex(
                name: "IX_Guardians_MobileNumber",
                table: "Guardians");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceNotificationLogs_GuardianId",
                table: "AttendanceNotificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_LinkedGuardianId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "AlternativeMobileNumber",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "EmergencyContactNumber",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "EmployerName",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "GuardianCode",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "IsPrimaryGuardian",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "MonthlyIncome",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "PermanentAddress",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "PortalAccessEnabled",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "PresentAddress",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "RelationType",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "GuardianId",
                table: "AttendanceNotificationLogs");

            migrationBuilder.DropColumn(
                name: "LinkedGuardianId",
                table: "Admissions");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Guardians",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "MobileNumber",
                table: "Guardians",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Guardians",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Relation",
                table: "Guardians",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Guardians",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Relation" },
                values: new object[] { "Guardian One", "Father" });

            migrationBuilder.UpdateData(
                table: "Guardians",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Relation", "StudentId" },
                values: new object[] { "Guardian Two", "Mother", 2 });

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_StudentId",
                table: "Guardians",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guardians_Students_StudentId",
                table: "Guardians",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
