CREATE OR ALTER PROCEDURE sp_GetStudentPaymentsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT
            p.Id,
            p.PaidAt AS PaymentDate,
            p.Method,
            p.ReferenceNo,
            p.Amount,
            p.LateFee,
            p.DiscountAmount,
            fi.InvoiceNo,

            COUNT(*) OVER () AS TotalRecords
FROM Payments p WITH(NOLOCK)
INNER JOIN FeeInvoices fi WITH(NOLOCK) ON p.FeeInvoiceId = fi.Id AND fi.IsDeleted = 0
        WHERE p.IsDeleted = 0
          AND fi.StudentId = @StudentId
          AND (@SearchTerm IS NULL OR p.ReferenceNo LIKE '%' + @SearchTerm + '%' OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%')
    
ORDER BY p.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO