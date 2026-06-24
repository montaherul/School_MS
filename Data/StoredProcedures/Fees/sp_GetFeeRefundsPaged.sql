-- ============================================================================
-- Stored Procedure: sp_GetFeeRefundsPaged
-- Purpose: Get paginated fee refunds
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeRefundsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT 
            fr.Id,
            fr.FeePaymentId,
            p.FeeInvoiceId,
            fi.InvoiceNo,
            fi.StudentId,
            s.FullName AS StudentName,
            fr.RefundAmount,
            fr.RefundMethod,
            fr.ReferenceNo,
            fr.Reason,
            fr.IsApproved,
            fr.RefundDate,

            COUNT(*) OVER () AS TotalRecords
        FROM 
FeeRefunds fr WITH(NOLOCK)
        INNER JOIN 
Payments p WITH(NOLOCK) ON fr.FeePaymentId = p.Id
        INNER JOIN 
FeeInvoices fi WITH(NOLOCK) ON p.FeeInvoiceId = fi.Id
        INNER JOIN 
Students s WITH(NOLOCK) ON fi.StudentId = s.Id
        WHERE 
            fr.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%'
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR fr.Reason LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY fr.RefundDate DESC, fr.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
