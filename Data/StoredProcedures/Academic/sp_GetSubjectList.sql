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

    WITH SubjectData AS (
        SELECT 
            s.Id,
            s.Code,
            s.Name,
            ROW_NUMBER() OVER (ORDER BY s.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Subjects s
        WHERE 
            s.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR s.Code LIKE '%' + @SearchTerm + '%'
                OR s.Name LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        Code,
        Name,
        TotalCount AS TotalRecords
    FROM 
        SubjectData
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
