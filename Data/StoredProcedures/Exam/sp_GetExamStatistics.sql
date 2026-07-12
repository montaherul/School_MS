CREATE OR ALTER PROCEDURE sp_GetExamStatistics
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Summary
    SELECT
        e.Id AS ExamId,
        e.Name AS ExamName,
        (SELECT COUNT(*) FROM ExamClasses WHERE ExamId = @ExamId AND IsDeleted = 0) AS ClassCount,
        (SELECT COUNT(*) FROM ExamClasses ec
            INNER JOIN ExamSections es ON es.ExamClassId = ec.Id AND es.IsDeleted = 0
            WHERE ec.ExamId = @ExamId AND ec.IsDeleted = 0) AS SectionCount,
        (SELECT COUNT(*) FROM ExamSubjects WHERE ExamId = @ExamId AND IsDeleted = 0) AS SubjectCount,
        (SELECT COUNT(*) FROM ExamSubjects WHERE ExamId = @ExamId AND IsDeleted = 0 AND TeacherId IS NULL) AS SubjectsWithoutTeacher,
        (SELECT COUNT(*) FROM ExamSubjects WHERE ExamId = @ExamId AND IsDeleted = 0 AND IsActive = 0) AS InactiveSubjects,
        (SELECT COUNT(*) FROM ExamSubjectComponents esc
            INNER JOIN ExamSubjects esj ON esj.Id = esc.ExamSubjectId AND esj.IsDeleted = 0
            WHERE esj.ExamId = @ExamId AND esc.IsDeleted = 0) AS ComponentCount;

    -- Class breakdown
    SELECT
        ec.Id AS ExamClassId,
        ec.ClassId,
        ec.ClassName,
        ec.SortOrder,
        (SELECT COUNT(*) FROM ExamSections WHERE ExamClassId = ec.Id AND IsDeleted = 0) AS SectionCount,
        (SELECT COUNT(*) FROM ExamSubjects WHERE ExamId = @ExamId AND ClassId = ec.ClassId AND IsDeleted = 0) AS SubjectCount,
        (SELECT COUNT(*) FROM ExamSubjects WHERE ExamId = @ExamId AND ClassId = ec.ClassId AND IsDeleted = 0 AND TeacherId IS NULL) AS SubjectsWithoutTeacher
    FROM ExamClasses ec
    WHERE ec.ExamId = @ExamId AND ec.IsDeleted = 0
    ORDER BY ec.SortOrder;

    -- Subject breakdown
    SELECT
        esj.Id AS ExamSubjectId,
        esj.SubjectName,
        esj.SubjectCode,
        c.Name AS ClassName,
        esj.FullMarks,
        esj.PassMarks,
        esj.IsOptional,
        esj.IsReligionSubject,
        CASE WHEN esj.TeacherId IS NULL THEN 'No' ELSE 'Yes' END AS HasTeacher,
        (SELECT COUNT(*) FROM ExamSubjectComponents WHERE ExamSubjectId = esj.Id AND IsDeleted = 0) AS ComponentCount
    FROM ExamSubjects esj
    INNER JOIN Classes c ON c.Id = esj.ClassId
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0
    ORDER BY c.DisplayOrder, esj.SubjectName;

    -- Subjects without components
    SELECT
        esj.Id AS ExamSubjectId,
        esj.SubjectName,
        c.Name AS ClassName
    FROM ExamSubjects esj
    INNER JOIN Classes c ON c.Id = esj.ClassId
    LEFT JOIN ExamSubjectComponents esc ON esc.ExamSubjectId = esj.Id AND esc.IsDeleted = 0
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0 AND esc.Id IS NULL;
END;
