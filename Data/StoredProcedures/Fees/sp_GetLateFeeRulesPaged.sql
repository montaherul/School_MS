-- ============================================================================
-- Stored Procedure: sp_GetLateFeeRulesPaged
-- Purpose: Get paginated late fee rules
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetLateFeeRulesPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT 
            lfr.Id,
            lfr.Name,
            lfr.GraceDays,
            lfr.FeeType,
            lfr.FeeValue,
            lfr.MaxFee,
            lfr.SchoolClassId,
            c.Name AS ClassName,
            lfr.FeeCategoryId,
            fc.Name AS FeeCategoryName,
            lfr.IsActive,

            COUNT(*) OVER () AS TotalRecords
        FROM 
LateFeeRules lfr WITH(NOLOCK)
        LEFT JOIN 
Classes c WITH(NOLOCK) ON lfr.SchoolClassId = c.Id
        LEFT JOIN 
FeeCategories fc WITH(NOLOCK) ON lfr.FeeCategoryId = fc.Id
        WHERE 
            lfr.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR lfr.Name LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY lfr.Name, lfr.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
