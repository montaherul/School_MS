-- ============================================================================
-- Stored Procedure: sp_GetEmployeesPaged
-- Purpose: Get paginated employee list with search and filters for enterprise grid
-- Author: School Management System
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetEmployeesPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @DepartmentId INT = 0,
    @DesignationId INT = 0,
    @IsTeachingStaff BIT = NULL,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT 
        e.Id,
        e.EmployeeCode,
        e.FullName,
        COALESCE(d.Name, '') AS DepartmentName,
        COALESCE(desig.Name, '') AS DesignationName,
        e.Phone,
        e.Email,
        e.Status,
        e.IsTeachingStaff,
        e.JoiningDate,
        e.ProfilePicturePath,
        COALESCE(e.NIDNumber, '') AS NIDNumber,
        COALESCE(e.EmergencyContactName, '') AS EmergencyContactName,
        COALESCE(e.EmergencyContactPhone, '') AS EmergencyContactPhone,
        COALESCE(e.Remarks, '') AS Remarks,

        COUNT(*) OVER () AS TotalRecords
    FROM Employees e WITH(NOLOCK)
    LEFT JOIN Departments d WITH(NOLOCK) ON e.DepartmentId = d.Id AND d.IsDeleted = 0
    LEFT JOIN Designations desig WITH(NOLOCK) ON e.DesignationId = desig.Id AND desig.IsDeleted = 0
    WHERE e.IsDeleted = 0
        AND (@DepartmentId = 0 OR e.DepartmentId = @DepartmentId)
        AND (@DesignationId = 0 OR e.DesignationId = @DesignationId)
        AND (@IsTeachingStaff IS NULL OR e.IsTeachingStaff = @IsTeachingStaff)
        AND (@Status IS NULL OR @Status = '' OR e.Status = @Status)
        AND (
            @SearchTerm IS NULL OR @SearchTerm = ''
            OR e.FullName LIKE '%' + @SearchTerm + '%'
            OR e.EmployeeCode LIKE '%' + @SearchTerm + '%'
            OR e.Phone LIKE '%' + @SearchTerm + '%'
            OR e.Email LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY e.FullName ASC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
