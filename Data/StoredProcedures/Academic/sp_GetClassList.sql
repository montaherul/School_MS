-- ============================================================================
-- Stored Procedure: sp_GetClassList
-- Purpose: Get paginated class list with section and student counts
-- Author: School Management System
-- Created: May 4, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetClassList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH ClassData AS (
        SELECT 
            c.Id,
            c.Name,
            c.SortOrder,
            (SELECT COUNT(*) FROM Sections WHERE SchoolClassId = c.Id AND IsDeleted = 0) AS SectionCount,
            (SELECT COUNT(*) FROM Students WHERE ClassId = c.Id AND IsDeleted = 0) AS StudentCount,
            ROW_NUMBER() OVER (ORDER BY c.SortOrder, c.Id) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Classes c
        WHERE 
            c.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR c.Name LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        Name,
        SortOrder,
        SectionCount,
        StudentCount,
        TotalCount AS TotalRecords
    FROM 
        ClassData
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
