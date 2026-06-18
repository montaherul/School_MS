CREATE OR ALTER PROCEDURE sp_GetMonthlyCollectionReport
    @Year INT,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT YEAR(p.PaidAt) AS [Year], MONTH(p.PaidAt) AS [Month],
           SUM(p.Amount) AS TotalCollected, COUNT(*) AS TransactionCount,
           COUNT(*) OVER() AS TotalRecords
    FROM Payments p
    JOIN FeeInvoices fi ON p.FeeInvoiceId = fi.Id
    WHERE p.IsDeleted = 0 AND YEAR(p.PaidAt) = @Year
    GROUP BY YEAR(p.PaidAt), MONTH(p.PaidAt)
    ORDER BY [Year] DESC, [Month] DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
