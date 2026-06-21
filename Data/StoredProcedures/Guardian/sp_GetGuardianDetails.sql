CREATE OR ALTER PROCEDURE [dbo].[sp_GetGuardianDetails]
    @GuardianId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        g.Id,
        g.GuardianCode,
        g.FirstName,
        g.LastName,
        (g.FirstName + ' ' + g.LastName) AS FullName,
        g.Gender,
        g.DateOfBirth,
        CASE g.RelationType
            WHEN 1 THEN 'Father'
            WHEN 2 THEN 'Mother'
            WHEN 3 THEN 'LegalGuardian'
            WHEN 4 THEN 'Grandfather'
            WHEN 5 THEN 'Grandmother'
            WHEN 6 THEN 'Uncle'
            WHEN 7 THEN 'Aunt'
            WHEN 8 THEN 'Brother'
            WHEN 9 THEN 'Sister'
            WHEN 10 THEN 'Other'
            ELSE 'Other'
        END AS RelationType,
        g.NationalId,
        g.PassportNumber,
        g.Occupation,
        g.EmployerName,
        g.MonthlyIncome,
        g.MobileNumber,
        g.AlternativeMobileNumber,
        g.Email,
        g.PresentAddress,
        g.PermanentAddress,
        g.PhotoPath,
        g.EmergencyContactName,
        g.EmergencyContactNumber,
        g.PortalAccessEnabled,
        CASE g.Status
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'PendingActivation'
            ELSE 'Unknown'
        END AS Status,
        g.Remarks
    FROM Guardians g
    WHERE g.Id = @GuardianId AND g.IsDeleted = 0;

    -- Children section
    SELECT 
        s.Id AS StudentId,
        s.StudentNo,
        s.FullName,
        c.Name AS ClassName,
        sec.Name AS SectionName,
        s.RollNumber,
        CASE sg.Relationship
            WHEN 1 THEN 'Father'
            WHEN 2 THEN 'Mother'
            WHEN 3 THEN 'LegalGuardian'
            WHEN 4 THEN 'Grandfather'
            WHEN 5 THEN 'Grandmother'
            WHEN 6 THEN 'Uncle'
            WHEN 7 THEN 'Aunt'
            WHEN 8 THEN 'Brother'
            WHEN 9 THEN 'Sister'
            WHEN 10 THEN 'Other'
            ELSE 'Other'
        END AS RelationshipToStudent
    FROM StudentGuardians sg
    JOIN Students s ON sg.StudentId = s.Id AND s.IsDeleted = 0
    LEFT JOIN Classes c ON s.ClassId = c.Id
    LEFT JOIN Sections sec ON s.SectionId = sec.Id
    WHERE sg.GuardianId = @GuardianId AND sg.IsDeleted = 0;
END
GO
