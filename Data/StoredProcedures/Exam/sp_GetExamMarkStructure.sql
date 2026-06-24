CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamMarkStructure]
    @ExamId INT
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
        sub.Name AS SubjectName,
        s.ClassId,
        cl.Name AS ClassName,
        s.StudentGroupId,
        sg.Name AS StudentGroupName,
        s.FullMarks,
        s.PassMarks,
        s.DisplayOrder,
        s.IsActive
FROM SubjectMarkStructures s WITH(NOLOCK)
INNER JOIN ExamComponents c WITH(NOLOCK) ON c.Id = s.ComponentId AND c.IsDeleted = 0
LEFT JOIN Subjects sub WITH(NOLOCK) ON s.SubjectId = sub.Id AND sub.IsDeleted = 0
LEFT JOIN SchoolClasses cl WITH(NOLOCK) ON s.ClassId = cl.Id AND cl.IsDeleted = 0
LEFT JOIN StudentGroups sg WITH(NOLOCK) ON s.StudentGroupId = sg.Id AND sg.IsDeleted = 0
    WHERE s.IsDeleted = 0
      AND s.ExamId = @ExamId
      AND c.IsActive = 1
    ORDER BY s.SubjectId, s.DisplayOrder;
END;
GO
