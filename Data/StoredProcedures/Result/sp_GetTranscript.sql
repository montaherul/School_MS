CREATE OR ALTER PROCEDURE [dbo].[sp_GetTranscript]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Student info
    SELECT 
        s.Id AS StudentId,
        s.FullName AS StudentName,
        s.StudentNo,
        s.RollNumber,
        s.DateOfBirth,
        s.FatherName,
        s.MotherName,
        s.ClassId,
        cl.Name AS ClassName,
        s.SectionId,
        sec.Name AS SectionName,
        s.StudentGroupId,
        sg.Name AS GroupName
    FROM Students s
    INNER JOIN Classes cl ON s.ClassId = cl.Id
    LEFT JOIN Sections sec ON s.SectionId = sec.Id
    LEFT JOIN StudentGroups sg ON s.StudentGroupId = sg.Id
    WHERE s.Id = @StudentId;

    -- All exam results across all academic years
    SELECT 
        ay.Id AS AcademicYearId,
        ay.Name AS AcademicYearName,
        e.Id AS ExamId,
        e.Name AS ExamName,
        e.Term,
        e.StartsOn,
        e.EndsOn,
        ser.TotalMarks,
        ser.TotalFullMarks,
        ser.Gpa,
        ser.Grade,
        ser.Position,
        ser.ClassPosition,
        ser.IsPassed,
        ser.FailedSubjectCount,
        ser.PassedSubjectCount
    FROM StudentExamResults ser
    INNER JOIN Exams e ON ser.ExamId = e.Id
    INNER JOIN AcademicYears ay ON e.AcademicYearId = ay.Id
    WHERE ser.StudentId = @StudentId
      AND ser.IsDeleted = 0
      AND e.IsDeleted = 0
      AND ser.Status IN (4, 5)
    ORDER BY ay.Id, e.EndsOn;

    -- All subject results
    SELECT 
        e.Id AS ExamId,
        e.Name AS ExamName,
        ssr.SubjectId,
        sub.Name AS SubjectName,
        sub.Code AS SubjectCode,
        ssr.MarksObtained,
        ssr.FullMarks,
        ssr.PassMarks,
        ssr.Grade,
        ssr.GradePoint,
        CAST(ssr.IsPassed AS BIT) AS IsPassed
    FROM StudentSubjectResults ssr
    INNER JOIN Exams e ON ssr.ExamId = e.Id
    INNER JOIN Subjects sub ON ssr.SubjectId = sub.Id
    WHERE ssr.StudentId = @StudentId
      AND ssr.IsDeleted = 0
      AND e.IsDeleted = 0
    ORDER BY e.EndsOn DESC, sub.DisplayOrder;

    -- Overall stats
    SELECT
        COUNT(DISTINCT ser.ExamId) AS TotalExamsTaken,
        COUNT(DISTINCT e.AcademicYearId) AS TotalAcademicYears,
        ROUND(AVG(ser.Gpa), 2) AS AverageGPA,
        MAX(ser.Gpa) AS BestGPA,
        COUNT(CASE WHEN ser.IsPassed = 1 THEN 1 END) AS PassedExams,
        COUNT(CASE WHEN ser.IsPassed = 0 THEN 1 END) AS FailedExams
    FROM StudentExamResults ser
    INNER JOIN Exams e ON ser.ExamId = e.Id
    WHERE ser.StudentId = @StudentId
      AND ser.IsDeleted = 0
      AND e.IsDeleted = 0
      AND ser.Status IN (4, 5);
END;
GO