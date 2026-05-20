using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StartsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionListResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ApplicationNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantNameBangla = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppliedClassId = table.Column<int>(type: "int", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantMobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherOrGuardianMobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlternativeNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherOccupation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotherName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotherOccupation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuardianName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuardianOccupation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Religion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthCertificateNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthCertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentSlipPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PresentVillage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PresentPostOffice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PresentThana = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PresentDistrict = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentVillage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentPostOffice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentThana = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentDistrict = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicturePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Admissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ApplicantNameBangla = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FatherOccupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MotherName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    MotherOccupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GuardianName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    GuardianOccupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FatherOrGuardianMobileNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApplicantMobileNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AlternativeNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ApplicantEmail = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaritalStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Religion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BirthCertificateNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthCertificatePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionDetails = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PaymentSlipPath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    PresentVillage = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PresentPostOffice = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PresentThana = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PresentDistrict = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PermanentVillage = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PermanentPostOffice = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PermanentThana = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PermanentDistrict = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AppliedClassId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AdmissionFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AdmissionFeePaid = table.Column<bool>(type: "bit", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByUserId = table.Column<int>(type: "int", nullable: true),
                    ProfilePicturePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdmitCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CardNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    PrintedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsGenerated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmitCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    TeacherProfileId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodNo = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackupRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    BackupAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Restored = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookIssues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReturnedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FineAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookIssues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ReservedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookReservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessionNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TotalCopies = table.Column<int>(type: "int", nullable: false),
                    AvailableCopies = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Circulars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Circulars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleLevel = table.Column<int>(type: "int", nullable: false),
                    IsTeachingRole = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LicenseNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Term = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    StartsOn = table.Column<DateOnly>(type: "date", nullable: false),
                    EndsOn = table.Column<DateOnly>(type: "date", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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
                    table.PrimaryKey("PK_ExamTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeeInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeInvoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeeStructures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    FeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeStructures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FineRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GraceDays = table.Column<int>(type: "int", nullable: false),
                    FinePerDay = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FineRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GpaConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MinMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GradePoint = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_GpaConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GradingRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MinMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GradePoint = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_GradingRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ToDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Allergies = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageThreadId = table.Column<int>(type: "int", nullable: false),
                    SenderUserId = table.Column<int>(type: "int", nullable: false),
                    ReceiverUserId = table.Column<int>(type: "int", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageThreads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageThreads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    AudienceRole = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PublishAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Module = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanRead = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeatingPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SeatNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    HallNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BlockNo = table.Column<int>(type: "int", nullable: true),
                    RowNo = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatingPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MinClass = table.Column<int>(type: "int", nullable: false),
                    MaxClass = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_StudentGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentListItemResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    StudentNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullNameBangla = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RollNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherOccupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MotherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MotherOccupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresentVillage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresentPostOffice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresentThana = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresentDistrict = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermanentVillage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermanentPostOffice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermanentThana = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermanentDistrict = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Religion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthCertificateNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherOrGuardianMobileNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePicturePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalRecords = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "StudentPromotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FromClassId = table.Column<int>(type: "int", nullable: false),
                    ToClassId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    PromotedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentPromotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentRouteAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TransportRouteId = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRouteAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudyMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ResourceUrl = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyMaterials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameBn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubjectGroup = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    IsReligionSubject = table.Column<bool>(type: "bit", nullable: false),
                    IsPractical = table.Column<bool>(type: "bit", nullable: false),
                    HasWritten = table.Column<bool>(type: "bit", nullable: false),
                    HasMCQ = table.Column<bool>(type: "bit", nullable: false),
                    HasCQ = table.Column<bool>(type: "bit", nullable: false),
                    HasPractical = table.Column<bool>(type: "bit", nullable: false),
                    HasLab = table.Column<bool>(type: "bit", nullable: false),
                    HasViva = table.Column<bool>(type: "bit", nullable: false),
                    HasOral = table.Column<bool>(type: "bit", nullable: false),
                    HasAssignment = table.Column<bool>(type: "bit", nullable: false),
                    HasContinuousAssessment = table.Column<bool>(type: "bit", nullable: false),
                    ReligionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DefaultFullMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DefaultPassMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Syllabi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Syllabi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransferCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CertificateNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferCertificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PickupDropSchedule = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportRoutes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ActivationToken = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ActivationTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    LockoutUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaccinationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    VaccineName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    VaccinatedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    NextDueOn = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccinationRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionApplicationId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionDocuments_Admissions_AdmissionApplicationId",
                        column: x => x.AdmissionApplicationId,
                        principalTable: "Admissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentTaskId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Marks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentSubmissions_Assignments_AssignmentTaskId",
                        column: x => x.AssignmentTaskId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ParentSectionId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sections_Sections_ParentSectionId",
                        column: x => x.ParentSectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultLocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    LockedByUserId = table.Column<int>(type: "int", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    CanUnlock = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultLocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultLocks_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultPublications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublicationNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultPublications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultPublications_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamTypeId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExamWeightage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_ExamConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamConfigurations_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamConfigurations_ExamTypes_ExamTypeId",
                        column: x => x.ExamTypeId,
                        principalTable: "ExamTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeeInvoiceId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_FeeInvoices_FeeInvoiceId",
                        column: x => x.FeeInvoiceId,
                        principalTable: "FeeInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    ExamDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartsAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndsAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    RoomNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSchedules_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSchedules_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    FullMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PassMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    WrittenMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MCQMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PracticalMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    VivaMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LabMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ContinuousAssessmentMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OralMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AssignmentMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSubjects_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Module = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Module = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    MotherName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Religion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NIDNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthCertificateNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    PresentAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PermanentAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DesignationId = table.Column<int>(type: "int", nullable: false),
                    EmployeeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsTeachingStaff = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ProfilePicturePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    SignaturePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Otp = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LoginAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogoutAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    StudentGroupId = table.Column<int>(type: "int", nullable: true),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    FullMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PassMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    WrittenMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MCQMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CQMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PracticalMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    VivaMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LabMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OralMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AssignmentMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ContinuousAssessmentMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CompetencyMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    BehaviourMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ParticipationMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    IsGroupSubject = table.Column<bool>(type: "bit", nullable: false),
                    IsReligionSubject = table.Column<bool>(type: "bit", nullable: false),
                    ReligionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_ClassSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSubjects_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassSubjects_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassSubjects_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FullNameBangla = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FatherOccupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MotherName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    MotherOccupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AlternativeNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaritalStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Religion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BirthCertificateNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProfilePicturePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    PresentVillage = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PresentPostOffice = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PresentThana = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PresentDistrict = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PermanentVillage = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PermanentPostOffice = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PermanentThana = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PermanentDistrict = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    RollNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    AssignedReligionSubjectId = table.Column<int>(type: "int", nullable: true),
                    StudentGroupId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Subjects_AssignedReligionSubjectId",
                        column: x => x.AssignedReligionSubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAcademicAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAcademicAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAcademicAssignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckInTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CheckOutTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAttendances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDocuments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeExperiences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeExperiences_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLeaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLeaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaves_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaves_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeQualifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ExamName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BoardOrUniversity = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    InstituteName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    GroupOrSubject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PassingYear = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Result = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CGPAOrDivision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CertificateFilePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeQualifications_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HouseRent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MedicalAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransportAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OtherAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Deduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FullNameBangla = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AlternativeNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaritalStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Religion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PassportNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PassportPath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    NationalIdNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NationalIdPath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Specialization = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    MotherName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    SpouseName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    PresentVillage = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PresentPostOffice = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PresentThana = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PresentDistrict = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PermanentVillage = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PermanentPostOffice = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PermanentThana = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PermanentDistrict = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProfilePicturePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teachers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassSubjectId = table.Column<int>(type: "int", nullable: false),
                    ComponentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectComponents_ClassSubjects_ClassSubjectId",
                        column: x => x.ClassSubjectId,
                        principalTable: "ClassSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinalResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    FinalGpa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalPosition = table.Column<int>(type: "int", nullable: false),
                    FinalClassPosition = table.Column<int>(type: "int", nullable: false),
                    FinalGrade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PromotionStatus = table.Column<int>(type: "int", nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    TotalFailedSubjects = table.Column<int>(type: "int", nullable: false),
                    PromotionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinalResults_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinalResults_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinalResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Guardians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Relation = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guardians_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarkEntryDrafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    WrittenMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MCQMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CQMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PracticalMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    VivaMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LabMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OralMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    CreatedByTeacherId = table.Column<int>(type: "int", nullable: false),
                    DraftSavedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkEntryDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarkEntryDrafts_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarkEntryDrafts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarkEntryDrafts_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeritResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    ClassPosition = table.Column<int>(type: "int", nullable: false),
                    GroupPosition = table.Column<int>(type: "int", nullable: true),
                    TotalMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Gpa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeritResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeritResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeritResults_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeritResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromotionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FromClassId = table.Column<int>(type: "int", nullable: false),
                    ToClassId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PromotedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PromotedByUserId = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionHistories_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionHistories_Classes_FromClassId",
                        column: x => x.FromClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionHistories_Classes_ToClassId",
                        column: x => x.ToClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionHistories_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReEvaluationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OldMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NewMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RequestReason = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReEvaluationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReEvaluationRequests_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReEvaluationRequests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReEvaluationRequests_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    PdfPath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Gpa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportCards_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportCards_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    OldMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NewMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OldGpa = table.Column<int>(type: "int", nullable: true),
                    NewGpa = table.Column<int>(type: "int", nullable: true),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    ChangeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultAuditLogs_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultAuditLogs_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultAuditLogs_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "StudentDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDocuments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentExamResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TotalMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFullMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Gpa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    ClassPosition = table.Column<int>(type: "int", nullable: false),
                    GroupPosition = table.Column<int>(type: "int", nullable: true),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    FailedSubjectCount = table.Column<int>(type: "int", nullable: false),
                    PassedSubjectCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentExamResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentExamResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentExamResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroupAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentGroupId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroupAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentGroupAssignments_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroupAssignments_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroupAssignments_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroupAssignments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentSubjectResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    MarksObtained = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FullMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PassMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    GradePoint = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSubjectResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSubjectResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentSubjectResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentSubjectResults_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassSubjectTeachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassSubjectId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSubjectTeachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSubjectTeachers_ClassSubjects_ClassSubjectId",
                        column: x => x.ClassSubjectId,
                        principalTable: "ClassSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassSubjectTeachers_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LessonPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    LessonDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Plan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPlans_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Marks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    WrittenMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MCQMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CQMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PracticalMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    VivaMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LabMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OralMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AssignmentMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ContinuousAssessmentMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CompetencyMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    BehaviourMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ParticipationMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MarksObtained = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GradePoint = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EnteredByTeacherId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Marks_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Marks_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Marks_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Marks_Teachers_EnteredByTeacherId",
                        column: x => x.EnteredByTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAttendances_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherClassAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    IsClassTeacher = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherClassAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherClassAssignments_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherClassAssignments_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherClassAssignments_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherClassAssignments_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherDocuments_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherLeaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherProfileId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    LeaveType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApproverRemarks = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherLeaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherLeaves_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLeaves_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherPerformances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvaluatorUserId = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherPerformances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherPerformances_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherPerformances_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherPerformances_Users_EvaluatorUserId",
                        column: x => x.EvaluatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSalaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherProfileId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    MonthYear = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Allowances = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Deductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSalaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherSalaries_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSubjectAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSubjectAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignments_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignments_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignments_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignments_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignments_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherTimetables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RoomNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherTimetables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherTimetables_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherTimetables_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherTimetables_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherTimetables_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarkAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkEntryId = table.Column<int>(type: "int", nullable: false),
                    OldMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NewMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarkAuditLogs_Marks_MarkEntryId",
                        column: x => x.MarkEntryId,
                        principalTable: "Marks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AcademicYears",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EndsOn", "IsActive", "IsDeleted", "Name", "StartsOn", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "2026", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });

            migrationBuilder.InsertData(
                table: "Admissions",
                columns: new[] { "Id", "AdmissionFee", "AdmissionFeePaid", "AlternativeNumber", "ApplicantEmail", "ApplicantMobileNumber", "ApplicantName", "ApplicantNameBangla", "ApplicationNo", "AppliedClassId", "BirthCertificateNo", "BirthCertificatePath", "BloodGroup", "Country", "CreatedAt", "CreatedBy", "DateOfBirth", "FatherName", "FatherOccupation", "FatherOrGuardianMobileNo", "Gender", "GuardianName", "GuardianOccupation", "IsDeleted", "MaritalStatus", "MotherName", "MotherOccupation", "Nationality", "PaymentMethod", "PaymentSlipPath", "PermanentDistrict", "PermanentPostOffice", "PermanentThana", "PermanentVillage", "PresentDistrict", "PresentPostOffice", "PresentThana", "PresentVillage", "ProfilePicturePath", "Religion", "ReviewedAt", "ReviewedByUserId", "Status", "TransactionDetails", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, 1500m, false, null, null, "01800000010", "Pending Applicant", null, "APP-2026-0001", 1, null, null, null, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(2019, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Applicant Father", null, "01800000001", "Female", "Applicant Guardian", null, false, "Single", "Applicant Mother", null, "Bangladeshi", null, null, null, null, null, null, null, null, null, null, null, "Islam", null, null, 1, null, null, null });

            migrationBuilder.InsertData(
                table: "Attendance",
                columns: new[] { "Id", "AttendanceDate", "CreatedAt", "CreatedBy", "CreatedByUserId", "IsDeleted", "PeriodNo", "Remarks", "SchoolClassId", "SectionId", "Status", "StudentId", "UpdatedAt", "UpdatedBy", "UpdatedByUserId" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 4, 25), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, false, null, null, 1, 1, 1, 1, null, null, null },
                    { 2, new DateOnly(2026, 4, 25), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, false, null, null, 1, 1, 2, 2, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AccessionNo", "Author", "AvailableCopies", "CreatedAt", "CreatedBy", "IsDeleted", "Title", "TotalCopies", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, "B-0001", "Academic Board", 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Primary Mathematics", 10, null, null });

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "Name", "SortOrder", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Class One", 1, null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Class Two", 2, null, null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Class Three", 3, null, null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Class Four", 4, null, null },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Class Five", 5, null, null },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Class Six", 6, null, null },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Class Seven", 7, null, null },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Class Eight", 8, null, null },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Class Nine", 9, null, null },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Class Ten", 10, null, null }
                });

            migrationBuilder.InsertData(
                table: "Exams",
                columns: new[] { "Id", "AcademicYearId", "CreatedAt", "CreatedBy", "EndsOn", "IsDeleted", "IsLocked", "LockedAt", "LockedByUserId", "Name", "StartsOn", "Status", "Term", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateOnly(2026, 6, 12), false, false, null, null, "Midterm", new DateOnly(2026, 6, 1), 1, 8, null, null });

            migrationBuilder.InsertData(
                table: "FeeInvoices",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DueDate", "InvoiceNo", "IsDeleted", "PaidAmount", "Status", "StudentId", "TotalAmount", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateOnly(2026, 5, 10), "INV-2026-0001", false, 2500m, 3, 1, 2500m, null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateOnly(2026, 5, 10), "INV-2026-0002", false, 1000m, 2, 2, 2500m, null, null }
                });

            migrationBuilder.InsertData(
                table: "GradingRules",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "DisplayOrder", "Grade", "GradePoint", "IsActive", "IsDeleted", "MaxMarks", "MinMarks", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "", 0, "A+", 5.0m, true, false, 100m, 80m, null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "", 0, "A", 4.0m, true, false, 79m, 70m, null, null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "", 0, "A-", 3.5m, true, false, 69m, 60m, null, null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "", 0, "B", 3.0m, true, false, 59m, 50m, null, null },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "", 0, "C", 2.0m, true, false, 49m, 40m, null, null },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "", 0, "D", 1.0m, true, false, 39m, 33m, null, null },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "", 0, "F", 0.0m, true, false, 32m, 0m, null, null }
                });

            migrationBuilder.InsertData(
                table: "Notices",
                columns: new[] { "Id", "AudienceRole", "Body", "CreatedAt", "CreatedBy", "IsDeleted", "PublishAt", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, "All", "Classes and office activities are active.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Welcome to the 2026 academic session", null, null });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CanCreate", "CanDelete", "CanRead", "CanUpdate", "Code", "CreatedAt", "CreatedBy", "IsDeleted", "Module", "ModuleName", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "View", false, false, true, false, "Dashboard.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Dashboard", "Dashboard", null, null },
                    { 2, "Create", true, false, false, false, "Dashboard.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Dashboard", "Dashboard", null, null },
                    { 3, "Edit", false, false, false, true, "Dashboard.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Dashboard", "Dashboard", null, null },
                    { 4, "Delete", false, true, false, false, "Dashboard.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Dashboard", "Dashboard", null, null },
                    { 5, "Approve", false, false, false, true, "Dashboard.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Dashboard", "Dashboard", null, null },
                    { 6, "Assign", false, false, false, true, "Dashboard.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Dashboard", "Dashboard", null, null },
                    { 7, "Publish", false, false, false, true, "Dashboard.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Dashboard", "Dashboard", null, null },
                    { 8, "Export", false, false, true, false, "Dashboard.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Dashboard", "Dashboard", null, null },
                    { 9, "Manage", true, true, true, true, "Dashboard.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Dashboard", "Dashboard", null, null },
                    { 10, "View", false, false, true, false, "Users.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Users", "Users", null, null },
                    { 11, "Create", true, false, false, false, "Users.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Users", "Users", null, null },
                    { 12, "Edit", false, false, false, true, "Users.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Users", "Users", null, null },
                    { 13, "Delete", false, true, false, false, "Users.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Users", "Users", null, null },
                    { 14, "Approve", false, false, false, true, "Users.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Users", "Users", null, null },
                    { 15, "Assign", false, false, false, true, "Users.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Users", "Users", null, null },
                    { 16, "Publish", false, false, false, true, "Users.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Users", "Users", null, null },
                    { 17, "Export", false, false, true, false, "Users.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Users", "Users", null, null },
                    { 18, "Manage", true, true, true, true, "Users.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Users", "Users", null, null },
                    { 19, "View", false, false, true, false, "Roles.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Roles", "Roles", null, null },
                    { 20, "Create", true, false, false, false, "Roles.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Roles", "Roles", null, null },
                    { 21, "Edit", false, false, false, true, "Roles.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Roles", "Roles", null, null },
                    { 22, "Delete", false, true, false, false, "Roles.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Roles", "Roles", null, null },
                    { 23, "Approve", false, false, false, true, "Roles.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Roles", "Roles", null, null },
                    { 24, "Assign", false, false, false, true, "Roles.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Roles", "Roles", null, null },
                    { 25, "Publish", false, false, false, true, "Roles.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Roles", "Roles", null, null },
                    { 26, "Export", false, false, true, false, "Roles.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Roles", "Roles", null, null },
                    { 27, "Manage", true, true, true, true, "Roles.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Roles", "Roles", null, null },
                    { 28, "View", false, false, true, false, "Permissions.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Permissions", "Permissions", null, null },
                    { 29, "Create", true, false, false, false, "Permissions.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Permissions", "Permissions", null, null },
                    { 30, "Edit", false, false, false, true, "Permissions.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Permissions", "Permissions", null, null },
                    { 31, "Delete", false, true, false, false, "Permissions.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Permissions", "Permissions", null, null },
                    { 32, "Approve", false, false, false, true, "Permissions.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Permissions", "Permissions", null, null },
                    { 33, "Assign", false, false, false, true, "Permissions.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Permissions", "Permissions", null, null },
                    { 34, "Publish", false, false, false, true, "Permissions.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Permissions", "Permissions", null, null },
                    { 35, "Export", false, false, true, false, "Permissions.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Permissions", "Permissions", null, null },
                    { 36, "Manage", true, true, true, true, "Permissions.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Permissions", "Permissions", null, null },
                    { 37, "View", false, false, true, false, "Admissions.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admissions", "Admissions", null, null },
                    { 38, "Create", true, false, false, false, "Admissions.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admissions", "Admissions", null, null },
                    { 39, "Edit", false, false, false, true, "Admissions.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admissions", "Admissions", null, null },
                    { 40, "Delete", false, true, false, false, "Admissions.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admissions", "Admissions", null, null },
                    { 41, "Approve", false, false, false, true, "Admissions.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admissions", "Admissions", null, null },
                    { 42, "Assign", false, false, false, true, "Admissions.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admissions", "Admissions", null, null },
                    { 43, "Publish", false, false, false, true, "Admissions.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admissions", "Admissions", null, null },
                    { 44, "Export", false, false, true, false, "Admissions.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admissions", "Admissions", null, null },
                    { 45, "Manage", true, true, true, true, "Admissions.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admissions", "Admissions", null, null },
                    { 46, "View", false, false, true, false, "Students.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Students", "Students", null, null },
                    { 47, "Create", true, false, false, false, "Students.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Students", "Students", null, null },
                    { 48, "Edit", false, false, false, true, "Students.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Students", "Students", null, null },
                    { 49, "Delete", false, true, false, false, "Students.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Students", "Students", null, null },
                    { 50, "Approve", false, false, false, true, "Students.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Students", "Students", null, null },
                    { 51, "Assign", false, false, false, true, "Students.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Students", "Students", null, null },
                    { 52, "Publish", false, false, false, true, "Students.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Students", "Students", null, null },
                    { 53, "Export", false, false, true, false, "Students.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Students", "Students", null, null },
                    { 54, "Manage", true, true, true, true, "Students.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Students", "Students", null, null },
                    { 55, "View", false, false, true, false, "Teachers.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Teachers", "Teachers", null, null },
                    { 56, "Create", true, false, false, false, "Teachers.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Teachers", "Teachers", null, null },
                    { 57, "Edit", false, false, false, true, "Teachers.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Teachers", "Teachers", null, null },
                    { 58, "Delete", false, true, false, false, "Teachers.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Teachers", "Teachers", null, null },
                    { 59, "Approve", false, false, false, true, "Teachers.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Teachers", "Teachers", null, null },
                    { 60, "Assign", false, false, false, true, "Teachers.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Teachers", "Teachers", null, null },
                    { 61, "Publish", false, false, false, true, "Teachers.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Teachers", "Teachers", null, null },
                    { 62, "Export", false, false, true, false, "Teachers.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Teachers", "Teachers", null, null },
                    { 63, "Manage", true, true, true, true, "Teachers.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Teachers", "Teachers", null, null },
                    { 64, "View", false, false, true, false, "Classes.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Classes", "Classes", null, null },
                    { 65, "Create", true, false, false, false, "Classes.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Classes", "Classes", null, null },
                    { 66, "Edit", false, false, false, true, "Classes.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Classes", "Classes", null, null },
                    { 67, "Delete", false, true, false, false, "Classes.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Classes", "Classes", null, null },
                    { 68, "Approve", false, false, false, true, "Classes.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Classes", "Classes", null, null },
                    { 69, "Assign", false, false, false, true, "Classes.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Classes", "Classes", null, null },
                    { 70, "Publish", false, false, false, true, "Classes.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Classes", "Classes", null, null },
                    { 71, "Export", false, false, true, false, "Classes.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Classes", "Classes", null, null },
                    { 72, "Manage", true, true, true, true, "Classes.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Classes", "Classes", null, null },
                    { 73, "View", false, false, true, false, "Sections.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Sections", "Sections", null, null },
                    { 74, "Create", true, false, false, false, "Sections.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Sections", "Sections", null, null },
                    { 75, "Edit", false, false, false, true, "Sections.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Sections", "Sections", null, null },
                    { 76, "Delete", false, true, false, false, "Sections.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Sections", "Sections", null, null },
                    { 77, "Approve", false, false, false, true, "Sections.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Sections", "Sections", null, null },
                    { 78, "Assign", false, false, false, true, "Sections.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Sections", "Sections", null, null },
                    { 79, "Publish", false, false, false, true, "Sections.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Sections", "Sections", null, null },
                    { 80, "Export", false, false, true, false, "Sections.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Sections", "Sections", null, null },
                    { 81, "Manage", true, true, true, true, "Sections.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Sections", "Sections", null, null },
                    { 82, "View", false, false, true, false, "Subjects.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Subjects", "Subjects", null, null },
                    { 83, "Create", true, false, false, false, "Subjects.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Subjects", "Subjects", null, null },
                    { 84, "Edit", false, false, false, true, "Subjects.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Subjects", "Subjects", null, null },
                    { 85, "Delete", false, true, false, false, "Subjects.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Subjects", "Subjects", null, null },
                    { 86, "Approve", false, false, false, true, "Subjects.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Subjects", "Subjects", null, null },
                    { 87, "Assign", false, false, false, true, "Subjects.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Subjects", "Subjects", null, null },
                    { 88, "Publish", false, false, false, true, "Subjects.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Subjects", "Subjects", null, null },
                    { 89, "Export", false, false, true, false, "Subjects.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Subjects", "Subjects", null, null },
                    { 90, "Manage", true, true, true, true, "Subjects.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Subjects", "Subjects", null, null },
                    { 91, "View", false, false, true, false, "Attendance.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Attendance", "Attendance", null, null },
                    { 92, "Create", true, false, false, false, "Attendance.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Attendance", "Attendance", null, null },
                    { 93, "Edit", false, false, false, true, "Attendance.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Attendance", "Attendance", null, null },
                    { 94, "Delete", false, true, false, false, "Attendance.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Attendance", "Attendance", null, null },
                    { 95, "Approve", false, false, false, true, "Attendance.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Attendance", "Attendance", null, null },
                    { 96, "Assign", false, false, false, true, "Attendance.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Attendance", "Attendance", null, null },
                    { 97, "Publish", false, false, false, true, "Attendance.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Attendance", "Attendance", null, null },
                    { 98, "Export", false, false, true, false, "Attendance.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Attendance", "Attendance", null, null },
                    { 99, "Manage", true, true, true, true, "Attendance.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Attendance", "Attendance", null, null },
                    { 100, "View", false, false, true, false, "Exams.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exams", "Exams", null, null },
                    { 101, "Create", true, false, false, false, "Exams.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exams", "Exams", null, null },
                    { 102, "Edit", false, false, false, true, "Exams.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exams", "Exams", null, null },
                    { 103, "Delete", false, true, false, false, "Exams.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exams", "Exams", null, null },
                    { 104, "Approve", false, false, false, true, "Exams.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exams", "Exams", null, null },
                    { 105, "Assign", false, false, false, true, "Exams.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exams", "Exams", null, null },
                    { 106, "Publish", false, false, false, true, "Exams.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exams", "Exams", null, null },
                    { 107, "Export", false, false, true, false, "Exams.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exams", "Exams", null, null },
                    { 108, "Manage", true, true, true, true, "Exams.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exams", "Exams", null, null },
                    { 109, "View", false, false, true, false, "Marks.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Marks", "Marks", null, null },
                    { 110, "Create", true, false, false, false, "Marks.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Marks", "Marks", null, null },
                    { 111, "Edit", false, false, false, true, "Marks.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Marks", "Marks", null, null },
                    { 112, "Delete", false, true, false, false, "Marks.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Marks", "Marks", null, null },
                    { 113, "Approve", false, false, false, true, "Marks.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Marks", "Marks", null, null },
                    { 114, "Assign", false, false, false, true, "Marks.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Marks", "Marks", null, null },
                    { 115, "Publish", false, false, false, true, "Marks.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Marks", "Marks", null, null },
                    { 116, "Export", false, false, true, false, "Marks.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Marks", "Marks", null, null },
                    { 117, "Manage", true, true, true, true, "Marks.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Marks", "Marks", null, null },
                    { 118, "View", false, false, true, false, "Assignments.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Assignments", "Assignments", null, null },
                    { 119, "Create", true, false, false, false, "Assignments.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Assignments", "Assignments", null, null },
                    { 120, "Edit", false, false, false, true, "Assignments.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Assignments", "Assignments", null, null },
                    { 121, "Delete", false, true, false, false, "Assignments.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Assignments", "Assignments", null, null },
                    { 122, "Approve", false, false, false, true, "Assignments.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Assignments", "Assignments", null, null },
                    { 123, "Assign", false, false, false, true, "Assignments.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Assignments", "Assignments", null, null },
                    { 124, "Publish", false, false, false, true, "Assignments.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Assignments", "Assignments", null, null },
                    { 125, "Export", false, false, true, false, "Assignments.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Assignments", "Assignments", null, null },
                    { 126, "Manage", true, true, true, true, "Assignments.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Assignments", "Assignments", null, null },
                    { 127, "View", false, false, true, false, "Fees.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Fees", "Fees", null, null },
                    { 128, "Create", true, false, false, false, "Fees.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Fees", "Fees", null, null },
                    { 129, "Edit", false, false, false, true, "Fees.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Fees", "Fees", null, null },
                    { 130, "Delete", false, true, false, false, "Fees.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Fees", "Fees", null, null },
                    { 131, "Approve", false, false, false, true, "Fees.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Fees", "Fees", null, null },
                    { 132, "Assign", false, false, false, true, "Fees.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Fees", "Fees", null, null },
                    { 133, "Publish", false, false, false, true, "Fees.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Fees", "Fees", null, null },
                    { 134, "Export", false, false, true, false, "Fees.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Fees", "Fees", null, null },
                    { 135, "Manage", true, true, true, true, "Fees.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Fees", "Fees", null, null },
                    { 136, "View", false, false, true, false, "Payments.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Payments", "Payments", null, null },
                    { 137, "Create", true, false, false, false, "Payments.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Payments", "Payments", null, null },
                    { 138, "Edit", false, false, false, true, "Payments.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Payments", "Payments", null, null },
                    { 139, "Delete", false, true, false, false, "Payments.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Payments", "Payments", null, null },
                    { 140, "Approve", false, false, false, true, "Payments.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Payments", "Payments", null, null },
                    { 141, "Assign", false, false, false, true, "Payments.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Payments", "Payments", null, null },
                    { 142, "Publish", false, false, false, true, "Payments.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Payments", "Payments", null, null },
                    { 143, "Export", false, false, true, false, "Payments.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Payments", "Payments", null, null },
                    { 144, "Manage", true, true, true, true, "Payments.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Payments", "Payments", null, null },
                    { 145, "View", false, false, true, false, "Library.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Library", "Library", null, null },
                    { 146, "Create", true, false, false, false, "Library.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Library", "Library", null, null },
                    { 147, "Edit", false, false, false, true, "Library.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Library", "Library", null, null },
                    { 148, "Delete", false, true, false, false, "Library.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Library", "Library", null, null },
                    { 149, "Approve", false, false, false, true, "Library.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Library", "Library", null, null },
                    { 150, "Assign", false, false, false, true, "Library.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Library", "Library", null, null },
                    { 151, "Publish", false, false, false, true, "Library.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Library", "Library", null, null },
                    { 152, "Export", false, false, true, false, "Library.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Library", "Library", null, null },
                    { 153, "Manage", true, true, true, true, "Library.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Library", "Library", null, null },
                    { 154, "View", false, false, true, false, "Transport.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Transport", "Transport", null, null },
                    { 155, "Create", true, false, false, false, "Transport.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Transport", "Transport", null, null },
                    { 156, "Edit", false, false, false, true, "Transport.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Transport", "Transport", null, null },
                    { 157, "Delete", false, true, false, false, "Transport.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Transport", "Transport", null, null },
                    { 158, "Approve", false, false, false, true, "Transport.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Transport", "Transport", null, null },
                    { 159, "Assign", false, false, false, true, "Transport.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Transport", "Transport", null, null },
                    { 160, "Publish", false, false, false, true, "Transport.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Transport", "Transport", null, null },
                    { 161, "Export", false, false, true, false, "Transport.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Transport", "Transport", null, null },
                    { 162, "Manage", true, true, true, true, "Transport.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Transport", "Transport", null, null },
                    { 163, "View", false, false, true, false, "Health.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Health", "Health", null, null },
                    { 164, "Create", true, false, false, false, "Health.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Health", "Health", null, null },
                    { 165, "Edit", false, false, false, true, "Health.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Health", "Health", null, null },
                    { 166, "Delete", false, true, false, false, "Health.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Health", "Health", null, null },
                    { 167, "Approve", false, false, false, true, "Health.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Health", "Health", null, null },
                    { 168, "Assign", false, false, false, true, "Health.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Health", "Health", null, null },
                    { 169, "Publish", false, false, false, true, "Health.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Health", "Health", null, null },
                    { 170, "Export", false, false, true, false, "Health.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Health", "Health", null, null },
                    { 171, "Manage", true, true, true, true, "Health.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Health", "Health", null, null },
                    { 172, "View", false, false, true, false, "Notifications.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Notifications", "Notifications", null, null },
                    { 173, "Create", true, false, false, false, "Notifications.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Notifications", "Notifications", null, null },
                    { 174, "Edit", false, false, false, true, "Notifications.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Notifications", "Notifications", null, null },
                    { 175, "Delete", false, true, false, false, "Notifications.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Notifications", "Notifications", null, null },
                    { 176, "Approve", false, false, false, true, "Notifications.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Notifications", "Notifications", null, null },
                    { 177, "Assign", false, false, false, true, "Notifications.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Notifications", "Notifications", null, null },
                    { 178, "Publish", false, false, false, true, "Notifications.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Notifications", "Notifications", null, null },
                    { 179, "Export", false, false, true, false, "Notifications.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Notifications", "Notifications", null, null },
                    { 180, "Manage", true, true, true, true, "Notifications.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Notifications", "Notifications", null, null },
                    { 181, "View", false, false, true, false, "Reports.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 182, "Create", true, false, false, false, "Reports.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 183, "Edit", false, false, false, true, "Reports.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 184, "Delete", false, true, false, false, "Reports.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 185, "Approve", false, false, false, true, "Reports.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 186, "Assign", false, false, false, true, "Reports.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 187, "Publish", false, false, false, true, "Reports.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 188, "Export", false, false, true, false, "Reports.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 189, "Manage", true, true, true, true, "Reports.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Reports", "Reports", null, null },
                    { 190, "View", false, false, true, false, "Settings.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 191, "Create", true, false, false, false, "Settings.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 192, "Edit", false, false, false, true, "Settings.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 193, "Delete", false, true, false, false, "Settings.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 194, "Approve", false, false, false, true, "Settings.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 195, "Assign", false, false, false, true, "Settings.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 196, "Publish", false, false, false, true, "Settings.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 197, "Export", false, false, true, false, "Settings.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 198, "Manage", true, true, true, true, "Settings.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Settings", "Settings", null, null },
                    { 199, "View", false, false, true, false, "Academic.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 200, "Create", true, false, false, false, "Academic.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 201, "Edit", false, false, false, true, "Academic.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 202, "Delete", false, true, false, false, "Academic.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 203, "Approve", false, false, false, true, "Academic.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 204, "Assign", false, false, false, true, "Academic.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 205, "Publish", false, false, false, true, "Academic.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 206, "Export", false, false, true, false, "Academic.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 207, "Manage", true, true, true, true, "Academic.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Academic", "Academic", null, null },
                    { 208, "View", false, false, true, false, "Admission.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 209, "Create", true, false, false, false, "Admission.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 210, "Edit", false, false, false, true, "Admission.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 211, "Delete", false, true, false, false, "Admission.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 212, "Approve", false, false, false, true, "Admission.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 213, "Assign", false, false, false, true, "Admission.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 214, "Publish", false, false, false, true, "Admission.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 215, "Export", false, false, true, false, "Admission.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 216, "Manage", true, true, true, true, "Admission.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Admission", "Admission", null, null },
                    { 217, "View", false, false, true, false, "Student.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 218, "Create", true, false, false, false, "Student.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 219, "Edit", false, false, false, true, "Student.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 220, "Delete", false, true, false, false, "Student.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 221, "Approve", false, false, false, true, "Student.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 222, "Assign", false, false, false, true, "Student.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 223, "Publish", false, false, false, true, "Student.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 224, "Export", false, false, true, false, "Student.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 225, "Manage", true, true, true, true, "Student.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Student", "Student", null, null },
                    { 226, "View", false, false, true, false, "Exam.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 227, "Create", true, false, false, false, "Exam.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 228, "Edit", false, false, false, true, "Exam.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 229, "Delete", false, true, false, false, "Exam.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 230, "Approve", false, false, false, true, "Exam.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 231, "Assign", false, false, false, true, "Exam.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 232, "Publish", false, false, false, true, "Exam.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 233, "Export", false, false, true, false, "Exam.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 234, "Manage", true, true, true, true, "Exam.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Exam", "Exam", null, null },
                    { 235, "View", false, false, true, false, "Result.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 236, "Create", true, false, false, false, "Result.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 237, "Edit", false, false, false, true, "Result.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 238, "Delete", false, true, false, false, "Result.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 239, "Approve", false, false, false, true, "Result.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 240, "Assign", false, false, false, true, "Result.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 241, "Publish", false, false, false, true, "Result.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 242, "Export", false, false, true, false, "Result.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 243, "Manage", true, true, true, true, "Result.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Result", "Result", null, null },
                    { 244, "View", false, false, true, false, "Communication.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 245, "Create", true, false, false, false, "Communication.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 246, "Edit", false, false, false, true, "Communication.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 247, "Delete", false, true, false, false, "Communication.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 248, "Approve", false, false, false, true, "Communication.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 249, "Assign", false, false, false, true, "Communication.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 250, "Publish", false, false, false, true, "Communication.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 251, "Export", false, false, true, false, "Communication.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 252, "Manage", true, true, true, true, "Communication.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Communication", "Communication", null, null },
                    { 253, "View", false, false, true, false, "System.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 254, "Create", true, false, false, false, "System.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 255, "Edit", false, false, false, true, "System.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 256, "Delete", false, true, false, false, "System.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 257, "Approve", false, false, false, true, "System.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 258, "Assign", false, false, false, true, "System.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 259, "Publish", false, false, false, true, "System.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 260, "Export", false, false, true, false, "System.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 261, "Manage", true, true, true, true, "System.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null },
                    { 262, "View", false, false, true, false, "AuditLogs.View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 263, "Create", true, false, false, false, "AuditLogs.Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 264, "Edit", false, false, false, true, "AuditLogs.Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 265, "Delete", false, true, false, false, "AuditLogs.Delete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 266, "Approve", false, false, false, true, "AuditLogs.Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 267, "Assign", false, false, false, true, "AuditLogs.Assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 268, "Publish", false, false, false, true, "AuditLogs.Publish", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 269, "Export", false, false, true, false, "AuditLogs.Export", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null },
                    { 270, "Manage", true, true, true, true, "AuditLogs.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "AuditLogs", "AuditLogs", null, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "System owner with all permissions", false, "Super Admin", null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Final approval and all modules", false, "Principal", null, null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Academic operations", false, "Assistant Head", null, null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Teaching and review", false, "Senior Lecturer", null, null },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Teaching operations", false, "Lecturer", null, null },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Admission, fees, reports", false, "Office Staff", null, null },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Student portal access", false, "Student", null, null }
                });

            migrationBuilder.InsertData(
                table: "SchoolProfiles",
                columns: new[] { "Id", "Address", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "Name", "Phone", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, "Dhaka, Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "info@school.local", false, "School Management System", "01000000000", null, null });

            migrationBuilder.InsertData(
                table: "StudentGroups",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Description", "DisplayOrder", "IsActive", "IsDeleted", "MaxClass", "MinClass", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "SCI", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Science Group", 1, true, false, 10, 9, "Science", null, null },
                    { 2, "BS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Business Studies Group", 2, true, false, 10, 9, "Business Studies", null, null },
                    { 3, "HUM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Humanities Group", 3, true, false, 10, 9, "Humanities", null, null }
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DefaultFullMarks", "DefaultPassMarks", "DisplayOrder", "HasAssignment", "HasCQ", "HasContinuousAssessment", "HasLab", "HasMCQ", "HasOral", "HasPractical", "HasViva", "HasWritten", "IsActive", "IsDeleted", "IsMandatory", "IsOptional", "IsPractical", "IsReligionSubject", "Name", "NameBn", "ReligionType", "SubjectGroup", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "BAN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "বাংলা", "", null, "", null, null },
                    { 2, "ENG", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "ইংরেজি", "", null, "", null, null },
                    { 3, "MAT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "গণিত", "", null, "", null, null },
                    { 4, "GSCI", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "সাধারণ বিজ্ঞান", "", null, "", null, null },
                    { 5, "SOC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "বাংলাদেশ ও বিশ্ব পরিচয়", "", null, "", null, null },
                    { 6, "REL", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "ধর্ম ও নৈতিক শিক্ষা", "", null, "", null, null },
                    { 7, "ART", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "চারুকলা", "", null, "", null, null },
                    { 8, "PE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "শারীরিক শিক্ষা", "", null, "", null, null },
                    { 9, "BAN1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "বাংলা ১ম পত্র", "", null, "", null, null },
                    { 10, "BAN2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "বাংলা ২য় পত্র", "", null, "", null, null },
                    { 11, "ENG1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "ইংরেজি ১ম পত্র", "", null, "", null, null },
                    { 12, "ENG2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "ইংরেজি ২য় পত্র", "", null, "", null, null },
                    { 13, "SCI", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "বিজ্ঞান", "", null, "", null, null },
                    { 14, "ICT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "তথ্য ও যোগাযোগ প্রযুক্তি", "", null, "", null, null },
                    { 15, "AGR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "কৃষি শিক্ষা", "", null, "", null, null },
                    { 16, "PHY", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "পদার্থবিজ্ঞান", "", null, "", null, null },
                    { 17, "CHE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "রসায়ন", "", null, "", null, null },
                    { 18, "BIO", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "জীববিজ্ঞান", "", null, "", null, null },
                    { 19, "HMA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "উচ্চতর গণিত", "", null, "", null, null },
                    { 20, "ACC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "হিসাববিজ্ঞান", "", null, "", null, null },
                    { 21, "FIN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "ফাইন্যান্স", "", null, "", null, null },
                    { 22, "BUS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "ব্যবসায় উদ্যোগ", "", null, "", null, null },
                    { 23, "HIS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "ইতিহাস", "", null, "", null, null },
                    { 24, "GEO", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "ভূগোল ও পরিবেশ", "", null, "", null, null },
                    { 25, "ECO", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "অর্থনীতি", "", null, "", null, null },
                    { 26, "CIV", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "নাগরিকতা", "", null, "", null, null },
                    { 27, "CAREER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "ক্যারিয়ার শিক্ষা", "", null, "", null, null },
                    { 28, "HEALTH", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "শারীরিক শিক্ষা, স্বাস্থ্য ও খেলাধুলা", "", null, "", null, null },
                    { 29, "HSC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "গার্হস্থ্য বিজ্ঞান", "", null, "", null, null },
                    { 30, "IRE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "ইসলাম ও নৈতিক শিক্ষা", "", null, "", null, null },
                    { 31, "HRE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "হিন্দুধর্ম ও নৈতিক শিক্ষা", "", null, "", null, null },
                    { 32, "BRE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "বৌদ্ধধর্ম ও নৈতিক শিক্ষা", "", null, "", null, null },
                    { 33, "CRE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 100m, 33m, 0, false, false, false, false, false, false, false, false, true, true, false, true, false, false, false, "খ্রিস্টধর্ম ও নৈতিক শিক্ষা", "", null, "", null, null }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "AlternativeNumber", "BloodGroup", "Country", "CreatedAt", "CreatedBy", "DateOfBirth", "Department", "Designation", "EmailAddress", "EmployeeId", "FatherName", "FullName", "FullNameBangla", "Gender", "IsDeleted", "JoiningDate", "MaritalStatus", "MobileNumber", "MotherName", "NationalIdNo", "NationalIdPath", "Nationality", "PassportNo", "PassportPath", "PermanentDistrict", "PermanentPostOffice", "PermanentThana", "PermanentVillage", "PresentDistrict", "PresentPostOffice", "PresentThana", "PresentVillage", "ProfilePicturePath", "Qualification", "Religion", "Specialization", "SpouseName", "Status", "TeacherNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { 1, null, null, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Senior Lecturer", null, null, null, "Senior Lecturer", null, "", false, null, "", "01000000001", null, null, null, "Bangladeshi", null, null, null, null, null, null, null, null, null, null, null, null, "", null, null, 1, "T-0001", null, null, null },
                    { 2, null, null, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Lecturer", null, null, null, "Class Teacher", null, "", false, null, "", "01000000002", null, null, null, "Bangladeshi", null, null, null, null, null, null, null, null, null, null, null, null, "", null, null, 1, "T-0002", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ActivationToken", "ActivationTokenExpiry", "CreatedAt", "CreatedBy", "Email", "FailedLoginAttempts", "IsDeleted", "IsEmailConfirmed", "LastLoginAt", "LockoutUntil", "PasswordHash", "PhoneNumber", "Status", "UpdatedAt", "UpdatedBy", "UserName" },
                values: new object[] { 1, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "admin@school.local", 0, false, true, null, null, "ChangeThisHash", null, 1, null, null, "admin" });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "CreatedAt", "CreatedBy", "FeeInvoiceId", "IsDeleted", "Method", "PaidAt", "ReferenceNo", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, 2500m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 1, false, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 11, 1 },
                    { 12, 1 },
                    { 13, 1 },
                    { 14, 1 },
                    { 15, 1 },
                    { 16, 1 },
                    { 17, 1 },
                    { 18, 1 },
                    { 19, 1 },
                    { 20, 1 },
                    { 21, 1 },
                    { 22, 1 },
                    { 23, 1 },
                    { 24, 1 },
                    { 25, 1 },
                    { 26, 1 },
                    { 27, 1 },
                    { 28, 1 },
                    { 29, 1 },
                    { 30, 1 },
                    { 31, 1 },
                    { 32, 1 },
                    { 33, 1 },
                    { 34, 1 },
                    { 35, 1 },
                    { 36, 1 },
                    { 37, 1 },
                    { 38, 1 },
                    { 39, 1 },
                    { 40, 1 },
                    { 41, 1 },
                    { 42, 1 },
                    { 43, 1 },
                    { 44, 1 },
                    { 45, 1 },
                    { 46, 1 },
                    { 47, 1 },
                    { 48, 1 },
                    { 49, 1 },
                    { 50, 1 },
                    { 51, 1 },
                    { 52, 1 },
                    { 53, 1 },
                    { 54, 1 },
                    { 55, 1 },
                    { 56, 1 },
                    { 57, 1 },
                    { 58, 1 },
                    { 59, 1 },
                    { 60, 1 },
                    { 61, 1 },
                    { 62, 1 },
                    { 63, 1 },
                    { 64, 1 },
                    { 65, 1 },
                    { 66, 1 },
                    { 67, 1 },
                    { 68, 1 },
                    { 69, 1 },
                    { 70, 1 },
                    { 71, 1 },
                    { 72, 1 },
                    { 73, 1 },
                    { 74, 1 },
                    { 75, 1 },
                    { 76, 1 },
                    { 77, 1 },
                    { 78, 1 },
                    { 79, 1 },
                    { 80, 1 },
                    { 81, 1 },
                    { 82, 1 },
                    { 83, 1 },
                    { 84, 1 },
                    { 85, 1 },
                    { 86, 1 },
                    { 87, 1 },
                    { 88, 1 },
                    { 89, 1 },
                    { 90, 1 },
                    { 91, 1 },
                    { 92, 1 },
                    { 93, 1 },
                    { 94, 1 },
                    { 95, 1 },
                    { 96, 1 },
                    { 97, 1 },
                    { 98, 1 },
                    { 99, 1 },
                    { 100, 1 },
                    { 101, 1 },
                    { 102, 1 },
                    { 103, 1 },
                    { 104, 1 },
                    { 105, 1 },
                    { 106, 1 },
                    { 107, 1 },
                    { 108, 1 },
                    { 109, 1 },
                    { 110, 1 },
                    { 111, 1 },
                    { 112, 1 },
                    { 113, 1 },
                    { 114, 1 },
                    { 115, 1 },
                    { 116, 1 },
                    { 117, 1 },
                    { 118, 1 },
                    { 119, 1 },
                    { 120, 1 },
                    { 121, 1 },
                    { 122, 1 },
                    { 123, 1 },
                    { 124, 1 },
                    { 125, 1 },
                    { 126, 1 },
                    { 127, 1 },
                    { 128, 1 },
                    { 129, 1 },
                    { 130, 1 },
                    { 131, 1 },
                    { 132, 1 },
                    { 133, 1 },
                    { 134, 1 },
                    { 135, 1 },
                    { 136, 1 },
                    { 137, 1 },
                    { 138, 1 },
                    { 139, 1 },
                    { 140, 1 },
                    { 141, 1 },
                    { 142, 1 },
                    { 143, 1 },
                    { 144, 1 },
                    { 145, 1 },
                    { 146, 1 },
                    { 147, 1 },
                    { 148, 1 },
                    { 149, 1 },
                    { 150, 1 },
                    { 151, 1 },
                    { 152, 1 },
                    { 153, 1 },
                    { 154, 1 },
                    { 155, 1 },
                    { 156, 1 },
                    { 157, 1 },
                    { 158, 1 },
                    { 159, 1 },
                    { 160, 1 },
                    { 161, 1 },
                    { 162, 1 },
                    { 163, 1 },
                    { 164, 1 },
                    { 165, 1 },
                    { 166, 1 },
                    { 167, 1 },
                    { 168, 1 },
                    { 169, 1 },
                    { 170, 1 },
                    { 171, 1 },
                    { 172, 1 },
                    { 173, 1 },
                    { 174, 1 },
                    { 175, 1 },
                    { 176, 1 },
                    { 177, 1 },
                    { 178, 1 },
                    { 179, 1 },
                    { 180, 1 },
                    { 181, 1 },
                    { 182, 1 },
                    { 183, 1 },
                    { 184, 1 },
                    { 185, 1 },
                    { 186, 1 },
                    { 187, 1 },
                    { 188, 1 },
                    { 189, 1 },
                    { 190, 1 },
                    { 191, 1 },
                    { 192, 1 },
                    { 193, 1 },
                    { 194, 1 },
                    { 195, 1 },
                    { 196, 1 },
                    { 197, 1 },
                    { 198, 1 },
                    { 199, 1 },
                    { 200, 1 },
                    { 201, 1 },
                    { 202, 1 },
                    { 203, 1 },
                    { 204, 1 },
                    { 205, 1 },
                    { 206, 1 },
                    { 207, 1 },
                    { 208, 1 },
                    { 209, 1 },
                    { 210, 1 },
                    { 211, 1 },
                    { 212, 1 },
                    { 213, 1 },
                    { 214, 1 },
                    { 215, 1 },
                    { 216, 1 },
                    { 217, 1 },
                    { 218, 1 },
                    { 219, 1 },
                    { 220, 1 },
                    { 221, 1 },
                    { 222, 1 },
                    { 223, 1 },
                    { 224, 1 },
                    { 225, 1 },
                    { 226, 1 },
                    { 227, 1 },
                    { 228, 1 },
                    { 229, 1 },
                    { 230, 1 },
                    { 231, 1 },
                    { 232, 1 },
                    { 233, 1 },
                    { 234, 1 },
                    { 235, 1 },
                    { 236, 1 },
                    { 237, 1 },
                    { 238, 1 },
                    { 239, 1 },
                    { 240, 1 },
                    { 241, 1 },
                    { 242, 1 },
                    { 243, 1 },
                    { 244, 1 },
                    { 245, 1 },
                    { 246, 1 },
                    { 247, 1 },
                    { 248, 1 },
                    { 249, 1 },
                    { 250, 1 },
                    { 251, 1 },
                    { 252, 1 },
                    { 253, 1 },
                    { 254, 1 },
                    { 255, 1 },
                    { 256, 1 },
                    { 257, 1 },
                    { 258, 1 },
                    { 259, 1 },
                    { 260, 1 },
                    { 261, 1 },
                    { 262, 1 },
                    { 263, 1 },
                    { 264, 1 },
                    { 265, 1 },
                    { 266, 1 },
                    { 267, 1 },
                    { 268, 1 },
                    { 269, 1 },
                    { 270, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 4, 2 },
                    { 5, 2 },
                    { 6, 2 },
                    { 7, 2 },
                    { 8, 2 },
                    { 9, 2 },
                    { 10, 2 },
                    { 11, 2 },
                    { 12, 2 },
                    { 13, 2 },
                    { 14, 2 },
                    { 15, 2 },
                    { 16, 2 },
                    { 17, 2 },
                    { 18, 2 },
                    { 19, 2 },
                    { 20, 2 },
                    { 21, 2 },
                    { 22, 2 },
                    { 23, 2 },
                    { 24, 2 },
                    { 25, 2 },
                    { 26, 2 },
                    { 27, 2 },
                    { 28, 2 },
                    { 29, 2 },
                    { 30, 2 },
                    { 31, 2 },
                    { 32, 2 },
                    { 33, 2 },
                    { 34, 2 },
                    { 35, 2 },
                    { 36, 2 },
                    { 37, 2 },
                    { 38, 2 },
                    { 39, 2 },
                    { 40, 2 },
                    { 41, 2 },
                    { 42, 2 },
                    { 43, 2 },
                    { 44, 2 },
                    { 45, 2 },
                    { 46, 2 },
                    { 47, 2 },
                    { 48, 2 },
                    { 49, 2 },
                    { 50, 2 },
                    { 51, 2 },
                    { 52, 2 },
                    { 53, 2 },
                    { 54, 2 },
                    { 55, 2 },
                    { 56, 2 },
                    { 57, 2 },
                    { 58, 2 },
                    { 59, 2 },
                    { 60, 2 },
                    { 61, 2 },
                    { 62, 2 },
                    { 63, 2 },
                    { 64, 2 },
                    { 65, 2 },
                    { 66, 2 },
                    { 67, 2 },
                    { 68, 2 },
                    { 69, 2 },
                    { 70, 2 },
                    { 71, 2 },
                    { 72, 2 },
                    { 73, 2 },
                    { 74, 2 },
                    { 75, 2 },
                    { 76, 2 },
                    { 77, 2 },
                    { 78, 2 },
                    { 79, 2 },
                    { 80, 2 },
                    { 81, 2 },
                    { 82, 2 },
                    { 83, 2 },
                    { 84, 2 },
                    { 85, 2 },
                    { 86, 2 },
                    { 87, 2 },
                    { 88, 2 },
                    { 89, 2 },
                    { 90, 2 },
                    { 91, 2 },
                    { 92, 2 },
                    { 93, 2 },
                    { 94, 2 },
                    { 95, 2 },
                    { 96, 2 },
                    { 97, 2 },
                    { 98, 2 },
                    { 99, 2 },
                    { 100, 2 },
                    { 101, 2 },
                    { 102, 2 },
                    { 103, 2 },
                    { 104, 2 },
                    { 105, 2 },
                    { 106, 2 },
                    { 107, 2 },
                    { 108, 2 },
                    { 109, 2 },
                    { 110, 2 },
                    { 111, 2 },
                    { 112, 2 },
                    { 113, 2 },
                    { 114, 2 },
                    { 115, 2 },
                    { 116, 2 },
                    { 117, 2 },
                    { 118, 2 },
                    { 119, 2 },
                    { 120, 2 },
                    { 121, 2 },
                    { 122, 2 },
                    { 123, 2 },
                    { 124, 2 },
                    { 125, 2 },
                    { 126, 2 },
                    { 127, 2 },
                    { 128, 2 },
                    { 129, 2 },
                    { 130, 2 },
                    { 131, 2 },
                    { 132, 2 },
                    { 133, 2 },
                    { 134, 2 },
                    { 135, 2 },
                    { 136, 2 },
                    { 137, 2 },
                    { 138, 2 },
                    { 139, 2 },
                    { 140, 2 },
                    { 141, 2 },
                    { 142, 2 },
                    { 143, 2 },
                    { 144, 2 },
                    { 145, 2 },
                    { 146, 2 },
                    { 147, 2 },
                    { 148, 2 },
                    { 149, 2 },
                    { 150, 2 },
                    { 151, 2 },
                    { 152, 2 },
                    { 153, 2 },
                    { 154, 2 },
                    { 155, 2 },
                    { 156, 2 },
                    { 157, 2 },
                    { 158, 2 },
                    { 159, 2 },
                    { 160, 2 },
                    { 161, 2 },
                    { 162, 2 },
                    { 163, 2 },
                    { 164, 2 },
                    { 165, 2 },
                    { 166, 2 },
                    { 167, 2 },
                    { 168, 2 },
                    { 169, 2 },
                    { 170, 2 },
                    { 171, 2 },
                    { 172, 2 },
                    { 173, 2 },
                    { 174, 2 },
                    { 175, 2 },
                    { 176, 2 },
                    { 177, 2 },
                    { 178, 2 },
                    { 179, 2 },
                    { 180, 2 },
                    { 181, 2 },
                    { 182, 2 },
                    { 183, 2 },
                    { 184, 2 },
                    { 185, 2 },
                    { 186, 2 },
                    { 187, 2 },
                    { 188, 2 },
                    { 189, 2 },
                    { 190, 2 },
                    { 191, 2 },
                    { 192, 2 },
                    { 193, 2 },
                    { 194, 2 },
                    { 195, 2 },
                    { 196, 2 },
                    { 197, 2 },
                    { 198, 2 },
                    { 199, 2 },
                    { 200, 2 },
                    { 201, 2 },
                    { 202, 2 },
                    { 203, 2 },
                    { 204, 2 },
                    { 205, 2 },
                    { 206, 2 },
                    { 207, 2 },
                    { 208, 2 },
                    { 209, 2 },
                    { 210, 2 },
                    { 211, 2 },
                    { 212, 2 },
                    { 213, 2 },
                    { 214, 2 },
                    { 215, 2 },
                    { 216, 2 },
                    { 217, 2 },
                    { 218, 2 },
                    { 219, 2 },
                    { 220, 2 },
                    { 221, 2 },
                    { 222, 2 },
                    { 223, 2 },
                    { 224, 2 },
                    { 225, 2 },
                    { 226, 2 },
                    { 227, 2 },
                    { 228, 2 },
                    { 229, 2 },
                    { 230, 2 },
                    { 231, 2 },
                    { 232, 2 },
                    { 233, 2 },
                    { 234, 2 },
                    { 235, 2 },
                    { 236, 2 },
                    { 237, 2 },
                    { 238, 2 },
                    { 239, 2 },
                    { 240, 2 },
                    { 241, 2 },
                    { 242, 2 },
                    { 243, 2 },
                    { 244, 2 },
                    { 245, 2 },
                    { 246, 2 },
                    { 247, 2 },
                    { 248, 2 },
                    { 249, 2 },
                    { 250, 2 },
                    { 251, 2 },
                    { 252, 2 },
                    { 253, 2 },
                    { 254, 2 },
                    { 255, 2 },
                    { 256, 2 },
                    { 257, 2 },
                    { 258, 2 },
                    { 259, 2 },
                    { 260, 2 },
                    { 261, 2 },
                    { 262, 2 },
                    { 263, 2 },
                    { 264, 2 },
                    { 265, 2 },
                    { 266, 2 },
                    { 267, 2 },
                    { 268, 2 },
                    { 269, 2 },
                    { 270, 2 },
                    { 1, 3 },
                    { 2, 3 },
                    { 3, 3 },
                    { 4, 3 },
                    { 5, 3 },
                    { 6, 3 },
                    { 7, 3 },
                    { 8, 3 },
                    { 9, 3 },
                    { 37, 3 },
                    { 38, 3 },
                    { 39, 3 },
                    { 40, 3 },
                    { 41, 3 },
                    { 42, 3 },
                    { 43, 3 },
                    { 44, 3 },
                    { 45, 3 },
                    { 46, 3 },
                    { 47, 3 },
                    { 48, 3 },
                    { 49, 3 },
                    { 50, 3 },
                    { 51, 3 },
                    { 52, 3 },
                    { 53, 3 },
                    { 54, 3 },
                    { 64, 3 },
                    { 65, 3 },
                    { 66, 3 },
                    { 67, 3 },
                    { 68, 3 },
                    { 69, 3 },
                    { 70, 3 },
                    { 71, 3 },
                    { 72, 3 },
                    { 73, 3 },
                    { 74, 3 },
                    { 75, 3 },
                    { 76, 3 },
                    { 77, 3 },
                    { 78, 3 },
                    { 79, 3 },
                    { 80, 3 },
                    { 81, 3 },
                    { 82, 3 },
                    { 83, 3 },
                    { 84, 3 },
                    { 85, 3 },
                    { 86, 3 },
                    { 87, 3 },
                    { 88, 3 },
                    { 89, 3 },
                    { 90, 3 },
                    { 91, 3 },
                    { 92, 3 },
                    { 93, 3 },
                    { 94, 3 },
                    { 95, 3 },
                    { 96, 3 },
                    { 97, 3 },
                    { 98, 3 },
                    { 99, 3 },
                    { 100, 3 },
                    { 101, 3 },
                    { 102, 3 },
                    { 103, 3 },
                    { 104, 3 },
                    { 105, 3 },
                    { 106, 3 },
                    { 107, 3 },
                    { 108, 3 },
                    { 109, 3 },
                    { 110, 3 },
                    { 111, 3 },
                    { 112, 3 },
                    { 113, 3 },
                    { 114, 3 },
                    { 115, 3 },
                    { 116, 3 },
                    { 117, 3 },
                    { 181, 3 },
                    { 182, 3 },
                    { 183, 3 },
                    { 184, 3 },
                    { 185, 3 },
                    { 186, 3 },
                    { 187, 3 },
                    { 188, 3 },
                    { 189, 3 },
                    { 199, 3 },
                    { 200, 3 },
                    { 201, 3 },
                    { 202, 3 },
                    { 203, 3 },
                    { 204, 3 },
                    { 205, 3 },
                    { 206, 3 },
                    { 207, 3 },
                    { 208, 3 },
                    { 209, 3 },
                    { 210, 3 },
                    { 211, 3 },
                    { 212, 3 },
                    { 213, 3 },
                    { 214, 3 },
                    { 215, 3 },
                    { 216, 3 },
                    { 217, 3 },
                    { 218, 3 },
                    { 219, 3 },
                    { 220, 3 },
                    { 221, 3 },
                    { 222, 3 },
                    { 223, 3 },
                    { 224, 3 },
                    { 225, 3 },
                    { 226, 3 },
                    { 227, 3 },
                    { 228, 3 },
                    { 229, 3 },
                    { 230, 3 },
                    { 231, 3 },
                    { 232, 3 },
                    { 233, 3 },
                    { 234, 3 },
                    { 235, 3 },
                    { 236, 3 },
                    { 237, 3 },
                    { 238, 3 },
                    { 239, 3 },
                    { 240, 3 },
                    { 241, 3 },
                    { 242, 3 },
                    { 243, 3 },
                    { 244, 3 },
                    { 245, 3 },
                    { 246, 3 },
                    { 247, 3 },
                    { 248, 3 },
                    { 249, 3 },
                    { 250, 3 },
                    { 251, 3 },
                    { 252, 3 },
                    { 1, 5 },
                    { 46, 5 },
                    { 64, 5 },
                    { 91, 5 },
                    { 92, 5 },
                    { 100, 5 },
                    { 109, 5 },
                    { 110, 5 },
                    { 118, 5 },
                    { 119, 5 },
                    { 181, 5 },
                    { 226, 5 },
                    { 1, 6 },
                    { 2, 6 },
                    { 3, 6 },
                    { 5, 6 },
                    { 6, 6 },
                    { 7, 6 },
                    { 8, 6 },
                    { 9, 6 },
                    { 37, 6 },
                    { 38, 6 },
                    { 39, 6 },
                    { 41, 6 },
                    { 42, 6 },
                    { 43, 6 },
                    { 44, 6 },
                    { 45, 6 },
                    { 46, 6 },
                    { 47, 6 },
                    { 48, 6 },
                    { 50, 6 },
                    { 51, 6 },
                    { 52, 6 },
                    { 53, 6 },
                    { 54, 6 },
                    { 127, 6 },
                    { 128, 6 },
                    { 129, 6 },
                    { 131, 6 },
                    { 132, 6 },
                    { 133, 6 },
                    { 134, 6 },
                    { 135, 6 },
                    { 136, 6 },
                    { 137, 6 },
                    { 138, 6 },
                    { 140, 6 },
                    { 141, 6 },
                    { 142, 6 },
                    { 143, 6 },
                    { 144, 6 },
                    { 181, 6 },
                    { 182, 6 },
                    { 183, 6 },
                    { 185, 6 },
                    { 186, 6 },
                    { 187, 6 },
                    { 188, 6 },
                    { 189, 6 },
                    { 208, 6 },
                    { 209, 6 },
                    { 210, 6 },
                    { 212, 6 },
                    { 213, 6 },
                    { 214, 6 },
                    { 215, 6 },
                    { 216, 6 },
                    { 217, 6 },
                    { 218, 6 },
                    { 219, 6 },
                    { 221, 6 },
                    { 222, 6 },
                    { 223, 6 },
                    { 224, 6 },
                    { 225, 6 },
                    { 1, 7 },
                    { 46, 7 },
                    { 91, 7 },
                    { 109, 7 },
                    { 118, 7 },
                    { 119, 7 },
                    { 127, 7 },
                    { 172, 7 },
                    { 217, 7 }
                });

            migrationBuilder.InsertData(
                table: "Sections",
                columns: new[] { "Id", "Capacity", "CreatedAt", "CreatedBy", "IsDeleted", "Name", "ParentSectionId", "SchoolClassId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", null, 1, null, null },
                    { 2, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", null, 1, null, null },
                    { 3, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", null, 2, null, null },
                    { 4, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", null, 2, null, null },
                    { 5, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", null, 3, null, null },
                    { 6, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", null, 3, null, null },
                    { 7, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", null, 4, null, null },
                    { 8, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", null, 4, null, null },
                    { 9, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", null, 5, null, null },
                    { 10, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", null, 5, null, null },
                    { 11, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", null, 6, null, null },
                    { 12, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", null, 6, null, null },
                    { 13, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", null, 7, null, null },
                    { 14, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", null, 7, null, null },
                    { 15, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", null, 8, null, null },
                    { 16, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", null, 8, null, null },
                    { 17, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Science", null, 9, null, null },
                    { 20, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies", null, 9, null, null },
                    { 23, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities", null, 9, null, null },
                    { 26, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Science", null, 10, null, null },
                    { 29, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies", null, 10, null, null },
                    { 32, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities", null, 10, null, null }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Sections",
                columns: new[] { "Id", "Capacity", "CreatedAt", "CreatedBy", "IsDeleted", "Name", "ParentSectionId", "SchoolClassId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 18, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Science A", 17, 9, null, null },
                    { 19, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Science B", 17, 9, null, null },
                    { 21, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies A", 20, 9, null, null },
                    { 22, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies B", 20, 9, null, null },
                    { 24, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities A", 23, 9, null, null },
                    { 25, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities B", 23, 9, null, null },
                    { 27, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Science A", 26, 10, null, null },
                    { 28, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Science B", 26, 10, null, null },
                    { 30, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies A", 29, 10, null, null },
                    { 31, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies B", 29, 10, null, null },
                    { 33, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities A", 32, 10, null, null },
                    { 34, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities B", 32, 10, null, null }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "AlternativeNumber", "AssignedReligionSubjectId", "BirthCertificateNo", "BloodGroup", "ClassId", "Country", "CreatedAt", "CreatedBy", "DateOfBirth", "EmailAddress", "FatherName", "FatherOccupation", "FullName", "FullNameBangla", "Gender", "IsDeleted", "MaritalStatus", "MobileNumber", "MotherName", "MotherOccupation", "Nationality", "PermanentDistrict", "PermanentPostOffice", "PermanentThana", "PermanentVillage", "PresentDistrict", "PresentPostOffice", "PresentThana", "PresentVillage", "ProfilePicturePath", "Religion", "RollNumber", "SectionId", "Status", "StudentGroupId", "StudentNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { 1, null, null, null, null, 1, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(2018, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Father One", null, "Sample Student One", null, "Male", false, "Single", "01700000010", "Mother One", null, "Bangladeshi", null, null, null, null, null, null, null, null, null, "Islam", 1, 1, 1, null, "STU-2026-0001", null, null, null },
                    { 2, null, null, null, null, 1, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(2018, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Father Two", null, "Sample Student Two", null, "Female", false, "Single", "01700000020", "Mother Two", null, "Bangladeshi", null, null, null, null, null, null, null, null, null, "Islam", 2, 1, 1, null, "STU-2026-0002", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Guardians",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "Name", "Occupation", "Phone", "Relation", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, false, "Guardian One", null, "01700000001", "Father", 1, null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, false, "Guardian Two", null, "01700000002", "Mother", 2, null, null }
                });

            migrationBuilder.InsertData(
                table: "Marks",
                columns: new[] { "Id", "AssignmentMarks", "BehaviourMarks", "CQMarks", "CompetencyMarks", "ContinuousAssessmentMarks", "CreatedAt", "CreatedBy", "CreatedByUserId", "EnteredByTeacherId", "ExamId", "Grade", "GradePoint", "IsDeleted", "IsLocked", "LabMarks", "LockedAt", "MCQMarks", "MarksObtained", "OralMarks", "ParticipationMarks", "PracticalMarks", "Status", "StudentId", "SubjectId", "SubmittedAt", "UpdatedAt", "UpdatedBy", "UpdatedByUserId", "VivaMarks", "WrittenMarks" },
                values: new object[,]
                {
                    { 1, null, null, null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, 1, 1, null, null, false, false, null, null, null, 86m, null, null, null, 4, 1, 1, null, null, null, null, null, null },
                    { 2, null, null, null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, 1, 1, null, null, false, false, null, null, null, 78m, null, null, null, 4, 2, 1, null, null, null, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionDocuments_AdmissionApplicationId",
                table: "AdmissionDocuments",
                column: "AdmissionApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_ApplicationNo",
                table: "Admissions",
                column: "ApplicationNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_AssignmentTaskId",
                table: "AssignmentSubmissions",
                column: "AssignmentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AccessionNo",
                table: "Books",
                column: "AccessionNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_SchoolClassId",
                table: "ClassSubjects",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_SectionId",
                table: "ClassSubjects",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_StudentGroupId",
                table: "ClassSubjects",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_SubjectId",
                table: "ClassSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjectTeachers_ClassSubjectId",
                table: "ClassSubjectTeachers",
                column: "ClassSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjectTeachers_TeacherId",
                table: "ClassSubjectTeachers",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAcademicAssignments_EmployeeId",
                table: "EmployeeAcademicAssignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendances_EmployeeId",
                table: "EmployeeAttendances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_EmployeeId",
                table: "EmployeeDocuments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeExperiences_EmployeeId",
                table: "EmployeeExperiences",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaves_ApprovedByUserId",
                table: "EmployeeLeaves",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaves_EmployeeId",
                table: "EmployeeLeaves",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeQualifications_EmployeeId",
                table: "EmployeeQualifications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DesignationId",
                table: "Employees",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NIDNumber",
                table: "Employees",
                column: "NIDNumber",
                unique: true,
                filter: "[NIDNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Phone",
                table: "Employees",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaries_EmployeeId",
                table: "EmployeeSalaries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamConfigurations_ClassId",
                table: "ExamConfigurations",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamConfigurations_ExamTypeId_ClassId",
                table: "ExamConfigurations",
                columns: new[] { "ExamTypeId", "ClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamSchedules_ExamId",
                table: "ExamSchedules",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSchedules_SubjectId",
                table: "ExamSchedules",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubjects_ExamId",
                table: "ExamSubjects",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubjects_SubjectId",
                table: "ExamSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTypes_Code",
                table: "ExamTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeInvoices_InvoiceNo",
                table: "FeeInvoices",
                column: "InvoiceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinalResults_AcademicYearId_StudentId",
                table: "FinalResults",
                columns: new[] { "AcademicYearId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinalResults_SchoolClassId",
                table: "FinalResults",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalResults_StudentId",
                table: "FinalResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_GpaConfigurations_Grade",
                table: "GpaConfigurations",
                column: "Grade",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GpaConfigurations_MinMarks_MaxMarks",
                table: "GpaConfigurations",
                columns: new[] { "MinMarks", "MaxMarks" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_StudentId",
                table: "Guardians",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlans_TeacherId",
                table: "LessonPlans",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkAuditLogs_MarkEntryId",
                table: "MarkAuditLogs",
                column: "MarkEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkEntryDrafts_ExamId_StudentId_SubjectId",
                table: "MarkEntryDrafts",
                columns: new[] { "ExamId", "StudentId", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarkEntryDrafts_StudentId",
                table: "MarkEntryDrafts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkEntryDrafts_SubjectId",
                table: "MarkEntryDrafts",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_EnteredByTeacherId",
                table: "Marks",
                column: "EnteredByTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_ExamId_StudentId_SubjectId",
                table: "Marks",
                columns: new[] { "ExamId", "StudentId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Marks_StudentId",
                table: "Marks",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_SubjectId",
                table: "Marks",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MeritResults_ExamId_SectionId_Position",
                table: "MeritResults",
                columns: new[] { "ExamId", "SectionId", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_MeritResults_ExamId_StudentId",
                table: "MeritResults",
                columns: new[] { "ExamId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeritResults_SectionId",
                table: "MeritResults",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeritResults_StudentId",
                table: "MeritResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_FeeInvoiceId",
                table: "Payments",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionHistories_AcademicYearId",
                table: "PromotionHistories",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionHistories_FromClassId",
                table: "PromotionHistories",
                column: "FromClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionHistories_StudentId_AcademicYearId",
                table: "PromotionHistories",
                columns: new[] { "StudentId", "AcademicYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromotionHistories_ToClassId",
                table: "PromotionHistories",
                column: "ToClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ReEvaluationRequests_ExamId_StudentId_SubjectId",
                table: "ReEvaluationRequests",
                columns: new[] { "ExamId", "StudentId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReEvaluationRequests_StudentId",
                table: "ReEvaluationRequests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReEvaluationRequests_SubjectId",
                table: "ReEvaluationRequests",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportCards_ExamId",
                table: "ReportCards",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportCards_StudentId",
                table: "ReportCards",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultAuditLogs_ExamId",
                table: "ResultAuditLogs",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultAuditLogs_StudentId",
                table: "ResultAuditLogs",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultAuditLogs_SubjectId",
                table: "ResultAuditLogs",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultLocks_ExamId",
                table: "ResultLocks",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultPublications_ExamId",
                table: "ResultPublications",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Sections_ParentSectionId",
                table: "Sections",
                column: "ParentSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_SchoolClassId",
                table: "Sections",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_StudentId",
                table: "StudentDocuments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentExamResults_ExamId_StudentId",
                table: "StudentExamResults",
                columns: new[] { "ExamId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentExamResults_StudentId",
                table: "StudentExamResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupAssignments_AcademicYearId",
                table: "StudentGroupAssignments",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupAssignments_SchoolClassId",
                table: "StudentGroupAssignments",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupAssignments_StudentGroupId",
                table: "StudentGroupAssignments",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupAssignments_StudentId_SchoolClassId_AcademicYearId",
                table: "StudentGroupAssignments",
                columns: new[] { "StudentId", "SchoolClassId", "AcademicYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroups_Code",
                table: "StudentGroups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_AssignedReligionSubjectId",
                table: "Students",
                column: "AssignedReligionSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassId_SectionId_RollNumber",
                table: "Students",
                columns: new[] { "ClassId", "SectionId", "RollNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_SectionId",
                table: "Students",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentGroupId",
                table: "Students",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentNo",
                table: "Students",
                column: "StudentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectResults_ExamId_StudentId_SubjectId",
                table: "StudentSubjectResults",
                columns: new[] { "ExamId", "StudentId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectResults_StudentId",
                table: "StudentSubjectResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectResults_SubjectId",
                table: "StudentSubjectResults",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComponents_ClassSubjectId_ComponentName",
                table: "SubjectComponents",
                columns: new[] { "ClassSubjectId", "ComponentName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Code",
                table: "Subjects",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAttendances_TeacherId",
                table: "TeacherAttendances",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_AcademicYearId",
                table: "TeacherClassAssignments",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_ClassId",
                table: "TeacherClassAssignments",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_SectionId",
                table: "TeacherClassAssignments",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId",
                table: "TeacherClassAssignments",
                columns: new[] { "TeacherId", "ClassId", "SectionId", "AcademicYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherDocuments_TeacherId",
                table: "TeacherDocuments",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLeaves_ApprovedByUserId",
                table: "TeacherLeaves",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLeaves_TeacherId",
                table: "TeacherLeaves",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPerformances_AcademicYearId",
                table: "TeacherPerformances",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPerformances_EvaluatorUserId",
                table: "TeacherPerformances",
                column: "EvaluatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPerformances_TeacherId",
                table: "TeacherPerformances",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_EmployeeId",
                table: "Teachers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TeacherNo",
                table: "Teachers",
                column: "TeacherNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_UserId",
                table: "Teachers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSalaries_TeacherId",
                table: "TeacherSalaries",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_AcademicYearId",
                table: "TeacherSubjectAssignments",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_ClassId",
                table: "TeacherSubjectAssignments",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_SectionId",
                table: "TeacherSubjectAssignments",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_SubjectId",
                table: "TeacherSubjectAssignments",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId",
                table: "TeacherSubjectAssignments",
                columns: new[] { "TeacherId", "SubjectId", "ClassId", "SectionId", "AcademicYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTimetables_ClassId",
                table: "TeacherTimetables",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTimetables_SectionId",
                table: "TeacherTimetables",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTimetables_SubjectId",
                table: "TeacherTimetables",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTimetables_TeacherId_DayOfWeek_StartTime",
                table: "TeacherTimetables",
                columns: new[] { "TeacherId", "DayOfWeek", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_SessionId",
                table: "UserSessions",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "AdmissionDocuments");

            migrationBuilder.DropTable(
                name: "AdmissionListResults");

            migrationBuilder.DropTable(
                name: "AdmitCards");

            migrationBuilder.DropTable(
                name: "AssignmentSubmissions");

            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BackupRecords");

            migrationBuilder.DropTable(
                name: "BookIssues");

            migrationBuilder.DropTable(
                name: "BookReservations");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Circulars");

            migrationBuilder.DropTable(
                name: "ClassSubjectTeachers");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "EmployeeAcademicAssignments");

            migrationBuilder.DropTable(
                name: "EmployeeAttendances");

            migrationBuilder.DropTable(
                name: "EmployeeDocuments");

            migrationBuilder.DropTable(
                name: "EmployeeExperiences");

            migrationBuilder.DropTable(
                name: "EmployeeLeaves");

            migrationBuilder.DropTable(
                name: "EmployeeQualifications");

            migrationBuilder.DropTable(
                name: "EmployeeSalaries");

            migrationBuilder.DropTable(
                name: "ExamConfigurations");

            migrationBuilder.DropTable(
                name: "ExamSchedules");

            migrationBuilder.DropTable(
                name: "ExamSubjects");

            migrationBuilder.DropTable(
                name: "FeeStructures");

            migrationBuilder.DropTable(
                name: "FinalResults");

            migrationBuilder.DropTable(
                name: "FineRules");

            migrationBuilder.DropTable(
                name: "GpaConfigurations");

            migrationBuilder.DropTable(
                name: "GradingRules");

            migrationBuilder.DropTable(
                name: "Guardians");

            migrationBuilder.DropTable(
                name: "LeaveApplications");

            migrationBuilder.DropTable(
                name: "LessonPlans");

            migrationBuilder.DropTable(
                name: "MarkAuditLogs");

            migrationBuilder.DropTable(
                name: "MarkEntryDrafts");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "MeritResults");

            migrationBuilder.DropTable(
                name: "MessageItems");

            migrationBuilder.DropTable(
                name: "MessageThreads");

            migrationBuilder.DropTable(
                name: "Notices");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PromotionHistories");

            migrationBuilder.DropTable(
                name: "ReEvaluationRequests");

            migrationBuilder.DropTable(
                name: "ReportCards");

            migrationBuilder.DropTable(
                name: "ResultAuditLogs");

            migrationBuilder.DropTable(
                name: "ResultLocks");

            migrationBuilder.DropTable(
                name: "ResultPublications");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "RollNumberAssignments");

            migrationBuilder.DropTable(
                name: "SchoolProfiles");

            migrationBuilder.DropTable(
                name: "SeatingPlans");

            migrationBuilder.DropTable(
                name: "StudentDocuments");

            migrationBuilder.DropTable(
                name: "StudentExamResults");

            migrationBuilder.DropTable(
                name: "StudentGroupAssignments");

            migrationBuilder.DropTable(
                name: "StudentListItemResults");

            migrationBuilder.DropTable(
                name: "StudentPromotions");

            migrationBuilder.DropTable(
                name: "StudentRouteAssignments");

            migrationBuilder.DropTable(
                name: "StudentSubjectResults");

            migrationBuilder.DropTable(
                name: "StudyMaterials");

            migrationBuilder.DropTable(
                name: "SubjectComponents");

            migrationBuilder.DropTable(
                name: "Syllabi");

            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.DropTable(
                name: "TeacherAttendances");

            migrationBuilder.DropTable(
                name: "TeacherClassAssignments");

            migrationBuilder.DropTable(
                name: "TeacherDocuments");

            migrationBuilder.DropTable(
                name: "TeacherLeaves");

            migrationBuilder.DropTable(
                name: "TeacherPerformances");

            migrationBuilder.DropTable(
                name: "TeacherSalaries");

            migrationBuilder.DropTable(
                name: "TeacherSubjectAssignments");

            migrationBuilder.DropTable(
                name: "TeacherTimetables");

            migrationBuilder.DropTable(
                name: "TransferCertificates");

            migrationBuilder.DropTable(
                name: "TransportRoutes");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "VaccinationRecords");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Admissions");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "ExamTypes");

            migrationBuilder.DropTable(
                name: "Marks");

            migrationBuilder.DropTable(
                name: "FeeInvoices");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "ClassSubjects");

            migrationBuilder.DropTable(
                name: "AcademicYears");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "StudentGroups");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Designations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
