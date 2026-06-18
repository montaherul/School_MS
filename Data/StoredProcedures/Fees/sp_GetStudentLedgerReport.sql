CREATE OR ALTER PROCEDURE sp_GetStudentLedgerReport
    @StudentId INT,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT fi.Id, fi.InvoiceNo, fi.DueDate, fi.TotalAmount, fi.PaidAmount,
           (fi.TotalAmount - fi.PaidAmount) AS DueAmount,
           CAST(fi.Status AS NVARCHAR(20)) AS Status,
           p.PaidAt, p.ReferenceNo, fi.LateFee, fi.DiscountAmount,
           COUNT(*) OVER() AS TotalRecords
    FROM FeeInvoices fi
    LEFT JOIN Payments p ON p.FeeInvoiceId = fi.Id AND p.IsDeleted = 0
    WHERE fi.StudentId = @StudentId AND fi.IsDeleted = 0
    ORDER BY fi.DueDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
