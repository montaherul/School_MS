CREATE OR ALTER PROCEDURE sp_GetEmployeeAttendanceAnalytics
    @Date DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TargetDate DATE = COALESCE(@Date, CAST(GETDATE() AS DATE));
    
    -- Department Attendance %
    SELECT 
        DepartmentId = d.Id,
        DepartmentName = d.Name,
        TotalRecords = COUNT(a.Id),
        PresentRecords = SUM(CASE WHEN a.Status = 1 OR a.Status = 3 THEN 1 ELSE 0 END),
        AttendancePercentage = CASE WHEN COUNT(a.Id) > 0 
             THEN CAST(SUM(CASE WHEN a.Status = 1 OR a.Status = 3 THEN 1 ELSE 0 END) AS DECIMAL(18,2)) / COUNT(a.Id) * 100
             ELSE 100.00 
        END
FROM Departments d WITH(NOLOCK)
LEFT JOIN Employees e WITH(NOLOCK) ON e.DepartmentId = d.Id AND e.IsDeleted = 0
LEFT JOIN EmployeeAttendances a WITH(NOLOCK) ON e.Id = a.EmployeeId AND a.IsDeleted = 0 AND CAST(a.AttendanceDate AS DATE) = @TargetDate
    WHERE d.IsDeleted = 0
    GROUP BY d.Id, d.Name;
END;
GO
