-- ============================================================================
-- Stored Procedure: sp_GetClassSubjectsPaged
-- Purpose: Get paginated class-subject mappings for Tabulator grid
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetClassSubjectsPaged
    @PageNumber INT = 1,
    @PageSize INT = 15,
    @ClassId INT = NULL,
    @GroupName NVARCHAR(50) = NULL,
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        cs.Id,
        cs.SchoolClassId,
        c.Name AS SchoolClassName,
        cs.SubjectId,
        s.Code AS SubjectCode,
        s.Name AS SubjectNameEn,
        s.NameBn AS SubjectNameBn,
        ISNULL(cs.GroupName, '') AS GroupName,
        cs.FullMarks,
        cs.PassMarks,
        cs.IsMandatory,
        cs.IsOptional,
        cs.IsReligionSubject,
        cs.ReligionType,
        cs.DisplayOrder,
        cs.IsActive,
        COUNT(*) OVER () AS TotalRecords
    FROM ClassSubjects cs WITH(NOLOCK)
    JOIN Classes c WITH(NOLOCK) ON cs.SchoolClassId = c.Id AND c.IsDeleted = 0
    JOIN Subjects s WITH(NOLOCK) ON cs.SubjectId = s.Id AND s.IsDeleted = 0
    WHERE cs.IsDeleted = 0
        AND (@ClassId IS NULL OR cs.SchoolClassId = @ClassId)
        AND (@GroupName IS NULL OR cs.GroupName = @GroupName)
        AND (
            @SearchTerm IS NULL
            OR c.Name LIKE '%' + @SearchTerm + '%'
            OR s.Name LIKE '%' + @SearchTerm + '%'
            OR s.NameBn LIKE '%' + @SearchTerm + '%'
            OR s.Code LIKE '%' + @SearchTerm + '%'
            OR cs.GroupName LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY c.SortOrder, cs.DisplayOrder, s.Code
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
