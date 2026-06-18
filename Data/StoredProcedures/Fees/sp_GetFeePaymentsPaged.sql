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

    WITH Data AS (
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
            ROW_NUMBER() OVER (ORDER BY p.PaidAt DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Payments p
        INNER JOIN 
            FeeInvoices fi ON p.FeeInvoiceId = fi.Id
        INNER JOIN 
            Students s ON fi.StudentId = s.Id
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
    )
    SELECT 
        Id, FeeInvoiceId, InvoiceNo, StudentId, StudentName,
        Amount, LateFee, DiscountAmount,
        Method, ReferenceNo, PaidAt, Remarks,
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
