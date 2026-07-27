-- ============================================================================
-- Stored Procedure: sp_GetScholarshipsPaged
-- Purpose: Get paginated scholarship records with class and category names
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetScholarshipsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        s.Id,
        s.Name,
        s.Description,
        s.DiscountType,
        s.Value,
        s.SchoolClassId,
        c.Name AS ClassName,
        s.FeeCategoryId,
        fc.Name AS FeeCategoryName,
        s.IsActive,

        COUNT(*) OVER () AS TotalRecords
    FROM
        Scholarships s WITH(NOLOCK)
        LEFT JOIN Classes c WITH(NOLOCK) ON s.SchoolClassId = c.Id
        LEFT JOIN FeeCategories fc WITH(NOLOCK) ON s.FeeCategoryId = fc.Id
    WHERE
        s.IsDeleted = 0
        AND (
            @SearchTerm IS NULL
            OR s.Name LIKE '%' + @SearchTerm + '%'
            OR s.Description LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY s.Name, s.Id
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
