CREATE OR ALTER PROCEDURE sp_GetAttendanceAnalytics
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Start DATE = COALESCE(@StartDate, DATEADD(day, -6, CAST(GETDATE() AS DATE)));
    DECLARE @End DATE = COALESCE(@EndDate, CAST(GETDATE() AS DATE));
    
    -- Daily Trend (last 7 days by default)
    SELECT 
        AttendanceDate,
        TotalStudents = COUNT(Id),
        PresentStudents = SUM(CASE WHEN Status = 1 OR Status = 3 THEN 1 ELSE 0 END),
        AttendancePercentage = CASE WHEN COUNT(Id) > 0 
             THEN CAST(SUM(CASE WHEN Status = 1 OR Status = 3 THEN 1 ELSE 0 END) AS DECIMAL(5,2)) / COUNT(Id) * 100
             ELSE 0.00
        END
    FROM Attendance
    WHERE AttendanceDate >= @Start AND AttendanceDate <= @End AND IsDeleted = 0
    GROUP BY AttendanceDate
    ORDER BY AttendanceDate ASC;
    
    -- Monthly Trend (last 6 months by default)
    DECLARE @SixMonthsAgo DATE = DATEADD(month, -5, CAST(GETDATE() AS DATE));
    SELECT 
        [Year] = YEAR(AttendanceDate),
        [Month] = MONTH(AttendanceDate),
        TotalStudents = COUNT(Id),
        PresentStudents = SUM(CASE WHEN Status = 1 OR Status = 3 THEN 1 ELSE 0 END),
        AttendancePercentage = CASE WHEN COUNT(Id) > 0 
             THEN CAST(SUM(CASE WHEN Status = 1 OR Status = 3 THEN 1 ELSE 0 END) AS DECIMAL(5,2)) / COUNT(Id) * 100
             ELSE 0.00
        END
    FROM Attendance
    WHERE AttendanceDate >= @SixMonthsAgo AND IsDeleted = 0
    GROUP BY YEAR(AttendanceDate), MONTH(AttendanceDate)
    ORDER BY YEAR(AttendanceDate) ASC, MONTH(AttendanceDate) ASC;
END;
GO
