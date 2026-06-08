CREATE OR ALTER PROCEDURE [dbo].[sp_GetTeacherAssignedSubjects]
    @TeacherId INT,
    @ClassId INT,
    @SectionId INT = NULL,
    @GroupId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT s.Id AS SubjectId, s.Name AS SubjectName, s.Code AS SubjectCode,
           sc.Id AS ClassId, sc.Name AS ClassName,
           sec.Id AS SectionId, sec.Name AS SectionName,
           sg.Id AS GroupId, sg.Name AS GroupName
    FROM TeacherSubjectAssignments tsa
    INNER JOIN Subjects s ON s.Id = tsa.SubjectId AND s.IsDeleted = 0
    INNER JOIN SchoolClasses sc ON sc.Id = tsa.ClassId AND sc.IsDeleted = 0
    LEFT JOIN Sections sec ON sec.Id = tsa.SectionId AND sec.IsDeleted = 0
    LEFT JOIN StudentGroups sg ON sg.Id = tsa.GroupId AND sg.IsDeleted = 0
    WHERE tsa.TeacherId = @TeacherId
      AND tsa.IsActive = 1 AND tsa.IsDeleted = 0
      AND tsa.ClassId = @ClassId
      AND (@SectionId IS NULL OR tsa.SectionId = @SectionId)
      AND (@GroupId IS NULL OR tsa.GroupId = @GroupId OR (tsa.GroupId IS NULL AND @GroupId IS NULL))
    ORDER BY s.Name;
END;
GO
