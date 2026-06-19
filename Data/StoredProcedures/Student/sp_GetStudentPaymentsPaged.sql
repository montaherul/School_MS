CREATE OR ALTER PROCEDURE sp_GetStudentPaymentsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    WITH Filtered AS (
        SELECT
            p.Id,
            p.PaidAt AS PaymentDate,
            p.Method,
            p.ReferenceNo,
            p.Amount,
            p.LateFee,
            p.DiscountAmount,
            fi.InvoiceNo,
            ROW_NUMBER() OVER (ORDER BY p.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM Payments p
        INNER JOIN FeeInvoices fi ON p.FeeInvoiceId = fi.Id AND fi.IsDeleted = 0
        WHERE p.IsDeleted = 0
          AND fi.StudentId = @StudentId
          AND (@SearchTerm IS NULL OR p.ReferenceNo LIKE '%' + @SearchTerm + '%' OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%')
    )
    SELECT Id, PaymentDate, Method, ReferenceNo, Amount, LateFee,
           DiscountAmount, InvoiceNo, TotalCount AS TotalRecords
    FROM Filtered
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO