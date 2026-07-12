CREATE OR ALTER PROCEDURE sp_GetTeacherLoadSummary
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalPeriods INT;
    SELECT @TotalPeriods = COUNT(*)
    FROM RoutinePeriods WITH(NOLOCK)
    WHERE IsDeleted = 0 AND IsActive = 1 AND IsBreak = 0;

    WITH TeacherDayStats AS (
        SELECT
            re.TeacherId,
            re.DayNumber,
            COUNT(re.Id) AS PeriodsThisDay
        FROM RoutineEntries re WITH(NOLOCK)
        WHERE re.IsDeleted = 0 AND re.AcademicYearId = @AcademicYearId
        GROUP BY re.TeacherId, re.DayNumber
    )
    SELECT
        t.Id AS TeacherId,
        e.FullName AS TeacherName,
        ISNULL(COUNT(re.Id), 0) AS TotalPeriodsPerWeek,
        ISNULL(COUNT(DISTINCT re.ClassId), 0) AS ClassesCount,
        ISNULL(COUNT(DISTINCT re.SubjectId), 0) AS SubjectsCount,
        CASE WHEN @TotalPeriods > 0
            THEN CAST(ROUND(ISNULL(COUNT(re.Id), 0) * 100.0 / @TotalPeriods, 1) AS DECIMAL(5,1))
            ELSE 0
        END AS UtilizationPercent,
        ISNULL(MAX(tds.PeriodsThisDay), 0) AS MaxPeriodsPerDay,
        ISNULL(COUNT(DISTINCT tds.DayNumber), 0) AS WorkingDays,
        CASE WHEN COUNT(DISTINCT tds.DayNumber) > 0
            THEN CAST(ROUND(ISNULL(COUNT(re.Id), 0) * 1.0 / COUNT(DISTINCT tds.DayNumber), 1) AS DECIMAL(5,1))
            ELSE 0
        END AS AveragePerDay,
        ISNULL((
            SELECT DayNumber AS [Key], PeriodsThisDay AS [Value]
            FROM TeacherDayStats tds2
            WHERE tds2.TeacherId = t.Id
            ORDER BY tds2.DayNumber
            FOR JSON PATH
        ), '[]') AS PeriodsByDay
    FROM Teachers t WITH(NOLOCK)
    INNER JOIN Employees e WITH(NOLOCK) ON t.EmployeeId = e.Id AND e.IsDeleted = 0
    LEFT JOIN RoutineEntries re WITH(NOLOCK) ON re.TeacherId = t.Id AND re.IsDeleted = 0 AND re.AcademicYearId = @AcademicYearId
    LEFT JOIN TeacherDayStats tds ON tds.TeacherId = t.Id
    WHERE t.IsDeleted = 0
    GROUP BY t.Id, e.FullName
    ORDER BY COUNT(re.Id) DESC;
END;
GO
