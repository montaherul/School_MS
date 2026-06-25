CREATE OR ALTER PROCEDURE sp_GetRoutineAnalytics
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalPeriods INT;
    SELECT @TotalPeriods = COUNT(*)
    FROM RoutinePeriods WITH(NOLOCK)
    WHERE IsDeleted = 0 AND IsActive = 1 AND IsBreak = 0;

    -- RS1: Teacher load distribution
    SELECT
        SUM(CASE WHEN EntryCount > @TotalPeriods THEN 1 ELSE 0 END) AS Overloaded,
        SUM(CASE WHEN EntryCount BETWEEN @TotalPeriods * 0.5 AND @TotalPeriods THEN 1 ELSE 0 END) AS Normal,
        SUM(CASE WHEN EntryCount < @TotalPeriods * 0.5 THEN 1 ELSE 0 END) AS Underloaded
    FROM (
        SELECT re.TeacherId, COUNT(re.Id) AS EntryCount
        FROM RoutineEntries re WITH(NOLOCK)
        WHERE re.IsDeleted = 0 AND re.AcademicYearId = @AcademicYearId
        GROUP BY re.TeacherId
    ) AS TeacherLoad;

    -- RS2: Room utilization ranges
    SELECT
        SUM(CASE WHEN UtilizationPercent > 80.0 THEN 1 ELSE 0 END) AS HighUtilization,
        SUM(CASE WHEN UtilizationPercent BETWEEN 50.0 AND 80.0 THEN 1 ELSE 0 END) AS MediumUtilization,
        SUM(CASE WHEN UtilizationPercent < 50.0 THEN 1 ELSE 0 END) AS LowUtilization
    FROM (
        SELECT
            r.Id,
            CASE WHEN @TotalPeriods > 0
                THEN CAST(ROUND(COUNT(re.Id) * 100.0 / @TotalPeriods, 1) AS DECIMAL(5,1))
                ELSE 0
            END AS UtilizationPercent
        FROM Rooms r WITH(NOLOCK)
        LEFT JOIN RoutineEntries re WITH(NOLOCK) ON re.RoomId = r.Id AND re.IsDeleted = 0 AND re.AcademicYearId = @AcademicYearId
        WHERE r.IsDeleted = 0 AND r.IsActive = 1
        GROUP BY r.Id
    ) AS RoomUtil;

    -- RS3: Period utilization (periods with most/least classes)
    SELECT
        rp.Id AS RoutinePeriodId,
        rp.Name AS PeriodName,
        rp.StartTime,
        rp.EndTime,
        COUNT(re.Id) AS ClassCount
    FROM RoutinePeriods rp WITH(NOLOCK)
    LEFT JOIN RoutineEntries re WITH(NOLOCK) ON re.RoutinePeriodId = rp.Id AND re.IsDeleted = 0 AND re.AcademicYearId = @AcademicYearId
    WHERE rp.IsDeleted = 0 AND rp.IsActive = 1 AND rp.IsBreak = 0
    GROUP BY rp.Id, rp.Name, rp.StartTime, rp.EndTime, rp.PeriodNumber
    ORDER BY rp.PeriodNumber;

    -- RS4: Conflict summary by type
    SELECT
        rc.ConflictType,
        COUNT(*) AS ConflictCount,
        SUM(CASE WHEN rc.IsResolved = 1 THEN 1 ELSE 0 END) AS ResolvedCount,
        SUM(CASE WHEN rc.IsResolved = 0 THEN 1 ELSE 0 END) AS UnresolvedCount
    FROM RoutineConflicts rc WITH(NOLOCK)
    INNER JOIN RoutineGenerations rg WITH(NOLOCK) ON rg.Id = rc.GenerationId AND rg.AcademicYearId = @AcademicYearId
    WHERE rc.IsDeleted = 0
    GROUP BY rc.ConflictType
    ORDER BY COUNT(*) DESC;
END;
GO
