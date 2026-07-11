-- ============================================================================
-- Stored Procedure: sp_GetAttendanceSummary
-- Purpose: Get employee attendance summary for a given year
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetAttendanceSummary
    @EmployeeId INT,
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Year IS NULL SET @Year = YEAR(GETDATE());

    SELECT
        COUNT(*) AS TotalRecords,
        SUM(CASE WHEN a.Status = 0 THEN 1 ELSE 0 END) AS Present,
        SUM(CASE WHEN a.Status = 1 THEN 1 ELSE 0 END) AS Absent,
        SUM(CASE WHEN a.Status = 2 THEN 1 ELSE 0 END) AS Leave,
        SUM(CASE WHEN a.Status = 3 THEN 1 ELSE 0 END) AS Late
    FROM EmployeeAttendance a WITH(NOLOCK)
    WHERE a.EmployeeId = @EmployeeId
        AND YEAR(a.AttendanceDate) = @Year
        AND a.IsDeleted = 0;

    -- Monthly breakdown
    SELECT
        MONTH(a.AttendanceDate) AS [Month],
        COUNT(*) AS TotalRecords,
        SUM(CASE WHEN a.Status = 0 THEN 1 ELSE 0 END) AS Present,
        SUM(CASE WHEN a.Status = 1 THEN 1 ELSE 0 END) AS Absent,
        SUM(CASE WHEN a.Status = 2 THEN 1 ELSE 0 END) AS Leave,
        SUM(CASE WHEN a.Status = 3 THEN 1 ELSE 0 END) AS Late
    FROM EmployeeAttendance a WITH(NOLOCK)
    WHERE a.EmployeeId = @EmployeeId
        AND YEAR(a.AttendanceDate) = @Year
        AND a.IsDeleted = 0
    GROUP BY MONTH(a.AttendanceDate)
    ORDER BY [Month];
END;
GO
