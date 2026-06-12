-- ============================================================
-- EXAM CRUD AUDIT - PHASE 11: Database Audit
-- Find invalid group data
-- ============================================================

-- 1. Find Class 1-8 records with non-General groups in Exams
SELECT e.Id AS ExamId, e.Name AS ExamName, sc.Name AS ClassName, sg.Name AS GroupName
FROM Exams e
INNER JOIN Classes sc ON e.ClassId = sc.Id
LEFT JOIN StudentGroups sg ON e.StudentGroupId = sg.Id
WHERE e.IsDeleted = 0
  AND (sg.Name IS NOT NULL AND sg.Name != '')
  AND sc.Name LIKE '%1%' OR sc.Name LIKE '%2%' OR sc.Name LIKE '%3%' OR sc.Name LIKE '%4%'
  OR sc.Name LIKE '%5%' OR sc.Name LIKE '%6%' OR sc.Name LIKE '%7%' OR sc.Name LIKE '%8%';

-- 2. Find Class 9-10 records with missing groups in Exams
SELECT e.Id AS ExamId, e.Name AS ExamName, sc.Name AS ClassName
FROM Exams e
INNER JOIN Classes sc ON e.ClassId = sc.Id
WHERE e.IsDeleted = 0
  AND e.StudentGroupId IS NULL
  AND (sc.Name LIKE '%9%' OR sc.Name LIKE '%10%' OR sc.Name LIKE '%Nine%' OR sc.Name LIKE '%Ten%');

-- 3. Find Students in Class 1-8 with non-null StudentGroupId
SELECT s.Id AS StudentId, s.FullName, sc.Name AS ClassName, sg.Name AS GroupName
FROM Students s
INNER JOIN Classes sc ON s.ClassId = sc.Id
LEFT JOIN StudentGroups sg ON s.StudentGroupId = sg.Id
WHERE s.IsDeleted = 0
  AND s.StudentGroupId IS NOT NULL
  AND dbo.ExtractClassNumber(sc.Name) BETWEEN 1 AND 8;

-- 4. Find Students in Class 9-10 with null StudentGroupId
SELECT s.Id AS StudentId, s.FullName, sc.Name AS ClassName
FROM Students s
INNER JOIN Classes sc ON s.ClassId = sc.Id
WHERE s.IsDeleted = 0
  AND s.StudentGroupId IS NULL
  AND dbo.ExtractClassNumber(sc.Name) BETWEEN 9 AND 10;

-- 5. Find class/group mismatches (subject with SubjectGroup not matching exam group)
SELECT es.Id AS ExamSubjectId, e.Name AS ExamName, sub.Name AS SubjectName,
       sub.SubjectGroup, sg.Name AS ExamGroupName
FROM ExamSubjects es
INNER JOIN Exams e ON es.ExamId = e.Id
INNER JOIN Subjects sub ON es.SubjectId = sub.Id
LEFT JOIN StudentGroups sg ON e.StudentGroupId = sg.Id
WHERE e.IsDeleted = 0 AND es.IsDeleted = 0
  AND e.StudentGroupId IS NOT NULL
  AND sg.Name IS NOT NULL
  AND sub.SubjectGroup != ''
  AND sub.SubjectGroup != 'Common'
  AND sub.SubjectGroup != sg.Name;
