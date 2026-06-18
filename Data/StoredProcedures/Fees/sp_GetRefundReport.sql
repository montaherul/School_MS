CREATE OR ALTER PROCEDURE sp_GetRefundReport
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT fr.Id, s.FullName AS StudentName, fi.InvoiceNo, fr.RefundAmount,
           fr.Reason, fr.IsApproved, fr.RefundDate,
           COUNT(*) OVER() AS TotalRecords
    FROM FeeRefunds fr
    JOIN Payments fp ON fr.FeePaymentId = fp.Id
    JOIN FeeInvoices fi ON fp.FeeInvoiceId = fi.Id
    JOIN Students s ON fi.StudentId = s.Id
    WHERE fr.IsDeleted = 0
    ORDER BY fr.RefundDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
