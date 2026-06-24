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


        SELECT 
            fc.Id,
            fc.Name,
            fc.Description,
            fc.DisplayOrder,
            fc.IsActive,

            COUNT(*) OVER () AS TotalRecords
        FROM 
FeeCategories fc WITH(NOLOCK)
        WHERE 
            fc.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR fc.Name LIKE '%' + @SearchTerm + '%'
                OR fc.Description LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY fc.DisplayOrder, fc.Name, fc.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
