CREATE OR ALTER PROCEDURE sp_GetRoutineEntriesPaged
    @AcademicYearId INT,
    @PageNumber INT = 1,
    @PageSize INT = 50,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @ClassId INT = NULL,
    @SectionId INT = NULL,
    @GroupId INT = NULL,
    @TeacherId INT = NULL,
    @RoomId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        re.Id,
        re.AcademicYearId,
        ay.Name AS AcademicYearName,
        re.ClassId,
        c.Name AS ClassName,
        re.SectionId,
        s.Name AS SectionName,
        re.GroupId,
        sg.Name AS GroupName,
        re.SubjectId,
        sub.Name AS SubjectName,
        re.TeacherId,
        e.FullName AS TeacherName,
        re.RoomId,
        r.RoomNo,
        re.RoutinePeriodId,
        rp.Name AS PeriodName,
        re.DayNumber,
        CASE re.DayNumber
            WHEN 0 THEN 'Sunday'
            WHEN 1 THEN 'Monday'
            WHEN 2 THEN 'Tuesday'
            WHEN 3 THEN 'Wednesday'
            WHEN 4 THEN 'Thursday'
            WHEN 5 THEN 'Friday'
            WHEN 6 THEN 'Saturday'
            ELSE 'Unknown'
        END AS DayName,
        re.IsLab,
        re.Note,

        COUNT(*) OVER () AS TotalRecords
    FROM RoutineEntries re WITH(NOLOCK)
    INNER JOIN AcademicYears ay WITH(NOLOCK) ON re.AcademicYearId = ay.Id AND ay.IsDeleted = 0
    INNER JOIN SchoolClasses c WITH(NOLOCK) ON re.ClassId = c.Id AND c.IsDeleted = 0
    INNER JOIN Subjects sub WITH(NOLOCK) ON re.SubjectId = sub.Id AND sub.IsDeleted = 0
    INNER JOIN Teachers t WITH(NOLOCK) ON re.TeacherId = t.Id AND t.IsDeleted = 0
    INNER JOIN Employees e WITH(NOLOCK) ON t.EmployeeId = e.Id AND e.IsDeleted = 0
    INNER JOIN Rooms r WITH(NOLOCK) ON re.RoomId = r.Id AND r.IsDeleted = 0
    INNER JOIN RoutinePeriods rp WITH(NOLOCK) ON re.RoutinePeriodId = rp.Id AND rp.IsDeleted = 0
    LEFT JOIN Sections s WITH(NOLOCK) ON re.SectionId = s.Id AND s.IsDeleted = 0
    LEFT JOIN StudentGroups sg WITH(NOLOCK) ON re.GroupId = sg.Id AND sg.IsDeleted = 0
    WHERE re.IsDeleted = 0
      AND re.AcademicYearId = @AcademicYearId
      AND (@ClassId IS NULL OR re.ClassId = @ClassId)
      AND (@SectionId IS NULL OR re.SectionId = @SectionId)
      AND (@GroupId IS NULL OR re.GroupId = @GroupId)
      AND (@TeacherId IS NULL OR re.TeacherId = @TeacherId)
      AND (@RoomId IS NULL OR re.RoomId = @RoomId)
      AND (@SearchTerm IS NULL OR sub.Name LIKE '%' + @SearchTerm + '%' OR e.FullName LIKE '%' + @SearchTerm + '%' OR r.RoomNo LIKE '%' + @SearchTerm + '%' OR c.Name LIKE '%' + @SearchTerm + '%')
    ORDER BY re.DayNumber, rp.PeriodNumber, c.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
