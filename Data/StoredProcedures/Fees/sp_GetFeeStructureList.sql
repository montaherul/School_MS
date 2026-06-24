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

            COUNT(*) OVER () AS TotalRecords
        FROM 
FeeStructures fs WITH(NOLOCK)
        INNER JOIN 
Classes c WITH(NOLOCK) ON fs.SchoolClassId = c.Id
        LEFT JOIN 
FeeCategories fc WITH(NOLOCK) ON fs.FeeCategoryId = fc.Id
        LEFT JOIN 
AcademicYears ay WITH(NOLOCK) ON fs.AcademicYearId = ay.Id
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
    
ORDER BY c.SortOrder, fc.DisplayOrder, fs.FeeName, fs.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
