-- ============================================================================
-- Stored Procedure: sp_GetSubjectsPaged
-- Purpose: Get paginated subject list with NCTB fields (TheoryMarks, PracticalMarks, etc.)
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetSubjectsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = NULL,
    @Category NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        s.Id,
        s.Code,
        s.Name,
        s.NameBn,
        s.Category,
        s.IsMandatory,
        s.IsOptional,
        s.IsPractical,
        s.TheoryMarks,
        s.PracticalMarks,
        s.PassMarks,
        s.Credit,
        s.NctbCode,
        s.IsActive,
        COUNT(*) OVER () AS TotalRecords
    FROM Subjects s WITH(NOLOCK)
    WHERE s.IsDeleted = 0
        AND (@Category IS NULL OR s.Category = @Category)
        AND (
            @SearchTerm IS NULL
            OR s.Code LIKE '%' + @SearchTerm + '%'
            OR s.Name LIKE '%' + @SearchTerm + '%'
            OR s.NameBn LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY s.Code
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
