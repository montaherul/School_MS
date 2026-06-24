-- ============================================================================
-- Stored Procedure: sp_GetStudentList
-- Purpose: Get paginated student records with optional search and filters
-- Author: School Management System
-- Created: May 12, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetStudentList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @ClassId INT = 0,
    @SectionId INT = 0,
    @Status INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Declare pagination variables
    DECLARE @Offset INT;
    SET @Offset = (@PageNumber - 1) * @PageSize;

    -- Build the base query with filters


        SELECT 
            s.Id,
            s.StudentNo,
            s.FullName,
            s.FullNameBangla,
            s.DateOfBirth,
            s.Gender,
            s.MobileNumber,
            s.EmailAddress,
            s.ClassId,
            c.Name AS ClassName,
            s.SectionId,
            sec.Name AS SectionName,
            s.RollNumber,
            s.ProfilePicturePath,
            -- Convert Enum Int to String Label
            CASE s.[Status]
                WHEN 1 THEN 'Active'
                WHEN 2 THEN 'Inactive'
                WHEN 3 THEN 'Graduated'
                WHEN 4 THEN 'Transferred'
                WHEN 5 THEN 'Dropped'
                ELSE 'Unknown'
            END AS [Status],
            s.FatherName,
            s.FatherOccupation,
            (SELECT TOP 1 g.MobileNumber 
FROM StudentGuardians sg WITH(NOLOCK) 
INNER JOIN Guardians g WITH(NOLOCK) ON sg.GuardianId = g.Id 
             WHERE sg.StudentId = s.Id AND sg.IsPrimaryGuardian = 1
            ) AS FatherOrGuardianMobileNo,
            s.MotherName,
            s.MotherOccupation,
            s.Religion,
            s.BloodGroup,
            s.PresentVillage,
            s.PresentPostOffice,
            s.PresentThana,
            s.PresentDistrict,
            s.PermanentVillage,
            s.PermanentPostOffice,
            s.PermanentThana,
            s.PermanentDistrict,
            s.BirthCertificateNo,
            s.Nationality,
            s.CreatedAt,

            COUNT(*) OVER () AS TotalRecords
        FROM 
Students s WITH(NOLOCK)
        LEFT JOIN 
Classes c WITH(NOLOCK) ON s.ClassId = c.Id
        LEFT JOIN 
Sections sec WITH(NOLOCK) ON s.SectionId = sec.Id
        WHERE 
            s.IsDeleted = 0
            AND (@ClassId = 0 OR s.ClassId = @ClassId)
            AND (@SectionId = 0 OR s.SectionId = @SectionId)
            AND (
                @SearchTerm IS NULL OR @SearchTerm = ''
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR s.StudentNo LIKE '%' + @SearchTerm + '%'
                OR s.MobileNumber LIKE '%' + @SearchTerm + '%'
                OR s.FatherName LIKE '%' + @SearchTerm + '%'
            )
            AND (@Status IS NULL OR s.Status = @Status)
    
ORDER BY s.CreatedAt DESC, s.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
