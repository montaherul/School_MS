CREATE OR ALTER PROCEDURE [dbo].[SP_Exam_DashboardSummary]
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0) AS TotalExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status = 5) AS PublishedExams,
        (SELECT COUNT(*) FROM Exams WHERE AcademicYearId = @AcademicYearId AND IsDeleted = 0 AND Status NOT IN (5, 7)) AS PendingExams,
        (SELECT COUNT(DISTINCT StudentId) FROM StudentExamResults ser
            INNER JOIN Exams e ON ser.ExamId = e.Id
            WHERE e.AcademicYearId = @AcademicYearId AND ser.IsDeleted = 0) AS StudentsAppeared,
        ROUND(
            (SELECT 100.0 * SUM(CASE WHEN ser.IsPassed = 1 THEN 1 ELSE 0 END) / NULLIF(COUNT(ser.Id), 0)
             FROM StudentExamResults ser INNER JOIN Exams e ON ser.ExamId = e.Id
             WHERE e.AcademicYearId = @AcademicYearId AND ser.IsDeleted = 0)
        , 1) AS PassRate,
        ROUND(
            (SELECT AVG(ser.Gpa) FROM StudentExamResults ser INNER JOIN Exams e ON ser.ExamId = e.Id
             WHERE e.AcademicYearId = @AcademicYearId AND ser.IsDeleted = 0)
        , 2) AS AverageGPA;
END;
GO
