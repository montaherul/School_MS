-- ============================================================================
-- Stored Procedure: sp_GetFeeDiscountsPaged
-- Purpose: Get paginated fee discount rules
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeDiscountsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT 
            fd.Id,
            fd.Name,
            fd.Description,
            fd.DiscountType,
            fd.Value,
            fd.SchoolClassId,
            c.Name AS ClassName,
            fd.FeeCategoryId,
            fc.Name AS FeeCategoryName,
            fd.FeeStructureId,
            fs.FeeName AS FeeStructureName,
            fd.IsActive,
            fd.ValidFrom,
            fd.ValidTo,

            COUNT(*) OVER () AS TotalRecords
        FROM 
FeeDiscounts fd WITH(NOLOCK)
        LEFT JOIN 
Classes c WITH(NOLOCK) ON fd.SchoolClassId = c.Id
        LEFT JOIN 
FeeCategories fc WITH(NOLOCK) ON fd.FeeCategoryId = fc.Id
        LEFT JOIN 
FeeStructures fs WITH(NOLOCK) ON fd.FeeStructureId = fs.Id
        WHERE 
            fd.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR fd.Name LIKE '%' + @SearchTerm + '%'
                OR fd.Description LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY fd.Name, fd.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
