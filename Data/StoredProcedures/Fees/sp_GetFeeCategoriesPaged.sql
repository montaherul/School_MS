-- ============================================================================
-- Stored Procedure: sp_GetFeeCategoriesPaged
-- Purpose: Get paginated fee categories
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeCategoriesPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH Data AS (
        SELECT 
            fc.Id,
            fc.Name,
            fc.Description,
            fc.DisplayOrder,
            fc.IsActive,
            ROW_NUMBER() OVER (ORDER BY fc.DisplayOrder, fc.Name, fc.Id) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FeeCategories fc
        WHERE 
            fc.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR fc.Name LIKE '%' + @SearchTerm + '%'
                OR fc.Description LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, Name, Description, DisplayOrder, IsActive, TotalCount AS TotalRecords
    FROM 
        Data
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
