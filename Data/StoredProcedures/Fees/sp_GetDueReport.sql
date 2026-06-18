CREATE OR ALTER PROCEDURE sp_GetDueReport
    @PageNumber INT = 1,
    @PageSize INT = 50,
    @ClassId INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT fi.Id, fi.InvoiceNo, s.FullName AS StudentName,
           c.Name AS ClassName, fi.DueDate, fi.TotalAmount, fi.PaidAmount,
           (fi.TotalAmount - fi.PaidAmount) AS DueAmount,
           DATEDIFF(DAY, fi.DueDate, GETDATE()) AS DaysOverdue,
           COUNT(*) OVER() AS TotalRecords
    FROM FeeInvoices fi
    JOIN Students s ON fi.StudentId = s.Id
    JOIN Classes c ON s.ClassId = c.Id
    WHERE fi.IsDeleted = 0 AND fi.Status IN (1, 2)
      AND (@ClassId = 0 OR s.ClassId = @ClassId)
    ORDER BY fi.DueDate
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
