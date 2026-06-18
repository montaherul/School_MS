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

    WITH Data AS (
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
            ROW_NUMBER() OVER (ORDER BY s.FullName, fw.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FeeWaivers fw
        INNER JOIN 
            Students s ON fw.StudentId = s.Id
        LEFT JOIN 
            FeeInvoices fi ON fw.FeeInvoiceId = fi.Id
        LEFT JOIN 
            FeeCategories fc ON fw.FeeCategoryId = fc.Id
        LEFT JOIN 
            FeeStructures fs ON fw.FeeStructureId = fs.Id
        WHERE 
            fw.IsDeleted = 0
            AND (@StudentId = 0 OR fw.StudentId = @StudentId)
            AND (
                @SearchTerm IS NULL 
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%'
                OR fw.Reason LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, StudentId, StudentName,
        FeeInvoiceId, InvoiceNo,
        FeeCategoryId, FeeCategoryName,
        FeeStructureId, FeeStructureName,
        WaiverType, WaiverValue, WaiverAmount,
        Reason, IsApproved, ValidFrom, ValidTo,
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
