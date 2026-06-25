CREATE OR ALTER PROCEDURE sp_GetTeacherLoadSummary
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalPeriods INT;
    SELECT @TotalPeriods = COUNT(*)
    FROM RoutinePeriods WITH(NOLOCK)
    WHERE IsDeleted = 0 AND IsActive = 1 AND IsBreak = 0;

    SELECT
        t.Id AS TeacherId,
        e.FullName AS TeacherName,
        COUNT(re.Id) AS TotalPeriodsPerWeek,
        COUNT(DISTINCT re.ClassId) AS ClassesCount,
        COUNT(DISTINCT re.SubjectId) AS SubjectsCount,
        CASE WHEN @TotalPeriods > 0
            THEN CAST(ROUND(COUNT(re.Id) * 100.0 / @TotalPeriods, 1) AS DECIMAL(5,1))
            ELSE 0
        END AS UtilizationPercent
    FROM Teachers t WITH(NOLOCK)
    INNER JOIN Employees e WITH(NOLOCK) ON t.EmployeeId = e.Id AND e.IsDeleted = 0
    LEFT JOIN RoutineEntries re WITH(NOLOCK) ON re.TeacherId = t.Id AND re.IsDeleted = 0 AND re.AcademicYearId = @AcademicYearId
    WHERE t.IsDeleted = 0
    GROUP BY t.Id, e.FullName
    ORDER BY COUNT(re.Id) DESC;
END;
GO
