-- ============================================================================
-- Stored Procedure: sp_GetClassSubjects
-- Purpose: Get class-subject mappings with full details
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetClassSubjects
    @ClassId INT = NULL,
    @AcademicYearId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        cs.Id,
        cs.SchoolClassId AS ClassId,
        c.Name AS ClassName,
        cs.SubjectId,
        s.Code AS SubjectCode,
        s.Name AS SubjectName,
        s.Category,
        s.IsMandatory,
        s.IsOptional,
        s.IsPractical,
        s.TheoryMarks,
        s.PracticalMarks,
        s.PassMarks,
        csg.StudentGroupId AS GroupId,
        g.Name AS GroupName,
        COUNT(*) OVER () AS TotalRecords
    FROM ClassSubjects cs WITH(NOLOCK)
    JOIN Classes c WITH(NOLOCK) ON cs.SchoolClassId = c.Id
    JOIN Subjects s WITH(NOLOCK) ON cs.SubjectId = s.Id
    LEFT JOIN ClassSubjectGroups csg WITH(NOLOCK) ON cs.Id = csg.ClassSubjectId AND csg.IsDeleted = 0
    LEFT JOIN StudentGroups g WITH(NOLOCK) ON csg.StudentGroupId = g.Id
    WHERE cs.IsDeleted = 0
        AND (@ClassId IS NULL OR cs.SchoolClassId = @ClassId)
    ORDER BY c.SortOrder, s.Code, g.DisplayOrder;
END;
GO
