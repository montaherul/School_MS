using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingInvitationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "EmployeeInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvitationCode",
                table: "EmployeeInvitations",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedAt",
                table: "EmployeeInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "EmployeeInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OnboardedAt",
                table: "EmployeeInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedEmployeeId",
                table: "EmployeeInvitations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "EmployeeInvitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "EmployeeInvitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InvitationStatus",
                table: "EmployeeInvitations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Started");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeInvitations_InvitationCode",
                table: "EmployeeInvitations");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeInvitations_InvitationToken",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "InvitationCode",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "OpenedAt",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "OnboardedAt",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "CreatedEmployeeId",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "InvitationStatus",
                table: "EmployeeInvitations");
        }
    }
}
