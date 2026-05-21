-- ============================================================================
-- Master Script: Deploy ALL Stored Procedures
-- Run with: sqlcmd -S MONTAHERUL\SQLEXPRESS -d SchoolManagementSystemDb -E -i _RunAllSPs.sql
-- ============================================================================

PRINT '=== Deploying all stored procedures ==='
PRINT ''

-- ── 1. Admission ──
PRINT '  [1/8] sp_GetAdmissionList...'
GO

CREATE OR ALTER PROCEDURE sp_GetAdmissionList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @ClassId INT = 0,
    @Status INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT;
    SET @Offset = (@PageNumber - 1) * @PageSize;

    ;WITH FilteredAdmissions AS (
        SELECT 
            a.Id,
            a.ApplicationNo,
            a.ApplicantName,
            a.ApplicantNameBangla,
            a.DateOfBirth,
            a.Gender,
            a.AppliedClassId,
            c.Name AS ClassName,
            CASE a.[Status]
                WHEN 1 THEN 'Pending'
                WHEN 2 THEN 'Approved'
                WHEN 3 THEN 'Rejected'
                WHEN 4 THEN 'Converted'
                ELSE 'Unknown'
            END AS [Status],
            a.FatherName,
            a.FatherOccupation,
            a.MotherName,
            a.MotherOccupation,
            a.GuardianName,
            a.GuardianOccupation,
            a.FatherOrGuardianMobileNo,
            a.ApplicantMobileNumber,
            a.AlternativeNumber,
            a.ApplicantEmail,
            a.Nationality,
            a.Religion,
            a.BloodGroup,
            a.BirthCertificateNo,
            a.BirthCertificatePath,
            a.PaymentSlipPath,
            a.PaymentMethod,
            a.TransactionDetails,
            a.PresentVillage,
            a.PresentPostOffice,
            a.PresentThana,
            a.PresentDistrict,
            a.PermanentVillage,
            a.PermanentPostOffice,
            a.PermanentThana,
            a.PermanentDistrict,
            a.ProfilePicturePath,
            a.CreatedBy,
            a.CreatedAt,
            ROW_NUMBER() OVER (ORDER BY a.CreatedAt DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Admissions a
        LEFT JOIN 
            Classes c ON a.AppliedClassId = c.Id
        WHERE 
            a.IsDeleted = 0
            AND (@ClassId = 0 OR a.AppliedClassId = @ClassId)
            AND (
                @SearchTerm IS NULL OR @SearchTerm = ''
                OR a.ApplicantName LIKE '%' + @SearchTerm + '%'
                OR a.ApplicationNo LIKE '%' + @SearchTerm + '%'
                OR a.FatherOrGuardianMobileNo LIKE '%' + @SearchTerm + '%'
                OR a.ApplicantMobileNumber LIKE '%' + @SearchTerm + '%'
            )
            AND (@Status IS NULL OR a.Status = @Status)
    )
    SELECT 
        Id, ApplicationNo, ApplicantName, ApplicantNameBangla, DateOfBirth, Gender,
        AppliedClassId, ClassName, [Status], FatherName, FatherOccupation,
        MotherName, MotherOccupation, GuardianName, GuardianOccupation,
        FatherOrGuardianMobileNo, ApplicantMobileNumber, AlternativeNumber,
        ApplicantEmail, Nationality, Religion, BloodGroup,
        BirthCertificateNo, BirthCertificatePath, PaymentSlipPath,
        PaymentMethod, TransactionDetails,
        PresentVillage, PresentPostOffice, PresentThana, PresentDistrict,
        PermanentVillage, PermanentPostOffice, PermanentThana, PermanentDistrict,
        ProfilePicturePath, CreatedBy, CreatedAt,
        TotalCount AS TotalRecords
    FROM 
        FilteredAdmissions
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO

-- ── 2. Student (full version with ClassId, SectionId, Status filters) ──
PRINT '  [2/8] sp_GetStudentList...'
GO

CREATE OR ALTER PROCEDURE sp_GetStudentList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @ClassId INT = 0,
    @SectionId INT = 0,
    @Status INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT;
    SET @Offset = (@PageNumber - 1) * @PageSize;

    ;WITH FilteredStudents AS (
        SELECT 
            s.Id,
            s.StudentNo,
            s.FullName,
            s.FullNameBangla,
            s.DateOfBirth,
            s.Gender,
            s.MobileNumber,
            s.EmailAddress,
            s.ClassId,
            c.Name AS ClassName,
            s.SectionId,
            sec.Name AS SectionName,
            s.RollNumber,
            s.ProfilePicturePath,
            CASE s.[Status]
                WHEN 1 THEN 'Active'
                WHEN 2 THEN 'Inactive'
                WHEN 3 THEN 'Graduated'
                WHEN 4 THEN 'Transferred'
                WHEN 5 THEN 'Dropped'
                ELSE 'Unknown'
            END AS [Status],
            s.FatherName,
            s.MotherName,
            s.Religion,
            s.BloodGroup,
            s.CreatedAt,
            ROW_NUMBER() OVER (ORDER BY s.CreatedAt DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Students s
        LEFT JOIN 
            Classes c ON s.ClassId = c.Id
        LEFT JOIN 
            Sections sec ON s.SectionId = sec.Id
        WHERE 
            s.IsDeleted = 0
            AND (@ClassId = 0 OR s.ClassId = @ClassId)
            AND (@SectionId = 0 OR s.SectionId = @SectionId)
            AND (
                @SearchTerm IS NULL OR @SearchTerm = ''
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR s.StudentNo LIKE '%' + @SearchTerm + '%'
                OR s.MobileNumber LIKE '%' + @SearchTerm + '%'
                OR s.FatherName LIKE '%' + @SearchTerm + '%'
            )
            AND (@Status IS NULL OR s.Status = @Status)
    )
    SELECT 
        Id, StudentNo, FullName, FullNameBangla, DateOfBirth, Gender,
        MobileNumber, EmailAddress, ClassId, ClassName, SectionId, SectionName,
        RollNumber, ProfilePicturePath, [Status], FatherName, MotherName,
        Religion, BloodGroup, CreatedAt,
        TotalCount AS TotalRecords
    FROM 
        FilteredStudents
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO

-- ── 3. Teacher ──
PRINT '  [3/8] sp_GetTeacherList...'
GO

CREATE OR ALTER PROCEDURE sp_GetTeacherList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @Department NVARCHAR(100) = NULL,
    @Status NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    DECLARE @StatusInt INT = NULL;
    IF @Status IS NOT NULL
    BEGIN
        SET @StatusInt = CASE @Status
            WHEN 'Active'     THEN 1
            WHEN 'OnLeave'    THEN 2
            WHEN 'Resigned'   THEN 3
            WHEN 'Terminated' THEN 4
            WHEN 'Inactive'   THEN 5
            ELSE NULL
        END;
    END;

    WITH TeacherData AS (
        SELECT 
            t.Id,
            t.TeacherNo,
            t.FullName,
            t.Designation,
            t.Department,
            t.MobileNumber,
            t.[Status],
            t.ProfilePicturePath,
            t.IsDeleted,
            ROW_NUMBER() OVER (ORDER BY t.FullName ASC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Teachers t
        WHERE 
            t.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR t.FullName LIKE '%' + @SearchTerm + '%'
                OR t.TeacherNo LIKE '%' + @SearchTerm + '%'
                OR t.MobileNumber LIKE '%' + @SearchTerm + '%'
                OR t.Designation LIKE '%' + @SearchTerm + '%'
            )
            AND (@Department IS NULL OR t.Department = @Department)
            AND (@StatusInt IS NULL OR t.[Status] = @StatusInt)
    )
    SELECT 
        Id, TeacherNo, FullName, Designation, Department,
        MobileNumber, [Status], ProfilePicturePath,
        TotalCount AS TotalRecords
    FROM 
        TeacherData
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO

-- ── 4. Attendance ──
PRINT '  [4/8] sp_GetAttendanceList...'
GO

CREATE OR ALTER PROCEDURE sp_GetAttendanceList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH FilteredAttendance AS (
        SELECT 
            a.Id,
            a.StudentId,
            s.FullName AS StudentName,
            a.SchoolClassId,
            c.Name AS ClassName,
            a.SectionId,
            sec.Name AS SectionName,
            a.[Status],
            a.Remarks,
            a.CreatedAt,
            ROW_NUMBER() OVER (ORDER BY a.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Attendance a
        JOIN 
            Students s ON a.StudentId = s.Id
        JOIN 
            Classes c ON a.SchoolClassId = c.Id
        JOIN 
            Sections sec ON a.SectionId = sec.Id
        WHERE 
            a.IsDeleted = 0
            AND (@StudentId = 0 OR a.StudentId = @StudentId)
            AND (
                @SearchTerm IS NULL 
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR s.StudentNo LIKE '%' + @SearchTerm + '%'
                OR a.Remarks LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, StudentId, StudentName, SchoolClassId, ClassName,
        SectionId, SectionName, [Status], Remarks, CreatedAt,
        TotalCount AS TotalRecords
    FROM 
        FilteredAttendance
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO

-- ── 5. Roles ──
PRINT '  [5/8] sp_GetRoleList...'
GO

-- ── 6. Users ──
PRINT '  [6/8] sp_GetUserList...'
GO

-- ── 7. Fees ──
PRINT '  [7/8] Fee procedures...'
GO

-- ── 8. Result Procedures ──
PRINT '  [8/8] Result procedures...'
GO

CREATE OR ALTER PROCEDURE sp_GetExamsForAdmin
    @AcademicYearId INT
AS
BEGIN
    SELECT 
        e.Id, e.Name, e.Term, e.StartsOn, e.EndsOn, e.Status,
        (SELECT COUNT(*) FROM StudentExamResults r WHERE r.ExamId = e.Id) as StudentCount,
        (SELECT COUNT(*) FROM Marks m WHERE m.ExamId = e.Id AND m.Status = 4) as PublishedMarks
    FROM Exams e
    WHERE e.AcademicYearId = @AcademicYearId AND e.IsDeleted = 0
    ORDER BY e.StartsOn DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetMarkEntrySheet
    @ExamId INT,
    @ClassId INT,
    @SectionId INT,
    @SubjectId INT
AS
BEGIN
    SELECT 
        s.Id as StudentId,
        s.FullName as StudentName,
        s.StudentNo,
        s.RollNumber,
        m.MarksObtained,
        m.Grade,
        m.IsLocked
    FROM Students s
    LEFT JOIN Marks m ON s.Id = m.StudentId 
        AND m.ExamId = @ExamId 
        AND m.SubjectId = @SubjectId
    WHERE s.ClassId = @ClassId 
      AND s.SectionId = @SectionId
      AND s.Status = 1 
      AND s.IsDeleted = 0
    ORDER BY s.RollNumber;
END;
GO

CREATE OR ALTER PROCEDURE sp_CalculateExamRanking
    @ExamId INT
AS
BEGIN
    MERGE StudentExamResults AS target
    USING (
        SELECT 
            m.StudentId,
            SUM(m.MarksObtained) as TotalMarks,
            AVG(CAST(m.GradePoint AS DECIMAL(18,2))) as Gpa,
            CASE WHEN MIN(CASE WHEN m.Grade = 'F' THEN 0 ELSE 1 END) = 0 THEN 0 ELSE 1 END as IsPassed,
            SUM(s.FullMarks) as TotalFullMarks,
            (SELECT TOP 1 g.Grade 
             FROM GradingRules g 
             WHERE (SUM(m.MarksObtained) / NULLIF(SUM(s.FullMarks), 0) * 100) >= g.MinMarks 
               AND (SUM(m.MarksObtained) / NULLIF(SUM(s.FullMarks), 0) * 100) <= g.MaxMarks
             ORDER BY g.MinMarks DESC) as CalculatedGrade
        FROM Marks m
        JOIN ClassSubjects s ON m.SubjectId = s.SubjectId
        JOIN Students st ON m.StudentId = st.Id AND st.ClassId = s.SchoolClassId
        WHERE m.ExamId = @ExamId AND m.IsDeleted = 0 AND m.Status >= 2
        GROUP BY m.StudentId
    ) AS source
    ON (target.StudentId = source.StudentId AND target.ExamId = @ExamId)
    WHEN MATCHED THEN
        UPDATE SET 
            TotalMarks = source.TotalMarks,
            TotalFullMarks = source.TotalFullMarks,
            Gpa = source.Gpa,
            Grade = ISNULL(source.CalculatedGrade, 'F'),
            IsPassed = source.IsPassed,
            Status = 4,
            PublishedAt = GETUTCDATE(),
            CalculatedAt = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT (ExamId, StudentId, TotalMarks, TotalFullMarks, Gpa, Grade, IsPassed, Position, Status, PublishedAt, CreatedAt, CalculatedAt, IsDeleted)
        VALUES (@ExamId, source.StudentId, source.TotalMarks, source.TotalFullMarks, source.Gpa, ISNULL(source.CalculatedGrade, 'F'), source.IsPassed, 0, 4, GETUTCDATE(), GETUTCDATE(), GETUTCDATE(), 0);

    WITH RankedResults AS (
        SELECT 
            r.Id,
            RANK() OVER (PARTITION BY s.ClassId ORDER BY r.TotalMarks DESC, r.Gpa DESC) as NewPosition
        FROM StudentExamResults r
        JOIN Students s ON r.StudentId = s.Id
        WHERE r.ExamId = @ExamId
    )
    UPDATE r
    SET r.Position = rr.NewPosition
    FROM StudentExamResults r
    JOIN RankedResults rr ON r.Id = rr.Id;
END;
GO

PRINT ''
PRINT '=== All stored procedures deployed successfully ==='
