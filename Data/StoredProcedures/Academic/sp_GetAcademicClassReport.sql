-- =============================================
-- Author:     SchoolManagementSystem
-- Description: Get class report with pre-computed counts (N+1 fix)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetAcademicClassReport]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.[Id],
        c.[Name],
        c.[Code],
        c.[Capacity],
        c.[IsActive],
        ISNULL(section_counts.SectionCount, 0) AS SectionCount,
        ISNULL(student_counts.StudentCount, 0) AS StudentCount,
        CASE
            WHEN c.[Capacity] > 0
            THEN ROUND(CAST(ISNULL(student_counts.StudentCount, 0) AS FLOAT) / c.[Capacity] * 100, 1)
            ELSE 0
        END AS OccupancyPercent,
        ISNULL(subject_counts.SubjectCount, 0) AS SubjectCount
    FROM [dbo].[Classes] c
    LEFT JOIN (
        SELECT [SchoolClassId], COUNT(*) AS SectionCount
        FROM [dbo].[Sections]
        WHERE [IsDeleted] = 0
        GROUP BY [SchoolClassId]
    ) section_counts ON section_counts.SchoolClassId = c.[Id]
    LEFT JOIN (
        SELECT [ClassId], COUNT(*) AS StudentCount
        FROM [dbo].[Students]
        WHERE [IsDeleted] = 0 AND [Status] = 1
        GROUP BY [ClassId]
    ) student_counts ON student_counts.ClassId = c.[Id]
    LEFT JOIN (
        SELECT [SchoolClassId], COUNT(*) AS SubjectCount
        FROM [dbo].[ClassSubjects]
        WHERE [IsDeleted] = 0
        GROUP BY [SchoolClassId]
    ) subject_counts ON subject_counts.SchoolClassId = c.[Id]
    WHERE c.[IsDeleted] = 0
    ORDER BY c.[Name];
END;
GO
