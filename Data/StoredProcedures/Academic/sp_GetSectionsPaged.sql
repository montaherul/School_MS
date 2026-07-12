-- ============================================================================
-- Stored Procedure: sp_GetSectionsPaged
-- Purpose: Get paginated section list with class details, group info, capacity
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetSectionsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = NULL,
    @ClassId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        s.Id,
        s.SchoolClassId,
        c.Name AS ClassName,
        s.Name,
        s.ParentSectionId,
        s.StudentGroupId,
        g.Name AS GroupName,
        s.Capacity,
        (SELECT COUNT(*) FROM Students WHERE SectionId = s.Id AND IsDeleted = 0) AS StudentCount,
        COUNT(*) OVER () AS TotalRecords
    FROM Sections s WITH(NOLOCK)
    JOIN Classes c WITH(NOLOCK) ON s.SchoolClassId = c.Id
    LEFT JOIN StudentGroups g WITH(NOLOCK) ON s.StudentGroupId = g.Id
    WHERE s.IsDeleted = 0
        AND (@ClassId IS NULL OR s.SchoolClassId = @ClassId)
        AND (
            @SearchTerm IS NULL
            OR s.Name LIKE '%' + @SearchTerm + '%'
            OR c.Name LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY c.SortOrder, s.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
