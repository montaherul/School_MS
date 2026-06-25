CREATE OR ALTER PROCEDURE sp_GetRoomScheduleGrid
    @RoomId INT,
    @DayNumber INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        re.RoutinePeriodId,
        rp.Name AS PeriodName,
        rp.StartTime,
        rp.EndTime,
        c.Name AS ClassName,
        sub.Name AS SubjectName,
        e.FullName AS TeacherName
    FROM RoutineEntries re WITH(NOLOCK)
    INNER JOIN RoutinePeriods rp WITH(NOLOCK) ON re.RoutinePeriodId = rp.Id AND rp.IsDeleted = 0
    INNER JOIN SchoolClasses c WITH(NOLOCK) ON re.ClassId = c.Id AND c.IsDeleted = 0
    INNER JOIN Subjects sub WITH(NOLOCK) ON re.SubjectId = sub.Id AND sub.IsDeleted = 0
    INNER JOIN Teachers t WITH(NOLOCK) ON re.TeacherId = t.Id AND t.IsDeleted = 0
    INNER JOIN Employees e WITH(NOLOCK) ON t.EmployeeId = e.Id AND e.IsDeleted = 0
    WHERE re.IsDeleted = 0
      AND re.RoomId = @RoomId
      AND (@DayNumber IS NULL OR re.DayNumber = @DayNumber)
    ORDER BY re.DayNumber, rp.PeriodNumber;
END;
GO
