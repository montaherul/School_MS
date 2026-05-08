-- ============================================================================
-- Stored Procedure: sp_GetAdmissionList
-- Purpose: Get paginated admission applications with optional search and filter
-- Author: School Management System
-- Created: May 4, 2026
-- ============================================================================

ALTER PROCEDURE sp_GetAdmissionList
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
    ;WITH FilteredAdmissions AS (
        SELECT 
            a.Id,
            a.ApplicationNo,
            a.ApplicantName,
            a.DateOfBirth,
            a.Gender,
            a.AppliedClassId,
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
            a.FatherOrGuardianMobileNo,
            a.ApplicantMobileNumber,
            a.AlternativeNumber,
            a.ApplicantEmail,
            a.Nationality,
            a.Religion,
            a.BloodGroup,
            a.NationalIdNo,
            a.BirthCertificateNo,
            a.PassportNo,
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
            ROW_NUMBER() OVER (ORDER BY a.CreatedAt DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Admissions a
        LEFT JOIN 
            Classes c ON a.AppliedClassId = c.Id
        WHERE 
            a.IsDeleted = 0
            AND (@ClassId = 0 OR a.AppliedClassId = @ClassId)
            AND (
                @SearchTerm IS NULL OR @SearchTerm = ''
                OR a.ApplicantName LIKE '%' + @SearchTerm + '%'
                OR a.ApplicationNo LIKE '%' + @SearchTerm + '%'
                OR a.FatherOrGuardianMobileNo LIKE '%' + @SearchTerm + '%'
                OR a.ApplicantMobileNumber LIKE '%' + @SearchTerm + '%'

            )
            AND (@Status IS NULL OR a.Status = @Status)
    )
    SELECT 
        Id,
        ApplicationNo,
        ApplicantName,
        DateOfBirth,
        Gender,
        AppliedClassId,
        ClassName,
        [Status],
        FatherName,
        FatherOccupation,
        MotherName,
        MotherOccupation,
        GuardianName,
        GuardianOccupation,
        FatherOrGuardianMobileNo,
        ApplicantMobileNumber,
        AlternativeNumber,
        ApplicantEmail,
        Nationality,
        Religion,
        BloodGroup,
        NationalIdNo,
        BirthCertificateNo,
        PassportNo,
        PaymentMethod,
        TransactionDetails,
        PresentVillage,
        PresentPostOffice,
        PresentThana,
        PresentDistrict,
        PermanentVillage,
        PermanentPostOffice,
        PermanentThana,
        PermanentDistrict,
        ProfilePicturePath,
        CreatedBy,
        CreatedAt,
        TotalCount AS TotalRecords
    FROM 
        FilteredAdmissions
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
