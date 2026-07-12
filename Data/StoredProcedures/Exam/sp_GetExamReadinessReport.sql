CREATE OR ALTER PROCEDURE sp_GetExamReadinessReport
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Summary stats
    SELECT
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0) AS TotalExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status = 0) AS DraftExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status >= 4) AS ReadyExams,
        (SELECT COUNT(*) FROM Classes c WHERE EXISTS (
            SELECT 1 FROM Exams e WHERE e.ClassId = c.Id AND e.AcademicYearId = @AcademicYearId AND e.IsDeleted = 0
        )) AS ClassesWithExams,
        (SELECT COUNT(*) FROM Classes WHERE IsDeleted = 0 AND IsActive = 1) AS TotalActiveClasses;

    -- Exams missing subjects
    SELECT
        e.Id AS ExamId,
        e.Name AS ExamName,
        c.Name AS ClassName,
        e.Status,
        COUNT(es.Id) AS SubjectCount
    FROM Exams e WITH(NOLOCK)
    INNER JOIN Classes c WITH(NOLOCK) ON e.ClassId = c.Id
    LEFT JOIN ExamSubjects es WITH(NOLOCK) ON es.ExamId = e.Id AND es.IsDeleted = 0
    WHERE e.AcademicYearId = @AcademicYearId AND e.IsDeleted = 0
    GROUP BY e.Id, e.Name, c.Name, e.Status
    HAVING COUNT(es.Id) = 0
    ORDER BY c.Name, e.Name;

    -- Exams missing schedule
    SELECT
        e.Id AS ExamId,
        e.Name AS ExamName,
        c.Name AS ClassName,
        COUNT(DISTINCT es.SubjectId) AS SubjectCount,
        COUNT(DISTINCT sch.SubjectId) AS ScheduledCount
    FROM Exams e WITH(NOLOCK)
    INNER JOIN Classes c WITH(NOLOCK) ON e.ClassId = c.Id
    INNER JOIN ExamSubjects es WITH(NOLOCK) ON es.ExamId = e.Id AND es.IsDeleted = 0
    LEFT JOIN ExamSchedules sch WITH(NOLOCK) ON sch.ExamId = e.Id AND sch.IsDeleted = 0
    WHERE e.AcademicYearId = @AcademicYearId AND e.IsDeleted = 0
    GROUP BY e.Id, e.Name, c.Name
    HAVING COUNT(DISTINCT es.SubjectId) > COUNT(DISTINCT sch.SubjectId)
    ORDER BY c.Name, e.Name;

    -- Exams without grading rules
    SELECT
        e.Id AS ExamId,
        e.Name AS ExamName,
        c.Name AS ClassName
    FROM Exams e WITH(NOLOCK)
    INNER JOIN Classes c WITH(NOLOCK) ON e.ClassId = c.Id
    WHERE e.AcademicYearId = @AcademicYearId AND e.IsDeleted = 0
        AND NOT EXISTS (SELECT 1 FROM GradingRules WHERE IsDeleted = 0)
    ORDER BY c.Name, e.Name;
END;
