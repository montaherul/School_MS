CREATE OR ALTER PROCEDURE sp_GetAdmissionRevenueReport
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NextDay DATE = DATEADD(DAY, 1, @ToDate);

    SELECT
        ISNULL(SUM(CASE WHEN r.IsRefunded = 0 THEN r.Amount ELSE 0 END), 0) AS TotalCollected,
        ISNULL(SUM(CASE WHEN r.IsRefunded = 1 THEN r.RefundAmount ELSE 0 END), 0) AS TotalRefunded,
        ISNULL(COUNT(CASE WHEN r.IsRefunded = 0 THEN 1 END), 0) AS TotalTransactions,
        ISNULL(COUNT(CASE WHEN r.IsRefunded = 1 THEN 1 END), 0) AS TotalRefunds
    FROM AdmissionReceipts r WITH(NOLOCK)
    WHERE r.IsDeleted = 0
      AND r.ReceiptDate >= @FromDate AND r.ReceiptDate < @NextDay;

    SELECT
        CAST(r.ReceiptDate AS DATE) AS CollectionDate,
        ISNULL(SUM(CASE WHEN r.IsRefunded = 0 THEN r.Amount ELSE 0 END), 0) AS DailyTotal,
        ISNULL(COUNT(CASE WHEN r.IsRefunded = 0 THEN 1 END), 0) AS DailyCount
    FROM AdmissionReceipts r WITH(NOLOCK)
    WHERE r.IsDeleted = 0
      AND r.ReceiptDate >= @FromDate AND r.ReceiptDate < @NextDay
    GROUP BY CAST(r.ReceiptDate AS DATE)
    ORDER BY CollectionDate;
END;
GO
