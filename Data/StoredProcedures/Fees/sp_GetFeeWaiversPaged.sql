-- ============================================================================
-- Stored Procedure: sp_GetFeeWaiversPaged
-- Purpose: Get paginated fee waivers
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeWaiversPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT 
            fw.Id,
            fw.StudentId,
            s.FullName AS StudentName,
            fw.FeeInvoiceId,
            fi.InvoiceNo,
            fw.FeeCategoryId,
            fc.Name AS FeeCategoryName,
            fw.FeeStructureId,
            fs.FeeName AS FeeStructureName,
            fw.WaiverType,
            fw.WaiverValue,
            fw.WaiverAmount,
            fw.Reason,
            fw.IsApproved,
            fw.ValidFrom,
            fw.ValidTo,

            COUNT(*) OVER () AS TotalRecords
        FROM 
FeeWaivers fw WITH(NOLOCK)
        INNER JOIN 
Students s WITH(NOLOCK) ON fw.StudentId = s.Id
        LEFT JOIN 
FeeInvoices fi WITH(NOLOCK) ON fw.FeeInvoiceId = fi.Id
        LEFT JOIN 
FeeCategories fc WITH(NOLOCK) ON fw.FeeCategoryId = fc.Id
        LEFT JOIN 
FeeStructures fs WITH(NOLOCK) ON fw.FeeStructureId = fs.Id
        WHERE 
            fw.IsDeleted = 0
            AND (@StudentId = 0 OR fw.StudentId = @StudentId)
            AND (
                @SearchTerm IS NULL 
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%'
                OR fw.Reason LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY s.FullName, fw.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
