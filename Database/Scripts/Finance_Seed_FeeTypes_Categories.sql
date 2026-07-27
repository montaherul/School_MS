-- ============================================================================
-- Script: Finance_Seed_FeeTypes_Categories.sql
-- Purpose: Seed all predefined FeeCategories and FeeTypes for the enterprise
-- School Management System.
-- Covers: Admission, Academic, Examination, Inventory, Certificate, Other Charges
-- Excludes: Transport, Library, Hostel (separate modules)
-- ============================================================================

-- ===========================
-- 1. FEE CATEGORIES
-- ===========================
IF NOT EXISTS (SELECT 1 FROM FeeCategories WHERE Name = 'Admission')
    INSERT INTO FeeCategories (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Admission', 'Admission-related fees (application, registration, admission, etc.)', 1, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeCategories WHERE Name = 'Academic')
    INSERT INTO FeeCategories (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Academic', 'Regular academic fees (tuition, session, lab, etc.)', 2, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeCategories WHERE Name = 'Examination')
    INSERT INTO FeeCategories (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Examination', 'Exam-related fees (mid-term, final, board, etc.)', 3, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeCategories WHERE Name = 'Inventory')
    INSERT INTO FeeCategories (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Inventory', 'Inventory and supplies fees (uniform, books, stationery, ID card, etc.)', 4, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeCategories WHERE Name = 'Certificate')
    INSERT INTO FeeCategories (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Certificate', 'Certificate and document fees (TC, transcript, character cert, etc.)', 5, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeCategories WHERE Name = 'Other Charges')
    INSERT INTO FeeCategories (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Other Charges', 'Miscellaneous charges (events, tours, clubs, donations, etc.)', 6, 1, 'system', SYSDATETIME(), 0);

-- ===========================
-- 2. FEE TYPES
-- ===========================
DECLARE @AdmissionCatId INT, @AcademicCatId INT, @ExamCatId INT,
        @InventoryCatId INT, @CertificateCatId INT, @OtherCatId INT;

SELECT @AdmissionCatId = Id FROM FeeCategories WHERE Name = 'Admission';
SELECT @AcademicCatId = Id FROM FeeCategories WHERE Name = 'Academic';
SELECT @ExamCatId = Id FROM FeeCategories WHERE Name = 'Examination';
SELECT @InventoryCatId = Id FROM FeeCategories WHERE Name = 'Inventory';
SELECT @CertificateCatId = Id FROM FeeCategories WHERE Name = 'Certificate';
SELECT @OtherCatId = Id FROM FeeCategories WHERE Name = 'Other Charges';

-- 2a. Admission Fee Types
IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Application Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Application Fee', 'One-time application/submission fee', 1, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Registration Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Registration Fee', 'Student registration/enrollment fee', 2, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Admission Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Admission Fee', 'One-time admission/entry fee', 3, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Prospectus Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Prospectus Fee', 'Prospectus/booklet fee', 4, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Admission Test Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Admission Test Fee', 'Admission test/examination fee', 5, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Security Deposit')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Security Deposit', 'Refundable security deposit', 6, 1, 'system', SYSDATETIME(), 0);

-- 2b. Academic Fee Types
IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Monthly Tuition')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Monthly Tuition', 'Monthly tuition fee', 1, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Session Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Session Fee', 'Per-session/annual fee', 2, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Annual Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Annual Fee', 'Annual subscription/development fee', 3, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Development Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Development Fee', 'School development/building fund', 4, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Smart Class Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Smart Class Fee', 'Smart classroom / multimedia fee', 5, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'ICT Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('ICT Fee', 'Information & communication technology lab fee', 6, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Laboratory Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Laboratory Fee', 'Science lab / practical lab fee', 7, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Sports Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Sports Fee', 'Sports and physical education fee', 8, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Cultural Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Cultural Fee', 'Cultural program and activities fee', 9, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Magazine Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Magazine Fee', 'School magazine / annual publication fee', 10, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Student Welfare Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Student Welfare Fee', 'Student welfare / health / emergency fund', 11, 1, 'system', SYSDATETIME(), 0);

-- 2c. Examination Fee Types
IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Mid-Term Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Mid-Term Fee', 'Mid-term examination fee', 1, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Final Exam Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Final Exam Fee', 'Final / annual examination fee', 2, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Practical Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Practical Fee', 'Practical / lab examination fee', 3, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Board Registration Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Board Registration Fee', 'Education board registration fee', 4, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Board Exam Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Board Exam Fee', 'Board examination / SSC fee', 5, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Admit Card Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Admit Card Fee', 'Admit card / hall ticket fee', 6, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Marksheet Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Marksheet Fee', 'Marksheet / grade sheet / transcript fee', 7, 1, 'system', SYSDATETIME(), 0);

-- 2d. Inventory Fee Types (Uniform, Books, Stationery, etc.)
IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Uniform')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Uniform', 'School uniform (shirt, pants, tie, blazer, etc.)', 1, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Books')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Books', 'Textbooks and reference books', 2, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Exercise Books')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Exercise Books', 'Exercise / practice notebooks', 3, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'School Diary')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('School Diary', 'School diary / planner', 4, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'ID Card')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('ID Card', 'Student ID card', 5, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Smart Card')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Smart Card', 'Smart card / RFID card', 6, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Stationery')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Stationery', 'General stationery items (pen, pencil, eraser, etc.)', 7, 1, 'system', SYSDATETIME(), 0);

-- 2e. Certificate Fee Types
IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Character Certificate')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Character Certificate', 'Character / conduct certificate', 1, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Transcript')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Transcript', 'Academic transcript / grade sheet', 2, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Transfer Certificate')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Transfer Certificate', 'Transfer certificate (TC)', 3, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Duplicate Certificate')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Duplicate Certificate', 'Duplicate certificate / document copy', 4, 1, 'system', SYSDATETIME(), 0);

-- 2f. Other Charges Fee Types
IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Event Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Event Fee', 'School event / program participation fee', 1, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Study Tour Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Study Tour Fee', 'Educational study tour / field trip fee', 2, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Club Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Club Fee', 'Student club / society membership fee', 3, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Alumni Fee')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Alumni Fee', 'Alumni association / network fee', 4, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Donation')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Donation', 'Voluntary donation / contribution', 5, 1, 'system', SYSDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM FeeTypes WHERE Name = 'Miscellaneous Charge')
    INSERT INTO FeeTypes (Name, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Miscellaneous Charge', 'Other miscellaneous / ad-hoc charge', 6, 1, 'system', SYSDATETIME(), 0);

PRINT 'Finance seed data: FeeCategories and FeeTypes populated successfully.';
GO
