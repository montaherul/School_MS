-- ============================================================================
-- Stored Procedure: sp_GetAcademicDashboard
-- Purpose: Get aggregated academic dashboard metrics in a single query
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetAcademicDashboard
    @AcademicYearId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT COUNT(*) FROM Classes WHERE IsDeleted = 0) AS TotalClasses,
        (SELECT COUNT(*) FROM Sections WHERE IsDeleted = 0) AS TotalSections,
        (SELECT COUNT(*) FROM Subjects WHERE IsDeleted = 0) AS TotalSubjects,
        (SELECT COUNT(*) FROM Employees WHERE IsDeleted = 0 AND IsTeachingStaff = 1) AS TotalTeachers,
        (SELECT COUNT(*) FROM Students WHERE IsDeleted = 0) AS TotalStudents,
        (SELECT COUNT(*) FROM Rooms WHERE IsDeleted = 0) AS TotalRooms,
        (SELECT COUNT(*) FROM SchoolSessions WHERE IsDeleted = 0) AS TotalSessions,
        (SELECT COUNT(*) FROM SchoolShifts WHERE IsDeleted = 0) AS TotalShifts,
        (SELECT COUNT(*) FROM Buildings WHERE IsDeleted = 0) AS TotalBuildings,
        (SELECT COUNT(*) FROM AcademicYears WHERE IsDeleted = 0 AND IsActive = 1) AS ActiveAcademicYears,
        (SELECT COUNT(*) FROM StudentGroups WHERE IsDeleted = 0 AND Code = 'SCIENCE') AS ScienceGroupCount,
        (SELECT COUNT(*) FROM StudentGroups WHERE IsDeleted = 0 AND Code = 'BUSINESS') AS BusinessGroupCount,
        (SELECT COUNT(*) FROM StudentGroups WHERE IsDeleted = 0 AND Code = 'HUMANITIES') AS HumanitiesGroupCount;
END;
GO
