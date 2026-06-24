CREATE OR ALTER PROCEDURE [dbo].[sp_GetSubjectMarkStructure]
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
FROM SubjectMarkStructures s WITH(NOLOCK)
INNER JOIN ExamComponents c WITH(NOLOCK) ON c.Id = s.ComponentId AND c.IsDeleted = 0
    WHERE s.IsDeleted = 0
      AND s.IsActive = 1
      AND c.IsActive = 1
      AND (s.SubjectId = @SubjectId
        OR (s.SubjectId IS NULL AND s.ClassId = @ClassId)
        OR (s.SubjectId IS NULL AND s.ClassId IS NULL AND s.StudentGroupId = @StudentGroupId))
    ORDER BY s.DisplayOrder, c.DisplayOrder;
END;
GO
