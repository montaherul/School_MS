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

    WITH Data AS (
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
            ROW_NUMBER() OVER (ORDER BY lfr.Name) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            LateFeeRules lfr
        LEFT JOIN 
            Classes c ON lfr.SchoolClassId = c.Id
        LEFT JOIN 
            FeeCategories fc ON lfr.FeeCategoryId = fc.Id
        WHERE 
            lfr.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR lfr.Name LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, Name, GraceDays, FeeType, FeeValue, MaxFee,
        SchoolClassId, ClassName, FeeCategoryId, FeeCategoryName, IsActive,
        TotalCount AS TotalRecords
    FROM 
        Data
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
