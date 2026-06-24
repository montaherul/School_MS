CREATE OR ALTER PROCEDURE sp_GetClassAttendanceAnalytics
    @Date DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TargetDate DATE = COALESCE(@Date, CAST(GETDATE() AS DATE));
    
    -- Class Attendance %
    SELECT 
        ClassId = c.Id,
        ClassName = c.Name,
        TotalRecords = COUNT(a.Id),
        PresentRecords = SUM(CASE WHEN a.Status = 1 OR a.Status = 3 THEN 1 ELSE 0 END),
        AttendancePercentage = CASE WHEN COUNT(a.Id) > 0 
             THEN CAST(SUM(CASE WHEN a.Status = 1 OR a.Status = 3 THEN 1 ELSE 0 END) AS DECIMAL(18,2)) / COUNT(a.Id) * 100
             ELSE 100.00 
        END
FROM Classes c WITH(NOLOCK)
LEFT JOIN Attendance a WITH(NOLOCK) ON c.Id = a.SchoolClassId AND a.IsDeleted = 0 AND a.AttendanceDate = @TargetDate
    WHERE c.IsDeleted = 0
    GROUP BY c.Id, c.Name;

    -- Group Attendance %
    SELECT 
        GroupId = g.Id,
        GroupName = g.Name,
        TotalRecords = COUNT(a.Id),
        PresentRecords = SUM(CASE WHEN a.Status = 1 OR a.Status = 3 THEN 1 ELSE 0 END),
        AttendancePercentage = CASE WHEN COUNT(a.Id) > 0 
             THEN CAST(SUM(CASE WHEN a.Status = 1 OR a.Status = 3 THEN 1 ELSE 0 END) AS DECIMAL(18,2)) / COUNT(a.Id) * 100
             ELSE 100.00 
        END
FROM StudentGroups g WITH(NOLOCK)
LEFT JOIN Students s WITH(NOLOCK) ON s.StudentGroupId = g.Id AND s.IsDeleted = 0
LEFT JOIN Attendance a WITH(NOLOCK) ON s.Id = a.StudentId AND a.IsDeleted = 0 AND a.AttendanceDate = @TargetDate
    WHERE g.IsDeleted = 0 AND g.IsActive = 1
    GROUP BY g.Id, g.Name;
END;
GO
