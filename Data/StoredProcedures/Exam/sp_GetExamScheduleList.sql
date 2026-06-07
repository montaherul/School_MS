CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamScheduleList]
    @ExamId INT = NULL,
    @ClassId INT = NULL,
    @SectionId INT = NULL,
    @SubjectId INT = NULL,
    @AcademicYearId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        esch.Id,
        esch.ExamId,
        e.Name AS ExamName,
        esch.SubjectId,
        sub.Name AS SubjectName,
        sub.Code AS SubjectCode,
        esch.ExamDate,
        esch.StartsAt,
        esch.EndsAt,
        esch.RoomNo,
        esch.Instructions
    FROM ExamSchedules esch
    INNER JOIN Exams e ON esch.ExamId = e.Id
    INNER JOIN Subjects sub ON esch.SubjectId = sub.Id
    WHERE e.IsDeleted = 0
      AND (@ExamId IS NULL OR esch.ExamId = @ExamId)
      AND (@SubjectId IS NULL OR esch.SubjectId = @SubjectId)
      AND (@AcademicYearId IS NULL OR e.AcademicYearId = @AcademicYearId)
    ORDER BY esch.ExamDate, esch.StartsAt;
END;
GO
