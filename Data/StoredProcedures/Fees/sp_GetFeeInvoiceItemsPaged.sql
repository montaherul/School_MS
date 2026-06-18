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

    WITH Data AS (
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
            ROW_NUMBER() OVER (ORDER BY fii.Id) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FeeInvoiceItems fii
        INNER JOIN 
            FeeInvoices fi ON fii.FeeInvoiceId = fi.Id
        LEFT JOIN 
            FeeStructures fs ON fii.FeeStructureId = fs.Id
        LEFT JOIN 
            FeeCategories fc ON fii.FeeCategoryId = fc.Id
        WHERE 
            fii.IsDeleted = 0
            AND (@FeeInvoiceId = 0 OR fii.FeeInvoiceId = @FeeInvoiceId)
            AND (
                @SearchTerm IS NULL 
                OR fii.Description LIKE '%' + @SearchTerm + '%'
                OR fs.FeeName LIKE '%' + @SearchTerm + '%'
                OR fc.Name LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, FeeInvoiceId, InvoiceNo,
        FeeStructureId, FeeStructureName,
        FeeCategoryId, FeeCategoryName,
        Description, Amount, DiscountAmount, NetAmount,
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
