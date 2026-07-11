-- ============================================================================
-- Stored Procedure: sp_GetEmployeeDashboard
-- Purpose: Get aggregated employee statistics for HR dashboard
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetEmployeeDashboard
AS
BEGIN
    SET NOCOUNT ON;

    -- Aggregated counts
    SELECT
        COUNT(*) AS TotalEmployees,
        SUM(CASE WHEN e.IsTeachingStaff = 1 THEN 1 ELSE 0 END) AS TeachingStaff,
        SUM(CASE WHEN e.Status = 'Active' THEN 1 ELSE 0 END) AS ActiveEmployees,
        SUM(CASE WHEN e.Status = 'Inactive' THEN 1 ELSE 0 END) AS InactiveEmployees,
        SUM(CASE WHEN e.Status = 'On Leave' THEN 1 ELSE 0 END) AS OnLeaveEmployees,
        SUM(CASE WHEN e.Status = 'Resigned' THEN 1 ELSE 0 END) AS ResignedEmployees,
        SUM(CASE WHEN e.Status = 'Retired' THEN 1 ELSE 0 END) AS RetiredEmployees,
        SUM(CASE WHEN YEAR(e.JoiningDate) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS NewHiresThisYear
    FROM Employees e WITH(NOLOCK)
    WHERE e.IsDeleted = 0;

    -- Department distribution
    SELECT
        COALESCE(d.Name, 'Unknown') AS DepartmentName,
        COUNT(*) AS [Count],
        SUM(CASE WHEN e.IsTeachingStaff = 1 THEN 1 ELSE 0 END) AS TeachingCount,
        SUM(CASE WHEN e.IsTeachingStaff = 0 THEN 1 ELSE 0 END) AS NonTeachingCount
    FROM Employees e WITH(NOLOCK)
    LEFT JOIN Departments d WITH(NOLOCK) ON e.DepartmentId = d.Id AND d.IsDeleted = 0
    WHERE e.IsDeleted = 0
    GROUP BY d.Name
    ORDER BY COUNT(*) DESC;

    -- Status distribution
    SELECT
        e.Status,
        COUNT(*) AS [Count]
    FROM Employees e WITH(NOLOCK)
    WHERE e.IsDeleted = 0
    GROUP BY e.Status;

    -- Birthdays this month
    SELECT
        e.Id,
        e.FullName,
        COALESCE(desig.Name, '') AS Designation,
        e.DateOfBirth,
        e.ProfilePicturePath
    FROM Employees e WITH(NOLOCK)
    LEFT JOIN Designations desig WITH(NOLOCK) ON e.DesignationId = desig.Id AND desig.IsDeleted = 0
    WHERE e.IsDeleted = 0
        AND e.Status = 'Active'
        AND MONTH(e.DateOfBirth) = MONTH(GETDATE())
    ORDER BY DAY(e.DateOfBirth) ASC;

    -- Recent hires
    SELECT TOP 10
        e.Id,
        e.FullName,
        COALESCE(desig.Name, '') AS Designation,
        COALESCE(d.Name, '') AS Department,
        e.JoiningDate,
        e.ProfilePicturePath
    FROM Employees e WITH(NOLOCK)
    LEFT JOIN Departments d WITH(NOLOCK) ON e.DepartmentId = d.Id AND d.IsDeleted = 0
    LEFT JOIN Designations desig WITH(NOLOCK) ON e.DesignationId = desig.Id AND desig.IsDeleted = 0
    WHERE e.IsDeleted = 0 AND e.Status = 'Active'
    ORDER BY e.JoiningDate DESC;
END;
GO
