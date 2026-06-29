using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmissionFinanceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DevelopmentFee",
                table: "AdmissionFeeStructures",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LaboratoryFee",
                table: "AdmissionFeeStructures",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LibraryFee",
                table: "AdmissionFeeStructures",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RegistrationFee",
                table: "AdmissionFeeStructures",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DevelopmentFee",
                table: "AdmissionFeeStructures");

            migrationBuilder.DropColumn(
                name: "LaboratoryFee",
                table: "AdmissionFeeStructures");

            migrationBuilder.DropColumn(
                name: "LibraryFee",
                table: "AdmissionFeeStructures");

            migrationBuilder.DropColumn(
                name: "RegistrationFee",
                table: "AdmissionFeeStructures");
        }
    }
}
