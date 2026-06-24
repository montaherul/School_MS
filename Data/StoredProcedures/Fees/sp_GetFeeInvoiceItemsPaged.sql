-- ============================================================================
-- Stored Procedure: sp_GetFeeInvoiceItemsPaged
-- Purpose: Get paginated fee invoice items
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeInvoiceItemsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @FeeInvoiceId INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT 
            fii.Id,
            fii.FeeInvoiceId,
            fi.InvoiceNo,
            fii.FeeStructureId,
            fs.FeeName AS FeeStructureName,
            fii.FeeCategoryId,
            fc.Name AS FeeCategoryName,
            fii.Description,
            fii.Amount,
            fii.DiscountAmount,
            fii.NetAmount,

            COUNT(*) OVER () AS TotalRecords
        FROM 
FeeInvoiceItems fii WITH(NOLOCK)
        INNER JOIN 
FeeInvoices fi WITH(NOLOCK) ON fii.FeeInvoiceId = fi.Id
        LEFT JOIN 
FeeStructures fs WITH(NOLOCK) ON fii.FeeStructureId = fs.Id
        LEFT JOIN 
FeeCategories fc WITH(NOLOCK) ON fii.FeeCategoryId = fc.Id
        WHERE 
            fii.IsDeleted = 0
            AND (@FeeInvoiceId = 0 OR fii.FeeInvoiceId = @FeeInvoiceId)
            AND (
                @SearchTerm IS NULL 
                OR fii.Description LIKE '%' + @SearchTerm + '%'
                OR fs.FeeName LIKE '%' + @SearchTerm + '%'
                OR fc.Name LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY fii.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
