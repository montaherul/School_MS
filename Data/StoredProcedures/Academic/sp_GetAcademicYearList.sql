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


        SELECT 
            y.Id,
            y.Name,
            y.StartsOn,
            y.EndsOn,
            y.IsActive,

            COUNT(*) OVER () AS TotalRecords
        FROM 
AcademicYears y WITH(NOLOCK)
        WHERE 
            y.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR y.Name LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY y.StartsOn DESC, y.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
