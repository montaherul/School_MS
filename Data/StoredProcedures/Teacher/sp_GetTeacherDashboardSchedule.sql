CREATE OR ALTER PROCEDURE sp_GetTeacherDashboardSchedule
    @TeacherId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.Name AS SubjectName,
        c.Name AS ClassName,
        sec.Name AS SectionName,
        tt.DayOfWeek,
        tt.StartTime,
        tt.EndTime,
        tt.RoomNo
FROM TeacherTimetables tt WITH(NOLOCK)
INNER JOIN Subjects s WITH(NOLOCK) ON s.Id = tt.SubjectId AND s.IsDeleted = 0
INNER JOIN SchoolClasses c WITH(NOLOCK) ON c.Id = tt.ClassId AND c.IsDeleted = 0
INNER JOIN Sections sec WITH(NOLOCK) ON sec.Id = tt.SectionId AND sec.IsDeleted = 0
    WHERE tt.TeacherId = @TeacherId
      AND tt.IsDeleted = 0
    ORDER BY
        CASE tt.DayOfWeek
            WHEN 'Sunday' THEN 1 WHEN 'Monday' THEN 2 WHEN 'Tuesday' THEN 3
            WHEN 'Wednesday' THEN 4 WHEN 'Thursday' THEN 5 WHEN 'Friday' THEN 6
            WHEN 'Saturday' THEN 7 ELSE 8
        END,
        tt.StartTime;
END;
