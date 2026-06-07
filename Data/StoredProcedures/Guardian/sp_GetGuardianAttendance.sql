CREATE OR ALTER PROCEDURE [dbo].[sp_GetGuardianAttendance]
    @GuardianId INT,
    @StudentId INT,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Verify access
    IF NOT EXISTS (SELECT 1 FROM StudentGuardians WHERE GuardianId = @GuardianId AND StudentId = @StudentId AND IsDeleted = 0)
    BEGIN
        SELECT 0 AS TotalDays, 0 AS PresentCount, 0 AS AbsentCount, 0 AS LateCount, 0 AS LeaveCount, CAST(0 AS DECIMAL(10,2)) AS AttendancePercentage;
        RETURN;
    END

    DECLARE @TotalDays INT, @PresentCount INT, @AbsentCount INT, @LateCount INT, @LeaveCount INT;

    SELECT 
        @TotalDays = COUNT(*),
        @PresentCount = SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END),
        @AbsentCount = SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END),
        @LateCount = SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END),
        @LeaveCount = SUM(CASE WHEN Status = 4 THEN 1 ELSE 0 END)
    FROM Attendance a
    WHERE a.StudentId = @StudentId AND a.IsDeleted = 0
      AND (@FromDate IS NULL OR a.AttendanceDate >= @FromDate)
      AND (@ToDate IS NULL OR a.AttendanceDate <= @ToDate);

    SELECT 
        ISNULL(@TotalDays, 0) AS TotalDays,
        ISNULL(@PresentCount, 0) AS PresentCount,
        ISNULL(@AbsentCount, 0) AS AbsentCount,
        ISNULL(@LateCount, 0) AS LateCount,
        ISNULL(@LeaveCount, 0) AS LeaveCount,
        CASE WHEN ISNULL(@TotalDays, 0) = 0 THEN CAST(0 AS DECIMAL(10,2))
             ELSE CAST(ROUND((CAST(ISNULL(@PresentCount, 0) + ISNULL(@LateCount, 0) AS FLOAT) / @TotalDays) * 100, 2) AS DECIMAL(10,2))
        END AS AttendancePercentage;

    -- Per-day records
    SELECT 
        a.Id,
        a.StudentId,
        a.AttendanceDate,
        CASE a.Status WHEN 1 THEN 'Present' WHEN 2 THEN 'Absent' WHEN 3 THEN 'Late' WHEN 4 THEN 'Leave' ELSE 'Unknown' END AS StatusName,
        a.Status AS StatusId,
        a.Remarks,
        c.Name AS ClassName,
        sec.Name AS SectionName
    FROM Attendance a
    LEFT JOIN Classes c ON a.SchoolClassId = c.Id
    LEFT JOIN Sections sec ON a.SectionId = sec.Id
    WHERE a.StudentId = @StudentId AND a.IsDeleted = 0
      AND (@FromDate IS NULL OR a.AttendanceDate >= @FromDate)
      AND (@ToDate IS NULL OR a.AttendanceDate <= @ToDate)
    ORDER BY a.AttendanceDate DESC;
END
GO
