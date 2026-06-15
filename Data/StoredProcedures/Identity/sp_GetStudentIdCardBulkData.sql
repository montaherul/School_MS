-- ============================================================================
-- Stored Procedure: sp_GetStudentIdCardBulkData
-- Purpose: Get full student data for ID card PDF generation by comma-separated IDs
-- Author: School Management System
-- Created: June 16, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetStudentIdCardBulkData
    @Ids NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdsTable TABLE (Id INT);

    INSERT INTO @IdsTable (Id)
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@Ids, ',')
    WHERE LTRIM(RTRIM(value)) <> '' AND ISNUMERIC(value) = 1;

    SELECT
        s.Id,
        s.StudentNo,
        s.FullName,
        s.FullNameBangla,
        s.DateOfBirth,
        s.Gender,
        s.FatherName,
        s.FatherOccupation,
        s.MotherName,
        s.MotherOccupation,
        g.FullName AS GuardianName,
        s.MobileNumber,
        s.EmailAddress,
        s.Nationality,
        s.BloodGroup,
        s.Religion,
        s.ClassId,
        s.SectionId,
        s.StudentGroupId,
        s.OptionalSubjectId,
        s.RollNumber,
        s.PresentVillage,
        s.PresentPostOffice,
        s.PresentThana,
        s.PresentDistrict,
        s.PermanentVillage,
        s.PermanentPostOffice,
        s.PermanentThana,
        s.PermanentDistrict,
        s.ProfilePicturePath,
        s.UserId,
        c.Name AS ClassName,
        sec.Name AS SectionName,
        sg.Name AS GroupName,
        ISNULL(g.MobileNumber, '') AS GuardianMobileNumber
    FROM Students s
    INNER JOIN @IdsTable t ON s.Id = t.Id
    LEFT JOIN Classes c ON s.ClassId = c.Id AND c.IsDeleted = 0
    LEFT JOIN Sections sec ON s.SectionId = sec.Id AND sec.IsDeleted = 0
    LEFT JOIN StudentGroups sg ON s.StudentGroupId = sg.Id AND sg.IsDeleted = 0
    LEFT JOIN StudentGuardians sg_guard ON sg_guard.StudentId = s.Id AND sg_guard.IsPrimaryGuardian = 1 AND sg_guard.IsDeleted = 0
    LEFT JOIN Guardians g ON sg_guard.GuardianId = g.Id AND g.IsDeleted = 0
    WHERE s.IsDeleted = 0;
END;
GO
