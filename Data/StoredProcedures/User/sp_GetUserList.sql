-- ============================================================================
-- Stored Procedure: sp_GetUserList
-- Purpose: Get paginated user list with search, status, role, and user type filtering.
--          Includes linked entity info (Employee, Guardian, Student).
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetUserList
(
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(200) = NULL,
    @Status INT = NULL,
    @Role NVARCHAR(100) = NULL,
    @UserType NVARCHAR(50) = NULL  -- 'Employee', 'Guardian', 'Student', 'System'
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT
            u.Id,
            u.UserName,
            u.Email,
            u.PhoneNumber,
            u.Status,
            u.IsDeleted,
            u.EmployeeId,

            -- Linked entity info
            CAST(
                CASE
                    WHEN emp.Id IS NOT NULL THEN N'Employee'
                    WHEN gdn.Id IS NOT NULL THEN N'Guardian'
                    ELSE N'System'
                END AS NVARCHAR(50)
            ) AS UserType,

            CAST(
                COALESCE(
                    emp.FullName,
                    gdn.FullName,
                    N'â€”'
                ) AS NVARCHAR(200)
            ) AS LinkedEntityName,

            emp.IsTeachingStaff,

            -- Roles
            (
                SELECT STRING_AGG(r.Name, ', ')
FROM UserRoles ur WITH(NOLOCK)
INNER JOIN Roles r WITH(NOLOCK) ON ur.RoleId = r.Id
                WHERE ur.UserId = u.Id AND r.IsDeleted = 0
            ) AS RolesText,

            -- Pagination

            COUNT(*) OVER () AS TotalRecords

FROM Users u WITH(NOLOCK)

        -- Employee link (Employee.UserId -> User.Id)
LEFT JOIN Employees emp WITH(NOLOCK)
            ON emp.UserId = u.Id AND emp.IsDeleted = 0

        -- Guardian link (Guardian.UserId -> User.Id)
LEFT JOIN Guardians gdn WITH(NOLOCK)
            ON gdn.UserId = u.Id AND gdn.IsDeleted = 0

        WHERE
            u.IsDeleted = 0

            -- Search
            AND (
                @SearchTerm IS NULL OR @SearchTerm = ''
                OR u.UserName LIKE '%' + @SearchTerm + '%'
                OR u.Email LIKE '%' + @SearchTerm + '%'
                OR u.PhoneNumber LIKE '%' + @SearchTerm + '%'
                OR emp.FullName LIKE '%' + @SearchTerm + '%'
                OR gdn.FullName LIKE '%' + @SearchTerm + '%'
            )

            -- Status Filter
            AND (@Status IS NULL OR u.Status = @Status)

            -- Role Filter
            AND (
                @Role IS NULL
                OR EXISTS (
                    SELECT 1 FROM UserRoles ur
INNER JOIN Roles r WITH(NOLOCK) ON ur.RoleId = r.Id
                    WHERE ur.UserId = u.Id AND r.Name = @Role AND r.IsDeleted = 0
                )
            )

            -- User Type Filter
            AND (
                @UserType IS NULL
                OR (@UserType = N'Employee' AND emp.Id IS NOT NULL)
                OR (@UserType = N'Guardian' AND gdn.Id IS NOT NULL)
                OR (@UserType = N'System' AND emp.Id IS NULL AND gdn.Id IS NULL)
                OR (@UserType = N'Student' AND EXISTS (
                    SELECT 1 FROM UserRoles ur
INNER JOIN Roles r WITH(NOLOCK) ON ur.RoleId = r.Id
                    WHERE ur.UserId = u.Id AND r.Name = N'Student' AND r.IsDeleted = 0
                ))
            )
    
ORDER BY u.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END
GO
