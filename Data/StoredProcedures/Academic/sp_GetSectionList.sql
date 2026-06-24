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


        SELECT 
            s.Id,
            s.Name,
            s.SchoolClassId,
            c.Name AS ClassName,
            (SELECT COUNT(*) FROM Students WHERE SectionId = s.Id AND IsDeleted = 0) AS StudentCount,

            COUNT(*) OVER () AS TotalRecords
        FROM 
Sections s WITH(NOLOCK)
        JOIN 
Classes c WITH(NOLOCK) ON s.SchoolClassId = c.Id
        WHERE 
            s.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR s.Name LIKE '%' + @SearchTerm + '%'
                OR c.Name LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY s.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
