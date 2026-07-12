-- ============================================================================
-- Stored Procedure: sp_GetTeacherAssignments
-- Purpose: Get teacher assignments with full details for a given academic year
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetTeacherAssignments
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        tsa.Id,
        tsa.TeacherId,
        e.FullName AS TeacherName,
        e.EmployeeCode,
        tsa.SubjectId,
        s.Name AS SubjectName,
        tsa.ClassId,
        c.Name AS ClassName,
        tsa.SectionId,
        sec.Name AS SectionName,
        tsa.GroupId,
        g.Name AS GroupName,
        tsa.AcademicYearId,
        ay.Name AS AcademicYearName,
        ISNULL(tca.IsClassTeacher, 0) AS IsClassTeacher,
        tsa.CreatedAt
    FROM TeacherSubjectAssignments tsa WITH(NOLOCK)
    JOIN Employees e WITH(NOLOCK) ON tsa.TeacherId = e.Id
    JOIN Subjects s WITH(NOLOCK) ON tsa.SubjectId = s.Id
    JOIN Classes c WITH(NOLOCK) ON tsa.ClassId = c.Id
    LEFT JOIN Sections sec WITH(NOLOCK) ON tsa.SectionId = sec.Id
    LEFT JOIN StudentGroups g WITH(NOLOCK) ON tsa.GroupId = g.Id
    JOIN AcademicYears ay WITH(NOLOCK) ON tsa.AcademicYearId = ay.Id
    LEFT JOIN TeacherClassAssignments tca WITH(NOLOCK)
        ON tsa.TeacherId = tca.TeacherId
        AND tsa.ClassId = tca.ClassId
        AND (tsa.SectionId = tca.SectionId OR (tsa.SectionId IS NULL AND tca.SectionId IS NULL))
        AND (tsa.GroupId = tca.GroupId OR (tsa.GroupId IS NULL AND tca.GroupId IS NULL))
        AND tsa.AcademicYearId = tca.AcademicYearId
        AND tca.IsActive = 1
        AND tca.IsDeleted = 0
    WHERE tsa.IsDeleted = 0
        AND (@AcademicYearId IS NULL OR tsa.AcademicYearId = @AcademicYearId)
    ORDER BY c.SortOrder, s.Name, e.FullName;
END;
GO
