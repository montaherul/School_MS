-- ============================================================================
-- Stored Procedure: sp_GetFeePaymentsPaged
-- Purpose: Get paginated fee payments
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeePaymentsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @FeeInvoiceId INT = 0,
    @PaymentMethod INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT 
            p.Id,
            p.FeeInvoiceId,
            fi.InvoiceNo,
            fi.StudentId,
            s.FullName AS StudentName,
            p.Amount,
            p.LateFee,
            p.DiscountAmount,
            p.Method,
            p.ReferenceNo,
            p.PaidAt,
            p.Remarks,

            COUNT(*) OVER () AS TotalRecords
        FROM 
Payments p WITH(NOLOCK)
        INNER JOIN 
FeeInvoices fi WITH(NOLOCK) ON p.FeeInvoiceId = fi.Id
        INNER JOIN 
Students s WITH(NOLOCK) ON fi.StudentId = s.Id
        WHERE 
            p.IsDeleted = 0
            AND (@FeeInvoiceId = 0 OR p.FeeInvoiceId = @FeeInvoiceId)
            AND (@PaymentMethod = 0 OR p.Method = @PaymentMethod)
            AND (
                @SearchTerm IS NULL 
                OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%'
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR p.ReferenceNo LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY p.PaidAt DESC, p.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
