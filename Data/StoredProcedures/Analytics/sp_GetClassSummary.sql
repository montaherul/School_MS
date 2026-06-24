CREATE OR ALTER PROCEDURE [dbo].[sp_GetClassSummary]
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.ClassId,
        c.Name AS ClassName,
        COUNT(ser.Id) AS TotalStudents,
        SUM(CASE WHEN ser.IsPassed = 1 THEN 1 ELSE 0 END) AS PassedCount,
        SUM(CASE WHEN ser.IsPassed = 0 THEN 1 ELSE 0 END) AS FailedCount,
        ROUND(AVG(ser.Gpa), 2) AS AverageGPA,
        MAX(ser.Gpa) AS HighestGPA,
        MIN(ser.Gpa) AS LowestGPA,
        ROUND(100.0 * SUM(CASE WHEN ser.IsPassed = 1 THEN 1 ELSE 0 END) / NULLIF(COUNT(ser.Id), 0), 1) AS PassPercentage
FROM StudentExamResults ser WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON ser.StudentId = s.Id
INNER JOIN Classes c WITH(NOLOCK) ON s.ClassId = c.Id
    WHERE ser.ExamId = @ExamId AND ser.IsDeleted = 0
    GROUP BY s.ClassId, c.Name
    ORDER BY c.Name;
END;
GO
