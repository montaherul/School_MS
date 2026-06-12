SET QUOTED_IDENTIFIER ON;
USE SchoolManagementSystemDb;
GO

-- Clean up existing exam data to avoid duplicates
DELETE FROM AdmitCards;
DELETE FROM ExamSchedules;
DELETE FROM ExamSubjects;
DELETE FROM Exams;
DELETE FROM ExamTypes;
DBCC CHECKIDENT ('Exams', RESEED, 0);
DBCC CHECKIDENT ('ExamTypes', RESEED, 0);
DBCC CHECKIDENT ('ExamSubjects', RESEED, 0);
DBCC CHECKIDENT ('ExamSchedules', RESEED, 0);
DBCC CHECKIDENT ('AdmitCards', RESEED, 0);
GO

-- 1. ExamTypes
-- Columns: Id(IDENTITY), Name, Code, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted
SET IDENTITY_INSERT ExamTypes ON;
INSERT INTO ExamTypes (Id, Name, Code, Description, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted)
VALUES
(1, 'First Terminal', 'FIRST_TERMINAL', 'First terminal examination', 1, 1, 'system', GETDATE(), 0),
(2, 'Half Yearly', 'HALF_YEARLY', 'Mid-term examination', 2, 1, 'system', GETDATE(), 0),
(3, 'Second Terminal', 'SECOND_TERMINAL', 'Second terminal examination', 3, 1, 'system', GETDATE(), 0),
(4, 'Annual', 'ANNUAL', 'Annual examination', 4, 1, 'system', GETDATE(), 0),
(5, 'Final', 'FINAL', 'Final examination', 5, 1, 'system', GETDATE(), 0),
(6, 'Pre-Test', 'PRE_TEST', 'Pre-test examination', 6, 1, 'system', GETDATE(), 0),
(7, 'Test', 'TEST', 'Regular test examination', 7, 1, 'system', GETDATE(), 0);
SET IDENTITY_INSERT ExamTypes OFF;
GO

-- 2. Exams
-- Columns: Id(IDENTITY), Name, Term(int), Status(int), AcademicYearId, StartsOn, EndsOn, IsLocked, LockedAt, LockedByUserId, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted, StudentGroupId, ClassId, SectionId
-- ExamTerm enum: FirstTerminal=1, HalfYearly=2, SecondTerminal=3, Annual=4, Final=5, PreTest=6, Test=7, Other=8
-- Status 8 = Draft
SET IDENTITY_INSERT Exams ON;
INSERT INTO Exams (Id, Name, Term, Status, AcademicYearId, ClassId, SectionId, StudentGroupId, StartsOn, EndsOn, IsLocked, CreatedBy, CreatedAt, IsDeleted)
SELECT 
    ROW_NUMBER() OVER (ORDER BY c.Id) as Id,
    CASE 
        WHEN et.Id = 2 THEN 'Half Yearly Examination - ' + c.Name
        WHEN et.Id = 4 THEN 'Annual Examination - ' + c.Name
        WHEN et.Id = 5 THEN 'Final Examination - ' + c.Name
        ELSE 'First Terminal Examination - ' + c.Name
    END as Name,
    et.Id as Term,
    8 as Status,
    ay.Id as AcademicYearId,
    c.Id as ClassId,
    NULL as SectionId,
    NULL as StudentGroupId,
    DATEADD(day, 1, GETDATE()) as StartsOn,
    DATEADD(day, 15, GETDATE()) as EndsOn,
    0 as IsLocked,
    'system' as CreatedBy,
    GETDATE() as CreatedAt,
    0 as IsDeleted
FROM AcademicYears ay
CROSS JOIN Classes c
CROSS JOIN ExamTypes et
WHERE ay.IsActive = 1 AND c.IsActive = 1 AND et.Id IN (2, 4, 5)
AND NOT EXISTS (
    SELECT 1 FROM Exams ex WHERE ex.Name LIKE '%' + c.Name + '%' AND ex.Term = et.Id
);
SET IDENTITY_INSERT Exams OFF;
GO

-- 3. ExamSubjects
-- Columns: Id(IDENTITY), ExamId, SubjectId, FullMarks, PassMarks, IsOptional, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted, ExamDate, ExamDuration, ExamStartTime, IsActive, RoomNumber, TeacherId, TotalAssignmentMarks, TotalMCQMarks, TotalPracticalMarks, TotalVivaMarks, TotalWrittenMarks
INSERT INTO ExamSubjects (ExamId, SubjectId, FullMarks, PassMarks, IsOptional, CreatedBy, CreatedAt, IsDeleted, ExamDate, ExamDuration, ExamStartTime, IsActive, RoomNumber, TotalAssignmentMarks, TotalMCQMarks, TotalPracticalMarks, TotalVivaMarks, TotalWrittenMarks)
SELECT 
    e.Id as ExamId,
    s.Id as SubjectId,
    s.DefaultFullMarks as FullMarks,
    s.DefaultPassMarks as PassMarks,
    s.IsOptional as IsOptional,
    'system' as CreatedBy,
    GETDATE() as CreatedAt,
    0 as IsDeleted,
    DATEADD(day, (s.DisplayOrder % 10) + 1, e.StartsOn) as ExamDate,
    180 as ExamDuration,
    CAST('09:00:00' AS TIME) as ExamStartTime,
    1 as IsActive,
    'Room ' + CAST((s.DisplayOrder % 5) + 101 AS NVARCHAR) as RoomNumber,
    CASE WHEN s.IsPractical = 1 THEN 30 ELSE 0 END as TotalAssignmentMarks,
    CASE WHEN s.IsMandatory = 1 THEN 30 ELSE 0 END as TotalMCQMarks,
    CASE WHEN s.IsPractical = 1 THEN 40 ELSE 0 END as TotalPracticalMarks,
    0 as TotalVivaMarks,
    CASE WHEN s.IsMandatory = 1 THEN 70 ELSE 100 END as TotalWrittenMarks
FROM Exams e
JOIN Subjects s ON s.IsActive = 1 AND s.IsDeleted = 0
WHERE e.Status = 8 AND e.IsDeleted = 0
AND NOT EXISTS (
    SELECT 1 FROM ExamSubjects es WHERE es.ExamId = e.Id AND es.SubjectId = s.Id
);
GO

-- 4. ExamSchedules (NO IsActive column in DB)
-- Columns: Id(IDENTITY), ExamId, SubjectId, ExamDate, StartsAt, EndsAt, RoomNo, Instructions, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted, ClassId, SectionId, StudentGroupId
INSERT INTO ExamSchedules (ExamId, SubjectId, ExamDate, StartsAt, EndsAt, RoomNo, Instructions, CreatedBy, CreatedAt, IsDeleted, ClassId, SectionId, StudentGroupId)
SELECT 
    es.ExamId,
    es.SubjectId,
    es.ExamDate,
    es.ExamStartTime as StartsAt,
    DATEADD(MINUTE, es.ExamDuration, es.ExamStartTime) as EndsAt,
    es.RoomNumber as RoomNo,
    'Please bring your admit card and write your name on the answer sheet.' as Instructions,
    'system' as CreatedBy,
    GETDATE() as CreatedAt,
    0 as IsDeleted,
    e.ClassId,
    e.SectionId,
    e.StudentGroupId
FROM ExamSubjects es
JOIN Exams e ON e.Id = es.ExamId
WHERE es.IsDeleted = 0 AND e.IsDeleted = 0
AND NOT EXISTS (
    SELECT 1 FROM ExamSchedules sched WHERE sched.ExamId = es.ExamId AND sched.SubjectId = es.SubjectId
);
GO

-- 5. AdmitCards (NO IsActive column in DB)
-- Columns: Id(IDENTITY), ExamId, StudentId, CardNo, PrintedAt, IsGenerated, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted, AdmitCardNumber, IsIssued, IssuedAt, RollNumber, SeatNumber
INSERT INTO AdmitCards (ExamId, StudentId, CardNo, AdmitCardNumber, RollNumber, SeatNumber, IsIssued, IsGenerated, CreatedBy, CreatedAt, IsDeleted)
SELECT 
    e.Id as ExamId,
    st.Id as StudentId,
    'ADM-' + CAST(e.Id AS NVARCHAR) + '-' + CAST(st.Id AS NVARCHAR) as CardNo,
    'ADM-' + CAST(e.Id AS NVARCHAR) + '-' + RIGHT('0000' + CAST(st.Id AS NVARCHAR), 4) as AdmitCardNumber,
    st.RollNumber,
    CAST(st.RollNumber AS NVARCHAR) as SeatNumber,
    0 as IsIssued,
    1 as IsGenerated,
    'system' as CreatedBy,
    GETDATE() as CreatedAt,
    0 as IsDeleted
FROM Exams e
JOIN Students st ON st.ClassId = e.ClassId AND st.IsDeleted = 0
WHERE e.Status = 8 AND e.IsDeleted = 0
AND NOT EXISTS (
    SELECT 1 FROM AdmitCards ac WHERE ac.ExamId = e.Id AND ac.StudentId = st.Id
);
GO

PRINT 'Seed data insertion completed successfully!';
GO
SELECT 'ExamTypes' AS Tbl, COUNT(*) AS Cnt FROM ExamTypes
UNION ALL SELECT 'Exams', COUNT(*) FROM Exams
UNION ALL SELECT 'ExamSubjects', COUNT(*) FROM ExamSubjects
UNION ALL SELECT 'ExamSchedules', COUNT(*) FROM ExamSchedules
UNION ALL SELECT 'AdmitCards', COUNT(*) FROM AdmitCards;
GO
