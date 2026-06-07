CREATE OR ALTER PROCEDURE [dbo].[sp_GetResultSummary]
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Overall stats
    SELECT
        COUNT(ser.Id) AS TotalStudents,
        SUM(CASE WHEN ser.IsPassed = 1 THEN 1 ELSE 0 END) AS PassedCount,
        SUM(CASE WHEN ser.IsPassed = 0 THEN 1 ELSE 0 END) AS FailedCount,
        ROUND(AVG(ser.Gpa), 2) AS AverageGPA,
        MAX(ser.Gpa) AS HighestGPA,
        MIN(ser.Gpa) AS LowestGPA,
        ROUND(100.0 * SUM(CASE WHEN ser.IsPassed = 1 THEN 1 ELSE 0 END) / NULLIF(COUNT(ser.Id), 0), 1) AS PassPercentage
    FROM StudentExamResults ser
    WHERE ser.ExamId = @ExamId AND ser.IsDeleted = 0;

    -- Grade distribution
    SELECT ser.Grade, COUNT(*) AS Count
    FROM StudentExamResults ser
    WHERE ser.ExamId = @ExamId AND ser.IsDeleted = 0
    GROUP BY ser.Grade
    ORDER BY CASE ser.Grade
        WHEN 'A+' THEN 1 WHEN 'A' THEN 2 WHEN 'A-' THEN 3
        WHEN 'B+' THEN 4 WHEN 'B' THEN 5 WHEN 'B-' THEN 6
        WHEN 'C+' THEN 7 WHEN 'C' THEN 8 WHEN 'D' THEN 9
        WHEN 'F' THEN 10 ELSE 11
    END;

    -- Class-wise stats
    SELECT 
        s.ClassId,
        cl.Name AS ClassName,
        COUNT(ser.Id) AS TotalStudents,
        SUM(CASE WHEN ser.IsPassed = 1 THEN 1 ELSE 0 END) AS PassedCount,
        ROUND(AVG(ser.Gpa), 2) AS AverageGPA
    FROM StudentExamResults ser
    INNER JOIN Students s ON ser.StudentId = s.Id
    INNER JOIN Classes cl ON s.ClassId = cl.Id
    WHERE ser.ExamId = @ExamId AND ser.IsDeleted = 0
    GROUP BY s.ClassId, cl.Name
    ORDER BY cl.Name;

    -- Group-wise stats
    SELECT 
        s.StudentGroupId,
        sg.Name AS GroupName,
        COUNT(ser.Id) AS TotalStudents,
        SUM(CASE WHEN ser.IsPassed = 1 THEN 1 ELSE 0 END) AS PassedCount,
        ROUND(AVG(ser.Gpa), 2) AS AverageGPA
    FROM StudentExamResults ser
    INNER JOIN Students s ON ser.StudentId = s.Id
    LEFT JOIN StudentGroups sg ON s.StudentGroupId = sg.Id
    WHERE ser.ExamId = @ExamId AND ser.IsDeleted = 0 AND s.StudentGroupId IS NOT NULL
    GROUP BY s.StudentGroupId, sg.Name
    ORDER BY sg.Name;

    -- Top 10 students
    SELECT TOP 10
        ser.Id,
        ser.StudentId,
        s.FullName AS StudentName,
        s.RollNumber,
        cl.Name AS ClassName,
        ser.Gpa,
        ser.Grade,
        ser.Position,
        ser.ClassPosition
    FROM StudentExamResults ser
    INNER JOIN Students s ON ser.StudentId = s.Id
    INNER JOIN Classes cl ON s.ClassId = cl.Id
    WHERE ser.ExamId = @ExamId AND ser.IsDeleted = 0
    ORDER BY ser.Gpa DESC;
END;
GO