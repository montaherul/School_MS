CREATE OR ALTER PROCEDURE [dbo].[sp_GetStudentTrend]
    @StudentId INT,
    @AcademicYearId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.Id AS ExamId,
        e.Name AS ExamName,
        e.Term,
        e.StartsOn,
        e.EndsOn,
        e.AcademicYearId,
        ay.Name AS AcademicYearName,
        ser.TotalMarks,
        ser.TotalFullMarks,
        ser.Gpa,
        ser.Grade,
        ser.ClassPosition,
        ser.GroupPosition,
        CAST(ser.IsPassed AS BIT) AS IsPassed,
        ser.PassedSubjectCount,
        ser.FailedSubjectCount
FROM StudentExamResults ser WITH(NOLOCK)
INNER JOIN Exams e WITH(NOLOCK) ON ser.ExamId = e.Id
LEFT JOIN AcademicYears ay WITH(NOLOCK) ON e.AcademicYearId = ay.Id
    WHERE ser.StudentId = @StudentId
      AND ser.IsDeleted = 0
      AND e.IsDeleted = 0
      AND (@AcademicYearId IS NULL OR e.AcademicYearId = @AcademicYearId)
    ORDER BY e.EndsOn ASC;
END;
GO
