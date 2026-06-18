-- ============================================================================
-- Stored Procedure: sp_GetFeeStructureList
-- Purpose: Get paginated fee structure with class and category names
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeStructureList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @SchoolClassId INT = 0,
    @FeeCategoryId INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH FeeData AS (
        SELECT 
            fs.Id,
            fs.SchoolClassId,
            c.Name AS ClassName,
            fs.FeeCategoryId,
            fc.Name AS FeeCategoryName,
            fs.AcademicYearId,
            ay.Name AS AcademicYearName,
            fs.FeeName,
            fs.Description,
            fs.Amount,
            fs.IsRecurring,
            fs.Frequency,
            fs.DueDay,
            fs.IsActive,
            ROW_NUMBER() OVER (ORDER BY c.SortOrder, fc.DisplayOrder, fs.FeeName) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FeeStructures fs
        INNER JOIN 
            Classes c ON fs.SchoolClassId = c.Id
        LEFT JOIN 
            FeeCategories fc ON fs.FeeCategoryId = fc.Id
        LEFT JOIN 
            AcademicYears ay ON fs.AcademicYearId = ay.Id
        WHERE 
            fs.IsDeleted = 0
            AND (@SchoolClassId = 0 OR fs.SchoolClassId = @SchoolClassId)
            AND (@FeeCategoryId = 0 OR fs.FeeCategoryId = @FeeCategoryId)
            AND (
                @SearchTerm IS NULL 
                OR fs.FeeName LIKE '%' + @SearchTerm + '%'
                OR c.Name LIKE '%' + @SearchTerm + '%'
                OR fc.Name LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        SchoolClassId,
        ClassName,
        FeeCategoryId,
        FeeCategoryName,
        AcademicYearId,
        AcademicYearName,
        FeeName,
        Description,
        Amount,
        IsRecurring,
        Frequency,
        DueDay,
        IsActive,
        TotalCount AS TotalRecords
    FROM 
        FeeData
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
