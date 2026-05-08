using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    PassportNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NationalIdNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthCertificateNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionDetails = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
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
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    StartsOn = table.Column<DateOnly>(type: "date", nullable: false),
                    EndsOn = table.Column<DateOnly>(type: "date", nullable: false),
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
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSchedules", x => x.Id);
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
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSubjects", x => x.Id);
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
                name: "GradingRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MinMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GradePoint = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                name: "Marks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    MarksObtained = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EnteredByTeacherId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marks", x => x.Id);
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
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultPublications", x => x.Id);
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
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    NationalIdNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                        name: "FK_Teachers_Users_UserId",
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
                name: "ClassSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    StudentGroupId = table.Column<int>(type: "int", nullable: true),
                    SectionId = table.Column<int>(type: "int", nullable: true),
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
                    PassportNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NationalIdNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                        name: "FK_Students_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
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
                    ApproverRemarks = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
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
                    Evaluator = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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

            migrationBuilder.InsertData(
                table: "AcademicYears",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EndsOn", "IsActive", "IsDeleted", "Name", "StartsOn", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "2026", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });

            migrationBuilder.InsertData(
                table: "Admissions",
                columns: new[] { "Id", "AdmissionFee", "AdmissionFeePaid", "AlternativeNumber", "ApplicantEmail", "ApplicantMobileNumber", "ApplicantName", "ApplicantNameBangla", "ApplicationNo", "AppliedClassId", "BirthCertificateNo", "BloodGroup", "Country", "CreatedAt", "CreatedBy", "DateOfBirth", "FatherName", "FatherOccupation", "FatherOrGuardianMobileNo", "Gender", "GuardianName", "GuardianOccupation", "IsDeleted", "MaritalStatus", "MotherName", "MotherOccupation", "NationalIdNo", "Nationality", "PassportNo", "PaymentMethod", "PermanentDistrict", "PermanentPostOffice", "PermanentThana", "PermanentVillage", "PresentDistrict", "PresentPostOffice", "PresentThana", "PresentVillage", "ProfilePicturePath", "Religion", "ReviewedAt", "ReviewedByUserId", "Status", "TransactionDetails", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, 1500m, false, null, null, "01800000010", "Pending Applicant", null, "APP-2026-0001", 1, null, null, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(2019, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Applicant Father", null, "01800000001", "Female", "Applicant Guardian", null, false, "Single", "Applicant Mother", null, null, "Bangladeshi", null, null, null, null, null, null, null, null, null, null, null, "Islam", null, null, 1, null, null, null });

            migrationBuilder.InsertData(
                table: "Attendance",
                columns: new[] { "Id", "AttendanceDate", "CreatedAt", "CreatedBy", "IsDeleted", "PeriodNo", "Remarks", "SchoolClassId", "SectionId", "Status", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 4, 25), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, null, null, 1, 1, 1, 1, null, null },
                    { 2, new DateOnly(2026, 4, 25), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, null, null, 1, 1, 2, 2, null, null }
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
                columns: new[] { "Id", "AcademicYearId", "CreatedAt", "CreatedBy", "EndsOn", "IsDeleted", "Name", "StartsOn", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateOnly(2026, 6, 12), false, "Midterm", new DateOnly(2026, 6, 1), null, null });

            migrationBuilder.InsertData(
                table: "FeeInvoices",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DueDate", "InvoiceNo", "IsDeleted", "PaidAmount", "Status", "StudentId", "TotalAmount", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateOnly(2026, 5, 10), "INV-2026-0001", false, 2500m, 3, 1, 2500m, null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateOnly(2026, 5, 10), "INV-2026-0002", false, 1000m, 2, 2, 2500m, null, null }
                });

            migrationBuilder.InsertData(
                table: "Marks",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EnteredByTeacherId", "ExamId", "IsDeleted", "MarksObtained", "Status", "StudentId", "SubjectId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 1, 1, false, 86m, 4, 1, 1, null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", 1, 1, false, 78m, 4, 2, 1, null, null }
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
                    { 261, "Manage", true, true, true, true, "System.Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "System", "System", null, null }
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
                table: "Subjects",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "BAN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "বাংলা", null, null },
                    { 2, "ENG", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "ইংরেজি", null, null },
                    { 3, "MAT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "গণিত", null, null },
                    { 4, "SCI", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "সাধারণ বিজ্ঞান", null, null },
                    { 5, "SOC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "বাংলাদেশ ও বিশ্ব পরিচয়", null, null },
                    { 6, "REL", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "ধর্ম ও নৈতিক শিক্ষা", null, null },
                    { 7, "ART", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "চারুকলা", null, null },
                    { 8, "PE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "শারীরিক শিক্ষা", null, null },
                    { 9, "BAN1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "বাংলা ১ম পত্র", null, null },
                    { 10, "BAN2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "বাংলা ২য় পত্র", null, null },
                    { 11, "ENG1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "ইংরেজি ১ম পত্র", null, null },
                    { 12, "ENG2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "ইংরেজি ২য় পত্র", null, null },
                    { 13, "SCI", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "বিজ্ঞান", null, null },
                    { 14, "ICT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "তথ্য ও যোগাযোগ প্রযুক্তি", null, null },
                    { 15, "AGR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "কৃষি শিক্ষা", null, null },
                    { 16, "PHY", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "পদার্থবিজ্ঞান", null, null },
                    { 17, "CHE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "রসায়ন", null, null },
                    { 18, "BIO", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "জীববিজ্ঞান", null, null },
                    { 19, "HMA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "উচ্চতর গণিত", null, null },
                    { 20, "ACC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "হিসাববিজ্ঞান", null, null },
                    { 21, "FIN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "ফাইন্যান্স", null, null },
                    { 22, "BUS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "ব্যবসায় উদ্যোগ", null, null },
                    { 23, "HIS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "ইতিহাস", null, null },
                    { 24, "GEO", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "ভূগোল ও পরিবেশ", null, null },
                    { 25, "ECO", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "অর্থনীতি", null, null },
                    { 26, "CIV", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "নাগরিকতা", null, null },
                    { 27, "CAREER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "ক্যারিয়ার শিক্ষা", null, null },
                    { 28, "HEALTH", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "শারীরিক শিক্ষা, স্বাস্থ্য ও খেলাধুলা", null, null },
                    { 29, "HSC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "গার্হস্থ্য বিজ্ঞান", null, null }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "AlternativeNumber", "BloodGroup", "Country", "CreatedAt", "CreatedBy", "DateOfBirth", "Department", "Designation", "EmailAddress", "FatherName", "FullName", "FullNameBangla", "Gender", "IsDeleted", "JoiningDate", "MaritalStatus", "MobileNumber", "MotherName", "NationalIdNo", "Nationality", "PassportNo", "PermanentDistrict", "PermanentPostOffice", "PermanentThana", "PermanentVillage", "PresentDistrict", "PresentPostOffice", "PresentThana", "PresentVillage", "ProfilePicturePath", "Qualification", "Religion", "Specialization", "SpouseName", "Status", "TeacherNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { 1, null, null, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Senior Lecturer", null, null, "Senior Lecturer", null, "", false, null, "", "01000000001", null, null, "Bangladeshi", null, null, null, null, null, null, null, null, null, null, null, "", null, null, 1, "T-0001", null, null, null },
                    { 2, null, null, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Lecturer", null, null, "Class Teacher", null, "", false, null, "", "01000000002", null, null, "Bangladeshi", null, null, null, null, null, null, null, null, null, null, null, "", null, null, 1, "T-0002", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ActivationToken", "ActivationTokenExpiry", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "IsEmailConfirmed", "LastLoginAt", "PasswordHash", "PhoneNumber", "Status", "UpdatedAt", "UpdatedBy", "UserName" },
                values: new object[] { 1, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "admin@school.local", false, true, null, "ChangeThisHash", null, 1, null, null, "admin" });

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
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "Name", "SchoolClassId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", 1, null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", 1, null, null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", 2, null, null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", 2, null, null },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", 3, null, null },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", 3, null, null },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", 4, null, null },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", 4, null, null },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", 5, null, null },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", 5, null, null },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", 6, null, null },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", 6, null, null },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", 7, null, null },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", 7, null, null },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", 8, null, null },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", 8, null, null },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", 9, null, null },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", 9, null, null },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Science", 9, null, null },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies", 9, null, null },
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities", 9, null, null },
                    { 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "A", 10, null, null },
                    { 23, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "B", 10, null, null },
                    { 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Science", 10, null, null },
                    { 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Business Studies", 10, null, null },
                    { 26, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", false, "Humanities", 10, null, null }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "AlternativeNumber", "BirthCertificateNo", "BloodGroup", "ClassId", "Country", "CreatedAt", "CreatedBy", "DateOfBirth", "EmailAddress", "FatherName", "FatherOccupation", "FullName", "FullNameBangla", "Gender", "IsDeleted", "MaritalStatus", "MobileNumber", "MotherName", "MotherOccupation", "NationalIdNo", "Nationality", "PassportNo", "PermanentDistrict", "PermanentPostOffice", "PermanentThana", "PermanentVillage", "PresentDistrict", "PresentPostOffice", "PresentThana", "PresentVillage", "ProfilePicturePath", "Religion", "RollNumber", "SectionId", "Status", "StudentNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { 1, null, null, null, 1, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(2018, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Father One", null, "Sample Student One", null, "Male", false, "Single", "01700000010", "Mother One", null, null, "Bangladeshi", null, null, null, null, null, null, null, null, null, null, "Islam", 1, 1, 1, "STU-2026-0001", null, null, null },
                    { 2, null, null, null, 1, "Bangladesh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", new DateTime(2018, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Father Two", null, "Sample Student Two", null, "Female", false, "Single", "01700000020", "Mother Two", null, null, "Bangladeshi", null, null, null, null, null, null, null, null, null, null, "Islam", 2, 1, 1, "STU-2026-0002", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Guardians",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "Name", "Occupation", "Phone", "Relation", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, false, "Guardian One", null, "01700000001", "Father", 1, null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, false, "Guardian Two", null, "01700000002", "Mother", 2, null, null }
                });

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
                name: "IX_FeeInvoices_InvoiceNo",
                table: "FeeInvoices",
                column: "InvoiceNo",
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
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_FeeInvoiceId",
                table: "Payments",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_SchoolClassId",
                table: "Sections",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_StudentId",
                table: "StudentDocuments",
                column: "StudentId");

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
                name: "IX_Students_StudentNo",
                table: "Students",
                column: "StudentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId");

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
                name: "IX_TeacherDocuments_TeacherId",
                table: "TeacherDocuments",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLeaves_TeacherId",
                table: "TeacherLeaves",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPerformances_AcademicYearId",
                table: "TeacherPerformances",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPerformances_TeacherId",
                table: "TeacherPerformances",
                column: "TeacherId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionDocuments");

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
                name: "Exams");

            migrationBuilder.DropTable(
                name: "ExamSchedules");

            migrationBuilder.DropTable(
                name: "ExamSubjects");

            migrationBuilder.DropTable(
                name: "FeeStructures");

            migrationBuilder.DropTable(
                name: "FineRules");

            migrationBuilder.DropTable(
                name: "GradingRules");

            migrationBuilder.DropTable(
                name: "Guardians");

            migrationBuilder.DropTable(
                name: "LeaveApplications");

            migrationBuilder.DropTable(
                name: "LessonPlans");

            migrationBuilder.DropTable(
                name: "Marks");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

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
                name: "ReportCards");

            migrationBuilder.DropTable(
                name: "ResultPublications");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SchoolProfiles");

            migrationBuilder.DropTable(
                name: "SeatingPlans");

            migrationBuilder.DropTable(
                name: "StudentDocuments");

            migrationBuilder.DropTable(
                name: "StudentPromotions");

            migrationBuilder.DropTable(
                name: "StudentRouteAssignments");

            migrationBuilder.DropTable(
                name: "StudyMaterials");

            migrationBuilder.DropTable(
                name: "Syllabi");

            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.DropTable(
                name: "TeacherAttendances");

            migrationBuilder.DropTable(
                name: "TeacherDocuments");

            migrationBuilder.DropTable(
                name: "TeacherLeaves");

            migrationBuilder.DropTable(
                name: "TeacherPerformances");

            migrationBuilder.DropTable(
                name: "TeacherSalaries");

            migrationBuilder.DropTable(
                name: "TransferCertificates");

            migrationBuilder.DropTable(
                name: "TransportRoutes");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "VaccinationRecords");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Admissions");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "ClassSubjects");

            migrationBuilder.DropTable(
                name: "FeeInvoices");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "AcademicYears");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "StudentGroups");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Classes");
        }
    }
}
