CREATE OR ALTER PROCEDURE sp_GetDailyCollectionReport
    @CollectionDate DATE,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    DECLARE @NextDay DATE = DATEADD(DAY, 1, @CollectionDate);

    SELECT p.Id, fi.InvoiceNo, s.FullName AS StudentName, p.Amount,
           CAST(p.Method AS NVARCHAR(20)) AS PaymentMethod,
           p.ReferenceNo, p.PaidAt,
           COUNT(*) OVER() AS TotalRecords
    FROM Payments p
    JOIN FeeInvoices fi ON p.FeeInvoiceId = fi.Id
    JOIN Students s ON fi.StudentId = s.Id
    WHERE p.IsDeleted = 0 AND p.PaidAt >= @CollectionDate AND p.PaidAt < @NextDay
    ORDER BY p.PaidAt DESC, p.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
