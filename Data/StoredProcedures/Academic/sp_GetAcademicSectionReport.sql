-- =============================================
-- Author:     SchoolManagementSystem
-- Description: Get section report with pre-computed occupancy (N+1 fix)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetAcademicSectionReport]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.[Id],
        s.[Name],
        ISNULL(c.[Name], '') AS ClassName,
        ISNULL(g.[Name], '') AS GroupName,
        s.[Capacity],
        ISNULL(stu.Occupied, 0) AS Occupied,
        CASE
            WHEN s.[Capacity] > 0
            THEN ROUND(CAST(ISNULL(stu.Occupied, 0) AS FLOAT) / s.[Capacity] * 100, 1)
            ELSE 0
        END AS OccupancyPercent
    FROM [dbo].[Sections] s
    LEFT JOIN [dbo].[Classes] c ON c.[Id] = s.[SchoolClassId] AND c.[IsDeleted] = 0
    LEFT JOIN [dbo].[StudentGroups] g ON g.[Id] = s.[StudentGroupId] AND g.[IsDeleted] = 0
    LEFT JOIN (
        SELECT [SectionId], COUNT(*) AS Occupied
        FROM [dbo].[Students]
        WHERE [IsDeleted] = 0 AND [Status] = 1
        GROUP BY [SectionId]
    ) stu ON stu.[SectionId] = s.[Id]
    WHERE s.[IsDeleted] = 0
    ORDER BY c.[Name], s.[Name];
END;
GO
