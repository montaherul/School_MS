-- ============================================================================
-- Stored Procedure: sp_GetUserList
-- Purpose: Get paginated user list with concatenated roles
-- Author: School Management System
-- Created: May 4, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetUserList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH UserData AS (
        SELECT 
            u.Id,
            u.UserName,
            u.Email,
            u.PhoneNumber,
            u.[Status],
            u.IsDeleted,
            (
                SELECT STRING_AGG(r.Name, ', ')
                FROM UserRoles ur
                JOIN Roles r ON ur.RoleId = r.Id
                WHERE ur.UserId = u.Id AND r.IsDeleted = 0
            ) AS RolesText,
            ROW_NUMBER() OVER (ORDER BY u.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Users u
        WHERE 
            u.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR u.UserName LIKE '%' + @SearchTerm + '%'
                OR u.Email LIKE '%' + @SearchTerm + '%'
                OR u.PhoneNumber LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        UserName,
        Email,
        PhoneNumber,
        [Status],
        IsDeleted,
        RolesText,
        TotalCount AS TotalRecords
    FROM 
        UserData
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
