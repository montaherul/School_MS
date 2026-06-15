==========================================================
SEED DATA AUDIT - MISSING DATA INSERT STATEMENTS
Generated: Phase 33 Database Seed Audit
Database: SchoolManagementSystemDb
Server: MONTAHERUL\SQLEXPRESS
==========================================================
DO NOT EXECUTE BLINDLY - Review each section before running.
All IDs are calculated based on existing data.
==========================================================

-- ===========================================================
-- SECTION 1: RESULT SETTINGS (CRITICAL - 0 records)
-- ===========================================================
-- ResultSettings table is empty. Must seed default configuration.

INSERT INTO ResultSettings (
    AcademicYearId, OptionalSubjectMode, FailSubjectMode,
    OptionalBonusMaxGPA, BestOfCount, RequirePassedOptionalOnly,
    MaxFailedCompulsoryAllowed, MinimumPromotionGPA, IncludeReligionInGPA,
    AutoCalculateComponentTotal, GpaRoundingPrecision,
    IsActive, CreatedBy, CreatedAt
)
VALUES
-- Active Academic Year (Id=1, 2026)
(1, 1, 0, 0.50, 1, 1, 0, 1.00, 1, 1, 2, 1, 'system', GETUTCDATE()),
-- Previous Academic Year (Id=8, 2024-2025)
(8, 1, 0, 0.50, 1, 1, 0, 1.00, 1, 1, 2, 1, 'system', GETUTCDATE());

-- ===========================================================
-- SECTION 2: ACADEMIC YEAR FIX (HIGH - both years active)
-- ===========================================================
-- ISSUE: Both AcademicYears have IsActive=1
-- FIX: Deactivate the older year

UPDATE AcademicYears SET IsActive = 0 WHERE Id = 8;

-- ===========================================================
-- SECTION 3: DUPLICATE ROLE FIX (HIGH)
-- ===========================================================
-- ISSUE: Two roles named 'Admin' (Id=8 and Id=26)
-- FIX: Rename one to disambiguate

UPDATE Roles SET Name = 'Exam Controller' WHERE Id = 26 AND Name = 'Admin';

-- ===========================================================
-- SECTION 4: EXAM SUBJECT LEGACY COLUMNS (MEDIUM)
-- ===========================================================
-- All 221 ExamSubjects have TotalWrittenMarks=70, TotalMCQMarks=30
-- These are legacy columns that should be removed per Phase 32A.
-- No action needed here - the code migration handles this.

-- ===========================================================
-- SECTION 5: STUDENT GROUP ASSIGNMENTS (MEDIUM)
-- ===========================================================
-- StudentGroupAssignments table is empty
-- Students exist but have no group assignments via junction table
-- The 3 students are all in Class One (no groups needed for class 1-8)

-- No INSERT needed: Classes 1-8 don't require group assignments.

-- ===========================================================
-- SECTION 6: MISSING COMPONENTS IN SUBJECTMARKSTRUCTURE (LOW)
-- ===========================================================
-- All 34 subjects already have SubjectMarkStructure entries.
-- Components seeded: WRITTEN, MCQ, PRACTICAL, VIVA, ASSIGNMENT,
--                     LAB, PORTFOLIO, PRESENTATION, ORAL, CT
-- No missing components detected.

-- ===========================================================
-- SECTION 7: CALENDAR HOLIDAY VERIFICATION (LOW)
-- ===========================================================
-- All required Bangladesh holidays are present:
-- Language Day (Feb 21), Independence Day (Mar 26),
-- Pohela Boishakh (Apr 14), May Day (May 1),
-- National Mourning Day (Aug 15), Victory Day (Dec 16),
-- Christmas (Dec 25), Eid-ul-Fitr, Eid-ul-Adha,
-- Ashura, Miladunnabi, Janmashtami, Durga Puja,
-- Buddha Purnima
-- 144 holidays total (46 non-weekly + 98 weekly)
-- No missing holidays.

-- ===========================================================
-- SECTION 8: GRADING RULES (COMPLETE)
-- ===========================================================
-- All 7 grading rules verified:
-- A+ (80-100 = 5.00), A (70-79 = 4.00), A- (60-69 = 3.50),
-- B (50-59 = 3.00), C (40-49 = 2.00), D (33-39 = 1.00),
-- F (0-32 = 0.00)
-- No missing grading rules.

-- ===========================================================
-- SECTION 9: EXAM TYPES (COMPLETE)
-- ===========================================================
-- 7 exam types verified:
-- First Terminal, Half Yearly, Second Terminal,
-- Annual, Final, Pre-Test, Test
-- No missing exam types.

-- ===========================================================
-- SECTION 10: PERMISSIONS (COMPLETE)
-- ===========================================================
-- 602 permissions across 47 modules
-- All required modules present:
-- Calendar (13), Exam (13), Marks (13), Result (13), Reports (13)
-- No missing modules.

-- ===========================================================
-- SUMMARY: ONLY 2 CRITICAL FIXES NEEDED
-- ===========================================================
-- 1. Insert ResultSettings (0 -> 2 records)
-- 2. Fix duplicate Admin role (rename Id=26 to 'Exam Controller')
-- 3. Deactivate old AcademicYear (Id=8)
