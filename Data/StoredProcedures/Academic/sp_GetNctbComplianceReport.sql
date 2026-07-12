-- ============================================================================
-- Stored Procedure: sp_GetNctbComplianceReport
-- Purpose: Get NCTB compliance aggregated data for a given academic year
-- Author: School Management System
-- Created: July 11, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetNctbComplianceReport
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AcademicYearName NVARCHAR(30);
    DECLARE @CoreCount INT, @ElectiveCount INT, @VocationalCount INT, @ReligionCount INT, @TotalSubjects INT;
    DECLARE @TotalClasses INT, @MappedClasses INT, @GroupCount INT;
    DECLARE @HasScience BIT, @HasBusiness BIT, @HasHumanities BIT;
    DECLARE @HasPrimary BIT, @HasSecondary BIT, @HasIslamicStudies BIT;

    SELECT @AcademicYearName = Name FROM AcademicYears WITH(NOLOCK) WHERE Id = @AcademicYearId AND IsDeleted = 0;

    SELECT @CoreCount = COUNT(*) FROM Subjects WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND Category = N'Core';
    SELECT @ElectiveCount = COUNT(*) FROM Subjects WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND Category = N'Elective';
    SELECT @VocationalCount = COUNT(*) FROM Subjects WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND Category = N'Vocational';
    SELECT @ReligionCount = COUNT(*) FROM Subjects WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND IsReligionSubject = 1;
    SELECT @TotalSubjects = COUNT(*) FROM Subjects WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1;

    SELECT @TotalClasses = COUNT(*) FROM Classes WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1;
    SELECT @MappedClasses = COUNT(DISTINCT cs.SchoolClassId)
    FROM ClassSubjects cs WITH(NOLOCK)
    INNER JOIN Subjects s WITH(NOLOCK) ON cs.SubjectId = s.Id
    WHERE s.IsDeleted = 0 AND s.IsActive = 1 AND cs.IsActive = 1;

    SELECT @GroupCount = COUNT(*) FROM StudentGroups WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1;

    SELECT @HasScience = CASE WHEN EXISTS (
        SELECT 1 FROM StudentGroups WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND Name LIKE N'%Science%'
    ) THEN 1 ELSE 0 END;

    SELECT @HasBusiness = CASE WHEN EXISTS (
        SELECT 1 FROM StudentGroups WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND Name LIKE N'%Business%'
    ) THEN 1 ELSE 0 END;

    SELECT @HasHumanities = CASE WHEN EXISTS (
        SELECT 1 FROM StudentGroups WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND Name LIKE N'%Humanities%'
    ) THEN 1 ELSE 0 END;

    SELECT @HasPrimary = CASE WHEN EXISTS (
        SELECT 1 FROM Classes WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1
        AND (Name LIKE N'1%' OR Name LIKE N'2%' OR Name LIKE N'3%' OR Name LIKE N'4%' OR Name LIKE N'5%')
    ) THEN 1 ELSE 0 END;

    SELECT @HasSecondary = CASE WHEN EXISTS (
        SELECT 1 FROM Classes WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1
        AND (Name LIKE N'6%' OR Name LIKE N'7%' OR Name LIKE N'8%' OR Name LIKE N'9%' OR Name LIKE N'10%')
    ) THEN 1 ELSE 0 END;

    SELECT @HasIslamicStudies = CASE WHEN EXISTS (
        SELECT 1 FROM Subjects WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND IsReligionSubject = 1
        AND (ReligionType LIKE N'%Islam%')
    ) THEN 1 ELSE 0 END;

    SELECT
        @AcademicYearId AS AcademicYearId,
        @AcademicYearName AS AcademicYearName,
        @CoreCount AS CoreSubjectCount,
        @ElectiveCount AS ElectiveSubjectCount,
        @VocationalCount AS VocationalSubjectCount,
        @ReligionCount AS ReligionSubjectCount,
        @TotalSubjects AS TotalSubjectCount,
        @GroupCount AS GroupCount,
        @TotalClasses AS TotalClassCount,
        @MappedClasses AS MappedClassCount,
        @HasScience AS HasScienceGroup,
        @HasBusiness AS HasBusinessStudiesGroup,
        @HasHumanities AS HasHumanitiesGroup,
        CASE WHEN @CoreCount >= 6 THEN 1 ELSE 0 END AS HasCompulsoryCoreSubjects,
        CASE WHEN @ReligionCount >= 2 THEN 1 ELSE 0 END AS HasAllReligionTypes,
        @HasPrimary AS HasPrimaryClasses,
        @HasSecondary AS HasSecondaryClasses,
        @HasIslamicStudies AS HasIslamicStudies,
        STUFF((SELECT N', ' + Name FROM Subjects WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND Category = N'Core' ORDER BY Name FOR XML PATH('')), 1, 2, N'') AS CoreSubjectNames,
        STUFF((SELECT N', ' + Name FROM Subjects WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND Category = N'Elective' ORDER BY Name FOR XML PATH('')), 1, 2, N'') AS ElectiveSubjectNames,
        STUFF((SELECT N', ' + Name FROM Subjects WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND Category = N'Vocational' ORDER BY Name FOR XML PATH('')), 1, 2, N'') AS VocationalSubjectNames,
        STUFF((SELECT N', ' + Name FROM Subjects WITH(NOLOCK) WHERE IsDeleted = 0 AND IsActive = 1 AND IsReligionSubject = 1 ORDER BY Name FOR XML PATH('')), 1, 2, N'') AS ReligionSubjectNames;

END;
GO
