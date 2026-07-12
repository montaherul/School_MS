using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExamGroupEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DesignationId",
                table: "Employees");

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "Subjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "NctbCode",
                table: "Subjects",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PassMarks",
                table: "Subjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PracticalMarks",
                table: "Subjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TheoryMarks",
                table: "Subjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewGroupId",
                table: "PromotionHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewRollNumber",
                table: "PromotionHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewSectionId",
                table: "PromotionHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromotioSessionId",
                table: "PromotionHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RollGenerationMethod",
                table: "PromotionHistories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromotioSessionId",
                table: "FinalResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlternateMobile",
                table: "Employees",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BanglaName",
                table: "Employees",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrivingLicenseNo",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNo",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpouseName",
                table: "Employees",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TIN",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHigherSecondary",
                table: "Classes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AcademicYears",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "AcademicYears",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "AcademicYears",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AcademicYears",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalFloors = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassProgressionRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromClassId = table.Column<int>(type: "int", nullable: false),
                    ToClassId = table.Column<int>(type: "int", nullable: false),
                    ProgressionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassProgressionRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassProgressionRules_Classes_FromClassId",
                        column: x => x.FromClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassProgressionRules_Classes_ToClassId",
                        column: x => x.ToClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAwards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AwardName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AwardedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AwardDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CertificatePath = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAwards_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBankAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoutingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeBankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeBankAccounts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDisciplinaryActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocumentPath = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDisciplinaryActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDisciplinaryActions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePromotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PreviousDesignationId = table.Column<int>(type: "int", nullable: false),
                    NewDesignationId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PromotionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    NewSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeePromotions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTrainings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    TrainingName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InstitutionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertificatePath = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTrainings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTrainings_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTransfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FromDepartmentId = table.Column<int>(type: "int", nullable: false),
                    ToDepartmentId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTransfers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Term = table.Column<int>(type: "int", nullable: false),
                    ExamType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ExamGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamGroups_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Term = table.Column<int>(type: "int", nullable: false),
                    ExamType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClassIdsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateDataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromotioSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    SessionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PromotionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExecutedByUserId = table.Column<int>(type: "int", nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_PromotioSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotioSessions_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportCardPrintQueueItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: true),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    CompletedItems = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportCardPrintQueueItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolSessions_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SchoolShifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
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
                    table.PrimaryKey("PK_SchoolShifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_SubjectCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamGroupClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamGroupId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamGroupClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamGroupClasses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamGroupClasses_ExamGroups_ExamGroupId",
                        column: x => x.ExamGroupId,
                        principalTable: "ExamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamGroupSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamGroupClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamGroupSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamGroupSections_ExamGroupClasses_ExamGroupClassId",
                        column: x => x.ExamGroupClassId,
                        principalTable: "ExamGroupClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamGroupSections_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamGroupSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamGroupClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    IsReligionSubject = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ExamGroupSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamGroupSubjects_ExamGroupClasses_ExamGroupClassId",
                        column: x => x.ExamGroupClassId,
                        principalTable: "ExamGroupClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamGroupSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamGroupSubjects_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ExamGroupSubjectComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamGroupSubjectId = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    MaxMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PassMarks = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamGroupSubjectComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamGroupSubjectComponents_ExamComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "ExamComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamGroupSubjectComponents_ExamGroupSubjects_ExamGroupSubjectId",
                        column: x => x.ExamGroupSubjectId,
                        principalTable: "ExamGroupSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AcademicYears",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "IsCurrent", "IsLocked", "Status" },
                values: new object[] { "", false, false, "Active" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsHigherSecondary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsHigherSecondary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsHigherSecondary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsHigherSecondary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsHigherSecondary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsHigherSecondary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsHigherSecondary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsHigherSecondary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsHigherSecondary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 10,
                column: "IsHigherSecondary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Vocational", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Vocational", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Religion", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Religion", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Religion", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Religion", 1m, null, 33m, 0m, 100m });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Category", "Credit", "NctbCode", "PassMarks", "PracticalMarks", "TheoryMarks" },
                values: new object[] { "Core", 1m, null, 33m, 0m, 100m });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingId",
                table: "Rooms",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionHistories_PromotioSessionId",
                table: "PromotionHistories",
                column: "PromotioSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalResults_PromotioSessionId",
                table: "FinalResults",
                column: "PromotioSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BirthCertificateNo",
                table: "Employees",
                column: "BirthCertificateNo",
                unique: true,
                filter: "[BirthCertificateNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DesignationId",
                table: "Employees",
                column: "DesignationId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DrivingLicenseNo",
                table: "Employees",
                column: "DrivingLicenseNo",
                unique: true,
                filter: "[DrivingLicenseNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCardNumber",
                table: "Employees",
                column: "EmployeeCardNumber",
                unique: true,
                filter: "[EmployeeCardNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IsTeachingStaff",
                table: "Employees",
                column: "IsTeachingStaff",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_JoiningDate",
                table: "Employees",
                column: "JoiningDate",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PassportNo",
                table: "Employees",
                column: "PassportNo",
                unique: true,
                filter: "[PassportNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_QRVerificationCode",
                table: "Employees",
                column: "QRVerificationCode",
                unique: true,
                filter: "[QRVerificationCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Status",
                table: "Employees",
                column: "Status",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TIN",
                table: "Employees",
                column: "TIN",
                unique: true,
                filter: "[TIN] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                filter: "[UserId] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInvitations_Email",
                table: "EmployeeInvitations",
                column: "Email",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_Code",
                table: "Buildings",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ClassProgressionRules_FromClassId_ToClassId",
                table: "ClassProgressionRules",
                columns: new[] { "FromClassId", "ToClassId" },
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ClassProgressionRules_ToClassId",
                table: "ClassProgressionRules",
                column: "ToClassId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAwards_EmployeeId",
                table: "EmployeeAwards",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBankAccounts_EmployeeId",
                table: "EmployeeBankAccounts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDisciplinaryActions_EmployeeId",
                table: "EmployeeDisciplinaryActions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePromotions_EmployeeId",
                table: "EmployeePromotions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTrainings_EmployeeId",
                table: "EmployeeTrainings",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTransfers_EmployeeId",
                table: "EmployeeTransfers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupClasses_ClassId",
                table: "ExamGroupClasses",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupClasses_ExamGroupId_ClassId",
                table: "ExamGroupClasses",
                columns: new[] { "ExamGroupId", "ClassId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroups_AcademicYearId_Name",
                table: "ExamGroups",
                columns: new[] { "AcademicYearId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSections_ExamGroupClassId_SectionId",
                table: "ExamGroupSections",
                columns: new[] { "ExamGroupClassId", "SectionId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSections_SectionId",
                table: "ExamGroupSections",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSubjectComponents_ComponentId",
                table: "ExamGroupSubjectComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSubjectComponents_ExamGroupSubjectId_ComponentId",
                table: "ExamGroupSubjectComponents",
                columns: new[] { "ExamGroupSubjectId", "ComponentId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSubjects_ExamGroupClassId_SubjectId",
                table: "ExamGroupSubjects",
                columns: new[] { "ExamGroupClassId", "SubjectId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSubjects_SubjectId",
                table: "ExamGroupSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamGroupSubjects_TeacherId",
                table: "ExamGroupSubjects",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotioSessions_AcademicYearId_Status",
                table: "PromotioSessions",
                columns: new[] { "AcademicYearId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSessions_AcademicYearId_Name",
                table: "SchoolSessions",
                columns: new[] { "AcademicYearId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolShifts_Code",
                table: "SchoolShifts",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCategories_Code",
                table: "SubjectCategories",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCategories_Name_Code",
                table: "SubjectCategories",
                columns: new[] { "Name", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_FinalResults_PromotioSessions_PromotioSessionId",
                table: "FinalResults",
                column: "PromotioSessionId",
                principalTable: "PromotioSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionHistories_PromotioSessions_PromotioSessionId",
                table: "PromotionHistories",
                column: "PromotioSessionId",
                principalTable: "PromotioSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinalResults_PromotioSessions_PromotioSessionId",
                table: "FinalResults");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionHistories_PromotioSessions_PromotioSessionId",
                table: "PromotionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "ClassProgressionRules");

            migrationBuilder.DropTable(
                name: "EmployeeAwards");

            migrationBuilder.DropTable(
                name: "EmployeeBankAccounts");

            migrationBuilder.DropTable(
                name: "EmployeeDisciplinaryActions");

            migrationBuilder.DropTable(
                name: "EmployeePromotions");

            migrationBuilder.DropTable(
                name: "EmployeeTrainings");

            migrationBuilder.DropTable(
                name: "EmployeeTransfers");

            migrationBuilder.DropTable(
                name: "ExamGroupSections");

            migrationBuilder.DropTable(
                name: "ExamGroupSubjectComponents");

            migrationBuilder.DropTable(
                name: "ExamTemplates");

            migrationBuilder.DropTable(
                name: "PromotioSessions");

            migrationBuilder.DropTable(
                name: "ReportCardPrintQueueItems");

            migrationBuilder.DropTable(
                name: "SchoolSessions");

            migrationBuilder.DropTable(
                name: "SchoolShifts");

            migrationBuilder.DropTable(
                name: "SubjectCategories");

            migrationBuilder.DropTable(
                name: "ExamGroupSubjects");

            migrationBuilder.DropTable(
                name: "ExamGroupClasses");

            migrationBuilder.DropTable(
                name: "ExamGroups");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_BuildingId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_PromotionHistories_PromotioSessionId",
                table: "PromotionHistories");

            migrationBuilder.DropIndex(
                name: "IX_FinalResults_PromotioSessionId",
                table: "FinalResults");

            migrationBuilder.DropIndex(
                name: "IX_Employees_BirthCertificateNo",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DesignationId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DrivingLicenseNo",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_EmployeeCardNumber",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_IsTeachingStaff",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_JoiningDate",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PassportNo",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_QRVerificationCode",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Status",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_TIN",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_UserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeInvitations_Email",
                table: "EmployeeInvitations");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "NctbCode",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "PassMarks",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "PracticalMarks",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "TheoryMarks",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "NewGroupId",
                table: "PromotionHistories");

            migrationBuilder.DropColumn(
                name: "NewRollNumber",
                table: "PromotionHistories");

            migrationBuilder.DropColumn(
                name: "NewSectionId",
                table: "PromotionHistories");

            migrationBuilder.DropColumn(
                name: "PromotioSessionId",
                table: "PromotionHistories");

            migrationBuilder.DropColumn(
                name: "RollGenerationMethod",
                table: "PromotionHistories");

            migrationBuilder.DropColumn(
                name: "PromotioSessionId",
                table: "FinalResults");

            migrationBuilder.DropColumn(
                name: "AlternateMobile",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BanglaName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseNo",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PassportNo",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SpouseName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "TIN",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsHigherSecondary",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AcademicYears");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 9,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 10,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 11,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 12,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 13,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 14,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 15,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 16,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 17,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 18,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 19,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 20,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 21,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 22,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 23,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 24,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 25,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 26,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 27,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 28,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 29,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 30,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 31,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 32,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 33,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 34,
                column: "Category",
                value: "");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DesignationId",
                table: "Employees",
                column: "DesignationId");
        }
    }
}
