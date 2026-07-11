-- ============================================================================
-- Stored Procedure: sp_GetEmployeeDetails
-- Purpose: Get full employee details with all related data for profile view
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetEmployeeDetails
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Main employee record
    SELECT
        e.Id, e.EmployeeCode, e.FullName, e.BanglaName,
        e.FatherName, e.MotherName, e.SpouseName,
        e.Gender, e.MaritalStatus, e.DateOfBirth, e.BloodGroup,
        e.Religion, e.Nationality,
        e.NIDNumber, e.BirthCertificateNo, e.PassportNo, e.TIN, e.DrivingLicenseNo,
        e.Phone, e.AlternateMobile, e.Email,
        e.PresentAddress, e.PermanentAddress,
        e.JoiningDate, e.EmployeeType, e.IsTeachingStaff, e.Status,
        e.ProfilePicturePath, e.SignaturePath,
        e.EmergencyContactName, e.EmergencyContactPhone, e.Remarks,
        COALESCE(d.Name, '') AS Department,
        COALESCE(desig.Name, '') AS Designation,
        COALESCE(u.UserName, '') AS Username,
        e.EmployeeCardNumber, e.CardIssueDate, e.CardExpiryDate,
        e.CardPrintedAt, e.CardVersion, e.QRVerificationCode
    FROM Employees e WITH(NOLOCK)
    LEFT JOIN Departments d WITH(NOLOCK) ON e.DepartmentId = d.Id AND d.IsDeleted = 0
    LEFT JOIN Designations desig WITH(NOLOCK) ON e.DesignationId = desig.Id AND desig.IsDeleted = 0
    LEFT JOIN Users u WITH(NOLOCK) ON e.UserId = u.Id AND u.IsDeleted = 0
    WHERE e.Id = @EmployeeId AND e.IsDeleted = 0;

    -- Qualifications
    SELECT Id, EmployeeId, ExamName, BoardOrUniversity, InstituteName,
           GroupOrSubject, PassingYear, Result, CGPAOrDivision, CertificateFilePath
    FROM EmployeeQualifications WITH(NOLOCK)
    WHERE EmployeeId = @EmployeeId AND IsDeleted = 0
    ORDER BY PassingYear DESC;

    -- Documents
    SELECT Id, EmployeeId, DocumentType, DocumentName, FilePath, ExpiryDate, Remarks
    FROM EmployeeDocuments WITH(NOLOCK)
    WHERE EmployeeId = @EmployeeId AND IsDeleted = 0;

    -- Experience
    SELECT Id, EmployeeId, OrganizationName, Designation, StartDate, EndDate, Remarks
    FROM EmployeeExperiences WITH(NOLOCK)
    WHERE EmployeeId = @EmployeeId AND IsDeleted = 0
    ORDER BY StartDate DESC;

    -- Bank Accounts
    SELECT Id, EmployeeId, BankName, BranchName, AccountNumber, RoutingNumber, AccountType, IsDefault, IsActive
    FROM EmployeeBankAccounts WITH(NOLOCK)
    WHERE EmployeeId = @EmployeeId AND IsDeleted = 0;

    -- Promotions
    SELECT Id, EmployeeId, PreviousDesignationId, NewDesignationId, Reason, PromotionDate, PreviousSalary, NewSalary, Remarks
    FROM EmployeePromotions WITH(NOLOCK)
    WHERE EmployeeId = @EmployeeId AND IsDeleted = 0
    ORDER BY PromotionDate DESC;

    -- Transfers
    SELECT Id, EmployeeId, FromDepartmentId, ToDepartmentId, Reason, TransferDate, Remarks
    FROM EmployeeTransfers WITH(NOLOCK)
    WHERE EmployeeId = @EmployeeId AND IsDeleted = 0
    ORDER BY TransferDate DESC;

    -- Training
    SELECT Id, EmployeeId, TrainingName, InstitutionName, Duration, StartDate, EndDate, CertificatePath, Remarks
    FROM EmployeeTrainings WITH(NOLOCK)
    WHERE EmployeeId = @EmployeeId AND IsDeleted = 0
    ORDER BY StartDate DESC;

    -- Awards
    SELECT Id, EmployeeId, AwardName, AwardedBy, AwardDate, Description, CertificatePath
    FROM EmployeeAwards WITH(NOLOCK)
    WHERE EmployeeId = @EmployeeId AND IsDeleted = 0
    ORDER BY AwardDate DESC;

    -- Disciplinary Actions
    SELECT Id, EmployeeId, ActionType, Reason, ActionDate, Description, DocumentPath,
           IsResolved, ResolvedAt, ResolutionRemarks
    FROM EmployeeDisciplinaryActions WITH(NOLOCK)
    WHERE EmployeeId = @EmployeeId AND IsDeleted = 0
    ORDER BY ActionDate DESC;
END;
GO
