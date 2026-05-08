-- ======================================================
-- Stored Procedures for Result Management Module
-- ======================================================

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetExamsForAdmin')
    DROP PROCEDURE sp_GetExamsForAdmin;
GO

CREATE PROCEDURE sp_GetExamsForAdmin
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

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetMarkEntrySheet')
    DROP PROCEDURE sp_GetMarkEntrySheet;
GO

CREATE PROCEDURE sp_GetMarkEntrySheet
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

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CalculateExamRanking')
    DROP PROCEDURE sp_CalculateExamRanking;
GO

CREATE PROCEDURE sp_CalculateExamRanking
    @ExamId INT
AS
BEGIN
    -- 1. Clear existing results for this exam (or we could merge)
    -- DELETE FROM StudentExamResults WHERE ExamId = @ExamId;

    -- 2. Calculate and Update/Insert StudentExamResults
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
        WHERE m.ExamId = @ExamId AND m.IsDeleted = 0 AND m.Status >= 2 -- At least Submitted
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
            Status = 4, -- Published
            PublishedAt = GETUTCDATE(),
            CalculatedAt = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT (ExamId, StudentId, TotalMarks, TotalFullMarks, Gpa, Grade, IsPassed, Position, Status, PublishedAt, CreatedAt, CalculatedAt, IsDeleted)
        VALUES (@ExamId, source.StudentId, source.TotalMarks, source.TotalFullMarks, source.Gpa, ISNULL(source.CalculatedGrade, 'F'), source.IsPassed, 0, 4, GETUTCDATE(), GETUTCDATE(), GETUTCDATE(), 0);

    -- 3. Calculate Positions within each class
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

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetStudentReportCard')
    DROP PROCEDURE sp_GetStudentReportCard;
GO

CREATE PROCEDURE sp_GetStudentReportCard
    @ExamId INT,
    @StudentId INT
AS
BEGIN
    -- Return Header Info
    SELECT 
        s.FullName, s.StudentNo, c.Name as ClassName, sec.Name as SectionName, s.RollNumber,
        e.Name as ExamName, r.TotalMarks, r.Gpa, r.Position, r.IsPassed
    FROM Students s
    JOIN SchoolClasses c ON s.ClassId = c.Id
    JOIN Sections sec ON s.SectionId = sec.Id
    JOIN Exams e ON e.Id = @ExamId
    LEFT JOIN StudentExamResults r ON r.StudentId = s.Id AND r.ExamId = @ExamId
    WHERE s.Id = @StudentId;

    -- Return Subject Details
    SELECT 
        sub.Name as SubjectName,
        m.MarksObtained,
        m.Grade,
        m.GradePoint
    FROM Marks m
    JOIN Subjects sub ON m.SubjectId = sub.Id
    WHERE m.ExamId = @ExamId AND m.StudentId = @StudentId
    ORDER BY sub.Name;
END;
GO
