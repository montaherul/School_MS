-- ============================================================================
-- Stored Procedure: sp_GetStudentList
-- Purpose: Get paginated student list with optional search
-- Author: School Management System
-- Created: May 4, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetStudentList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH FilteredStudents AS (
        SELECT 
            s.Id,
            s.StudentNo,
            s.FullName,
            c.Name AS ClassName,
            sec.Name AS SectionName,
            s.RollNumber,
            CAST(s.[Status] AS NVARCHAR(50)) AS [Status],
            s.FatherName,
            s.FatherOccupation,
            s.MotherName,
            s.MotherOccupation,
            s.MobileNumber,
            s.EmailAddress,
            s.PresentVillage,
            s.PresentPostOffice,
            s.PresentThana,
            s.PresentDistrict,
            s.PermanentVillage,
            s.PermanentPostOffice,
            s.PermanentThana,
            s.PermanentDistrict,
            s.BloodGroup,
            s.Religion,
            s.Nationality,
            s.BirthCertificateNo,
            s.ProfilePicturePath,
            (SELECT TOP 1 Phone FROM Guardians WHERE StudentId = s.Id) AS FatherOrGuardianMobileNo,
            ROW_NUMBER() OVER (ORDER BY s.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Students s
        JOIN 
            Classes c ON s.ClassId = c.Id
        JOIN 
            Sections sec ON s.SectionId = sec.Id
        WHERE 
            s.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR s.StudentNo LIKE '%' + @SearchTerm + '%'
                OR s.MobileNumber LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        StudentNo,
        FullName,
        ClassName,
        SectionName,
        RollNumber,
        [Status],
        FatherName,
        FatherOccupation,
        MotherName,
        MotherOccupation,
        MobileNumber,
        EmailAddress,
        PresentVillage,
        PresentPostOffice,
        PresentThana,
        PresentDistrict,
        PermanentVillage,
        PermanentPostOffice,
        PermanentThana,
        PermanentDistrict,
        BloodGroup,
        Religion,
        Nationality,
        BirthCertificateNo,
        ProfilePicturePath,
        FatherOrGuardianMobileNo,
        TotalCount AS TotalRecords
    FROM 
        FilteredStudents
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
