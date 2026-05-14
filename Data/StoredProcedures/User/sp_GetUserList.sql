-- ============================================================================
-- Stored Procedure: sp_GetUserList
-- Purpose: Get paginated user list with search, status, role filtering
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetUserList
(
    @PageNumber INT = 1,
    @PageSize INT = 10,

    @SearchTerm NVARCHAR(200) = NULL,
    @Status INT = NULL,
    @Role NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    ;WITH UserData AS
    (
        SELECT
            u.Id,
            u.UserName,
            u.Email,
            u.PhoneNumber,
            u.Status,
            u.IsDeleted,

            -- Roles
            (
                SELECT STRING_AGG(r.Name, ', ')
                FROM UserRoles ur
                INNER JOIN Roles r
                    ON ur.RoleId = r.Id
                WHERE ur.UserId = u.Id
                    AND r.IsDeleted = 0
            ) AS RolesText,

            -- Pagination
            ROW_NUMBER() OVER (ORDER BY u.Id DESC) AS RowNum,
            COUNT(*) OVER() AS TotalCount

        FROM Users u

        WHERE
            u.IsDeleted = 0

            -- Search
            AND
            (
                @SearchTerm IS NULL
                OR @SearchTerm = ''

                OR u.UserName LIKE '%' + @SearchTerm + '%'
                OR u.Email LIKE '%' + @SearchTerm + '%'
                OR u.PhoneNumber LIKE '%' + @SearchTerm + '%'
            )

            -- Status Filter
            AND
            (
                @Status IS NULL
                OR u.Status = @Status
            )

            -- Role Filter
            AND
            (
                @Role IS NULL
                OR EXISTS
                (
                    SELECT 1
                    FROM UserRoles ur
                    INNER JOIN Roles r
                        ON ur.RoleId = r.Id
                    WHERE ur.UserId = u.Id
                        AND r.Name = @Role
                        AND r.IsDeleted = 0
                )
            )
    )

    SELECT
        Id,
        UserName,
        Email,
        PhoneNumber,
        Status,

        CASE Status
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'Locked'
            WHEN 4 THEN 'Pending'
            ELSE 'Unknown'
        END AS StatusText,

        IsDeleted,
        RolesText,

        TotalCount AS TotalRecords,

        CEILING(CAST(TotalCount AS FLOAT) / @PageSize) AS LastPage,

        @PageNumber AS CurrentPage

    FROM UserData

    WHERE
        RowNum > @Offset
        AND RowNum <= (@Offset + @PageSize)

    ORDER BY RowNum;
END
GO