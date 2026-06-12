-- ============================================================
-- EXAM CRUD AUDIT - PHASE 11: Data Correction Scripts
-- SAFE corrections - NO DELETE operations
-- ============================================================

-- ============================================================
-- HELPER: Create function to extract class number from class name
-- Run this first if the function doesn't exist
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExtractClassNumber]') AND type IN (N'FN', N'IF', N'TF', N'FS', N'FT'))
EXEC('
CREATE FUNCTION [dbo].[ExtractClassNumber](@className NVARCHAR(100))
RETURNS INT
AS
BEGIN
    DECLARE @num INT = 0;
    DECLARE @i INT = 1;
    DECLARE @len INT = LEN(@className);
    DECLARE @digits NVARCHAR(10) = '''';

    WHILE @i <= @len
    BEGIN
        IF SUBSTRING(@className, @i, 1) LIKE ''[0-9]''
            SET @digits = @digits + SUBSTRING(@className, @i, 1);
        SET @i = @i + 1;
    END;

    IF LEN(@digits) > 0
        SET @num = TRY_CAST(@digits AS INT);

    RETURN ISNULL(@num, 0);
END;
');

-- ============================================================
-- 1. CORRECT: Set StudentGroupId = NULL for Class 1-8 students
-- who have a group assigned (should be General only)
-- ============================================================
BEGIN TRANSACTION;
UPDATE s
SET s.StudentGroupId = NULL,
    s.UpdatedAt = GETDATE(),
    s.UpdatedBy = 'SYSTEM_AUDIT'
FROM Students s
INNER JOIN Classes sc ON s.ClassId = sc.Id
WHERE s.IsDeleted = 0
  AND s.StudentGroupId IS NOT NULL
   AND dbo.ExtractClassNumber(sc.Name) BETWEEN 1 AND 8;
PRINT 'Corrected Students: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));
COMMIT TRANSACTION;

-- ============================================================
-- 2. CORRECT: Assign default Group for Class 9-10 exams with missing group
-- Note: This requires knowing the default group - uncomment and modify as needed
-- ============================================================
-- BEGIN TRANSACTION;
-- UPDATE e
-- SET e.StudentGroupId = (SELECT TOP 1 Id FROM StudentGroups WHERE Name = 'Science' AND IsActive = 1),
--     e.UpdatedAt = GETDATE(),
--     e.UpdatedBy = 'SYSTEM_AUDIT'
-- FROM Exams e
-- INNER JOIN Classes sc ON e.ClassId = sc.Id
-- WHERE e.IsDeleted = 0
--   AND e.StudentGroupId IS NULL
--   AND dbo.ExtractClassNumber(sc.Name) BETWEEN 9 AND 10;
-- PRINT 'Corrected Exams: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));
-- COMMIT TRANSACTION;

-- ============================================================
-- 3. CORRECT: Remove invalid exam subjects where subject group
-- doesn't match exam group (cross-group subjects)
-- Note: This soft-deletes them (sets IsDeleted = 1)
-- ============================================================
BEGIN TRANSACTION;
UPDATE es
SET es.IsDeleted = 1,
    es.UpdatedAt = GETDATE(),
    es.UpdatedBy = 'SYSTEM_AUDIT'
FROM ExamSubjects es
INNER JOIN Exams e ON es.ExamId = e.Id
INNER JOIN Subjects sub ON es.SubjectId = sub.Id
LEFT JOIN StudentGroups sg ON e.StudentGroupId = sg.Id
WHERE e.IsDeleted = 0
  AND es.IsDeleted = 0
  AND e.StudentGroupId IS NOT NULL
  AND sg.Name IS NOT NULL
  AND sub.SubjectGroup != ''
  AND sub.SubjectGroup != 'Common'
  AND sub.SubjectGroup != 'General'
  AND sub.SubjectGroup != sg.Name;
PRINT 'Soft-deleted ExamSubjects: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));
COMMIT TRANSACTION;

-- ============================================================
-- 4. AUDIT REPORT: Show corrected data summary
-- ============================================================
SELECT 'Students with invalid groups (Class 1-8)' AS IssueType, COUNT(*) AS Count
FROM Students s
INNER JOIN Classes sc ON s.ClassId = sc.Id
WHERE s.IsDeleted = 0
  AND s.StudentGroupId IS NOT NULL
  AND dbo.ExtractClassNumber(sc.Name) BETWEEN 1 AND 8
UNION ALL
SELECT 'Students missing groups (Class 9-10)' AS IssueType, COUNT(*) AS Count
FROM Students s
INNER JOIN Classes sc ON s.ClassId = sc.Id
WHERE s.IsDeleted = 0
  AND s.StudentGroupId IS NULL
  AND dbo.ExtractClassNumber(sc.Name) BETWEEN 9 AND 10
UNION ALL
SELECT 'Exams with invalid group combinations' AS IssueType, COUNT(*) AS Count
FROM Exams e
INNER JOIN Classes sc ON e.ClassId = sc.Id
LEFT JOIN StudentGroups sg ON e.StudentGroupId = sg.Id
WHERE e.IsDeleted = 0
  AND (
    (dbo.ExtractClassNumber(sc.Name) BETWEEN 1 AND 8 AND e.StudentGroupId IS NOT NULL)
    OR
    (dbo.ExtractClassNumber(sc.Name) BETWEEN 9 AND 10 AND e.StudentGroupId IS NULL)
  )
UNION ALL
SELECT 'Cross-group exam subjects' AS IssueType, COUNT(*) AS Count
FROM ExamSubjects es
INNER JOIN Exams e ON es.ExamId = e.Id
INNER JOIN Subjects sub ON es.SubjectId = sub.Id
LEFT JOIN StudentGroups sg ON e.StudentGroupId = sg.Id
WHERE e.IsDeleted = 0 AND es.IsDeleted = 0
  AND e.StudentGroupId IS NOT NULL
  AND sg.Name IS NOT NULL
  AND sub.SubjectGroup != ''
  AND sub.SubjectGroup != 'Common'
  AND sub.SubjectGroup != 'General'
  AND sub.SubjectGroup != sg.Name;
