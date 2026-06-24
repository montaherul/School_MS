CREATE OR ALTER PROCEDURE [dbo].[sp_GetResultPublicationDashboard]
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- All exam publication statuses
    SELECT 
        e.Id AS ExamId,
        e.Name AS ExamName,
        e.Term,
        e.StartsOn,
        e.EndsOn,
        e.Status,
        e.IsLocked,
        e.LockedAt,
        e.LockedByUserId,
        COUNT(ser.Id) AS TotalResults,
        SUM(CASE WHEN ser.Status IN (4, 5) THEN 1 ELSE 0 END) AS PublishedResults,
        SUM(CASE WHEN ser.Status = 3 THEN 1 ELSE 0 END) AS ApprovedResults,
        SUM(CASE WHEN ser.Status = 2 THEN 1 ELSE 0 END) AS ReviewedResults,
        SUM(CASE WHEN ser.Status = 1 THEN 1 ELSE 0 END) AS SubmittedResults,
        SUM(CASE WHEN ser.Status = 0 OR ser.Status IS NULL THEN 1 ELSE 0 END) AS DraftResults,
        CASE WHEN e.IsLocked = 1 THEN e.LockedAt ELSE NULL END AS LockedDateTime
FROM Exams e WITH(NOLOCK)
LEFT JOIN StudentExamResults ser WITH(NOLOCK) ON e.Id = ser.ExamId AND ser.IsDeleted = 0
    WHERE e.AcademicYearId = @AcademicYearId AND e.IsDeleted = 0
    GROUP BY e.Id, e.Name, e.Term, e.StartsOn, e.EndsOn, e.Status, e.IsLocked, e.LockedAt, e.LockedByUserId
    ORDER BY e.EndsOn DESC;

    -- Summary stats
    SELECT
        COUNT(DISTINCT e.Id) AS TotalExams,
        SUM(CASE WHEN e.Status = 4 OR e.Status = 5 THEN 1 ELSE 0 END) AS PublishedExams,
        SUM(CASE WHEN e.Status = 3 THEN 1 ELSE 0 END) AS ApprovedExams,
        SUM(CASE WHEN e.Status = 2 THEN 1 ELSE 0 END) AS ReviewedExams,
        SUM(CASE WHEN e.Status = 1 THEN 1 ELSE 0 END) AS SubmittedExams,
        SUM(CASE WHEN e.Status = 0 THEN 1 ELSE 0 END) AS DraftExams,
        COUNT(ser.Id) AS TotalStudentResults,
        SUM(CASE WHEN ser.Status IN (4, 5) THEN 1 ELSE 0 END) AS TotalPublishedResults
FROM Exams e WITH(NOLOCK)
LEFT JOIN StudentExamResults ser WITH(NOLOCK) ON e.Id = ser.ExamId AND ser.IsDeleted = 0
    WHERE e.AcademicYearId = @AcademicYearId AND e.IsDeleted = 0;
END;
GO