CREATE OR ALTER PROCEDURE sp_GetRoomUtilization
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalPeriods INT;
    SELECT @TotalPeriods = COUNT(*)
    FROM RoutinePeriods WITH(NOLOCK)
    WHERE IsDeleted = 0 AND IsActive = 1 AND IsBreak = 0;

    SELECT
        r.Id AS RoomId,
        r.RoomNo,
        r.Building,
        r.Capacity,
        COUNT(re.Id) AS TotalPeriodsPerWeek,
        COUNT(DISTINCT re.DayNumber) * COUNT(DISTINCT re.RoutinePeriodId) AS UsedPeriods,
        CASE WHEN @TotalPeriods > 0
            THEN CAST(ROUND(COUNT(re.Id) * 100.0 / @TotalPeriods, 1) AS DECIMAL(5,1))
            ELSE 0
        END AS UtilizationPercent
    FROM Rooms r WITH(NOLOCK)
    LEFT JOIN RoutineEntries re WITH(NOLOCK) ON re.RoomId = r.Id AND re.IsDeleted = 0 AND re.AcademicYearId = @AcademicYearId
    WHERE r.IsDeleted = 0 AND r.IsActive = 1
    GROUP BY r.Id, r.RoomNo, r.Building, r.Capacity
    ORDER BY COUNT(re.Id) DESC;
END;
GO
