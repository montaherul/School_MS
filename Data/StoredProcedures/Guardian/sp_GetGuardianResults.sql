CREATE OR ALTER PROCEDURE [dbo].[sp_GetGuardianResults]
    @GuardianId INT,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM StudentGuardians WHERE GuardianId = @GuardianId AND StudentId = @StudentId AND IsDeleted = 0)
    BEGIN
        SELECT 0 AS Result;
        RETURN;
    END

    -- Exam-level results
    SELECT 
        ser.Id,
        ser.ExamId,
        e.Name AS ExamName,
        e.StartsOn,
        e.EndsOn,
        ser.TotalMarks,
        ser.TotalFullMarks,
        ser.Gpa,
        ser.Grade,
        ser.Position,
        ser.ClassPosition,
        ser.GroupPosition,
        CAST(ser.IsPassed AS BIT) AS IsPassed,
        ser.FailedSubjectCount,
        ser.PassedSubjectCount,
        ser.PublishedAt,
        ser.Status
    FROM StudentExamResults ser
    LEFT JOIN Exams e ON ser.ExamId = e.Id
    WHERE ser.StudentId = @StudentId AND ser.IsDeleted = 0
    ORDER BY ser.ExamId DESC;

    -- Subject-level results
    SELECT 
        ssr.Id,
        ssr.ExamId,
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
    LEFT JOIN Exams e ON ssr.ExamId = e.Id
    LEFT JOIN Subjects sub ON ssr.SubjectId = sub.Id
    WHERE ssr.StudentId = @StudentId AND ssr.IsDeleted = 0
    ORDER BY ssr.ExamId DESC, sub.Name;
END
GO
