CREATE OR ALTER PROCEDURE sp_GetClassCollectionSummary
    @AcademicYearId INT = 0,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT c.Name AS ClassName,
           ISNULL(SUM(fi.TotalAmount), 0) AS TotalAssigned,
           ISNULL(SUM(fi.PaidAmount), 0) AS TotalCollected,
           ISNULL(SUM(fi.TotalAmount - fi.PaidAmount), 0) AS TotalDue,
           CASE WHEN ISNULL(SUM(fi.TotalAmount), 0) > 0
                THEN (ISNULL(SUM(fi.PaidAmount), 0) * 100.0 / NULLIF(SUM(fi.TotalAmount), 0))
                ELSE 0 END AS CollectionRate,
           COUNT(DISTINCT fi.StudentId) AS StudentCount,
           COUNT(*) OVER() AS TotalRecords
FROM FeeInvoices fi WITH(NOLOCK)
JOIN Students s WITH(NOLOCK) ON fi.StudentId = s.Id
JOIN Classes c WITH(NOLOCK) ON s.ClassId = c.Id
    WHERE fi.IsDeleted = 0 AND (@AcademicYearId = 0 OR fi.AcademicYearId = @AcademicYearId)
    GROUP BY c.Name, c.Id
    ORDER BY c.Name, c.Id
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
