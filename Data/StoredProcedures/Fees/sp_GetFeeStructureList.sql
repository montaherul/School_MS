-- ============================================================================
-- Stored Procedure: sp_GetFeeStructureList
-- Purpose: Get paginated fee structure with class names
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeStructureList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH FeeData AS (
        SELECT 
            fs.Id,
            fs.SchoolClassId,
            c.Name AS ClassName,
            fs.FeeName,
            fs.Amount,
            fs.IsRecurring,
            ROW_NUMBER() OVER (ORDER BY c.SortOrder, fs.FeeName) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FeeStructures fs
        INNER JOIN 
            Classes c ON fs.SchoolClassId = c.Id
        WHERE 
            fs.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR fs.FeeName LIKE '%' + @SearchTerm + '%'
                OR c.Name LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        SchoolClassId,
        ClassName,
        FeeName,
        Amount,
        IsRecurring,
        TotalCount AS TotalRecords
    FROM 
        FeeData
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
