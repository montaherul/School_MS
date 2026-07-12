-- ============================================================================
-- Stored Procedure: sp_GetAcademicYearsPaged
-- Purpose: Get paginated academic years with new fields (Code, IsCurrent, IsLocked, Status)
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetAcademicYearsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        y.Id,
        y.Name,
        y.Code,
        y.StartsOn,
        y.EndsOn,
        y.IsActive,
        y.IsCurrent,
        y.IsLocked,
        y.Status,
        y.CreatedAt,
        COUNT(*) OVER () AS TotalRecords
    FROM AcademicYears y WITH(NOLOCK)
    WHERE y.IsDeleted = 0
        AND (
            @SearchTerm IS NULL
            OR y.Name LIKE '%' + @SearchTerm + '%'
            OR y.Code LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY y.StartsOn DESC, y.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
