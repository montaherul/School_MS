-- ============================================================================
-- Stored Procedure: sp_GetSectionList
-- Purpose: Get paginated section list with class details
-- Author: School Management System
-- Created: May 4, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetSectionList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH FilteredSections AS (
        SELECT 
            s.Id,
            s.Name,
            s.SchoolClassId,
            c.Name AS ClassName,
            (SELECT COUNT(*) FROM Students WHERE SectionId = s.Id AND IsDeleted = 0) AS StudentCount,
            ROW_NUMBER() OVER (ORDER BY s.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Sections s
        JOIN 
            Classes c ON s.SchoolClassId = c.Id
        WHERE 
            s.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR s.Name LIKE '%' + @SearchTerm + '%'
                OR c.Name LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        Name,
        SchoolClassId,
        ClassName,
        StudentCount,
        TotalCount AS TotalRecords
    FROM 
        FilteredSections
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
