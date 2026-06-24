CREATE OR ALTER PROCEDURE [dbo].[SP_MarkEntry_GetGrid]
    @ExamId INT,
    @ClassId INT,
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.Id AS StudentId,
        s.FullName AS StudentName,
        s.RollNumber,
        COALESCE(m.MarksObtained, 0) AS MarksObtained,
        m.Grade,
        m.Status,
        CAST(COALESCE(m.IsLocked, 0) AS BIT) AS IsLocked
FROM Students s WITH(NOLOCK)
INNER JOIN ExamSubjects es WITH(NOLOCK) ON es.ExamId = @ExamId AND es.SubjectId = @SubjectId AND es.IsDeleted = 0
LEFT JOIN Marks m WITH(NOLOCK) ON m.ExamId = @ExamId AND m.SubjectId = @SubjectId AND m.StudentId = s.Id AND m.IsDeleted = 0
    WHERE s.ClassId = @ClassId
      AND s.Status = 1
      AND s.IsDeleted = 0
    ORDER BY s.RollNumber;
END;
GO
