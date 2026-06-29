-- ============================================================================
-- Index Script: IX_AdmissionModule
-- Purpose: Performance indexes for Admission Module queries
-- Author: School Management System
-- Phase: XX+19 — Performance Optimization
-- ============================================================================

-- 1. Filtered index on Status for admission list/dashboard queries
--    Covers the common WHERE IsDeleted = 0 AND Status = @Status filter
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionApplications_Status' AND object_id = OBJECT_ID('Admissions'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AdmissionApplications_Status
        ON Admissions([Status])
        INCLUDE (Id, ApplicationNo, ApplicantName, AppliedClassId, CreatedAt)
        WHERE IsDeleted = 0;
END
GO

-- 2. Filtered index on AppliedClassId for class-filtered admission lists
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionApplications_AppliedClassId' AND object_id = OBJECT_ID('Admissions'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AdmissionApplications_AppliedClassId
        ON Admissions(AppliedClassId)
        INCLUDE (Id, ApplicationNo, ApplicantName, [Status], CreatedAt)
        WHERE IsDeleted = 0;
END
GO

-- 3. Unique index on ApplicationNo for fast lookup + duplicate prevention
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionApplications_ApplicationNo' AND object_id = OBJECT_ID('Admissions'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_AdmissionApplications_ApplicationNo
        ON Admissions(ApplicationNo)
        INCLUDE (Id, ApplicantName, [Status], AppliedClassId, CreatedAt)
        WHERE IsDeleted = 0;
END
GO

-- 4. Filtered index on CreatedAt for dashboard/trend queries
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionApplications_CreatedAt' AND object_id = OBJECT_ID('Admissions'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AdmissionApplications_CreatedAt
        ON Admissions(CreatedAt DESC)
        INCLUDE (Id, ApplicationNo, [Status])
        WHERE IsDeleted = 0;
END
GO

-- 5. Filtered index on ReviewedByUserId for reviewer-based queries
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionApplications_ReviewedBy' AND object_id = OBJECT_ID('Admissions'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AdmissionApplications_ReviewedBy
        ON Admissions(ReviewedByUserId)
        INCLUDE (Id, ApplicationNo, [Status])
        WHERE IsDeleted = 0 AND ReviewedByUserId IS NOT NULL;
END
GO

-- 6. Index on AdmissionApplicationId for document queries
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionDocuments_ApplicationId' AND object_id = OBJECT_ID('AdmissionDocuments'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AdmissionDocuments_ApplicationId
        ON AdmissionDocuments(AdmissionApplicationId)
        INCLUDE (Id, DocumentType, FilePath, VerificationStatus)
        WHERE IsDeleted = 0;
END
GO

-- 7. Index for GuardianCode lookups
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Guardians_GuardianCode' AND object_id = OBJECT_ID('Guardians'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Guardians_GuardianCode
        ON Guardians(GuardianCode)
        INCLUDE (Id, FullName, Email, MobileNumber)
        WHERE IsDeleted = 0;
END
GO

-- 8. Index on Email for guardian lookup during conversion
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Guardians_Email' AND object_id = OBJECT_ID('Guardians'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Guardians_Email
        ON Guardians(Email)
        INCLUDE (Id, FullName, GuardianCode)
        WHERE IsDeleted = 0 AND Email IS NOT NULL;
END
GO

PRINT 'Admission Module indexes created successfully.';
GO
