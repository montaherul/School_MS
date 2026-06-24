-- ============================================================================
-- Stored Procedure: sp_GetEmployeeIdCardBulkData
-- Purpose: Get full employee data for ID card PDF generation by comma-separated IDs
-- Author: School Management System
-- Created: June 16, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetEmployeeIdCardBulkData
    @Ids NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdsTable TABLE (Id INT);

    INSERT INTO @IdsTable (Id)
    SELECT CAST(value AS INT)
FROM STRING_SPLIT WITH(NOLOCK)(@Ids, ',')
    WHERE LTRIM(RTRIM(value)) <> '' AND ISNUMERIC(value) = 1;

    SELECT
        e.Id,
        e.EmployeeCode,
        e.FullName,
        e.FatherName,
        e.MotherName,
        e.Gender,
        e.DateOfBirth,
        e.BloodGroup,
        e.Religion,
        e.Nationality,
        e.NIDNumber,
        e.BirthCertificateNo,
        e.Phone,
        e.Email,
        e.PresentAddress,
        e.PermanentAddress,
        e.JoiningDate,
        COALESCE(d.Name, '') AS Department,
        COALESCE(desig.Name, '') AS Designation,
        e.EmployeeType,
        e.IsTeachingStaff,
        e.Status,
        e.ProfilePicturePath,
        e.SignaturePath,
        e.EmergencyContactName,
        e.EmergencyContactPhone,
        e.Remarks,
        e.EmployeeCardNumber,
        e.CardIssueDate,
        e.CardExpiryDate,
        e.CardPrintedAt,
        e.CardVersion,
        e.QRVerificationCode
FROM Employees e WITH(NOLOCK)
    INNER JOIN @IdsTable t ON e.Id = t.Id
LEFT JOIN Departments d WITH(NOLOCK) ON e.DepartmentId = d.Id AND d.IsDeleted = 0
LEFT JOIN Designations desig WITH(NOLOCK) ON e.DesignationId = desig.Id AND desig.IsDeleted = 0
    WHERE e.IsDeleted = 0;
END;
GO
