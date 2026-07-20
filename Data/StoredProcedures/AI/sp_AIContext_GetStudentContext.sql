CREATE PROCEDURE [dbo].[sp_AIContext_GetStudentContext]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Result 1: Student + Class + Section + Group
    SELECT
        s.[Id] AS StudentId,
        s.[FullName] AS StudentName,
        s.[StudentNo],
        ISNULL(c.[Name], 'N/A') AS ClassName,
        ISNULL(sec.[Name], 'N/A') AS SectionName,
        sg.[Name] AS GroupName
    FROM [dbo].[Students] s
    LEFT JOIN [dbo].[Classes] c ON c.[Id] = s.[ClassId]
    LEFT JOIN [dbo].[Sections] sec ON sec.[Id] = s.[SectionId]
    LEFT JOIN [dbo].[StudentGroups] sg ON sg.[Id] = s.[StudentGroupId]
    WHERE s.[Id] = @StudentId AND s.[IsDeleted] = 0;

    -- Result 2: School name
    SELECT TOP 1 ISNULL([SchoolName], 'School') AS SchoolName
    FROM [dbo].[SchoolSettings]
    WHERE [IsDeleted] = 0;

    -- Result 3: Active academic year
    SELECT TOP 1 [Name] AS AcademicYearName
    FROM [dbo].[AcademicYears]
    WHERE [IsActive] = 1 AND [IsDeleted] = 0;

    -- Result 4: Class subjects
    SELECT sub.[Name] AS SubjectName
    FROM [dbo].[ClassSubjects] cs
    INNER JOIN [dbo].[Subjects] sub ON sub.[Id] = cs.[SubjectId]
    WHERE cs.[SchoolClassId] = (SELECT [ClassId] FROM [dbo].[Students] WHERE [Id] = @StudentId)
      AND cs.[IsDeleted] = 0
      AND sub.[IsDeleted] = 0;
END
