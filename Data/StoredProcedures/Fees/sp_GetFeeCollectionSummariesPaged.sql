-- ============================================================================
-- Stored Procedure: sp_GetFeeCollectionSummariesPaged
-- Purpose: Get paginated fee collection summaries
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeCollectionSummariesPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH Data AS (
        SELECT 
            fcs.Id,
            fcs.CollectionDate,
            fcs.TotalCollected,
            fcs.TotalDiscounted,
            fcs.TotalRefunded,
            fcs.TotalTransactions,
            fcs.PaymentMethod,
            fcs.IsDailySummary,
            ROW_NUMBER() OVER (ORDER BY fcs.CollectionDate DESC, fcs.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FeeCollectionSummaries fcs
        WHERE 
            fcs.IsDeleted = 0
            AND (@FromDate IS NULL OR fcs.CollectionDate >= @FromDate)
            AND (@ToDate IS NULL OR fcs.CollectionDate <= @ToDate)
            AND (
                @SearchTerm IS NULL 
                OR CAST(fcs.CollectionDate AS NVARCHAR) LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, CollectionDate, TotalCollected, TotalDiscounted,
        TotalRefunded, TotalTransactions, PaymentMethod, IsDailySummary,
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
