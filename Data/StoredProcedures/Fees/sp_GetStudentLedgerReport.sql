CREATE OR ALTER PROCEDURE sp_GetStudentLedgerReport
    @StudentId INT,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        fl.Id,
        CAST(fl.TransactionType AS NVARCHAR(20)) AS TransactionType,
        ISNULL(fi.InvoiceNo, '') AS InvoiceNo,
        fl.TransactionDate,
        fl.Debit,
        fl.Credit,
        SUM(fl.Debit - fl.Credit) OVER (ORDER BY fl.TransactionDate, fl.Id ROWS UNBOUNDED PRECEDING) AS Balance,
        ISNULL(fl.Description, '') AS Description,
        COUNT(*) OVER() AS TotalRecords
    FROM FeeLedgers fl WITH(NOLOCK)
    LEFT JOIN FeeInvoices fi WITH(NOLOCK) ON fi.Id = fl.FeeInvoiceId AND fi.IsDeleted = 0
    WHERE fl.StudentId = @StudentId AND fl.IsDeleted = 0
    ORDER BY fl.TransactionDate DESC, fl.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
