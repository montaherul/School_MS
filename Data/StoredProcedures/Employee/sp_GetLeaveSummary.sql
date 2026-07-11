-- ============================================================================
-- Stored Procedure: sp_GetLeaveSummary
-- Purpose: Get employee leave summary for a given year
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetLeaveSummary
    @EmployeeId INT,
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Year IS NULL SET @Year = YEAR(GETDATE());

    SELECT
        COUNT(*) AS TotalLeaves,
        ISNULL(SUM(l.TotalDays), 0) AS TotalLeaveDays,
        ISNULL(AVG(l.TotalDays * 1.0), 0) AS AvgLeaveDays
    FROM LeaveApplications l WITH(NOLOCK)
    WHERE l.EmployeeId = @EmployeeId
        AND YEAR(l.FromDate) = @Year;

    -- Leave by type
    SELECT
        l.LeaveType,
        COUNT(*) AS [Count],
        ISNULL(SUM(l.TotalDays), 0) AS TotalDays
    FROM LeaveApplications l WITH(NOLOCK)
    WHERE l.EmployeeId = @EmployeeId
        AND YEAR(l.FromDate) = @Year
    GROUP BY l.LeaveType;
END;
GO
