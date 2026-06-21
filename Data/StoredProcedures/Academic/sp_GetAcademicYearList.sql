-- ============================================================================
-- Stored Procedure: sp_GetAcademicYearList
-- Purpose: Get paginated academic years with search
-- Author: School Management System
-- Created: May 6, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetAcademicYearList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH YearData AS (
        SELECT 
            y.Id,
            y.Name,
            y.StartsOn,
            y.EndsOn,
            y.IsActive,
            ROW_NUMBER() OVER (ORDER BY y.StartsOn DESC, y.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            AcademicYears y
        WHERE 
            y.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR y.Name LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        Name,
        StartsOn,
        EndsOn,
        IsActive,
        TotalCount AS TotalRecords
    FROM 
        YearData
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
