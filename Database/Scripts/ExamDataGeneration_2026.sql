-- ============================================================
-- COMPLETE EXAM DATA GENERATION FOR ACADEMIC YEAR 2026
-- ============================================================
-- Generates full enterprise-grade exam lifecycle for:
--   First Terminal 2026: Classes 1,2,3,4,5
--   Half Yearly 2026:    Classes 6,7,8, 9Sci/9Bus/9Hum, 10Sci/10Bus/10Hum
-- ============================================================
-- Usage: Run against SchoolManagementSystemDb on MONTAHERUL\SQLEXPRESS
-- ============================================================

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_WARNINGS ON;
SET ANSI_PADDING ON;
SET ANSI_NULL_DFLT_ON ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Now DATETIME2 = SYSUTCDATETIME();
    DECLARE @SYSTEM NVARCHAR(64) = 'system';
    DECLARE @AcademicYearId INT = 1; -- 2026

    PRINT '============================================================';
    PRINT 'EXAM DATA GENERATION 2026 - START';
    PRINT '============================================================';

    -- ============================================================
    -- 1. CLASS SUBJECTS FOR MISSING CLASSES
    -- ============================================================
    PRINT '--- 1. Adding ClassSubjects for missing classes ---';

    -- Classes 4-5 (Primary): Same as Classes 1-3
    -- Subjects: 1(BAN),2(ENG),3(MAT),4(GSCI),5(SOC),6(REL),7(ART),8(PE),34(MUS)
    SET IDENTITY_INSERT ClassSubjects ON;
    INSERT INTO ClassSubjects (Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, IsMandatory, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, 1, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES
        (49,  4, 1,  NULL, NULL, 100, 33, 1),
        (50,  4, 2,  NULL, NULL, 100, 33, 2),
        (51,  4, 3,  NULL, NULL, 100, 33, 3),
        (52,  4, 4,  NULL, NULL, 100, 33, 4),
        (53,  4, 5,  NULL, NULL, 100, 33, 5),
        (54,  4, 6,  NULL, NULL, 100, 33, 6),
        (55,  4, 7,  NULL, NULL, 100, 33, 7),
        (56,  4, 8,  NULL, NULL, 100, 33, 8),
        (57,  4, 34, NULL, NULL, 100, 33, 9),
        (58,  5, 1,  NULL, NULL, 100, 33, 1),
        (59,  5, 2,  NULL, NULL, 100, 33, 2),
        (60,  5, 3,  NULL, NULL, 100, 33, 3),
        (61,  5, 4,  NULL, NULL, 100, 33, 4),
        (62,  5, 5,  NULL, NULL, 100, 33, 5),
        (63,  5, 6,  NULL, NULL, 100, 33, 6),
        (64,  5, 7,  NULL, NULL, 100, 33, 7),
        (65,  5, 8,  NULL, NULL, 100, 33, 8),
        (66,  5, 34, NULL, NULL, 100, 33, 9)
    ) CS(Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder)
    WHERE NOT EXISTS (SELECT 1 FROM ClassSubjects WHERE Id = CS.Id);
    SET IDENTITY_INSERT ClassSubjects OFF;

    -- Classes 6-8 (Junior Secondary): Bangla 1st(9), Bangla 2nd(10), English 1st(11), English 2nd(12),
    -- Mathematics(3), Science(13), ICT(14), Religion(6), Career Ed(27), PE/Health(28)
    SET IDENTITY_INSERT ClassSubjects ON;
    INSERT INTO ClassSubjects (Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, IsMandatory, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, 1, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES
        (67,  6, 9,  NULL, NULL, 100, 33, 1),
        (68,  6, 10, NULL, NULL, 100, 33, 2),
        (69,  6, 11, NULL, NULL, 100, 33, 3),
        (70,  6, 12, NULL, NULL, 100, 33, 4),
        (71,  6, 3,  NULL, NULL, 100, 33, 5),
        (72,  6, 13, NULL, NULL, 100, 33, 6),
        (73,  6, 14, NULL, NULL, 100, 33, 7),
        (74,  6, 6,  NULL, NULL, 100, 33, 8),
        (75,  6, 27, NULL, NULL, 100, 33, 9),
        (76,  6, 28, NULL, NULL, 100, 33, 10),
        (77,  7, 9,  NULL, NULL, 100, 33, 1),
        (78,  7, 10, NULL, NULL, 100, 33, 2),
        (79,  7, 11, NULL, NULL, 100, 33, 3),
        (80,  7, 12, NULL, NULL, 100, 33, 4),
        (81,  7, 3,  NULL, NULL, 100, 33, 5),
        (82,  7, 13, NULL, NULL, 100, 33, 6),
        (83,  7, 14, NULL, NULL, 100, 33, 7),
        (84,  7, 6,  NULL, NULL, 100, 33, 8),
        (85,  7, 27, NULL, NULL, 100, 33, 9),
        (86,  7, 28, NULL, NULL, 100, 33, 10),
        (87,  8, 9,  NULL, NULL, 100, 33, 1),
        (88,  8, 10, NULL, NULL, 100, 33, 2),
        (89,  8, 11, NULL, NULL, 100, 33, 3),
        (90,  8, 12, NULL, NULL, 100, 33, 4),
        (91,  8, 3,  NULL, NULL, 100, 33, 5),
        (92,  8, 13, NULL, NULL, 100, 33, 6),
        (93,  8, 14, NULL, NULL, 100, 33, 7),
        (94,  8, 6,  NULL, NULL, 100, 33, 8),
        (95,  8, 27, NULL, NULL, 100, 33, 9),
        (96,  8, 28, NULL, NULL, 100, 33, 10)
    ) CS(Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder)
    WHERE NOT EXISTS (SELECT 1 FROM ClassSubjects WHERE Id = CS.Id);
    SET IDENTITY_INSERT ClassSubjects OFF;

    -- Class 9 Business Studies (GroupId=2): Bangla 1st(9), Bangla 2nd(10), English 1st(11), English 2nd(12),
    -- Mathematics(3), Science(13), ICT(14), Accounting(20), Finance(21), Business Ent(22)
    SET IDENTITY_INSERT ClassSubjects ON;
    INSERT INTO ClassSubjects (Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, IsMandatory, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, 1, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES
        (97,  9, 9,  2, NULL, 100, 33, 1),
        (98,  9, 10, 2, NULL, 100, 33, 2),
        (99,  9, 11, 2, NULL, 100, 33, 3),
        (100, 9, 12, 2, NULL, 100, 33, 4),
        (101, 9, 3,  2, NULL, 100, 33, 5),
        (102, 9, 13, 2, NULL, 100, 33, 6),
        (103, 9, 14, 2, NULL, 100, 33, 7),
        (104, 9, 20, 2, NULL, 100, 33, 8),
        (105, 9, 21, 2, NULL, 100, 33, 9),
        (106, 9, 22, 2, NULL, 100, 33, 10)
    ) CS(Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder)
    WHERE NOT EXISTS (SELECT 1 FROM ClassSubjects WHERE Id = CS.Id);
    SET IDENTITY_INSERT ClassSubjects OFF;

    -- Class 9 Humanities (GroupId=3): Bangla 1st(9), Bangla 2nd(10), English 1st(11), English 2nd(12),
    -- Mathematics(3), ICT(14), History(23), Geography(24), Economics(25), Civics(26)
    SET IDENTITY_INSERT ClassSubjects ON;
    INSERT INTO ClassSubjects (Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, IsMandatory, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, 1, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES
        (107, 9, 9,  3, NULL, 100, 33, 1),
        (108, 9, 10, 3, NULL, 100, 33, 2),
        (109, 9, 11, 3, NULL, 100, 33, 3),
        (110, 9, 12, 3, NULL, 100, 33, 4),
        (111, 9, 3,  3, NULL, 100, 33, 5),
        (112, 9, 14, 3, NULL, 100, 33, 6),
        (113, 9, 23, 3, NULL, 100, 33, 7),
        (114, 9, 24, 3, NULL, 100, 33, 8),
        (115, 9, 25, 3, NULL, 100, 33, 9),
        (116, 9, 26, 3, NULL, 100, 33, 10)
    ) CS(Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder)
    WHERE NOT EXISTS (SELECT 1 FROM ClassSubjects WHERE Id = CS.Id);
    SET IDENTITY_INSERT ClassSubjects OFF;

    -- Class 10 Science (GroupId=1): Bangla 1st(9), Bangla 2nd(10), English 1st(11), English 2nd(12),
    -- ICT(14), Physics(16), Chemistry(17), Biology(18), Higher Math(19)
    SET IDENTITY_INSERT ClassSubjects ON;
    INSERT INTO ClassSubjects (Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, IsMandatory, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, 1, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES
        (117, 10, 9,  1, NULL, 100, 33, 1),
        (118, 10, 10, 1, NULL, 100, 33, 2),
        (119, 10, 11, 1, NULL, 100, 33, 3),
        (120, 10, 12, 1, NULL, 100, 33, 4),
        (121, 10, 14, 1, NULL, 100, 33, 5),
        (122, 10, 16, 1, NULL, 100, 33, 6),
        (123, 10, 17, 1, NULL, 100, 33, 7),
        (124, 10, 18, 1, NULL, 100, 33, 8),
        (125, 10, 19, 1, NULL, 100, 33, 9)
    ) CS(Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder)
    WHERE NOT EXISTS (SELECT 1 FROM ClassSubjects WHERE Id = CS.Id);
    SET IDENTITY_INSERT ClassSubjects OFF;

    -- Class 10 Humanities (GroupId=3): Bangla 1st(9), Bangla 2nd(10), English 1st(11), English 2nd(12),
    -- Mathematics(3), ICT(14), History(23), Geography(24), Economics(25), Civics(26)
    SET IDENTITY_INSERT ClassSubjects ON;
    INSERT INTO ClassSubjects (Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, IsMandatory, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, 1, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES
        (126, 10, 9,  3, NULL, 100, 33, 1),
        (127, 10, 10, 3, NULL, 100, 33, 2),
        (128, 10, 11, 3, NULL, 100, 33, 3),
        (129, 10, 12, 3, NULL, 100, 33, 4),
        (130, 10, 3,  3, NULL, 100, 33, 5),
        (131, 10, 14, 3, NULL, 100, 33, 6),
        (132, 10, 23, 3, NULL, 100, 33, 7),
        (133, 10, 24, 3, NULL, 100, 33, 8),
        (134, 10, 25, 3, NULL, 100, 33, 9),
        (135, 10, 26, 3, NULL, 100, 33, 10)
    ) CS(Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder)
    WHERE NOT EXISTS (SELECT 1 FROM ClassSubjects WHERE Id = CS.Id);
    SET IDENTITY_INSERT ClassSubjects OFF;

    PRINT 'ClassSubjects added for classes 4,5,6,7,8, 9Bus,9Hum,10Sci,10Hum';

    -- ============================================================
    -- 2. ADD STUDENTS FOR MISSING CLASSES
    -- ============================================================
    PRINT '--- 2. Adding Students for classes without students ---';

    -- We need a UserId for each student. Since we don't have user accounts for these,
    -- we create them without UserId (nullable). The Students.UserId is optional.
    -- Section A for each class:
    --   Class 4 -> Section 7 (Class 4 A)
    --   Class 5 -> Section 9 (Class 5 A)
    --   Class 6 -> Section 11 (Class 6 A)
    --   Class 7 -> Section 13 (Class 7 A)
    --   Class 8 -> Section 15 (Class 8 A)
    --   Class 9 Business -> Section 21 (Class 9 Business A)
    --   Class 9 Humanities -> Section 24 (Class 9 Humanities A)
    --   Class 10 Science -> Section 27 (Class 10 Science A)
    --   Class 10 Humanities -> Section 33 (Class 10 Humanities A)

    -- Student IDs start at 13 (12 existing)
    SET IDENTITY_INSERT Students ON;
    INSERT INTO Students (
        Id, StudentNo, FullName, DateOfBirth, Gender, FatherName, MotherName,
        MobileNumber, Nationality, Country, MaritalStatus, Religion, AssignedReligionSubjectId,
        ClassId, SectionId, RollNumber, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    SELECT Id, StudentNo, FullName, DateOfBirth, Gender, FatherName, MotherName,
        MobileNumber, N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30,
        ClassId, SectionId, RollNumber, 1 /*Active*/, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES
        -- Class 4 Section A (Id=7), rolls 1-2
        (13, N'STU-2026-0013', N'Shamima Akhter',       '2015-02-14', N'Female', N'Khorshed Alam',  N'Nargis Begum',   N'01720000013', 4, 7, 1),
        (14, N'STU-2026-0014', N'Rony Miah',             '2015-07-20', N'Male',   N'Abul Hossain',   N'Shahida Begum',  N'01720000014', 4, 7, 2),
        -- Class 5 Section A (Id=9), rolls 1-2
        (15, N'STU-2026-0015', N'Nusrat Jahan',          '2014-03-08', N'Female', N'Abdur Rashid',   N'Fatima Begum',   N'01720000015', 5, 9, 1),
        (16, N'STU-2026-0016', N'Mehedi Hasan',           '2014-09-25', N'Male',   N'Jinnat Ali',     N'Sofina Begum',   N'01720000016', 5, 9, 2),
        -- Class 6 Section A (Id=11), rolls 1-2
        (17, N'STU-2026-0017', N'Sabina Yasmin',         '2013-01-12', N'Female', N'Abdul Malek',    N'Momena Begum',   N'01720000017', 6, 11, 1),
        (18, N'STU-2026-0018', N'Kamruzzaman',           '2013-06-30', N'Male',   N'Abdul Aziz',     N'Jahanara Begum', N'01720000018', 6, 11, 2),
        -- Class 7 Section A (Id=13), rolls 1-2
        (19, N'STU-2026-0019', N'Farida Yesmin',         '2012-04-18', N'Female', N'Mizanur Rahman', N'Aleya Begum',    N'01720000019', 7, 13, 1),
        (20, N'STU-2026-0020', N'Abul Hasan',            '2012-10-05', N'Male',   N'Sirajul Islam',  N'Ayesha Khatun',  N'01720000020', 7, 13, 2),
        -- Class 8 Section A (Id=15), rolls 1-2
        (21, N'STU-2026-0021', N'Samsun Nahar',          '2011-02-28', N'Female', N'Abdul Goni',     N'Rahena Begum',   N'01720000021', 8, 15, 1),
        (22, N'STU-2026-0022', N'Hasan Ali',              '2011-08-15', N'Male',   N'Sobahan Ali',    N'Rokeya Begum',   N'01720000022', 8, 15, 2),
        -- Class 9 Business Studies A (SectionId=21), rolls 1-2
        (23, N'STU-2026-0023', N'Shahinur Rahman',       '2009-03-22', N'Male',   N'Abdur Rouf',     N'Hosneara Begum', N'01720000023', 9, 21, 1),
        (24, N'STU-2026-0024', N'Tahmina Akhter',        '2009-11-10', N'Female', N'Abul Kalam',     N'Shahida Parvin', N'01720000024', 9, 21, 2),
        -- Class 9 Humanities A (SectionId=24), rolls 1-2
        (25, N'STU-2026-0025', N'Jahangir Alam',         '2009-05-08', N'Male',   N'Shamsuzzaman',   N'Jennatun Nessa', N'01720000025', 9, 24, 1),
        (26, N'STU-2026-0026', N'Roksana Begum',         '2009-12-20', N'Female', N'Abdul Mannan',   N'Shahin Ara',     N'01720000026', 9, 24, 2),
        -- Class 10 Science A (SectionId=27), rolls 1-2
        (27, N'STU-2026-0027', N'Shah Alam',             '2008-01-15', N'Male',   N'Nurul Huda',     N'Rahima Khatun',  N'01720000027', 10, 27, 1),
        (28, N'STU-2026-0028', N'Rahima Akhter',         '2008-07-30', N'Female', N'Mofijuddin',     N'Fatima Khatun',  N'01720000028', 10, 27, 2),
        -- Class 10 Humanities A (SectionId=33), rolls 1-2
        (29, N'STU-2026-0029', N'Abdur Rahim',           '2008-05-12', N'Male',   N'Abdul Hakim',    N'Parvin Begum',   N'01720000029', 10, 33, 1),
        (30, N'STU-2026-0030', N'Shahnaj Parvin',        '2008-10-25', N'Female', N'Shahjahan Ali',  N'Momena Khatun',  N'01720000030', 10, 33, 2)
    ) S(Id, StudentNo, FullName, DateOfBirth, Gender, FatherName, MotherName,
        MobileNumber, ClassId, SectionId, RollNumber)
    WHERE NOT EXISTS (SELECT 1 FROM Students WHERE Id = S.Id);
    SET IDENTITY_INSERT Students OFF;

    PRINT 'Students added for classes 4-10 (Ids 13-30)';

    -- Fix StudentGroupId for students in group-based classes (9-10)
    -- Map SectionId -> StudentGroupId:
    --   Section 18 (Class 9 Science A) -> Group 1, Section 21 (Class 9 Business A) -> Group 2,
    --   Section 24 (Class 9 Humanities A) -> Group 3, Section 27 (Class 10 Science A) -> Group 1,
    --   Section 30 (Class 10 Business A) -> Group 2, Section 33 (Class 10 Humanities A) -> Group 3
    UPDATE Students
    SET StudentGroupId = CASE SectionId
        WHEN 18 THEN 1 WHEN 19 THEN 1
        WHEN 21 THEN 2 WHEN 22 THEN 2
        WHEN 24 THEN 3 WHEN 25 THEN 3
        WHEN 27 THEN 1 WHEN 28 THEN 1
        WHEN 30 THEN 2 WHEN 31 THEN 2
        WHEN 33 THEN 3 WHEN 34 THEN 3
        ELSE StudentGroupId
    END
    WHERE ClassId IN (9,10) AND StudentGroupId IS NULL;

    PRINT 'StudentGroupId updated for Class 9-10 students';

    -- ============================================================
    -- 3. CREATE EXAM RECORDS
    -- ============================================================
    PRINT '--- 3. Creating Exam records ---';

    -- First Terminal = 1, HalfYearly = 2 (ExamTerm enum)
    -- Published = 5 (ResultWorkflowStatus enum)
    -- AcademicYearId = 1 (2026)

    -- We need to find the next available Exam ID
    DECLARE @NextExamId INT;
    SELECT @NextExamId = ISNULL(MAX(Id), 0) + 1 FROM Exams;

    -- Track exam IDs for later sections
    -- We'll create a temp table to store the mapping
    CREATE TABLE #ExamMap (
        RowNum INT IDENTITY(1,1),
        ExamId INT,
        ExamName NVARCHAR(200),
        ClassId INT,
        StudentGroupId INT,
        Term INT,
        SectionId INT,
        SubjectCount INT
    );

    -- First Terminal 2026 - Classes 1-5 (no group)
    -- @NextExamId through @NextExamId+4
    SET IDENTITY_INSERT Exams ON;

    INSERT INTO Exams (Id, Name, Term, Status, AcademicYearId, ClassId, StudentGroupId, StartsOn, EndsOn, IsLocked, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT Id, Name, Term, 5 /*Published*/, @AcademicYearId, ClassId, StudentGroupId, StartsOn, EndsOn, 0, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES
        (@NextExamId,      N'First Terminal Examination 2026 - Class 1', 1, 1, NULL, '2026-04-01', '2026-04-06'),
        (@NextExamId + 1,  N'First Terminal Examination 2026 - Class 2', 1, 2, NULL, '2026-04-01', '2026-04-06'),
        (@NextExamId + 2,  N'First Terminal Examination 2026 - Class 3', 1, 3, NULL, '2026-04-01', '2026-04-06'),
        (@NextExamId + 3,  N'First Terminal Examination 2026 - Class 4', 1, 4, NULL, '2026-04-01', '2026-04-06'),
        (@NextExamId + 4,  N'First Terminal Examination 2026 - Class 5', 1, 5, NULL, '2026-04-01', '2026-04-06')
    ) E(Id, Name, Term, ClassId, StudentGroupId, StartsOn, EndsOn)
    WHERE NOT EXISTS (SELECT 1 FROM Exams WHERE Id = E.Id);

    SET IDENTITY_INSERT Exams OFF;

    -- Half Yearly 2026 - Classes 6-8 (no group)
    SET IDENTITY_INSERT Exams ON;
    INSERT INTO Exams (Id, Name, Term, Status, AcademicYearId, ClassId, StudentGroupId, StartsOn, EndsOn, IsLocked, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT Id, Name, Term, 5 /*Published*/, @AcademicYearId, ClassId, StudentGroupId, StartsOn, EndsOn, 0, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES
        (@NextExamId + 5,  N'Half Yearly Examination 2026 - Class 6',     2, 6, NULL, '2026-08-01', '2026-08-12'),
        (@NextExamId + 6,  N'Half Yearly Examination 2026 - Class 7',     2, 7, NULL, '2026-08-01', '2026-08-12'),
        (@NextExamId + 7,  N'Half Yearly Examination 2026 - Class 8',     2, 8, NULL, '2026-08-01', '2026-08-12')
    ) E(Id, Name, Term, ClassId, StudentGroupId, StartsOn, EndsOn)
    WHERE NOT EXISTS (SELECT 1 FROM Exams WHERE Id = E.Id);
    SET IDENTITY_INSERT Exams OFF;

    -- Half Yearly 2026 - Classes 9-10 with groups
    SET IDENTITY_INSERT Exams ON;
    INSERT INTO Exams (Id, Name, Term, Status, AcademicYearId, ClassId, StudentGroupId, StartsOn, EndsOn, IsLocked, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT Id, Name, Term, 5 /*Published*/, @AcademicYearId, ClassId, StudentGroupId, StartsOn, EndsOn, 0, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES
        (@NextExamId + 8,  N'Half Yearly Examination 2026 - Class 9 Science',          2, 9, 1, '2026-08-01', '2026-08-12'),
        (@NextExamId + 9,  N'Half Yearly Examination 2026 - Class 9 Business Studies', 2, 9, 2, '2026-08-01', '2026-08-12'),
        (@NextExamId + 10, N'Half Yearly Examination 2026 - Class 9 Humanities',       2, 9, 3, '2026-08-01', '2026-08-12'),
        (@NextExamId + 11, N'Half Yearly Examination 2026 - Class 10 Science',          2, 10, 1, '2026-08-01', '2026-08-12'),
        (@NextExamId + 12, N'Half Yearly Examination 2026 - Class 10 Business Studies', 2, 10, 2, '2026-08-01', '2026-08-12'),
        (@NextExamId + 13, N'Half Yearly Examination 2026 - Class 10 Humanities',       2, 10, 3, '2026-08-01', '2026-08-12')
    ) E(Id, Name, Term, ClassId, StudentGroupId, StartsOn, EndsOn)
    WHERE NOT EXISTS (SELECT 1 FROM Exams WHERE Id = E.Id);
    SET IDENTITY_INSERT Exams OFF;

    -- Populate #ExamMap
    INSERT INTO #ExamMap (ExamId, ExamName, ClassId, StudentGroupId, Term, SectionId, SubjectCount)
    SELECT Id, Name, ClassId, StudentGroupId, Term, NULL, 0
    FROM Exams
    WHERE Id >= @NextExamId
    ORDER BY Id;

    PRINT 'Created ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' exams';

    -- ============================================================
    -- 4. EXAM SUBJECTS
    -- ============================================================
    PRINT '--- 4. Creating ExamSubjects ---';

    DECLARE @NextESId INT;
    SELECT @NextESId = ISNULL(MAX(Id), 0) + 1 FROM ExamSubjects;

    -- We need to map each exam to its subjects based on class/group
    -- Primary (Class 1-5): Subjects 1,2,3,4,5,6 (Bangla,Eng,Math,GenSci,BGS,Religion)
    -- Class 6-8: Subjects 9,10,11,12,3,13,14,6,27,28 (BAN1,BAN2,ENG1,ENG2,Math,Sci,ICT,Rel,Car,Health)
    -- Class 9-10 Science (Group 1): Subjects 9,10,11,12,14,16,17,18,19 (BAN1,BAN2,ENG1,ENG2,ICT,Phy,Che,Bio,HMath)
    -- Class 9-10 Business (Group 2): Subjects 9,10,11,12,3,13,14,20,21,22 (BAN1,BAN2,ENG1,ENG2,Math,Sci,ICT,Acc,Fin,Bus)
    -- Class 9-10 Humanities (Group 3): Subjects 9,10,11,12,3,14,23,24,25,26 (BAN1,BAN2,ENG1,ENG2,Math,ICT,His,Geo,Eco,Civ)

    SET IDENTITY_INSERT ExamSubjects ON;

    -- Class 1-5 First Terminal: Subjects 1-6
    INSERT INTO ExamSubjects (Id, ExamId, SubjectId, ClassId, StudentGroupId, FullMarks, PassMarks, IsOptional, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT ROW_NUMBER() OVER (ORDER BY E.ExamId, S.SubjectId) + @NextESId - 1,
           E.ExamId, S.SubjectId, E.ClassId, E.StudentGroupId, 100, 33, 0, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM #ExamMap E
    CROSS APPLY (
        SELECT SubjectId FROM (VALUES (1),(2),(3),(4),(5),(6)) T(SubjectId)
    ) S
    WHERE E.ClassId BETWEEN 1 AND 5
      AND NOT EXISTS (
          SELECT 1 FROM ExamSubjects ES
          WHERE ES.ExamId = E.ExamId AND ES.SubjectId = S.SubjectId AND ES.IsDeleted = 0
      );

    -- Class 6-8 Half Yearly: Subjects 9,10,11,12,3,13,14,6,27,28
    INSERT INTO ExamSubjects (Id, ExamId, SubjectId, ClassId, StudentGroupId, FullMarks, PassMarks, IsOptional, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT ROW_NUMBER() OVER (ORDER BY E.ExamId, S.SubjectId) + @NextESId - 1 + 1000,  -- offset to avoid collisions
           E.ExamId, S.SubjectId, E.ClassId, E.StudentGroupId, 100, 33, 0, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM #ExamMap E
    CROSS APPLY (
        SELECT SubjectId FROM (VALUES (9),(10),(11),(12),(3),(13),(14),(6),(27),(28)) T(SubjectId)
    ) S
    WHERE E.ClassId BETWEEN 6 AND 8
      AND NOT EXISTS (
          SELECT 1 FROM ExamSubjects ES
          WHERE ES.ExamId = E.ExamId AND ES.SubjectId = S.SubjectId AND ES.IsDeleted = 0
      );

    -- Class 9-10 Science (Group 1): Subjects 9,10,11,12,14,16,17,18,19
    INSERT INTO ExamSubjects (Id, ExamId, SubjectId, ClassId, StudentGroupId, FullMarks, PassMarks, IsOptional, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT ROW_NUMBER() OVER (ORDER BY E.ExamId, S.SubjectId) + @NextESId - 1 + 2000,
           E.ExamId, S.SubjectId, E.ClassId, E.StudentGroupId, 100, 33, 0, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM #ExamMap E
    CROSS APPLY (
        SELECT SubjectId FROM (VALUES (9),(10),(11),(12),(14),(16),(17),(18),(19)) T(SubjectId)
    ) S
    WHERE E.StudentGroupId = 1
      AND NOT EXISTS (
          SELECT 1 FROM ExamSubjects ES
          WHERE ES.ExamId = E.ExamId AND ES.SubjectId = S.SubjectId AND ES.IsDeleted = 0
      );

    -- Class 9-10 Business (Group 2): Subjects 9,10,11,12,3,13,14,20,21,22
    INSERT INTO ExamSubjects (Id, ExamId, SubjectId, ClassId, StudentGroupId, FullMarks, PassMarks, IsOptional, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT ROW_NUMBER() OVER (ORDER BY E.ExamId, S.SubjectId) + @NextESId - 1 + 3000,
           E.ExamId, S.SubjectId, E.ClassId, E.StudentGroupId, 100, 33, 0, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM #ExamMap E
    CROSS APPLY (
        SELECT SubjectId FROM (VALUES (9),(10),(11),(12),(3),(13),(14),(20),(21),(22)) T(SubjectId)
    ) S
    WHERE E.StudentGroupId = 2
      AND NOT EXISTS (
          SELECT 1 FROM ExamSubjects ES
          WHERE ES.ExamId = E.ExamId AND ES.SubjectId = S.SubjectId AND ES.IsDeleted = 0
      );

    -- Class 9-10 Humanities (Group 3): Subjects 9,10,11,12,3,14,23,24,25,26
    INSERT INTO ExamSubjects (Id, ExamId, SubjectId, ClassId, StudentGroupId, FullMarks, PassMarks, IsOptional, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT ROW_NUMBER() OVER (ORDER BY E.ExamId, S.SubjectId) + @NextESId - 1 + 4000,
           E.ExamId, S.SubjectId, E.ClassId, E.StudentGroupId, 100, 33, 0, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM #ExamMap E
    CROSS APPLY (
        SELECT SubjectId FROM (VALUES (9),(10),(11),(12),(3),(14),(23),(24),(25),(26)) T(SubjectId)
    ) S
    WHERE E.StudentGroupId = 3
      AND NOT EXISTS (
          SELECT 1 FROM ExamSubjects ES
          WHERE ES.ExamId = E.ExamId AND ES.SubjectId = S.SubjectId AND ES.IsDeleted = 0
      );

    SET IDENTITY_INSERT ExamSubjects OFF;

    -- Update SubjectCount in #ExamMap
    UPDATE M
    SET M.SubjectCount = (SELECT COUNT(*) FROM ExamSubjects ES WHERE ES.ExamId = M.ExamId AND ES.IsDeleted = 0)
    FROM #ExamMap M;

    PRINT 'ExamSubjects created';

    -- ============================================================
    -- 5. EXAM SCHEDULES
    -- ============================================================
    PRINT '--- 5. Creating ExamSchedules ---';

    DECLARE @NextESchedId INT;
    SELECT @NextESchedId = ISNULL(MAX(Id), 0) + 1 FROM ExamSchedules;

    SET IDENTITY_INSERT ExamSchedules ON;

    -- Generate schedule: each subject gets a different date, starting from exam StartsOn
    -- We use a deterministic row_number approach
    INSERT INTO ExamSchedules (Id, ExamId, SubjectId, ClassId, StudentGroupId, SectionId, ExamDate, StartsAt, EndsAt, RoomNo, Instructions, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT
        ROW_NUMBER() OVER (ORDER BY ES.ExamId, S.RowNum) + @NextESchedId - 1,
        ES.ExamId, ES.SubjectId, ES.ClassId, ES.StudentGroupId, NULL,
        DATEADD(DAY, S.RowNum - 1, E.StartsOn),
        CAST('10:00:00' AS TIME), CAST('12:00:00' AS TIME),
        N'Room-' + CAST(ES.ClassId AS NVARCHAR) + '-' + RIGHT('00' + CAST(S.RowNum AS NVARCHAR), 2),
        N'Bring own stationery. No electronic devices allowed.',
        @Now, @SYSTEM, NULL, NULL, 0
    FROM ExamSubjects ES
    INNER JOIN Exams E ON E.Id = ES.ExamId
    INNER JOIN (
        SELECT SubjectId, ROW_NUMBER() OVER (ORDER BY SubjectId) AS RowNum
        FROM (SELECT DISTINCT SubjectId FROM ExamSubjects) _
    ) S ON S.SubjectId = ES.SubjectId
    WHERE ES.ExamId IN (SELECT ExamId FROM #ExamMap)
      AND NOT EXISTS (
          SELECT 1 FROM ExamSchedules ESched
          WHERE ESched.ExamId = ES.ExamId AND ESched.SubjectId = ES.SubjectId AND ESched.ClassId = ES.ClassId
            AND (ESched.StudentGroupId = ES.StudentGroupId OR (ESched.StudentGroupId IS NULL AND ES.StudentGroupId IS NULL))
            AND ESched.IsDeleted = 0
      );

    SET IDENTITY_INSERT ExamSchedules OFF;

    PRINT 'ExamSchedules created';

    -- ============================================================
    -- 6. SUBJECT MARK STRUCTURES (where missing)
    -- ============================================================
    PRINT '--- 6. Creating SubjectMarkStructures ---';

    -- Primary classes 1-5: Written(1)=70, MCQ(2)=30 for all subjects
    -- Class 6-8: Written(1)=70, MCQ(2)=30 for all subjects
    -- Class 9-10 Science (Group 1) - Science subjects (16,17,18): Written(1)=50, CQ(5)=25, Practical(3)=25
    -- Class 9-10 Science (Group 1) - Non-science: Written(1)=70, MCQ(2)=30
    -- Class 9-10 Business (Group 2): Written(1)=70, MCQ(2)=30
    -- Class 9-10 Humanities (Group 3): Written(1)=70, MCQ(2)=30

    DECLARE @NextSMSId INT;
    SELECT @NextSMSId = ISNULL(MAX(Id), 0) + 1 FROM SubjectMarkStructures;

    SET IDENTITY_INSERT SubjectMarkStructures ON;

    -- First check & create for Primary classes 4-5 (1-3 already done in EnterpriseSeed)
    -- For each (ClassId, SubjectId, StudentGroupId = NULL) in ClassSubjects for classes 4-5
    -- Create Written(1)=70, MCQ(2)=30
    INSERT INTO SubjectMarkStructures (Id, ComponentId, ClassId, SubjectId, StudentGroupId, FullMarks, PassMarks, DisplayOrder, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT
        ROW_NUMBER() OVER (ORDER BY CS.SubjectId, C.ComponentId) + @NextSMSId - 1,
        C.ComponentId, NULL AS ClassId, CS.SubjectId, NULL,
        CASE WHEN C.ComponentId = 1 THEN 70 ELSE 30 END,
        CASE WHEN C.ComponentId = 1 THEN 23 ELSE 10 END,
        C.ComponentId, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (
        SELECT DISTINCT SubjectId
        FROM ClassSubjects
        WHERE SchoolClassId IN (4,5)
          AND IsDeleted = 0
    ) CS
    CROSS JOIN (SELECT 1 AS ComponentId UNION ALL SELECT 2) C
    WHERE NOT EXISTS (
        SELECT 1 FROM SubjectMarkStructures SMS
        WHERE SMS.ComponentId = C.ComponentId
          AND SMS.SubjectId = CS.SubjectId
          AND SMS.StudentGroupId IS NULL
          AND SMS.IsDeleted = 0
    );

    -- Class 6-8: Written(1)=70, MCQ(2)=30 for all subjects
    INSERT INTO SubjectMarkStructures (Id, ComponentId, ClassId, SubjectId, StudentGroupId, FullMarks, PassMarks, DisplayOrder, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT
        ROW_NUMBER() OVER (ORDER BY CS.SubjectId, C.ComponentId) + @NextSMSId - 1 + 500,
        C.ComponentId, NULL AS ClassId, CS.SubjectId, NULL,
        CASE WHEN C.ComponentId = 1 THEN 70 ELSE 30 END,
        CASE WHEN C.ComponentId = 1 THEN 23 ELSE 10 END,
        C.ComponentId, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (
        SELECT DISTINCT SubjectId
        FROM ClassSubjects
        WHERE SchoolClassId IN (6,7,8)
          AND IsDeleted = 0
    ) CS
    CROSS JOIN (SELECT 1 AS ComponentId UNION ALL SELECT 2) C
    WHERE NOT EXISTS (
        SELECT 1 FROM SubjectMarkStructures SMS
        WHERE SMS.ComponentId = C.ComponentId
          AND SMS.SubjectId = CS.SubjectId
          AND SMS.StudentGroupId IS NULL
          AND SMS.IsDeleted = 0
    );

    -- Class 9-10 Science (Group 1): Science subjects (16-19) get Written=50, CQ=25, Practical=25
    -- Non-science subjects (9-14) get Written=70, MCQ=30
    -- Science subjects
    INSERT INTO SubjectMarkStructures (Id, ComponentId, ClassId, SubjectId, StudentGroupId, FullMarks, PassMarks, DisplayOrder, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT
        ROW_NUMBER() OVER (ORDER BY CS.SubjectId, C.ComponentId) + @NextSMSId - 1 + 1000,
        C.ComponentId, NULL AS ClassId, CS.SubjectId, 1,
        C.FullMarks, C.PassMarks,
        C.ComponentId, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (
        SELECT DISTINCT SubjectId
        FROM ClassSubjects
        WHERE SchoolClassId IN (9,10) AND StudentGroupId = 1 AND SubjectId IN (16,17,18,19)
          AND IsDeleted = 0
    ) CS
    CROSS APPLY (
        SELECT * FROM (VALUES
            (1, 50, 17),
            (5, 25, 8),  -- CQ component
            (3, 25, 8)   -- Practical
        ) T(ComponentId, FullMarks, PassMarks)
    ) C
    WHERE NOT EXISTS (
        SELECT 1 FROM SubjectMarkStructures SMS
        WHERE SMS.ComponentId = C.ComponentId
          AND SMS.SubjectId = CS.SubjectId
          AND SMS.StudentGroupId = 1
          AND SMS.IsDeleted = 0
    );

    -- Non-science subjects for class 9-10 Science
    INSERT INTO SubjectMarkStructures (Id, ComponentId, ClassId, SubjectId, StudentGroupId, FullMarks, PassMarks, DisplayOrder, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT
        ROW_NUMBER() OVER (ORDER BY CS.SubjectId, C.ComponentId) + @NextSMSId - 1 + 1500,
        C.ComponentId, NULL AS ClassId, CS.SubjectId, 1,
        CASE WHEN C.ComponentId = 1 THEN 70 ELSE 30 END,
        CASE WHEN C.ComponentId = 1 THEN 23 ELSE 10 END,
        C.ComponentId, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (
        SELECT DISTINCT SubjectId
        FROM ClassSubjects
        WHERE SchoolClassId IN (9,10) AND StudentGroupId = 1 AND SubjectId NOT IN (16,17,18,19)
          AND IsDeleted = 0
    ) CS
    CROSS JOIN (SELECT 1 AS ComponentId UNION ALL SELECT 2) C
    WHERE NOT EXISTS (
        SELECT 1 FROM SubjectMarkStructures SMS
        WHERE SMS.ComponentId = C.ComponentId
          AND SMS.SubjectId = CS.SubjectId
          AND SMS.StudentGroupId = 1
          AND SMS.IsDeleted = 0
    );

    -- Class 9-10 Business (Group 2): Written=70, MCQ=30 for all subjects
    INSERT INTO SubjectMarkStructures (Id, ComponentId, ClassId, SubjectId, StudentGroupId, FullMarks, PassMarks, DisplayOrder, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT
        ROW_NUMBER() OVER (ORDER BY CS.SubjectId, C.ComponentId) + @NextSMSId - 1 + 2000,
        C.ComponentId, NULL AS ClassId, CS.SubjectId, 2,
        CASE WHEN C.ComponentId = 1 THEN 70 ELSE 30 END,
        CASE WHEN C.ComponentId = 1 THEN 23 ELSE 10 END,
        C.ComponentId, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (
        SELECT DISTINCT SubjectId
        FROM ClassSubjects
        WHERE SchoolClassId IN (9,10) AND StudentGroupId = 2
          AND IsDeleted = 0
    ) CS
    CROSS JOIN (SELECT 1 AS ComponentId UNION ALL SELECT 2) C
    WHERE NOT EXISTS (
        SELECT 1 FROM SubjectMarkStructures SMS
        WHERE SMS.ComponentId = C.ComponentId
          AND SMS.SubjectId = CS.SubjectId
          AND SMS.StudentGroupId = 2
          AND SMS.IsDeleted = 0
    );

    -- Class 9-10 Humanities (Group 3): Written=70, MCQ=30 for all subjects
    INSERT INTO SubjectMarkStructures (Id, ComponentId, ClassId, SubjectId, StudentGroupId, FullMarks, PassMarks, DisplayOrder, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT
        ROW_NUMBER() OVER (ORDER BY CS.SubjectId, C.ComponentId) + @NextSMSId - 1 + 4000,
        C.ComponentId, NULL AS ClassId, CS.SubjectId, 3,
        CASE WHEN C.ComponentId = 1 THEN 70 ELSE 30 END,
        CASE WHEN C.ComponentId = 1 THEN 23 ELSE 10 END,
        C.ComponentId, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM (
        SELECT DISTINCT SubjectId
        FROM ClassSubjects
        WHERE SchoolClassId IN (9,10) AND StudentGroupId = 3
          AND IsDeleted = 0
    ) CS
    CROSS JOIN (SELECT 1 AS ComponentId UNION ALL SELECT 2) C
    WHERE NOT EXISTS (
        SELECT 1 FROM SubjectMarkStructures SMS
        WHERE SMS.ComponentId = C.ComponentId
          AND SMS.SubjectId = CS.SubjectId
          AND SMS.StudentGroupId = 3
          AND SMS.IsDeleted = 0
    );

    SET IDENTITY_INSERT SubjectMarkStructures OFF;

    PRINT 'SubjectMarkStructures created';

    -- ============================================================
    -- 7. MARKS ENTRY - Generate marks for ALL students in ALL exams
    -- ============================================================
    PRINT '--- 7. Creating MarkEntries ---';

    DECLARE @NextMarkId INT;
    SELECT @NextMarkId = ISNULL(MAX(Id), 0) + 1 FROM Marks;

    -- Helper: compute grade from marks
    -- A+: >= 80, A: >= 70, A-: >= 60, B: >= 50, C: >= 40, D: >= 33, F: < 33

    -- We generate marks using RAND() with CHECKSUM for deterministic-ish but varied results.
    -- Each student gets a unique "seed" per subject so marks are not uniform.

    -- Only create marks for exam-subjects that don't already have marks for that student
    -- We need to get the SectionId for each student

    SET IDENTITY_INSERT Marks ON;

    INSERT INTO Marks (
        Id, ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
        WrittenMarks, MCQMarks, CQMarks, PracticalMarks,
        MarksObtained, Grade, GradePoint,
        EnteredByTeacherId, Status, IsLocked, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY ES.ExamId, S.Id, ES.SubjectId) + @NextMarkId - 1,
        ES.ExamId, S.Id, ES.SubjectId, @AcademicYearId,
        ES.ClassId, S.SectionId, ES.StudentGroupId,
        -- WrittenMarks: 15-67 (if subject has Written 70) or 10-45 (if Written 50)
        CASE
            WHEN SG.GroupId IS NOT NULL AND ES.SubjectId IN (16,17,18,19)
                THEN ROUND(10 + (ABS(CHECKSUM(NEWID())) % 36), 0)  -- Written 50
            ELSE ROUND(15 + (ABS(CHECKSUM(NEWID())) % 53), 0)      -- Written 70
        END,
        -- MCQMarks: 5-28 (MCQ 30) or NULL if CQ-based
        CASE
            WHEN SG.GroupId IS NOT NULL AND ES.SubjectId IN (16,17,18,19) THEN NULL
            ELSE ROUND(5 + (ABS(CHECKSUM(NEWID())) % 24), 0)
        END,
        -- CQMarks: for science subjects
        CASE
            WHEN SG.GroupId IS NOT NULL AND ES.SubjectId IN (16,17,18,19)
                THEN ROUND(5 + (ABS(CHECKSUM(NEWID())) % 21), 0)
            ELSE NULL
        END,
        -- PracticalMarks: for science subjects
        CASE
            WHEN SG.GroupId IS NOT NULL AND ES.SubjectId IN (16,17,18,19)
                THEN ROUND(5 + (ABS(CHECKSUM(NEWID())) % 21), 0)
            ELSE NULL
        END,
        -- MarksObtained: calculated from components
        CASE
            WHEN SG.GroupId IS NOT NULL AND ES.SubjectId IN (16,17,18,19)
                THEN ROUND(10 + (ABS(CHECKSUM(NEWID())) % 36), 0) -- Written
                   + ROUND(5 + (ABS(CHECKSUM(NEWID())) % 21), 0)   -- CQ
                   + ROUND(5 + (ABS(CHECKSUM(NEWID())) % 21), 0)   -- Practical
            ELSE
                ROUND(15 + (ABS(CHECKSUM(NEWID())) % 53), 0)       -- Written
                + ROUND(5 + (ABS(CHECKSUM(NEWID())) % 24), 0)      -- MCQ
        END,
        -- Grade: placeholder (will be recomputed)
        NULL, NULL,
        1 /*Teacher 1*/, 5 /*Published*/, 0, @Now, @SYSTEM, NULL, NULL, 0
    FROM ExamSubjects ES
    INNER JOIN Students S ON S.ClassId = ES.ClassId AND (S.StudentGroupId = ES.StudentGroupId OR (S.StudentGroupId IS NULL AND ES.StudentGroupId IS NULL))
    OUTER APPLY (SELECT ES.StudentGroupId AS GroupId WHERE ES.StudentGroupId IS NOT NULL) SG
    WHERE ES.ExamId IN (SELECT ExamId FROM #ExamMap)
      AND ES.IsDeleted = 0
      AND S.IsDeleted = 0
      AND S.Status = 1  -- Active students only
      AND NOT EXISTS (
          SELECT 1 FROM Marks M
          WHERE M.ExamId = ES.ExamId AND M.StudentId = S.Id AND M.SubjectId = ES.SubjectId
      );

    SET IDENTITY_INSERT Marks OFF;

    -- Update Grades based on MarksObtained
    UPDATE Marks
    SET
        Grade = CASE
            WHEN MarksObtained >= 80 THEN N'A+'
            WHEN MarksObtained >= 70 THEN N'A'
            WHEN MarksObtained >= 60 THEN N'A-'
            WHEN MarksObtained >= 50 THEN N'B'
            WHEN MarksObtained >= 40 THEN N'C'
            WHEN MarksObtained >= 33 THEN N'D'
            ELSE N'F'
        END,
        GradePoint = CASE
            WHEN MarksObtained >= 80 THEN 5.00
            WHEN MarksObtained >= 70 THEN 4.00
            WHEN MarksObtained >= 60 THEN 3.50
            WHEN MarksObtained >= 50 THEN 3.00
            WHEN MarksObtained >= 40 THEN 2.00
            WHEN MarksObtained >= 33 THEN 1.00
            ELSE 0.00
        END
    WHERE ExamId IN (SELECT ExamId FROM #ExamMap)
      AND Grade IS NULL;

    PRINT 'Marks created';

    -- ============================================================
    -- 8. STUDENT SUBJECT RESULTS
    -- ============================================================
    PRINT '--- 8. Creating StudentSubjectResults ---';

    DECLARE @NextSSRId INT;
    SELECT @NextSSRId = ISNULL(MAX(Id), 0) + 1 FROM StudentSubjectResults;

    SET IDENTITY_INSERT StudentSubjectResults ON;

    INSERT INTO StudentSubjectResults (
        Id, ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
        IsOptionalSubject, IsReligionSubject,
        MarksObtained, FullMarks, PassMarks,
        Grade, GradePoint, IsPassed, CalculatedAt, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY ES.ExamId, S.Id, ES.SubjectId) + @NextSSRId - 1,
        ES.ExamId, S.Id, ES.SubjectId, @AcademicYearId,
        ES.ClassId, S.SectionId, ES.StudentGroupId,
        0, 0,
        M.MarksObtained, ES.FullMarks, ES.PassMarks,
        M.Grade, M.GradePoint,
        CASE WHEN M.MarksObtained >= 33 THEN 1 ELSE 0 END,
        @Now, @Now, @SYSTEM, NULL, NULL, 0
    FROM ExamSubjects ES
    INNER JOIN Students S ON S.ClassId = ES.ClassId AND (S.StudentGroupId = ES.StudentGroupId OR (S.StudentGroupId IS NULL AND ES.StudentGroupId IS NULL))
    INNER JOIN Marks M ON M.ExamId = ES.ExamId AND M.StudentId = S.Id AND M.SubjectId = ES.SubjectId
    WHERE ES.ExamId IN (SELECT ExamId FROM #ExamMap)
      AND ES.IsDeleted = 0
      AND S.IsDeleted = 0
      AND NOT EXISTS (
          SELECT 1 FROM StudentSubjectResults SSR
          WHERE SSR.ExamId = ES.ExamId AND SSR.StudentId = S.Id AND SSR.SubjectId = ES.SubjectId
      );

    SET IDENTITY_INSERT StudentSubjectResults OFF;

    PRINT 'StudentSubjectResults created';

    -- ============================================================
    -- 9. STUDENT EXAM RESULTS (aggregate)
    -- ============================================================
    PRINT '--- 9. Creating StudentExamResults ---';

    DECLARE @NextSERId INT;
    SELECT @NextSERId = ISNULL(MAX(Id), 0) + 1 FROM StudentExamResults;

    -- Compute aggregate per student per exam
    SET IDENTITY_INSERT StudentExamResults ON;

    INSERT INTO StudentExamResults (
        Id, ExamId, StudentId, AcademicYearId, ClassId, SectionId, StudentGroupId,
        TotalMarks, TotalFullMarks,
        Gpa, Grade, Position, ClassPosition, GroupPosition,
        IsPassed, FailedSubjectCount, PassedSubjectCount,
        Status, CalculatedAt, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY A.ExamId, A.StudentId) + @NextSERId - 1,
        A.ExamId, A.StudentId, @AcademicYearId, A.ClassId, A.SectionId, A.StudentGroupId,
        A.TotalMarks, A.TotalFullMarks,
        A.Gpa,
        CASE
            WHEN A.Gpa >= 5.00 THEN N'A+'
            WHEN A.Gpa >= 4.00 THEN N'A'
            WHEN A.Gpa >= 3.50 THEN N'A-'
            WHEN A.Gpa >= 3.00 THEN N'B'
            WHEN A.Gpa >= 2.00 THEN N'C'
            WHEN A.Gpa >= 1.00 THEN N'D'
            ELSE N'F'
        END,
        0, 0, NULL,
        CASE WHEN A.FailedCount = 0 THEN 1 ELSE 0 END,
        A.FailedCount, A.PassedCount,
        5 /*Published*/, @Now, @Now, @SYSTEM, NULL, NULL, 0
    FROM (
        SELECT
            SSR.ExamId, SSR.StudentId, SSR.ClassId, SSR.SectionId, SSR.StudentGroupId,
            SUM(SSR.MarksObtained) AS TotalMarks,
            SUM(SSR.FullMarks) AS TotalFullMarks,
            ROUND(AVG(SSR.GradePoint), 2) AS Gpa,
            SUM(CASE WHEN SSR.IsPassed = 0 THEN 1 ELSE 0 END) AS FailedCount,
            SUM(CASE WHEN SSR.IsPassed = 1 THEN 1 ELSE 0 END) AS PassedCount
        FROM StudentSubjectResults SSR
        WHERE SSR.ExamId IN (SELECT ExamId FROM #ExamMap)
        GROUP BY SSR.ExamId, SSR.StudentId, SSR.ClassId, SSR.SectionId, SSR.StudentGroupId
    ) A
    WHERE NOT EXISTS (
        SELECT 1 FROM StudentExamResults SER
        WHERE SER.ExamId = A.ExamId AND SER.StudentId = A.StudentId
    );

    SET IDENTITY_INSERT StudentExamResults OFF;

    PRINT 'StudentExamResults created';

    -- ============================================================
    -- 10. MERIT POSITION CALCULATION
    -- ============================================================
    PRINT '--- 10. Calculating merit positions ---';

    -- Calculate positions per exam using DENSE_RANK
    -- Order: Gpa DESC, TotalMarks DESC

    WITH RankedResults AS (
        SELECT
            Id,
            DENSE_RANK() OVER (
                PARTITION BY ExamId, ClassId
                ORDER BY Gpa DESC, TotalMarks DESC
            ) AS NewPosition,
            DENSE_RANK() OVER (
                PARTITION BY ExamId, ClassId
                ORDER BY Gpa DESC, TotalMarks DESC
            ) AS NewClassPosition,
            CASE
                WHEN StudentGroupId IS NOT NULL
                THEN DENSE_RANK() OVER (
                    PARTITION BY ExamId, ClassId, StudentGroupId
                    ORDER BY Gpa DESC, TotalMarks DESC
                )
                ELSE NULL
            END AS NewGroupPosition
        FROM StudentExamResults
        WHERE ExamId IN (SELECT ExamId FROM #ExamMap)
          AND Position = 0
    )
    UPDATE SER
    SET SER.Position = R.NewPosition,
        SER.ClassPosition = R.NewClassPosition,
        SER.GroupPosition = R.NewGroupPosition
    FROM StudentExamResults SER
    INNER JOIN RankedResults R ON R.Id = SER.Id;

    PRINT 'Merit positions calculated';

    -- ============================================================
    -- 11. FINAL RESULTS
    -- ============================================================
    PRINT '--- 11. Creating FinalResults ---';

    DECLARE @NextFRId INT;
    SELECT @NextFRId = ISNULL(MAX(Id), 0) + 1 FROM FinalResults;

    -- We use the Half Yearly exam results for FinalResults (most recent per class)
    -- For classes 1-5 (First Terminal) and 6-10 (Half Yearly)
    -- Since each class has only one exam in our set, we use them all

    SET IDENTITY_INSERT FinalResults ON;

    INSERT INTO FinalResults (
        Id, AcademicYearId, StudentId, SchoolClassId, SectionId, StudentGroupId,
        FinalGpa, FinalPosition, FinalClassPosition, FinalGrade,
        PromotionStatus, IsPassed, TotalFailedSubjects,
        CalculatedAt, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY SER.StudentId) + @NextFRId - 1,
        @AcademicYearId, SER.StudentId, SER.ClassId, SER.SectionId, SER.StudentGroupId,
        SER.Gpa, SER.Position, SER.ClassPosition,
        SER.Grade,
        CASE WHEN SER.IsPassed = 1 THEN 1 ELSE 3 END,  -- 1=Pending (to be promoted), 3=Repeat
        SER.IsPassed, SER.FailedSubjectCount,
        @Now, @Now, @SYSTEM, NULL, NULL, 0
    FROM StudentExamResults SER
    WHERE SER.ExamId IN (SELECT ExamId FROM #ExamMap)
      AND NOT EXISTS (
          SELECT 1 FROM FinalResults FR
          WHERE FR.AcademicYearId = @AcademicYearId AND FR.StudentId = SER.StudentId
      );

    -- Update PromotionStatus for passing students
    UPDATE FinalResults
    SET PromotionStatus = 2 /*Promoted*/
    WHERE AcademicYearId = @AcademicYearId
      AND IsPassed = 1
      AND PromotionStatus = 1;

    SET IDENTITY_INSERT FinalResults OFF;

    PRINT 'FinalResults created';

    -- ============================================================
    -- 12. RESULT PUBLICATIONS
    -- ============================================================
    PRINT '--- 12. Creating ResultPublications ---';

    DECLARE @NextRPId INT;
    SELECT @NextRPId = ISNULL(MAX(Id), 0) + 1 FROM ResultPublications;

    SET IDENTITY_INSERT ResultPublications ON;

    INSERT INTO ResultPublications (
        Id, ExamId, AcademicYearId, Status, PublishedAt, IsLocked, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY E.Id) + @NextRPId - 1,
        E.Id, @AcademicYearId, 5 /*Published*/, @Now, 0, @Now, @SYSTEM, NULL, NULL, 0
    FROM Exams E
    WHERE E.Id IN (SELECT ExamId FROM #ExamMap)
      AND NOT EXISTS (
          SELECT 1 FROM ResultPublications RP WHERE RP.ExamId = E.Id
      );

    SET IDENTITY_INSERT ResultPublications OFF;

    -- Update PublishedAt on StudentExamResults
    UPDATE SER
    SET SER.PublishedAt = @Now
    FROM StudentExamResults SER
    WHERE SER.ExamId IN (SELECT ExamId FROM #ExamMap)
      AND SER.PublishedAt IS NULL;

    PRINT 'ResultPublications created';

    -- ============================================================
    -- 13. ADMIT CARDS
    -- ============================================================
    PRINT '--- 13. Creating AdmitCards ---';

    DECLARE @NextACId INT;
    SELECT @NextACId = ISNULL(MAX(Id), 0) + 1 FROM AdmitCards;

    SET IDENTITY_INSERT AdmitCards ON;

    INSERT INTO AdmitCards (
        Id, ExamId, StudentId,
        CardNo, RollNumber, IsIssued, IsGenerated,
        CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY E.Id, S.Id) + @NextACId - 1,
        E.Id, S.Id,
        N'ADC-' + CAST(E.Id AS NVARCHAR) + '-' + RIGHT('0000' + CAST(S.RollNumber AS NVARCHAR), 4),
        S.RollNumber, 1, 1,
        @Now, @SYSTEM, NULL, NULL, 0
    FROM Exams E
    INNER JOIN Students S ON S.ClassId = E.ClassId AND (S.StudentGroupId = E.StudentGroupId OR (S.StudentGroupId IS NULL AND E.StudentGroupId IS NULL))
    WHERE E.Id IN (SELECT ExamId FROM #ExamMap)
      AND S.IsDeleted = 0
      AND S.Status = 1
      AND NOT EXISTS (
          SELECT 1 FROM AdmitCards AC
          WHERE AC.ExamId = E.Id AND AC.StudentId = S.Id
      );

    SET IDENTITY_INSERT AdmitCards OFF;

    PRINT 'AdmitCards created';

    -- ============================================================
    -- 14. COMPREHENSIVE VALIDATION
    -- ============================================================
    PRINT '';
    PRINT '============================================================';
    PRINT 'VALIDATION REPORT';
    PRINT '============================================================';

    -- 1. Exams created count
    DECLARE @ExamCount INT;
    SELECT @ExamCount = COUNT(*) FROM Exams WHERE Id IN (SELECT ExamId FROM #ExamMap);
    PRINT '1. Exams created: ' + CAST(@ExamCount AS NVARCHAR);

    -- 2. ExamSubjects count
    DECLARE @ESCount INT;
    SELECT @ESCount = COUNT(*) FROM ExamSubjects WHERE ExamId IN (SELECT ExamId FROM #ExamMap) AND IsDeleted = 0;
    PRINT '2. ExamSubjects: ' + CAST(@ESCount AS NVARCHAR);

    -- 3. ExamSchedules count
    DECLARE @ESchedCount INT;
    SELECT @ESchedCount = COUNT(*) FROM ExamSchedules WHERE ExamId IN (SELECT ExamId FROM #ExamMap) AND IsDeleted = 0;
    PRINT '3. ExamSchedules: ' + CAST(@ESchedCount AS NVARCHAR);

    -- 4. Marks count
    DECLARE @MarkCount INT;
    SELECT @MarkCount = COUNT(*) FROM Marks WHERE ExamId IN (SELECT ExamId FROM #ExamMap);
    PRINT '4. Marks: ' + CAST(@MarkCount AS NVARCHAR);

    -- 5. StudentSubjectResults count
    DECLARE @SSRCount INT;
    SELECT @SSRCount = COUNT(*) FROM StudentSubjectResults WHERE ExamId IN (SELECT ExamId FROM #ExamMap);
    PRINT '5. StudentSubjectResults: ' + CAST(@SSRCount AS NVARCHAR);

    -- 6. StudentExamResults count
    DECLARE @SERCount INT;
    SELECT @SERCount = COUNT(*) FROM StudentExamResults WHERE ExamId IN (SELECT ExamId FROM #ExamMap);
    PRINT '6. StudentExamResults: ' + CAST(@SERCount AS NVARCHAR);

    -- 7. FinalResults count
    DECLARE @FRCount INT;
    SELECT @FRCount = COUNT(*) FROM FinalResults WHERE AcademicYearId = @AcademicYearId;
    PRINT '7. FinalResults: ' + CAST(@FRCount AS NVARCHAR);

    -- 8. Report cards (StudentExamResults = generated report cards)
    PRINT '8. Report cards generated: ' + CAST(@SERCount AS NVARCHAR) + ' (StudentExamResults)';

    -- 9. AdmitCards count
    DECLARE @ACCount INT;
    SELECT @ACCount = COUNT(*) FROM AdmitCards WHERE ExamId IN (SELECT ExamId FROM #ExamMap);
    PRINT '9. AdmitCards: ' + CAST(@ACCount AS NVARCHAR);

    -- 10. ResultPublications count
    DECLARE @RPCount INT;
    SELECT @RPCount = COUNT(*) FROM ResultPublications WHERE ExamId IN (SELECT ExamId FROM #ExamMap);
    PRINT '10. ResultPublications: ' + CAST(@RPCount AS NVARCHAR);

    -- ============================================================
    -- VALIDATION CHECKS
    -- ============================================================
    PRINT '';
    PRINT '--- Validation Checks ---';

    -- Every exam has subjects
    DECLARE @ExamWithoutSubjects INT;
    SELECT @ExamWithoutSubjects = COUNT(*)
    FROM #ExamMap M
    WHERE NOT EXISTS (SELECT 1 FROM ExamSubjects ES WHERE ES.ExamId = M.ExamId AND ES.IsDeleted = 0);
    PRINT '✓ Exams without subjects: ' + CAST(@ExamWithoutSubjects AS NVARCHAR) + ' (should be 0)';

    -- Every subject has marks (for each student)
    DECLARE @SubjectWithoutMarks INT;
    SELECT @SubjectWithoutMarks = COUNT(*)
    FROM (
        SELECT DISTINCT ES.ExamId, ES.SubjectId, S.Id AS StudentId
        FROM ExamSubjects ES
        INNER JOIN Students S ON S.ClassId = ES.ClassId AND (S.StudentGroupId = ES.StudentGroupId OR (S.StudentGroupId IS NULL AND ES.StudentGroupId IS NULL))
        WHERE ES.ExamId IN (SELECT ExamId FROM #ExamMap)
          AND S.IsDeleted = 0 AND S.Status = 1
    ) Need
    WHERE NOT EXISTS (
        SELECT 1 FROM Marks M
        WHERE M.ExamId = Need.ExamId AND M.StudentId = Need.StudentId AND M.SubjectId = Need.SubjectId
    );
    PRINT '✓ Subject-student combos without marks: ' + CAST(@SubjectWithoutMarks AS NVARCHAR) + ' (should be 0)';

    -- Every student has results
    DECLARE @StudentsWithoutResults INT;
    SELECT @StudentsWithoutResults = COUNT(*)
    FROM (
        SELECT DISTINCT ExamId, StudentId
        FROM Marks
        WHERE ExamId IN (SELECT ExamId FROM #ExamMap)
    ) M
    WHERE NOT EXISTS (
        SELECT 1 FROM StudentExamResults SER
        WHERE SER.ExamId = M.ExamId AND SER.StudentId = M.StudentId
    );
    PRINT '✓ Students without ExamResults: ' + CAST(@StudentsWithoutResults AS NVARCHAR) + ' (should be 0)';

    -- Every exam published
    DECLARE @UnpublishedExams INT;
    SELECT @UnpublishedExams = COUNT(*)
    FROM Exams
    WHERE Id IN (SELECT ExamId FROM #ExamMap) AND Status <> 5;
    PRINT '✓ Unpublished exams: ' + CAST(@UnpublishedExams AS NVARCHAR) + ' (should be 0)';

    -- Every exam has publication
    DECLARE @ExamsWithoutPublication INT;
    SELECT @ExamsWithoutPublication = COUNT(*)
    FROM #ExamMap M
    WHERE NOT EXISTS (SELECT 1 FROM ResultPublications RP WHERE RP.ExamId = M.ExamId);
    PRINT '✓ Exams without ResultPublication: ' + CAST(@ExamsWithoutPublication AS NVARCHAR) + ' (should be 0)';

    -- Every student has admit card
    DECLARE @StudentsWithoutAdmitCard INT;
    SELECT @StudentsWithoutAdmitCard = COUNT(*)
    FROM (
        SELECT DISTINCT M.ExamId, M.StudentId
        FROM Marks M
        WHERE M.ExamId IN (SELECT ExamId FROM #ExamMap)
    ) M
    WHERE NOT EXISTS (
        SELECT 1 FROM AdmitCards AC
        WHERE AC.ExamId = M.ExamId AND AC.StudentId = M.StudentId
    );
    PRINT '✓ Students without AdmitCards: ' + CAST(@StudentsWithoutAdmitCard AS NVARCHAR) + ' (should be 0)';

    -- No orphan records (FK integrity)
    -- Orphan marks (exam doesn't exist)
    DECLARE @OrphanMarks INT;
    SELECT @OrphanMarks = COUNT(*) FROM Marks M
    WHERE NOT EXISTS (SELECT 1 FROM Exams E WHERE E.Id = M.ExamId);
    PRINT '✓ Orphan marks (no parent exam): ' + CAST(@OrphanMarks AS NVARCHAR) + ' (should be 0)';

    -- Orphan admit cards
    DECLARE @OrphanAdmitCards INT;
    SELECT @OrphanAdmitCards = COUNT(*) FROM AdmitCards AC
    WHERE NOT EXISTS (SELECT 1 FROM Exams E WHERE E.Id = AC.ExamId);
    PRINT '✓ Orphan admit cards: ' + CAST(@OrphanAdmitCards AS NVARCHAR) + ' (should be 0)';

    -- No duplicate exams (same name and class)
    DECLARE @DuplicateExams INT;
    SELECT @DuplicateExams = COUNT(*) - COUNT(DISTINCT CONCAT(Name, '-', ClassId, '-', ISNULL(CAST(StudentGroupId AS NVARCHAR), '')))
    FROM Exams
    WHERE Id IN (SELECT ExamId FROM #ExamMap);
    PRINT '✓ Duplicate exams (should be 0): ' + CAST(@DuplicateExams AS NVARCHAR);

    -- All ClassId populated in ExamSubjects
    DECLARE @NullClassId INT;
    SELECT @NullClassId = COUNT(*) FROM ExamSubjects WHERE ExamId IN (SELECT ExamId FROM #ExamMap) AND ClassId IS NULL;
    PRINT '✓ ExamSubjects with NULL ClassId: ' + CAST(@NullClassId AS NVARCHAR) + ' (should be 0)';

    -- Check dense ranking (no duplicate position bugs - positions should be 1,1,3 not 1,2,2)
    DECLARE @RankGapIssue INT;
    SELECT @RankGapIssue = COUNT(*)
    FROM (
        SELECT ExamId, ClassId, Position,
               LAG(Position) OVER (PARTITION BY ExamId, ClassId ORDER BY Position) AS PrevPos
        FROM StudentExamResults
        WHERE ExamId IN (SELECT ExamId FROM #ExamMap) AND Position > 0
    ) R
    WHERE R.PrevPos IS NOT NULL AND R.Position - R.PrevPos > 1 AND R.PrevPos > 1;  -- Allow gaps after ties
    PRINT '✓ Position gap anomalies: ' + CAST(@RankGapIssue AS NVARCHAR) + ' (check only if > 0)';

    -- Per-exam detail
    PRINT '';
    PRINT '--- Per-Exam Breakdown ---';
    SELECT
        E.Id AS ExamId, E.Name,
        COUNT(DISTINCT ES.SubjectId) AS Subjects,
        COUNT(DISTINCT M.StudentId) AS StudentsWithMarks,
        COUNT(DISTINCT SER.Id) AS StudentResults,
        COUNT(DISTINCT AC.Id) AS AdmitCards
    FROM Exams E
    LEFT JOIN ExamSubjects ES ON ES.ExamId = E.Id AND ES.IsDeleted = 0
    LEFT JOIN Marks M ON M.ExamId = E.Id
    LEFT JOIN StudentExamResults SER ON SER.ExamId = E.Id
    LEFT JOIN AdmitCards AC ON AC.ExamId = E.Id
    WHERE E.Id IN (SELECT ExamId FROM #ExamMap)
    GROUP BY E.Id, E.Name
    ORDER BY E.Id;

    -- ============================================================
    -- CLEANUP
    -- ============================================================
    DROP TABLE #ExamMap;

    PRINT '';
    PRINT '============================================================';
    PRINT 'EXAM DATA GENERATION 2026 - COMPLETE';
    PRINT '============================================================';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT 'ERROR: ' + ERROR_MESSAGE();
    PRINT 'Line: ' + CAST(ERROR_LINE() AS NVARCHAR);
    THROW;
END CATCH;
GO
