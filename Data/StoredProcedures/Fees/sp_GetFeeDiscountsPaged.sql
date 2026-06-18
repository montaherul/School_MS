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

    WITH Data AS (
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
            ROW_NUMBER() OVER (ORDER BY fd.Name) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FeeDiscounts fd
        LEFT JOIN 
            Classes c ON fd.SchoolClassId = c.Id
        LEFT JOIN 
            FeeCategories fc ON fd.FeeCategoryId = fc.Id
        LEFT JOIN 
            FeeStructures fs ON fd.FeeStructureId = fs.Id
        WHERE 
            fd.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR fd.Name LIKE '%' + @SearchTerm + '%'
                OR fd.Description LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, Name, Description, DiscountType, Value,
        SchoolClassId, ClassName,
        FeeCategoryId, FeeCategoryName,
        FeeStructureId, FeeStructureName,
        IsActive, ValidFrom, ValidTo,
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
