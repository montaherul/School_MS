using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRoutineModuleEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 560, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 561, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 562, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 563, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 564, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 565, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 566, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 567, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 568, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 569, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 570, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 571, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 572, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 603, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 604, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 521, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 599, 21 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 600, 21 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 601, 22 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 602, 22 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 521, 27 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 522, 27 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 527, 27 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 529, 27 });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Building = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Floor = table.Column<int>(type: "int", nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsLab = table.Column<bool>(type: "bit", nullable: false),
                    RequiresDoublePeriod = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoutineGenerations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalAssignments = table.Column<int>(type: "int", nullable: false),
                    SuccessfulAssignments = table.Column<int>(type: "int", nullable: false),
                    FailedAssignments = table.Column<int>(type: "int", nullable: false),
                    ConflictsDetected = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineGenerations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoutinePeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PeriodNumber = table.Column<int>(type: "int", nullable: false),
                    IsBreak = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutinePeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoutineVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntryCount = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutineVersions_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    PeriodsPerWeek = table.Column<int>(type: "int", nullable: false),
                    RequiresLab = table.Column<bool>(type: "bit", nullable: false),
                    RequiresDoublePeriod = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    MaxConsecutive = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectRequirements_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectRequirements_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectRequirements_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectRequirements_StudentGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectRequirements_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectRequirements_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkingDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    DayName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    IsWorkingDay = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoutineConflicts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenerationId = table.Column<int>(type: "int", nullable: true),
                    ConflictType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    ClassId = table.Column<int>(type: "int", nullable: true),
                    RoutinePeriodId = table.Column<int>(type: "int", nullable: true),
                    DayNumber = table.Column<int>(type: "int", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineConflicts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutineConflicts_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineConflicts_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineConflicts_RoutinePeriods_RoutinePeriodId",
                        column: x => x.RoutinePeriodId,
                        principalTable: "RoutinePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineConflicts_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineConflicts_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoutineEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    RoutinePeriodId = table.Column<int>(type: "int", nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    IsLab = table.Column<bool>(type: "bit", nullable: false),
                    GenerationId = table.Column<int>(type: "int", nullable: true),
                    VersionId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutineEntries_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineEntries_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineEntries_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineEntries_RoutinePeriods_RoutinePeriodId",
                        column: x => x.RoutinePeriodId,
                        principalTable: "RoutinePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineEntries_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineEntries_StudentGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineEntries_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineEntries_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    RoutinePeriodId = table.Column<int>(type: "int", nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAvailabilities_RoutinePeriods_RoutinePeriodId",
                        column: x => x.RoutinePeriodId,
                        principalTable: "RoutinePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherAvailabilities_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubstituteAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoutineEntryId = table.Column<int>(type: "int", nullable: false),
                    OriginalTeacherId = table.Column<int>(type: "int", nullable: false),
                    SubstituteTeacherId = table.Column<int>(type: "int", nullable: false),
                    AssignedById = table.Column<int>(type: "int", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PeriodNumber = table.Column<int>(type: "int", nullable: true),
                    DayNumber = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstituteAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubstituteAssignments_RoutineEntries_RoutineEntryId",
                        column: x => x.RoutineEntryId,
                        principalTable: "RoutineEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubstituteAssignments_Teachers_OriginalTeacherId",
                        column: x => x.OriginalTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubstituteAssignments_Teachers_SubstituteTeacherId",
                        column: x => x.SubstituteTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubstituteAssignments_Users_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 521,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.View", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 522,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Read", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 523,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Create", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 524,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Edit", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 525,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Update", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 526,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Delete", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 527,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Approve", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 528,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Assign", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 529,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Publish", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 530,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Export", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 531,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Print", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 532,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Generate", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 533,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Routine.Manage", "Routine", "Routine" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 534,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.View", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 535,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Read", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 536,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Create", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 537,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Edit", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 538,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Update", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 539,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Delete", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 540,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Approve", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 541,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Assign", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 542,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Publish", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 543,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Export", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 544,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Print", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 545,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Generate", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 546,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Manage", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 547,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.View", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 548,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Read", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 549,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Create", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 550,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Edit", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 551,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Update", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 552,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Delete", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 553,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Approve", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 554,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Assign", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Publish", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 556,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Export", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 557,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Print", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 558,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Generate", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 559,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Manage", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 560,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.View", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 561,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Read", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 562,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Create", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 563,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Edit", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 564,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Update", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 565,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Delete", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 566,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Approve", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 567,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Assign", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 568,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Publish", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 569,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Export", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 570,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Print", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 571,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Generate", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 572,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Manage", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 573,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.View", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 574,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Read", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 575,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Create", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 576,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Edit", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 577,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Update", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 578,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Delete", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 579,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Approve", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 580,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Assign", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 581,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Publish", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 582,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Export", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 583,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Print", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 584,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Generate", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 585,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Manage", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 586,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.View", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 587,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Read", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 588,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Create", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 589,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Edit", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 590,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Update", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 591,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Delete", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 592,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Approve", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 593,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Assign", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 594,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Publish", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 595,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Export", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 596,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Print", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 597,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Generate", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 598,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Manage", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 599,
                columns: new[] { "Action", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, "Notification.View", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 600,
                columns: new[] { "Action", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Read", false, "Notification.Read", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 601,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Create", true, false, "Notification.Create", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 602,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Edit", false, false, false, "Notification.Edit", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 603,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Update", false, false, "Notification.Update", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 604,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Delete", false, true, false, false, "Notification.Delete", "Notification", "Notification" });

            migrationBuilder.Sql(@"
SET IDENTITY_INSERT [Permissions] ON;
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 605)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (605, 'Approve', 0, 0, 0, 1, 'Notification.Approve', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Notification', 'Notification', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 606)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (606, 'Assign', 0, 0, 0, 1, 'Notification.Assign', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Notification', 'Notification', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 607)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (607, 'Publish', 0, 0, 0, 1, 'Notification.Publish', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Notification', 'Notification', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 608)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (608, 'Export', 0, 0, 1, 0, 'Notification.Export', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Notification', 'Notification', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 609)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (609, 'Print', 0, 0, 1, 0, 'Notification.Print', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Notification', 'Notification', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 610)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (610, 'Generate', 1, 0, 1, 0, 'Notification.Generate', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Notification', 'Notification', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 611)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (611, 'Manage', 1, 1, 1, 1, 'Notification.Manage', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Notification', 'Notification', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 612)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (612, 'Issue', 0, 0, 1, 1, 'Library.Issue', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Library', 'Library', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 613)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (613, 'Return', 0, 0, 1, 1, 'Library.Return', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Library', 'Library', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 614)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (614, 'View', 0, 0, 1, 0, 'Laboratory.View', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Laboratory', 'Laboratory', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 615)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (615, 'Manage', 1, 1, 1, 1, 'Laboratory.Manage', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Laboratory', 'Laboratory', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 616)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (616, 'Regenerate', 1, 0, 1, 1, 'Calendar.Regenerate', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Calendar', 'Calendar', NULL, NULL);
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Id] = 617)
    INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
    VALUES (617, 'Repair', 1, 0, 1, 1, 'Calendar.Repair', '2026-01-01T00:00:00.0000000Z', 'system', 0, 'Calendar', 'Calendar', NULL, NULL);
SET IDENTITY_INSERT [Permissions] OFF;
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 573 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (573, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 574 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (574, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 575 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (575, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 576 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (576, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 577 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (577, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 578 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (578, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 579 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (579, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 580 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (580, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 581 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (581, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 582 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (582, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 583 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (583, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 584 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (584, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 585 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (585, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 534 AND [RoleId] = 4)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (534, 4);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 599 AND [RoleId] = 25)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (599, 25);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 534 AND [RoleId] = 27)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (534, 27);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 535 AND [RoleId] = 27)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (535, 27);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 540 AND [RoleId] = 27)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (540, 27);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 542 AND [RoleId] = 27)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (542, 27);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 605 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (605, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 606 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (606, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 607 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (607, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 608 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (608, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 609 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (609, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 610 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (610, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 611 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (611, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 612 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (612, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 613 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (613, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 614 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (614, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 615 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (615, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 616 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (616, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 617 AND [RoleId] = 1)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (617, 1);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 605 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (605, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 606 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (606, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 607 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (607, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 608 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (608, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 609 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (609, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 610 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (610, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 611 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (611, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 612 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (612, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 613 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (613, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 614 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (614, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 615 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (615, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 616 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (616, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 617 AND [RoleId] = 2)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (617, 2);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 616 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (616, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 617 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (617, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 612 AND [RoleId] = 21)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (612, 21);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 613 AND [RoleId] = 21)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (613, 21);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 614 AND [RoleId] = 22)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (614, 22);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 615 AND [RoleId] = 22)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (615, 22);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 605 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (605, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 606 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (606, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 607 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (607, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 608 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (608, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 609 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (609, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 610 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (610, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 611 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (611, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 612 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (612, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 613 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (613, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 614 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (614, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 615 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (615, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 616 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (616, 26);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 617 AND [RoleId] = 26)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (617, 26);
");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomNo",
                table: "Rooms",
                column: "RoomNo",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineConflicts_ClassId",
                table: "RoutineConflicts",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineConflicts_RoomId",
                table: "RoutineConflicts",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineConflicts_RoutinePeriodId",
                table: "RoutineConflicts",
                column: "RoutinePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineConflicts_SubjectId",
                table: "RoutineConflicts",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineConflicts_TeacherId",
                table: "RoutineConflicts",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_AcademicYearId_DayNumber_RoutinePeriodId_ClassId_SectionId_GroupId",
                table: "RoutineEntries",
                columns: new[] { "AcademicYearId", "DayNumber", "RoutinePeriodId", "ClassId", "SectionId", "GroupId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_AcademicYearId_DayNumber_RoutinePeriodId_RoomId",
                table: "RoutineEntries",
                columns: new[] { "AcademicYearId", "DayNumber", "RoutinePeriodId", "RoomId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_AcademicYearId_DayNumber_RoutinePeriodId_TeacherId",
                table: "RoutineEntries",
                columns: new[] { "AcademicYearId", "DayNumber", "RoutinePeriodId", "TeacherId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_ClassId",
                table: "RoutineEntries",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_GroupId",
                table: "RoutineEntries",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_RoomId",
                table: "RoutineEntries",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_RoutinePeriodId",
                table: "RoutineEntries",
                column: "RoutinePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_SectionId",
                table: "RoutineEntries",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_SubjectId",
                table: "RoutineEntries",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_TeacherId",
                table: "RoutineEntries",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutinePeriods_PeriodNumber_IsBreak",
                table: "RoutinePeriods",
                columns: new[] { "PeriodNumber", "IsBreak" });

            migrationBuilder.CreateIndex(
                name: "IX_RoutineVersions_AcademicYearId",
                table: "RoutineVersions",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRequirements_AcademicYearId_ClassId_SectionId_GroupId_SubjectId_TeacherId",
                table: "SubjectRequirements",
                columns: new[] { "AcademicYearId", "ClassId", "SectionId", "GroupId", "SubjectId", "TeacherId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRequirements_ClassId",
                table: "SubjectRequirements",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRequirements_GroupId",
                table: "SubjectRequirements",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRequirements_SectionId",
                table: "SubjectRequirements",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRequirements_SubjectId",
                table: "SubjectRequirements",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRequirements_TeacherId",
                table: "SubjectRequirements",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteAssignments_AssignedById",
                table: "SubstituteAssignments",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteAssignments_OriginalTeacherId",
                table: "SubstituteAssignments",
                column: "OriginalTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteAssignments_RoutineEntryId",
                table: "SubstituteAssignments",
                column: "RoutineEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteAssignments_SubstituteTeacherId",
                table: "SubstituteAssignments",
                column: "SubstituteTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAvailabilities_RoutinePeriodId",
                table: "TeacherAvailabilities",
                column: "RoutinePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAvailabilities_TeacherId_DayNumber_RoutinePeriodId",
                table: "TeacherAvailabilities",
                columns: new[] { "TeacherId", "DayNumber", "RoutinePeriodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkingDays_AcademicYearId_DayNumber",
                table: "WorkingDays",
                columns: new[] { "AcademicYearId", "DayNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutineConflicts");

            migrationBuilder.DropTable(
                name: "RoutineGenerations");

            migrationBuilder.DropTable(
                name: "RoutineVersions");

            migrationBuilder.DropTable(
                name: "SubjectRequirements");

            migrationBuilder.DropTable(
                name: "SubstituteAssignments");

            migrationBuilder.DropTable(
                name: "TeacherAvailabilities");

            migrationBuilder.DropTable(
                name: "WorkingDays");

            migrationBuilder.DropTable(
                name: "RoutineEntries");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "RoutinePeriods");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 605, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 606, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 607, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 608, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 609, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 610, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 611, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 612, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 613, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 614, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 615, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 616, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 617, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 605, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 606, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 607, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 608, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 609, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 610, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 611, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 612, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 613, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 614, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 615, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 616, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 617, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 573, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 574, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 575, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 576, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 577, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 578, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 579, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 580, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 581, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 582, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 583, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 584, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 585, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 616, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 617, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 534, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 612, 21 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 613, 21 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 614, 22 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 615, 22 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 599, 25 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 605, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 606, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 607, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 608, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 609, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 610, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 611, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 612, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 613, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 614, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 615, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 616, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 617, 26 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 534, 27 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 535, 27 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 540, 27 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 542, 27 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 605);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 606);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 607);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 608);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 609);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 610);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 611);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 612);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 613);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 614);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 615);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 616);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 617);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 521,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.View", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 522,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Read", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 523,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Create", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 524,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Edit", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 525,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Update", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 526,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Delete", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 527,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Approve", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 528,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Assign", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 529,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Publish", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 530,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Export", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 531,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Print", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 532,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Generate", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 533,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Results.Manage", "Results", "Results" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 534,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.View", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 535,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Read", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 536,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Create", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 537,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Edit", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 538,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Update", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 539,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Delete", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 540,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Approve", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 541,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Assign", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 542,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Publish", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 543,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Export", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 544,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Print", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 545,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Generate", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 546,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Leave.Manage", "Leave", "Leave" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 547,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.View", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 548,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Read", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 549,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Create", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 550,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Edit", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 551,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Update", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 552,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Delete", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 553,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Approve", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 554,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Assign", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Publish", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 556,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Export", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 557,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Print", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 558,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Generate", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 559,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notice.Manage", "Notice", "Notice" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 560,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.View", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 561,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Read", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 562,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Create", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 563,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Edit", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 564,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Update", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 565,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Delete", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 566,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Approve", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 567,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Assign", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 568,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Publish", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 569,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Export", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 570,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Print", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 571,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Generate", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 572,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Calendar.Manage", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 573,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.View", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 574,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Read", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 575,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Create", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 576,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Edit", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 577,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Update", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 578,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Delete", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 579,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Approve", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 580,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Assign", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 581,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Publish", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 582,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Export", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 583,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Print", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 584,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Generate", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 585,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Profile.Manage", "Profile", "Profile" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 586,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.View", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 587,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Read", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 588,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Create", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 589,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Edit", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 590,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Update", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 591,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Delete", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 592,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Approve", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 593,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Assign", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 594,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Publish", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 595,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Export", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 596,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Print", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 597,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Generate", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 598,
                columns: new[] { "Code", "Module", "ModuleName" },
                values: new object[] { "Notification.Manage", "Notification", "Notification" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 599,
                columns: new[] { "Action", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Issue", true, "Library.Issue", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 600,
                columns: new[] { "Action", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Return", true, "Library.Return", "Library", "Library" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 601,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "View", false, true, "Laboratory.View", "Laboratory", "Laboratory" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 602,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Manage", true, true, true, "Laboratory.Manage", "Laboratory", "Laboratory" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 603,
                columns: new[] { "Action", "CanCreate", "CanRead", "Code", "Module", "ModuleName" },
                values: new object[] { "Regenerate", true, true, "Calendar.Regenerate", "Calendar", "Calendar" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 604,
                columns: new[] { "Action", "CanCreate", "CanDelete", "CanRead", "CanUpdate", "Code", "Module", "ModuleName" },
                values: new object[] { "Repair", true, false, true, true, "Calendar.Repair", "Calendar", "Calendar" });

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 560 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (560, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 561 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (561, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 562 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (562, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 563 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (563, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 564 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (564, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 565 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (565, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 566 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (566, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 567 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (567, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 568 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (568, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 569 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (569, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 570 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (570, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 571 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (571, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 572 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (572, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 603 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (603, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 604 AND [RoleId] = 3)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (604, 3);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 521 AND [RoleId] = 4)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (521, 4);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 599 AND [RoleId] = 21)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (599, 21);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 600 AND [RoleId] = 21)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (600, 21);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 601 AND [RoleId] = 22)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (601, 22);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 602 AND [RoleId] = 22)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (602, 22);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 521 AND [RoleId] = 27)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (521, 27);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 522 AND [RoleId] = 27)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (522, 27);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 527 AND [RoleId] = 27)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (527, 27);
IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [PermissionId] = 529 AND [RoleId] = 27)
    INSERT INTO [RolePermissions] ([PermissionId], [RoleId]) VALUES (529, 27);
");
        }
    }
}
