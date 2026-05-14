using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class MigrateTeacherDataToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Step 1: Ensure "Academic" Department and "Teacher" Designation exist ──────
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Departments WHERE Code = 'ACAD')
                    INSERT INTO Departments (Name, Code) VALUES ('Academic', 'ACAD');
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Designations WHERE Name = 'Teacher')
                    INSERT INTO Designations (Name) VALUES ('Teacher');
            ");

            // ── Step 2: Copy Teachers → Employees ────────────────────────────────────────
            // Uses TeacherNo as EmployeeCode. Skips any teacher already migrated.
            migrationBuilder.Sql(@"
                DECLARE @DeptId  BIGINT = (SELECT TOP 1 Id FROM Departments  WHERE Code = 'ACAD');
                DECLARE @DesigId BIGINT = (SELECT TOP 1 Id FROM Designations WHERE Name = 'Teacher');

                INSERT INTO Employees (
                    EmployeeCode, FullName, Gender, DateOfBirth, Phone, Email,
                    BloodGroup, Nationality,
                    PresentVillage, PresentPostOffice, PresentThana, PresentDistrict,
                    PermanentVillage, PermanentPostOffice, PermanentThana, PermanentDistrict,
                    JoiningDate, Salary, PhotoPath, IsActive, DepartmentId, DesignationId
                )
                SELECT
                    t.TeacherNo, t.FullName, t.Gender, t.DateOfBirth, t.MobileNumber, t.EmailAddress,
                    t.BloodGroup, t.Nationality,
                    t.PresentVillage, t.PresentPostOffice, t.PresentThana, t.PresentDistrict,
                    t.PermanentVillage, t.PermanentPostOffice, t.PermanentThana, t.PermanentDistrict,
                    ISNULL(t.JoiningDate, t.CreatedAt),
                    0,                                              -- Salary set via payroll later
                    t.ProfilePicturePath,
                    CASE WHEN t.Status = 0 THEN 1 ELSE 0 END,      -- 0 = Active enum value
                    @DeptId, @DesigId
                FROM Teachers t
                WHERE NOT EXISTS (
                    SELECT 1 FROM Employees e WHERE e.EmployeeCode = t.TeacherNo
                );
            ");

            // ── Step 3: Link ApplicationUsers to their new Employee records ──────────────
            migrationBuilder.Sql(@"
                UPDATE u
                SET    u.EmployeeId = e.Id
                FROM   Users u
                INNER JOIN Teachers  t ON t.UserId      = u.Id
                INNER JOIN Employees e ON e.EmployeeCode = t.TeacherNo
                WHERE  u.EmployeeId IS NULL;
            ");

            // ── Step 4: Copy TeacherDocuments → EmployeeDocuments ────────────────────────
            migrationBuilder.Sql(@"
                DECLARE @SystemUserId INT = (SELECT TOP 1 Id FROM Users ORDER BY Id);

                INSERT INTO EmployeeDocuments (
                    EmployeeId, DocumentType, FilePath, OriginalFileName, UploadedById, UploadedAt
                )
                SELECT
                    e.Id, td.DocumentType, td.FilePath, NULL,
                    ISNULL(t.UserId, @SystemUserId),
                    td.UploadedDate
                FROM TeacherDocuments td
                INNER JOIN Teachers  t ON t.Id          = td.TeacherId
                INNER JOIN Employees e ON e.EmployeeCode = t.TeacherNo
                WHERE NOT EXISTS (
                    SELECT 1 FROM EmployeeDocuments ed
                    WHERE ed.EmployeeId = e.Id AND ed.FilePath = td.FilePath
                );
            ");

            // ── Step 5: Copy latest TeacherSalary → EmployeeSalaryStructure ─────────────
            migrationBuilder.Sql(@"
                ;WITH LatestSalary AS (
                    SELECT ts.*,
                           ROW_NUMBER() OVER (PARTITION BY ts.TeacherProfileId ORDER BY ts.MonthYear DESC) AS rn
                    FROM TeacherSalaries ts
                )
                INSERT INTO EmployeeSalaryStructures (
                    EmployeeId, BasicSalary, HouseRent, MedicalAllowance,
                    TransportAllowance, OtherAllowance, TaxPercentage, ProvidentFund,
                    EffectiveFrom, IsActive, CreatedAt
                )
                SELECT
                    e.Id, ls.BasicSalary, 0, 0, 0, ls.Allowances, 0, 0,
                    ls.MonthYear, 1, GETUTCDATE()
                FROM LatestSalary ls
                INNER JOIN Teachers  t ON t.Id          = ls.TeacherProfileId
                INNER JOIN Employees e ON e.EmployeeCode = t.TeacherNo
                WHERE ls.rn = 1
                AND NOT EXISTS (
                    SELECT 1 FROM EmployeeSalaryStructures ess
                    WHERE ess.EmployeeId = e.Id AND ess.IsActive = 1
                );
            ");

            // ── Step 6: Copy TeacherAttendance → EmployeeAttendance ─────────────────────
            // AttendanceStatus enum: Present=0, Absent=1, Late=2, HalfDay=3, OnLeave=4
            migrationBuilder.Sql(@"
                INSERT INTO EmployeeAttendances (
                    EmployeeId, AttendanceDate, Status,
                    CheckInTime, CheckOutTime, Remarks, CreatedBy, CreatedAt
                )
                SELECT
                    e.Id, ta.AttendanceDate,
                    CASE ta.Status
                        WHEN 'Present'  THEN 0
                        WHEN 'Absent'   THEN 1
                        WHEN 'Late'     THEN 2
                        WHEN 'Half-Day' THEN 3
                        WHEN 'OnLeave'  THEN 4
                        ELSE 0
                    END,
                    NULL, NULL, ta.Remarks, 'migration', GETUTCDATE()
                FROM TeacherAttendances ta
                INNER JOIN Teachers  t ON t.Id          = ta.TeacherId
                INNER JOIN Employees e ON e.EmployeeCode = t.TeacherNo
                WHERE NOT EXISTS (
                    SELECT 1 FROM EmployeeAttendances ea
                    WHERE ea.EmployeeId = e.Id AND ea.AttendanceDate = ta.AttendanceDate
                );
            ");

            // ── Step 7: Copy TeacherLeaves → EmployeeLeaves ──────────────────────────────
            // LeaveStatus enum: Pending=0, Approved=1, Rejected=2
            migrationBuilder.Sql(@"
                -- Ensure any teacher leave types that don't already exist are created
                INSERT INTO LeaveTypes (Name, DefaultDaysPerYear, IsPaid, IsActive)
                SELECT DISTINCT tl.LeaveType, 0, 1, 1
                FROM TeacherLeaves tl
                WHERE NOT EXISTS (
                    SELECT 1 FROM LeaveTypes lt WHERE lt.Name = tl.LeaveType
                );

                INSERT INTO EmployeeLeaves (
                    EmployeeId, LeaveTypeId, StartDate, EndDate, TotalDays,
                    Reason, Status, ApprovedById, ApprovedAt, RejectionReason, Remarks, CreatedAt
                )
                SELECT
                    e.Id, lt.Id, tl.StartDate, tl.EndDate,
                    DATEDIFF(DAY, tl.StartDate, tl.EndDate) + 1,
                    tl.Reason,
                    CASE tl.Status
                        WHEN 'Approved' THEN 1
                        WHEN 'Rejected' THEN 2
                        ELSE 0
                    END,
                    tl.ApprovedByUserId, tl.ApprovedDate, NULL, tl.ApproverRemarks, GETUTCDATE()
                FROM TeacherLeaves tl
                INNER JOIN Teachers  t  ON t.Id          = tl.TeacherProfileId
                INNER JOIN Employees e  ON e.EmployeeCode = t.TeacherNo
                INNER JOIN LeaveTypes lt ON lt.Name       = tl.LeaveType
                WHERE NOT EXISTS (
                    SELECT 1 FROM EmployeeLeaves el
                    WHERE el.EmployeeId = e.Id AND el.StartDate = tl.StartDate
                      AND el.EndDate = tl.EndDate AND el.LeaveTypeId = lt.Id
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove all migrated data — children first to respect FK Restrict constraints.
            migrationBuilder.Sql(@"
                -- 1. Unlink Users from migrated employees
                UPDATE u SET u.EmployeeId = NULL
                FROM   Users u
                INNER JOIN Employees e ON e.Id          = u.EmployeeId
                INNER JOIN Teachers  t ON t.TeacherNo   = e.EmployeeCode;

                -- 2. Remove migrated salary structures
                DELETE ess FROM EmployeeSalaryStructures ess
                INNER JOIN Employees e ON e.Id = ess.EmployeeId
                INNER JOIN Teachers  t ON t.TeacherNo = e.EmployeeCode;

                -- 3. Remove migrated attendance records
                DELETE ea FROM EmployeeAttendances ea
                INNER JOIN Employees e ON e.Id = ea.EmployeeId
                INNER JOIN Teachers  t ON t.TeacherNo = e.EmployeeCode;

                -- 4. Remove migrated leave records
                DELETE el FROM EmployeeLeaves el
                INNER JOIN Employees e ON e.Id = el.EmployeeId
                INNER JOIN Teachers  t ON t.TeacherNo = e.EmployeeCode;

                -- 5. Remove migrated documents
                DELETE ed FROM EmployeeDocuments ed
                INNER JOIN Employees e ON e.Id = ed.EmployeeId
                INNER JOIN Teachers  t ON t.TeacherNo = e.EmployeeCode;

                -- 6. Remove the migrated employee rows themselves
                DELETE e FROM Employees e
                INNER JOIN Teachers t ON t.TeacherNo = e.EmployeeCode;
            ");
        }
    }
}
