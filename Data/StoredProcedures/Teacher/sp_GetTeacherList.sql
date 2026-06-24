-- ============================================================================
-- Stored Procedure: sp_GetTeacherList
-- Purpose: Get paginated teacher list with filtering and count
-- Updated to match current Employees schema (EmployeeCode, DepartmentId, DesignationId)
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetTeacherList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @Department NVARCHAR(100) = NULL,
    @Status NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT
            t.Id,
            e.EmployeeCode AS TeacherNo,
            e.FullName,
            d.Name AS Designation,
            dept.Name AS Department,
            e.Phone AS MobileNumber,
            e.[Status],
            e.ProfilePicturePath,
            t.IsDeleted,

            COUNT(*) OVER () AS TotalRecords
        FROM
Teachers t WITH(NOLOCK)
LEFT JOIN Employees e WITH(NOLOCK) ON t.EmployeeId = e.Id
LEFT JOIN Departments dept WITH(NOLOCK) ON e.DepartmentId = dept.Id
LEFT JOIN Designations d WITH(NOLOCK) ON e.DesignationId = d.Id
        WHERE
            t.IsDeleted = 0
            AND (e.IsDeleted = 0 OR e.Id IS NULL)
            AND (
                @SearchTerm IS NULL
                OR e.FullName LIKE '%' + @SearchTerm + '%'
                OR t.TeacherCode LIKE '%' + @SearchTerm + '%'
                OR e.Phone LIKE '%' + @SearchTerm + '%'
                OR d.Name LIKE '%' + @SearchTerm + '%'
            )
            AND (@Department IS NULL OR dept.Name = @Department)
            AND (@Status IS NULL OR e.[Status] = @Status)
    
ORDER BY e.FullName ASC, t.Id ASC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
