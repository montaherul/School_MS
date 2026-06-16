CREATE OR ALTER PROCEDURE [dbo].[SP_Exam_GetAllResults]
    @ExamId INT = NULL,
    @ClassId INT = NULL,
    @Status INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ser.Id,
        ser.ExamId,
        e.Name AS ExamName,
        e.Term,
        ser.StudentId,
        s.FullName AS StudentName,
        s.RollNumber,
        c.Name AS ClassName,
        ser.TotalMarks,
        ser.TotalFullMarks,
        ser.Gpa,
        ser.Grade,
        ser.ClassPosition,
        ser.GroupPosition,
        CAST(ser.IsPassed AS BIT) AS IsPassed,
        ser.FailedSubjectCount,
        ser.PassedSubjectCount,
        ser.Status,
        ser.PublishedAt
    FROM StudentExamResults ser
    INNER JOIN Exams e ON ser.ExamId = e.Id
    INNER JOIN Students s ON ser.StudentId = s.Id
    INNER JOIN Classes c ON s.ClassId = c.Id
    WHERE ser.IsDeleted = 0
      AND (@ExamId IS NULL OR ser.ExamId = @ExamId)
      AND (@ClassId IS NULL OR s.ClassId = @ClassId)
      AND (@Status IS NULL OR ser.Status = @Status)
    ORDER BY c.Name, s.RollNumber;
END;
GO
