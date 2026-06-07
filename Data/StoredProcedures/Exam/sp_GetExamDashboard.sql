CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamDashboard]
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Stats
    SELECT
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0) AS TotalExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status = 0) AS DraftExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status = 1) AS SubmittedExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status = 2) AS ReviewedExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status = 3) AS ApprovedExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status = 4) AS PublishedExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status = 5) AS LockedExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status = 6) AS UnpublishedExams,
        (SELECT COUNT(DISTINCT StudentId) FROM StudentExamResults ser
            INNER JOIN Exams e ON ser.ExamId = e.Id
            WHERE e.AcademicYearId = @AcademicYearId AND ser.IsDeleted = 0) AS StudentsAppeared;

    -- Status distribution (for chart)
    SELECT Status, COUNT(*) AS Count
    FROM Exams
    WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0
    GROUP BY Status
    ORDER BY Status;

    -- Recent exams
    SELECT TOP 10
        e.Id, e.Name, e.Term, e.StartsOn, e.EndsOn, e.Status, e.CreatedAt,
        (SELECT COUNT(*) FROM ExamSubjects es WHERE es.ExamId = e.Id) AS SubjectCount
    FROM Exams e
    WHERE e.AcademicYearId = @AcademicYearId AND e.IsDeleted = 0
    ORDER BY e.CreatedAt DESC;

    -- Pass rate by exam
    SELECT 
        e.Id AS ExamId,
        e.Name AS ExamName,
        COUNT(ser.Id) AS TotalStudents,
        SUM(CASE WHEN ser.IsPassed = 1 THEN 1 ELSE 0 END) AS PassedCount,
        SUM(CASE WHEN ser.IsPassed = 0 THEN 1 ELSE 0 END) AS FailedCount,
        CASE WHEN COUNT(ser.Id) > 0 
            THEN ROUND(100.0 * SUM(CASE WHEN ser.IsPassed = 1 THEN 1 ELSE 0 END) / COUNT(ser.Id), 1)
            ELSE 0 
        END AS PassPercentage
    FROM Exams e
    LEFT JOIN StudentExamResults ser ON e.Id = ser.ExamId AND ser.IsDeleted = 0
    WHERE e.AcademicYearId = @AcademicYearId AND e.IsDeleted = 0
    GROUP BY e.Id, e.Name
    ORDER BY e.Name;
END;
GO