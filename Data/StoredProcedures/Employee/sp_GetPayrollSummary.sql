-- ============================================================================
-- Stored Procedure: sp_GetPayrollSummary
-- Purpose: Get employee payroll/salary history
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetPayrollSummary
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.Id, s.EmployeeId, s.BasicSalary, s.HouseRent,
        s.MedicalAllowance, s.TransportAllowance, s.OtherAllowance,
        s.Deduction, s.TotalSalary, s.EffectiveFrom,
        e.FullName AS EmployeeName
    FROM EmployeeSalaries s WITH(NOLOCK)
    INNER JOIN Employees e WITH(NOLOCK) ON s.EmployeeId = e.Id AND e.IsDeleted = 0
    WHERE s.EmployeeId = @EmployeeId AND s.IsDeleted = 0
    ORDER BY s.EffectiveFrom DESC;

    -- Payroll summary stats
    SELECT
        COUNT(*) AS TotalSalaryRecords,
        ISNULL(MAX(s.TotalSalary), 0) AS CurrentSalary,
        ISNULL(AVG(s.TotalSalary), 0) AS AvgSalary,
        ISNULL(MIN(s.TotalSalary), 0) AS MinSalary,
        ISNULL(MAX(s.EffectiveFrom), '1900-01-01') AS LastUpdated
    FROM EmployeeSalaries s WITH(NOLOCK)
    WHERE s.EmployeeId = @EmployeeId AND s.IsDeleted = 0;
END;
GO
