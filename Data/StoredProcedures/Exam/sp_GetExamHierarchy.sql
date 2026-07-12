CREATE OR ALTER PROCEDURE sp_GetExamHierarchy
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Exam basic info
    SELECT 
        e.Id AS ExamId,
        e.Name AS ExamName,
        e.AcademicYearId,
        ay.Name AS AcademicYearName,
        e.Term,
        e.StartsOn,
        e.EndsOn,
        e.Status,
        e.IsPublished,
        e.IsLocked,
        e.IsArchived,
        e.ArchivedAt,
        e.ArchiveReason
    FROM Exams e
    LEFT JOIN AcademicYears ay ON ay.Id = e.AcademicYearId
    WHERE e.Id = @ExamId AND e.IsDeleted = 0;

    -- Classes in the exam
    SELECT 
        ec.Id AS ExamClassId,
        ec.ClassId,
        c.Name AS ClassName,
        c.Code AS ClassCode,
        c.DisplayOrder AS ClassOrder,
        ec.ClassName,
        ec.SortOrder,
        sg.Id AS StudentGroupId,
        sg.Name AS StudentGroupName,
        sg.Code AS StudentGroupCode
    FROM ExamClasses ec
    INNER JOIN Classes c ON c.Id = ec.ClassId
    LEFT JOIN StudentGroups sg ON sg.Id = c.StudentGroupId
    WHERE ec.ExamId = @ExamId AND ec.IsDeleted = 0
    ORDER BY ec.SortOrder, c.DisplayOrder;

    -- Sections per class
    SELECT 
        es.Id AS ExamSectionId,
        es.ExamClassId,
        es.SectionId,
        s.Name AS SectionName,
        s.Code AS SectionCode,
        es.SectionName
    FROM ExamSections es
    INNER JOIN Sections s ON s.Id = es.SectionId
    WHERE es.ExamClassId IN (
        SELECT Id FROM ExamClasses WHERE ExamId = @ExamId AND IsDeleted = 0
    ) AND es.IsDeleted = 0
    ORDER BY es.ExamClassId, s.Name;

    -- Subjects per class
    SELECT 
        esj.Id AS ExamSubjectId,
        esj.ClassId AS ExamClassId,
        esj.SubjectId,
        sub.Name AS SubjectName,
        sub.NameBn AS SubjectNameBn,
        sub.Code AS SubjectCode,
        sub.Credit,
        sub.NCTBCode,
        esj.IsOptional,
        esj.IsReligionSubject,
        esj.FullMarks,
        esj.PassMarks,
        esj.TeacherId,
        t.FullName AS TeacherName,
        t.EmployeeCode AS TeacherEmployeeCode,
        esj.SubjectName AS SnapshotSubjectName,
        esj.SubjectCode AS SnapshotSubjectCode,
        esj.TeacherName AS SnapshotTeacherName,
        esj.TeacherEmployeeCode AS SnapshotTeacherEmployeeCode,
        esj.Credit AS SnapshotCredit,
        esj.NCTBCode AS SnapshotNCTBCode,
        esj.SubjectType,
        esj.SubjectGroup,
        esj.TheoryMarks,
        esj.PracticalMarks
    FROM ExamSubjects esj
    INNER JOIN Subjects sub ON sub.Id = esj.SubjectId
    LEFT JOIN Teachers t ON t.Id = esj.TeacherId
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0
    ORDER BY esj.ClassId, sub.DisplayOrder, sub.Name;

    -- Components per subject
    SELECT 
        esc.Id AS ExamSubjectComponentId,
        esc.ExamSubjectId,
        esc.ComponentId,
        ec.Name AS ComponentName,
        ec.Code AS ComponentCode,
        ec.IsPractical,
        esc.MaxMarks,
        esc.PassMarks,
        esc.DisplayOrder,
        esc.Weight,
        esc.ComponentName AS SnapshotComponentName,
        esc.ComponentCode AS SnapshotComponentCode,
        esc.Weight AS SnapshotWeight
    FROM ExamSubjectComponents esc
    INNER JOIN ExamComponents ec ON ec.Id = esc.ComponentId
    WHERE esc.ExamSubjectId IN (
        SELECT Id FROM ExamSubjects WHERE ExamId = @ExamId AND IsDeleted = 0
    ) AND esc.IsDeleted = 0
    ORDER BY esc.ExamSubjectId, esc.DisplayOrder;
END;
