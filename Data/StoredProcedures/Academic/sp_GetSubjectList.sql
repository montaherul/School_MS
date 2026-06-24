-- ============================================================================
-- Stored Procedure: sp_GetSubjectList
-- Purpose: Get paginated subject list with search
-- Author: School Management System
-- Created: May 6, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetSubjectList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT 
            s.Id,
            s.Code,
            s.Name,

            COUNT(*) OVER () AS TotalRecords
        FROM 
Subjects s WITH(NOLOCK)
        WHERE 
            s.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR s.Code LIKE '%' + @SearchTerm + '%'
                OR s.Name LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY s.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
