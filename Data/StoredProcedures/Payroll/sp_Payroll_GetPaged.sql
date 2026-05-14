CREATE OR ALTER PROCEDURE sp_Payroll_GetPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @SortField NVARCHAR(50) = 'PayrollYear',
    @SortDirection NVARCHAR(10) = 'DESC',
    @DepartmentId BIGINT = NULL,
    @Status INT = NULL, -- PaymentStatus enum
    @Month INT = NULL,
    @Year INT = NULL,
    @CurrentUserId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Whitelist Sort Fields
    IF @SortField NOT IN ('PayrollMonth', 'PayrollYear', 'FullName', 'EmployeeCode', 'NetSalary', 'PaymentStatus')
    BEGIN
        SET @SortField = 'PayrollYear';
    END

    -- 2. Base Query
    ;WITH PayrollData AS (
        SELECT 
            p.Id,
            p.PayrollMonth,
            p.PayrollYear,
            p.WorkingDays,
            p.PresentDays,
            p.GrossSalary,
            p.NetSalary,
            p.PaymentStatus,
            p.PaymentDate,
            e.FullName,
            e.EmployeeCode,
            d.Name AS DepartmentName,
            ds.Name AS DesignationName
        FROM EmployeePayrolls p
        JOIN Employees e ON p.EmployeeId = e.Id
        JOIN Departments d ON e.DepartmentId = d.Id
        JOIN Designations ds ON e.DesignationId = ds.Id
        WHERE (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@Status IS NULL OR p.PaymentStatus = @Status)
          AND (@Month IS NULL OR p.PayrollMonth = @Month)
          AND (@Year IS NULL OR p.PayrollYear = @Year)
          AND (@Search IS NULL OR (
                e.FullName LIKE '%' + @Search + '%' OR
                e.EmployeeCode LIKE '%' + @Search + '%'
          ))
    )
    -- 3. Paged Result
    SELECT * FROM (
        SELECT 
            *,
            COUNT(*) OVER() AS TotalCount
        FROM PayrollData
    ) AS Result
    ORDER BY 
        CASE WHEN @SortField = 'PayrollYear' AND @SortDirection = 'ASC' THEN PayrollYear END ASC,
        CASE WHEN @SortField = 'PayrollYear' AND @SortDirection = 'DESC' THEN PayrollYear END DESC,
        CASE WHEN @SortField = 'PayrollMonth' AND @SortDirection = 'ASC' THEN PayrollMonth END ASC,
        CASE WHEN @SortField = 'PayrollMonth' AND @SortDirection = 'DESC' THEN PayrollMonth END DESC,
        CASE WHEN @SortField = 'FullName' AND @SortDirection = 'ASC' THEN FullName END ASC,
        CASE WHEN @SortField = 'FullName' AND @SortDirection = 'DESC' THEN FullName END DESC,
        CASE WHEN @SortField = 'NetSalary' AND @SortDirection = 'ASC' THEN NetSalary END ASC,
        CASE WHEN @SortField = 'NetSalary' AND @SortDirection = 'DESC' THEN NetSalary END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
