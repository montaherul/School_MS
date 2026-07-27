CREATE OR ALTER PROCEDURE sp_GetPaymentAllocationsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = NULL,
    @PaymentId INT = NULL,
    @FeeInvoiceId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    SELECT pa.Id, pa.PaymentId, p.ReferenceNo AS PaymentReference, pa.FeeInvoiceId, fi.InvoiceNo,
           pa.AllocatedAmount, pa.Remarks,
           COUNT(*) OVER() AS TotalRecords
    FROM PaymentAllocations pa
    LEFT JOIN Payments p ON p.Id = pa.PaymentId
    LEFT JOIN FeeInvoices fi ON fi.Id = pa.FeeInvoiceId
    WHERE pa.IsDeleted = 0
      AND (@SearchTerm IS NULL OR p.ReferenceNo LIKE '%' + @SearchTerm + '%' OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%')
      AND (@PaymentId IS NULL OR pa.PaymentId = @PaymentId)
      AND (@FeeInvoiceId IS NULL OR pa.FeeInvoiceId = @FeeInvoiceId)
    ORDER BY pa.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
