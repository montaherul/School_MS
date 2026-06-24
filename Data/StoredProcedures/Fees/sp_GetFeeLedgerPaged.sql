-- ============================================================================
-- Stored Procedure: sp_GetFeeLedgerPaged
-- Purpose: Get paginated fee ledger entries
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeLedgerPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT = 0,
    @TransactionType INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT 
            fl.Id,
            fl.StudentId,
            s.FullName AS StudentName,
            fl.FeeInvoiceId,
            fi.InvoiceNo,
            fl.FeePaymentId,
            fl.TransactionType,
            fl.Debit,
            fl.Credit,
            fl.Balance,
            fl.Description,
            fl.TransactionDate,

            COUNT(*) OVER () AS TotalRecords
        FROM 
FeeLedgers fl WITH(NOLOCK)
        INNER JOIN 
Students s WITH(NOLOCK) ON fl.StudentId = s.Id
        LEFT JOIN 
FeeInvoices fi WITH(NOLOCK) ON fl.FeeInvoiceId = fi.Id
        WHERE 
            fl.IsDeleted = 0
            AND (@StudentId = 0 OR fl.StudentId = @StudentId)
            AND (@TransactionType = 0 OR fl.TransactionType = @TransactionType)
            AND (
                @SearchTerm IS NULL 
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%'
                OR fl.Description LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY fl.TransactionDate DESC, fl.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
