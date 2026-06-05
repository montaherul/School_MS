CREATE OR ALTER PROCEDURE sp_GetEmployeeAttendanceList
    @PageNumber     INT            = 1,
    @PageSize       INT            = 10,
    @SearchTerm     NVARCHAR(MAX)  = NULL,
    @DepartmentId   INT            = 0,
    @DesignationId  INT            = 0,
    @EmployeeType   NVARCHAR(50)   = NULL,
    @IsTeachingStaff BIT           = NULL,
    @AttendanceDate DATE           = NULL,
    @Status         INT            = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    DECLARE @TargetDate DATE = COALESCE(@AttendanceDate, CAST(GETDATE() AS DATE));
    DECLARE @SchoolStartTime TIME = '08:00:00';
    
    SELECT TOP 1 @SchoolStartTime = SchoolStartTime FROM AttendanceSettings;

    WITH FilteredEmployees AS (
        SELECT
            e.Id AS EmployeeId,
            e.EmployeeCode,
            e.FullName AS EmployeeName,
            e.DepartmentId,
            ISNULL(dept.Name, '') AS DepartmentName,
            e.DesignationId,
            ISNULL(desg.Name, '') AS DesignationName,
            e.EmployeeType,
            e.IsTeachingStaff
        FROM
            Employees e
            LEFT JOIN Departments dept ON e.DepartmentId = dept.Id
            LEFT JOIN Designations desg ON e.DesignationId = desg.Id
        WHERE
            e.IsDeleted = 0
            AND e.Status = 'Active'
            AND (@DepartmentId = 0 OR e.DepartmentId = @DepartmentId)
            AND (@DesignationId = 0 OR e.DesignationId = @DesignationId)
            AND (@EmployeeType IS NULL OR @EmployeeType = '' OR e.EmployeeType = @EmployeeType)
            AND (@IsTeachingStaff IS NULL OR e.IsTeachingStaff = @IsTeachingStaff)
            AND (
                @SearchTerm IS NULL
                OR e.FullName LIKE '%' + @SearchTerm + '%'
                OR e.EmployeeCode LIKE '%' + @SearchTerm + '%'
            )
    ),
    AttendanceWithLate AS (
        SELECT
            fe.EmployeeId,
            fe.EmployeeCode,
            fe.EmployeeName,
            fe.DepartmentId,
            fe.DepartmentName,
            fe.DesignationId,
            fe.DesignationName,
            fe.EmployeeType,
            fe.IsTeachingStaff,
            a.Id AS AttendanceId,
            COALESCE(a.AttendanceDate, @TargetDate) AS AttendanceDate,
            a.CheckInTime,
            a.CheckOutTime,
            COALESCE(a.Status, 1) AS Status,
            a.Remarks,
            CASE 
                WHEN a.CheckInTime IS NOT NULL AND a.CheckInTime > @SchoolStartTime 
                THEN DATEDIFF(minute, CAST(@SchoolStartTime AS DATETIME), CAST(a.CheckInTime AS DATETIME))
                ELSE 0
            END AS LateMinutes
        FROM
            FilteredEmployees fe
            LEFT JOIN EmployeeAttendances a ON a.EmployeeId = fe.EmployeeId
                AND a.IsDeleted = 0
                AND CAST(a.AttendanceDate AS DATE) = @TargetDate
        WHERE
            (@Status = 0 OR ISNULL(a.Status, 1) = @Status)
    ),
    FinalCount AS (
        SELECT COUNT(*) AS TotalCount FROM AttendanceWithLate
    )
    SELECT
        Id = ISNULL(awl.AttendanceId, 0),
        awl.EmployeeId,
        awl.EmployeeCode,
        awl.EmployeeName,
        awl.DepartmentId,
        Department = awl.DepartmentName,
        awl.DesignationId,
        Designation = awl.DesignationName,
        awl.EmployeeType,
        awl.IsTeachingStaff,
        awl.AttendanceDate,
        awl.CheckInTime,
        awl.CheckOutTime,
        awl.Status,
        CASE awl.Status
            WHEN 1 THEN 'Present'
            WHEN 2 THEN 'Absent'
            WHEN 3 THEN 'Late'
            WHEN 4 THEN 'Leave'
            ELSE 'Present'
        END AS StatusName,
        Remarks = ISNULL(awl.Remarks, ''),
        awl.LateMinutes,
        TotalRecords = fc.TotalCount
    FROM
        AttendanceWithLate awl,
        FinalCount fc
    ORDER BY
        awl.EmployeeName ASC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO
