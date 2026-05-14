CREATE OR ALTER PROCEDURE sp_Employee_GetPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @SortField NVARCHAR(50) = 'CreatedAt',
    @SortDirection NVARCHAR(10) = 'DESC',
    @DepartmentId INT = NULL,
    @DesignationId INT = NULL,
    @Status INT = NULL,
    @CurrentUserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Whitelist Sort Fields for Safety
    IF @SortField NOT IN ('EmployeeCode', 'FullName', 'Email', 'JoiningDate', 'Id', 'Salary')
    BEGIN
        SET @SortField = 'Id';
    END

    -- 2. Base Query with Filtering
    ;WITH EmployeeData AS (
        SELECT 
            e.Id,
            e.EmployeeCode,
            e.FullName,
            e.Email,
            e.Phone,
            e.JoiningDate,
            e.IsActive AS Status,
            d.Name AS DepartmentName,
            ds.Name AS DesignationName
        FROM Employees e
        LEFT JOIN Departments d ON e.DepartmentId = d.Id
        LEFT JOIN Designations ds ON e.DesignationId = ds.Id
        WHERE 1=1
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@DesignationId IS NULL OR e.DesignationId = @DesignationId)
          AND (@Status IS NULL OR e.IsActive = @Status)
          AND (@Search IS NULL OR (
                e.EmployeeCode LIKE '%' + @Search + '%' OR
                e.FullName LIKE '%' + @Search + '%' OR
                e.Email LIKE '%' + @Search + '%' OR
                e.Phone LIKE '%' + @Search + '%'
          ))
    )
    -- 3. Final Paged Result
    SELECT * FROM (
        SELECT 
            *,
            COUNT(*) OVER() AS TotalCount
        FROM EmployeeData
    ) AS Result
    ORDER BY 
        CASE WHEN @SortField = 'EmployeeCode' AND @SortDirection = 'ASC' THEN EmployeeCode END ASC,
        CASE WHEN @SortField = 'EmployeeCode' AND @SortDirection = 'DESC' THEN EmployeeCode END DESC,
        CASE WHEN @SortField = 'FullName' AND @SortDirection = 'ASC' THEN FullName END ASC,
        CASE WHEN @SortField = 'FullName' AND @SortDirection = 'DESC' THEN FullName END DESC,
        CASE WHEN @SortField = 'JoiningDate' AND @SortDirection = 'ASC' THEN JoiningDate END ASC,
        CASE WHEN @SortField = 'JoiningDate' AND @SortDirection = 'DESC' THEN JoiningDate END DESC,
        CASE WHEN @SortField = 'Id' AND @SortDirection = 'ASC' THEN Id END ASC,
        CASE WHEN @SortField = 'Id' AND @SortDirection = 'DESC' THEN Id END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
