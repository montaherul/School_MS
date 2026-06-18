using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowResultWithDueToSchoolSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowResultWithDue",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "FeeWaivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "FeeWaivers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "FeeRefunds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "FeeRefunds",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowResultWithDue",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "FeeWaivers");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "FeeWaivers");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "FeeRefunds");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "FeeRefunds");
        }
    }
}
