CREATE OR ALTER PROCEDURE sp_GetExamReadiness
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Overview counts
    SELECT
        (SELECT COUNT(*) FROM ExamClasses WHERE ExamId = @ExamId AND IsDeleted = 0) AS ClassCount,
        (SELECT COUNT(*) FROM ExamClasses ec
            INNER JOIN ExamSections es ON es.ExamClassId = ec.Id AND es.IsDeleted = 0
            WHERE ec.ExamId = @ExamId AND ec.IsDeleted = 0) AS SectionCount,
        (SELECT COUNT(*) FROM ExamSubjects WHERE ExamId = @ExamId AND IsDeleted = 0) AS SubjectCount;

    -- Subjects missing teacher assignment
    SELECT
        esj.Id AS ExamSubjectId,
        esj.SubjectName,
        c.Name AS ClassName,
        'No teacher assigned' AS Issue
    FROM ExamSubjects esj
    INNER JOIN Classes c ON c.Id = esj.ClassId
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0 AND esj.TeacherId IS NULL;

    -- Subjects missing components
    SELECT
        esj.Id AS ExamSubjectId,
        esj.SubjectName,
        c.Name AS ClassName,
        'No components defined' AS Issue
    FROM ExamSubjects esj
    INNER JOIN Classes c ON c.Id = esj.ClassId
    LEFT JOIN ExamSubjectComponents esc ON esc.ExamSubjectId = esj.Id AND esc.IsDeleted = 0
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0 AND esc.Id IS NULL;

    -- Subjects without schedule
    SELECT
        esj.Id AS ExamSubjectId,
        esj.SubjectName,
        c.Name AS ClassName,
        'No exam schedule' AS Issue
    FROM ExamSubjects esj
    INNER JOIN Classes c ON c.Id = esj.ClassId
    LEFT JOIN ExamSchedules sch ON sch.ExamId = @ExamId AND sch.SubjectId = esj.SubjectId AND sch.IsDeleted = 0
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0 AND sch.Id IS NULL;

    -- Component marks validation
    SELECT
        esj.Id AS ExamSubjectId,
        esj.SubjectName,
        esj.FullMarks AS SubjectFullMarks,
        esj.PassMarks AS SubjectPassMarks,
        COALESCE(SUM(esc.MaxMarks), 0) AS ComponentMaxMarksSum,
        COALESCE(SUM(esc.PassMarks), 0) AS ComponentPassMarksSum,
        CASE 
            WHEN ABS(COALESCE(SUM(esc.MaxMarks), 0) - esj.FullMarks) > 0.01 THEN 'FAIL: MaxMarks mismatch'
            WHEN ABS(COALESCE(SUM(esc.PassMarks), 0) - esj.PassMarks) > 0.01 THEN 'WARN: PassMarks mismatch'
            ELSE 'PASS'
        END AS Result
    FROM ExamSubjects esj
    LEFT JOIN ExamSubjectComponents esc ON esc.ExamSubjectId = esj.Id AND esc.IsDeleted = 0
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0
    GROUP BY esj.Id, esj.SubjectName, esj.FullMarks, esj.PassMarks;
END;
