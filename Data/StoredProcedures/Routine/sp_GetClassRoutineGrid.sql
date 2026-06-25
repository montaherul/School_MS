CREATE OR ALTER PROCEDURE sp_GetClassRoutineGrid
    @AcademicYearId INT,
    @ClassId INT,
    @SectionId INT = NULL,
    @GroupId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        re.DayNumber,
        re.RoutinePeriodId,
        rp.Name AS PeriodName,
        rp.StartTime,
        rp.EndTime,
        sub.Name AS SubjectName,
        e.FullName AS TeacherName,
        rm.RoomNo
    FROM RoutineEntries re WITH(NOLOCK)
    INNER JOIN RoutinePeriods rp WITH(NOLOCK) ON re.RoutinePeriodId = rp.Id AND rp.IsDeleted = 0
    INNER JOIN Subjects sub WITH(NOLOCK) ON re.SubjectId = sub.Id AND sub.IsDeleted = 0
    INNER JOIN Teachers t WITH(NOLOCK) ON re.TeacherId = t.Id AND t.IsDeleted = 0
    INNER JOIN Employees e WITH(NOLOCK) ON t.EmployeeId = e.Id AND e.IsDeleted = 0
    INNER JOIN Rooms rm WITH(NOLOCK) ON re.RoomId = rm.Id AND rm.IsDeleted = 0
    WHERE re.IsDeleted = 0
      AND re.AcademicYearId = @AcademicYearId
      AND re.ClassId = @ClassId
      AND (@SectionId IS NULL OR re.SectionId = @SectionId)
      AND (@GroupId IS NULL OR re.GroupId = @GroupId)
    ORDER BY re.DayNumber, rp.PeriodNumber;
END;
GO
