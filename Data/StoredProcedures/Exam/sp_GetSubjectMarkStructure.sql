CREATE OR ALTER PROCEDURE [dbo].[sp_GetSubjectMarkStructure]
    @ExamId INT,
    @SubjectId INT,
    @ClassId INT = NULL,
    @StudentGroupId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.Id AS StructureId,
        s.ComponentId,
        c.Name AS ComponentName,
        c.Code AS ComponentCode,
        s.ExamId,
        s.SubjectId,
        s.ClassId,
        s.StudentGroupId,
        s.FullMarks,
        s.PassMarks,
        s.DisplayOrder,
        s.IsActive,
        c.DefaultFullMarks,
        c.DefaultPassMarks,
        c.IsPractical,
        c.IsOptional
    FROM SubjectMarkStructures s
    INNER JOIN ExamComponents c ON c.Id = s.ComponentId AND c.IsDeleted = 0
    WHERE s.IsDeleted = 0
      AND s.IsActive = 1
      AND c.IsActive = 1
      AND ((s.ExamId = @ExamId AND s.SubjectId = @SubjectId)
        OR (s.ExamId IS NULL AND s.SubjectId = @SubjectId)
        OR (s.ExamId IS NULL AND s.SubjectId IS NULL AND s.ClassId = @ClassId)
        OR (s.ExamId IS NULL AND s.SubjectId IS NULL AND s.ClassId IS NULL AND s.StudentGroupId = @StudentGroupId))
    ORDER BY s.DisplayOrder, c.DisplayOrder;
END;
GO
