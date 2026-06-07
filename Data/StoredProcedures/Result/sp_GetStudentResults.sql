CREATE OR ALTER PROCEDURE [dbo].[sp_GetStudentResults]
    @StudentId INT,
    @AcademicYearId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Exam-level results
    SELECT 
        ser.Id,
        ser.ExamId,
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
        ser.GroupPosition,
        CAST(ser.IsPassed AS BIT) AS IsPassed,
        ser.FailedSubjectCount,
        ser.PassedSubjectCount,
        ser.PublishedAt,
        ser.Status
    FROM StudentExamResults ser
    INNER JOIN Exams e ON ser.ExamId = e.Id
    WHERE ser.StudentId = @StudentId
      AND ser.IsDeleted = 0
      AND e.IsDeleted = 0
      AND ser.Status IN (4, 5) -- Published or Locked
      AND (@AcademicYearId IS NULL OR e.AcademicYearId = @AcademicYearId)
    ORDER BY e.EndsOn DESC;

    -- Subject-level results
    SELECT 
        ssr.Id,
        ssr.ExamId,
        e.Name AS ExamName,
        ssr.SubjectId,
        sub.Name AS SubjectName,
        sub.Code AS SubjectCode,
        ssr.IsOptionalSubject,
        ssr.IsReligionSubject,
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
      AND (@AcademicYearId IS NULL OR e.AcademicYearId = @AcademicYearId)
    ORDER BY e.EndsOn DESC, sub.DisplayOrder;
END;
GO