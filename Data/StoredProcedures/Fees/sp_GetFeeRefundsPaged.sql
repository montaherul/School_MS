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

    WITH Data AS (
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
            ROW_NUMBER() OVER (ORDER BY fr.RefundDate DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FeeRefunds fr
        INNER JOIN 
            Payments p ON fr.FeePaymentId = p.Id
        INNER JOIN 
            FeeInvoices fi ON p.FeeInvoiceId = fi.Id
        INNER JOIN 
            Students s ON fi.StudentId = s.Id
        WHERE 
            fr.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%'
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR fr.Reason LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, FeePaymentId, FeeInvoiceId, InvoiceNo, StudentId, StudentName,
        RefundAmount, RefundMethod, ReferenceNo, Reason, IsApproved, RefundDate,
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
