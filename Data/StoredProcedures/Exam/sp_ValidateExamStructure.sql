CREATE OR ALTER PROCEDURE sp_ValidateExamStructure
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate component marks sum equals subject marks
    SELECT
        'MarksStructure' AS CheckType,
        esj.Id AS SubjectId,
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
    FROM ExamSubjects esj WITH(NOLOCK)
    LEFT JOIN ExamSubjectComponents esc WITH(NOLOCK) ON esc.ExamSubjectId = esj.Id AND esc.IsDeleted = 0
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0
    GROUP BY esj.Id, esj.SubjectName, esj.FullMarks, esj.PassMarks
    HAVING ABS(COALESCE(SUM(esc.MaxMarks), 0) - esj.FullMarks) > 0.01 
        OR ABS(COALESCE(SUM(esc.PassMarks), 0) - esj.PassMarks) > 0.01;

    -- Validate subjects are assigned to the class
    SELECT
        'GroupAssignment' AS CheckType,
        esj.Id AS SubjectId,
        esj.SubjectName,
        ec.ClassName,
        cs.StudentGroupId AS ClassGroupId,
        cs.SubjectId AS ValidSubjectId,
        CASE WHEN cs.SubjectId IS NULL THEN 'FAIL: Subject not assigned to class' ELSE 'PASS' END AS Result
    FROM ExamSubjects esj WITH(NOLOCK)
    INNER JOIN ExamClasses ec WITH(NOLOCK) ON ec.ExamId = esj.ExamId AND ec.ClassId = esj.ClassId AND ec.IsDeleted = 0
    LEFT JOIN ClassSubjects cs WITH(NOLOCK) ON cs.SchoolClassId = ec.ClassId 
        AND cs.SubjectId = esj.SubjectId 
        AND cs.IsDeleted = 0 
        AND cs.IsActive = 1
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0
    AND cs.SubjectId IS NULL;

    -- Validate all subjects have at least one component
    SELECT
        'ComponentExistence' AS CheckType,
        esj.Id AS SubjectId,
        esj.SubjectName,
        ec.ClassName,
        'FAIL: No components defined' AS Result
    FROM ExamSubjects esj WITH(NOLOCK)
    INNER JOIN ExamClasses ec WITH(NOLOCK) ON ec.ExamId = esj.ExamId AND ec.ClassId = esj.ClassId AND ec.IsDeleted = 0
    LEFT JOIN ExamSubjectComponents esc WITH(NOLOCK) ON esc.ExamSubjectId = esj.Id AND esc.IsDeleted = 0
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0
    AND esc.Id IS NULL;
END;
