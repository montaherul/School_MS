CREATE OR ALTER PROCEDURE [dbo].[SP_ReportCard_Generate]
    @ExamId INT,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- School info
    SELECT TOP 1 SchoolName, Address AS SchoolAddress, EIIN, LogoPath AS SchoolLogoPath
    FROM SchoolSettings WHERE IsDeleted = 0;

    -- Student info
    SELECT s.Id AS StudentId, s.FullName AS StudentName, s.RollNumber, s.DateOfBirth,
        s.FatherName, s.MotherName, c.Name AS ClassName, sec.Name AS SectionName
    FROM Students s
    INNER JOIN Classes c ON s.ClassId = c.Id
    LEFT JOIN Sections sec ON s.SectionId = sec.Id
    WHERE s.Id = @StudentId;

    -- Exam info
    SELECT e.Id AS ExamId, e.Name AS ExamName, e.Term, e.StartsOn, e.EndsOn
    FROM Exams e WHERE e.Id = @ExamId;

    -- Subject results
    SELECT ssr.SubjectId, sub.Name AS SubjectName, sub.Code AS SubjectCode,
        ssr.FullMarks, ssr.PassMarks, ssr.MarksObtained, ssr.Grade, ssr.GradePoint,
        CAST(ssr.IsPassed AS BIT) AS IsPassed, ssr.IsOptionalSubject
    FROM StudentSubjectResults ssr
    INNER JOIN Subjects sub ON ssr.SubjectId = sub.Id
    WHERE ssr.ExamId = @ExamId AND ssr.StudentId = @StudentId AND ssr.IsDeleted = 0
    ORDER BY sub.DisplayOrder;

    -- Overall result
    SELECT ser.TotalMarks, ser.TotalFullMarks, ser.Gpa, ser.Grade,
        ser.ClassPosition, ser.GroupPosition, CAST(ser.IsPassed AS BIT) AS IsPassed,
        ser.FailedSubjectCount, ser.PassedSubjectCount, ser.PublishedAt
    FROM StudentExamResults ser
    WHERE ser.ExamId = @ExamId AND ser.StudentId = @StudentId AND ser.IsDeleted = 0;
END;
GO
