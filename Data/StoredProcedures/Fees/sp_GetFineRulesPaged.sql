-- ============================================================================
-- Stored Procedure: sp_GetFineRulesPaged
-- Purpose: Get paginated fine rules
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFineRulesPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH Data AS (
        SELECT 
            fr.Id,
            fr.Name,
            fr.GraceDays,
            fr.FinePerDay,
            ROW_NUMBER() OVER (ORDER BY fr.Name) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FineRules fr
        WHERE 
            fr.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR fr.Name LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, Name, GraceDays, FinePerDay, TotalCount AS TotalRecords
    FROM 
        Data
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
