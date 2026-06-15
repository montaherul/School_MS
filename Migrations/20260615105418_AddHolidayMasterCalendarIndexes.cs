using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddHolidayMasterCalendarIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AcademicCalendars_AcademicYearId",
                table: "AcademicCalendars");

            migrationBuilder.CreateTable(
                name: "EmployeeIdCardListResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTeachingStaff = table.Column<bool>(type: "bit", nullable: false),
                    EmploymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardPrintedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardVersion = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesignationName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "HolidayMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameBn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HolidayType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HolidayDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    Religion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentIdCardListResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    StudentCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RollNumber = table.Column<int>(type: "int", nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuardianName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdmissionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_AcademicYearId_Date",
                table: "AcademicCalendars",
                columns: new[] { "AcademicYearId", "Date" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_Date_IsEventDay",
                table: "AcademicCalendars",
                columns: new[] { "Date", "IsEventDay" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_Date_IsExamDay",
                table: "AcademicCalendars",
                columns: new[] { "Date", "IsExamDay" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_Date_IsHoliday",
                table: "AcademicCalendars",
                columns: new[] { "Date", "IsHoliday" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayMasters_HolidayDate",
                table: "HolidayMasters",
                column: "HolidayDate");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayMasters_Name_HolidayDate",
                table: "HolidayMasters",
                columns: new[] { "Name", "HolidayDate" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeIdCardListResults");

            migrationBuilder.DropTable(
                name: "HolidayMasters");

            migrationBuilder.DropTable(
                name: "StudentIdCardListResults");

            migrationBuilder.DropIndex(
                name: "IX_AcademicCalendars_AcademicYearId_Date",
                table: "AcademicCalendars");

            migrationBuilder.DropIndex(
                name: "IX_AcademicCalendars_Date_IsEventDay",
                table: "AcademicCalendars");

            migrationBuilder.DropIndex(
                name: "IX_AcademicCalendars_Date_IsExamDay",
                table: "AcademicCalendars");

            migrationBuilder.DropIndex(
                name: "IX_AcademicCalendars_Date_IsHoliday",
                table: "AcademicCalendars");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_AcademicYearId",
                table: "AcademicCalendars",
                column: "AcademicYearId");
        }
    }
}
