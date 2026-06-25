CREATE OR ALTER PROCEDURE sp_GetTeacherRoutineGrid
    @AcademicYearId INT,
    @TeacherId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        re.DayNumber,
        re.RoutinePeriodId,
        rp.Name AS PeriodName,
        rp.StartTime,
        rp.EndTime,
        c.Name AS ClassName,
        s.Name AS SectionName,
        sub.Name AS SubjectName,
        rm.RoomNo
    FROM RoutineEntries re WITH(NOLOCK)
    INNER JOIN RoutinePeriods rp WITH(NOLOCK) ON re.RoutinePeriodId = rp.Id AND rp.IsDeleted = 0
    INNER JOIN SchoolClasses c WITH(NOLOCK) ON re.ClassId = c.Id AND c.IsDeleted = 0
    INNER JOIN Subjects sub WITH(NOLOCK) ON re.SubjectId = sub.Id AND sub.IsDeleted = 0
    INNER JOIN Rooms rm WITH(NOLOCK) ON re.RoomId = rm.Id AND rm.IsDeleted = 0
    LEFT JOIN Sections s WITH(NOLOCK) ON re.SectionId = s.Id AND s.IsDeleted = 0
    WHERE re.IsDeleted = 0
      AND re.AcademicYearId = @AcademicYearId
      AND re.TeacherId = @TeacherId
    ORDER BY re.DayNumber, rp.PeriodNumber;
END;
GO
