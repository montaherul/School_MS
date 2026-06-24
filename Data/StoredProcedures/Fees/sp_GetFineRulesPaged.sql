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


        SELECT 
            fr.Id,
            fr.Name,
            fr.GraceDays,
            fr.FinePerDay,

            COUNT(*) OVER () AS TotalRecords
        FROM 
FineRules fr WITH(NOLOCK)
        WHERE 
            fr.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR fr.Name LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY fr.Name, fr.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
