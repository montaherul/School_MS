-- ============================================================================
-- Stored Procedure: sp_GetClassesPaged
-- Purpose: Get paginated class list with new fields (NameBn, Code, Capacity, IsHigherSecondary, etc.)
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetClassesPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        c.Id,
        c.Name,
        c.NameBn,
        c.Code,
        c.SortOrder,
        c.Capacity,
        c.IsGroupBased,
        c.IsHigherSecondary,
        c.IsActive,
        (SELECT COUNT(*) FROM Sections WHERE SchoolClassId = c.Id AND IsDeleted = 0) AS SectionCount,
        (SELECT COUNT(*) FROM Students WHERE ClassId = c.Id AND IsDeleted = 0) AS StudentCount,
        COUNT(*) OVER () AS TotalRecords
    FROM Classes c WITH(NOLOCK)
    WHERE c.IsDeleted = 0
        AND (
            @SearchTerm IS NULL
            OR c.Name LIKE '%' + @SearchTerm + '%'
            OR c.NameBn LIKE '%' + @SearchTerm + '%'
            OR c.Code LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY c.SortOrder, c.Id
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
