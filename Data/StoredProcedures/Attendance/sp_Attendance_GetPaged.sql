CREATE OR ALTER PROCEDURE sp_Attendance_GetPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @SortField NVARCHAR(50) = 'AttendanceDate',
    @SortDirection NVARCHAR(10) = 'DESC',
    @EmployeeId BIGINT = NULL,
    @DepartmentId BIGINT = NULL,
    @Status INT = NULL, -- 0: Absent, 1: Present, etc. (Enums)
    @DateFrom DATETIME = NULL,
    @DateTo DATETIME = NULL,
    @IsLate BIT = NULL,
    @CurrentUserId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Whitelist Sort Fields
    IF @SortField NOT IN ('AttendanceDate', 'FullName', 'EmployeeCode', 'Status', 'CheckInTime', 'CheckOutTime')
    BEGIN
        SET @SortField = 'AttendanceDate';
    END

    -- 2. Base Query
    ;WITH AttendanceData AS (
        SELECT 
            a.Id,
            a.AttendanceDate,
            a.Status,
            a.CheckInTime,
            a.CheckOutTime,
            a.Remarks,
            e.FullName,
            e.EmployeeCode,
            d.Name AS DepartmentName,
            ds.Name AS DesignationName,
            CASE WHEN a.CheckInTime > '09:15:00' THEN 1 ELSE 0 END AS IsLate -- Business rule example
        FROM EmployeeAttendances a
        JOIN Employees e ON a.EmployeeId = e.Id
        JOIN Departments d ON e.DepartmentId = d.Id
        JOIN Designations ds ON e.DesignationId = ds.Id
        WHERE (@EmployeeId IS NULL OR a.EmployeeId = @EmployeeId)
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@Status IS NULL OR a.Status = @Status)
          AND (@DateFrom IS NULL OR a.AttendanceDate >= @DateFrom)
          AND (@DateTo IS NULL OR a.AttendanceDate <= @DateTo)
          AND (@Search IS NULL OR (
                e.FullName LIKE '%' + @Search + '%' OR
                e.EmployeeCode LIKE '%' + @Search + '%'
          ))
          -- Add IsLate filter if needed (requires defining what 'late' means in SQL or column)
          AND (@IsLate IS NULL OR (CASE WHEN a.CheckInTime > '09:15:00' THEN 1 ELSE 0 END) = @IsLate)
    )
    -- 3. Paged Result
    SELECT * FROM (
        SELECT 
            *,
            COUNT(*) OVER() AS TotalCount
        FROM AttendanceData
    ) AS Result
    ORDER BY 
        CASE WHEN @SortField = 'AttendanceDate' AND @SortDirection = 'ASC' THEN AttendanceDate END ASC,
        CASE WHEN @SortField = 'AttendanceDate' AND @SortDirection = 'DESC' THEN AttendanceDate END DESC,
        CASE WHEN @SortField = 'FullName' AND @SortDirection = 'ASC' THEN FullName END ASC,
        CASE WHEN @SortField = 'FullName' AND @SortDirection = 'DESC' THEN FullName END DESC,
        CASE WHEN @SortField = 'Status' AND @SortDirection = 'ASC' THEN Status END ASC,
        CASE WHEN @SortField = 'Status' AND @SortDirection = 'DESC' THEN Status END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
