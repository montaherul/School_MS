-- ============================================================================
-- Stored Procedure: sp_GetAdmissionList
-- Purpose: Get paginated admission applications with optional search and filter
-- Author: School Management System
-- Created: May 4, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetAdmissionList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @ClassId INT = 0,
    @Status INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Declare pagination variables
    DECLARE @Offset INT;
    SET @Offset = (@PageNumber - 1) * @PageSize;

    -- Build the base query with filters


        SELECT 
            a.Id,
            a.ApplicationNo,
            a.ApplicantName,
            a.ApplicantNameBangla,
            a.DateOfBirth,
            a.Gender,
            a.AppliedClassId,
            a.AppliedStudentGroupId,
            sg.Name AS AppliedStudentGroupName,
            c.Name AS ClassName,
            -- Convert Enum Int to String Label
            CASE a.[Status]
                WHEN 1 THEN 'Pending'
                WHEN 2 THEN 'Approved'
                WHEN 3 THEN 'Rejected'
                WHEN 4 THEN 'Converted'
                ELSE 'Unknown'
            END AS [Status],
            a.FatherName,
            a.FatherOccupation,
            a.MotherName,
            a.MotherOccupation,
            a.GuardianName,
            a.GuardianOccupation,
            a.GuardianEmail,
            a.GuardianMobileNumber,
            a.GuardianRelationship,
            a.GuardianNationalId,
            a.GuardianAddress,
            a.GuardianPhoto,
            a.GuardianRemarks,
            a.LinkedGuardianId,
            a.FatherOrGuardianMobileNo,
            a.ApplicantMobileNumber,
            a.AlternativeNumber,
            a.ApplicantEmail,
            a.Nationality,
            a.Religion,
            a.BloodGroup,
            a.BirthCertificateNo,
            a.BirthCertificatePath,
            a.PaymentSlipPath,
            a.PaymentMethod,
            a.TransactionDetails,
            a.PresentVillage,
            a.PresentPostOffice,
            a.PresentThana,
            a.PresentDistrict,
            a.PermanentVillage,
            a.PermanentPostOffice,
            a.PermanentThana,
            a.PermanentDistrict,
            a.ProfilePicturePath,
            a.CreatedBy,
            a.CreatedAt,

            COUNT(*) OVER () AS TotalRecords
        FROM 
Admissions a WITH(NOLOCK)
        LEFT JOIN 
Classes c WITH(NOLOCK) ON a.AppliedClassId = c.Id
        LEFT JOIN 
StudentGroups sg WITH(NOLOCK) ON a.AppliedStudentGroupId = sg.Id
        WHERE 
            a.IsDeleted = 0
            AND (@ClassId = 0 OR a.AppliedClassId = @ClassId)
            AND (
                @SearchTerm IS NULL OR @SearchTerm = ''
                OR a.ApplicantName LIKE '%' + @SearchTerm + '%'
                OR a.ApplicantNameBangla LIKE '%' + @SearchTerm + '%'
                OR a.ApplicationNo LIKE '%' + @SearchTerm + '%'
                OR a.FatherOrGuardianMobileNo LIKE '%' + @SearchTerm + '%'
                OR a.ApplicantMobileNumber LIKE '%' + @SearchTerm + '%'

            )
            AND (@Status IS NULL OR a.Status = @Status)
    
ORDER BY a.CreatedAt DESC, a.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
