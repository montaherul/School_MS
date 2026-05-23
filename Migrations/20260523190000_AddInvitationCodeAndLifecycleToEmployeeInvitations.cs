using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    public partial class AddInvitationCodeAndLifecycleToEmployeeInvitations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvitationCode",
                table: "EmployeeInvitations",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "EmployeeInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedAt",
                table: "EmployeeInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "EmployeeInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE EmployeeInvitations
SET InvitationCode = CONCAT('INV-', YEAR(CreatedAt), '-', RIGHT('0000' + CAST(Id AS varchar(10)), 4))
WHERE InvitationCode = '' OR InvitationCode IS NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInvitations_InvitationCode",
                table: "EmployeeInvitations",
                column: "InvitationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInvitations_InvitationToken",
                table: "EmployeeInvitations",
                column: "InvitationToken",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeInvitations_InvitationCode",
                table: "EmployeeInvitations");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeInvitations_InvitationToken",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "InvitationCode",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "OpenedAt",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "EmployeeInvitations");
        }
    }
}
