-- ============================================================
-- Fix Section hierarchy for group-based classes 9 & 10
-- Creates parent sections per group and links sub-sections
-- ============================================================
SET NOCOUNT ON;

DECLARE @Now DATETIME2 = SYSUTCDATETIME();
DECLARE @CreatedBy NVARCHAR(64) = 'system';

-- ============ CLASS 9 ============

-- Science group (StudentGroupId=1): Create parent section if missing
IF NOT EXISTS (SELECT 1 FROM Sections WHERE SchoolClassId = 9 AND Name = 'Science' AND IsDeleted = 0)
BEGIN
    INSERT INTO Sections (SchoolClassId, Name, Capacity, ParentSectionId, StudentGroupId, CreatedBy, CreatedAt, IsDeleted)
    VALUES (9, 'Science', 60, NULL, 1, @CreatedBy, @Now, 0);
    PRINT 'Class 9: Created parent section Science (StudentGroupId=1)';
END

-- Business Studies group (StudentGroupId=2)
IF NOT EXISTS (SELECT 1 FROM Sections WHERE SchoolClassId = 9 AND Name = 'BusinessStudies' AND IsDeleted = 0)
BEGIN
    INSERT INTO Sections (SchoolClassId, Name, Capacity, ParentSectionId, StudentGroupId, CreatedBy, CreatedAt, IsDeleted)
    VALUES (9, 'BusinessStudies', 60, NULL, 2, @CreatedBy, @Now, 0);
    PRINT 'Class 9: Created parent section BusinessStudies (StudentGroupId=2)';
END

-- Humanities group (StudentGroupId=3)
IF NOT EXISTS (SELECT 1 FROM Sections WHERE SchoolClassId = 9 AND Name = 'Humanities' AND IsDeleted = 0)
BEGIN
    INSERT INTO Sections (SchoolClassId, Name, Capacity, ParentSectionId, StudentGroupId, CreatedBy, CreatedAt, IsDeleted)
    VALUES (9, 'Humanities', 60, NULL, 3, @CreatedBy, @Now, 0);
    PRINT 'Class 9: Created parent section Humanities (StudentGroupId=3)';
END

-- Link sub-sections (A, B) to their parent sections for Class 9
UPDATE Sections SET ParentSectionId = (SELECT TOP 1 Id FROM Sections WHERE SchoolClassId = 9 AND StudentGroupId = 1 AND ParentSectionId IS NULL AND Name = 'Science' AND IsDeleted = 0)
WHERE Id IN (18, 19) AND IsDeleted = 0;
PRINT 'Class 9: Linked Science sub-sections (Id=18,19) to parent';

UPDATE Sections SET ParentSectionId = (SELECT TOP 1 Id FROM Sections WHERE SchoolClassId = 9 AND StudentGroupId = 2 AND ParentSectionId IS NULL AND Name = 'BusinessStudies' AND IsDeleted = 0)
WHERE Id IN (21, 22) AND IsDeleted = 0;
PRINT 'Class 9: Linked BusinessStudies sub-sections (Id=21,22) to parent';

UPDATE Sections SET ParentSectionId = (SELECT TOP 1 Id FROM Sections WHERE SchoolClassId = 9 AND StudentGroupId = 3 AND ParentSectionId IS NULL AND Name = 'Humanities' AND IsDeleted = 0)
WHERE Id IN (24, 25) AND IsDeleted = 0;
PRINT 'Class 9: Linked Humanities sub-sections (Id=24,25) to parent';


-- ============ CLASS 10 ============

-- Science group (StudentGroupId=1)
IF NOT EXISTS (SELECT 1 FROM Sections WHERE SchoolClassId = 10 AND Name = 'Science' AND IsDeleted = 0)
BEGIN
    INSERT INTO Sections (SchoolClassId, Name, Capacity, ParentSectionId, StudentGroupId, CreatedBy, CreatedAt, IsDeleted)
    VALUES (10, 'Science', 60, NULL, 1, @CreatedBy, @Now, 0);
    PRINT 'Class 10: Created parent section Science (StudentGroupId=1)';
END

-- Business Studies group (StudentGroupId=2)
IF NOT EXISTS (SELECT 1 FROM Sections WHERE SchoolClassId = 10 AND Name = 'BusinessStudies' AND IsDeleted = 0)
BEGIN
    INSERT INTO Sections (SchoolClassId, Name, Capacity, ParentSectionId, StudentGroupId, CreatedBy, CreatedAt, IsDeleted)
    VALUES (10, 'BusinessStudies', 60, NULL, 2, @CreatedBy, @Now, 0);
    PRINT 'Class 10: Created parent section BusinessStudies (StudentGroupId=2)';
END

-- Humanities group (StudentGroupId=3)
IF NOT EXISTS (SELECT 1 FROM Sections WHERE SchoolClassId = 10 AND Name = 'Humanities' AND IsDeleted = 0)
BEGIN
    INSERT INTO Sections (SchoolClassId, Name, Capacity, ParentSectionId, StudentGroupId, CreatedBy, CreatedAt, IsDeleted)
    VALUES (10, 'Humanities', 60, NULL, 3, @CreatedBy, @Now, 0);
    PRINT 'Class 10: Created parent section Humanities (StudentGroupId=3)';
END

-- Link sub-sections to their parent sections for Class 10
UPDATE Sections SET ParentSectionId = (SELECT TOP 1 Id FROM Sections WHERE SchoolClassId = 10 AND StudentGroupId = 1 AND ParentSectionId IS NULL AND Name = 'Science' AND IsDeleted = 0)
WHERE Id IN (27, 28) AND IsDeleted = 0;
PRINT 'Class 10: Linked Science sub-sections (Id=27,28) to parent';

UPDATE Sections SET ParentSectionId = (SELECT TOP 1 Id FROM Sections WHERE SchoolClassId = 10 AND StudentGroupId = 2 AND ParentSectionId IS NULL AND Name = 'BusinessStudies' AND IsDeleted = 0)
WHERE Id IN (30, 31) AND IsDeleted = 0;
PRINT 'Class 10: Linked BusinessStudies sub-sections (Id=30,31) to parent';

UPDATE Sections SET ParentSectionId = (SELECT TOP 1 Id FROM Sections WHERE SchoolClassId = 10 AND StudentGroupId = 3 AND ParentSectionId IS NULL AND Name = 'Humanities' AND IsDeleted = 0)
WHERE Id IN (33, 34) AND IsDeleted = 0;
PRINT 'Class 10: Linked Humanities sub-sections (Id=33,34) to parent';

-- ============ VERIFY ============
PRINT '';
PRINT '============================================';
PRINT 'Final Section hierarchy for classes 9 & 10:';
PRINT '============================================';
SELECT 
    s.Id,
    s.Name AS SectionName,
    s.SchoolClassId,
    sg.Name AS StudentGroup,
    ps.Name AS ParentSection,
    CASE WHEN s.ParentSectionId IS NULL THEN 'PARENT' ELSE 'SUB' END AS Level
FROM Sections s
LEFT JOIN StudentGroups sg ON sg.Id = s.StudentGroupId
LEFT JOIN Sections ps ON ps.Id = s.ParentSectionId
WHERE s.SchoolClassId IN (9, 10) AND s.IsDeleted = 0
ORDER BY s.SchoolClassId, s.StudentGroupId, s.ParentSectionId, s.Id;
GO
