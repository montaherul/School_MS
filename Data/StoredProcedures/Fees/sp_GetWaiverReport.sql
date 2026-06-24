CREATE OR ALTER PROCEDURE sp_GetWaiverReport
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT fw.Id, s.FullName AS StudentName, fi.InvoiceNo, fw.WaiverAmount,
           fw.Reason, fw.IsApproved, fw.CreatedAt,
           ISNULL(fw.ApprovedBy, 'N/A') AS ApprovedBy,
           COUNT(*) OVER() AS TotalRecords
FROM FeeWaivers fw WITH(NOLOCK)
JOIN Students s WITH(NOLOCK) ON fw.StudentId = s.Id
LEFT JOIN FeeInvoices fi WITH(NOLOCK) ON fw.FeeInvoiceId = fi.Id
    WHERE fw.IsDeleted = 0
    ORDER BY fw.CreatedAt DESC, fw.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
