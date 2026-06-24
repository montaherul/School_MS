-- ============================================================================
-- Stored Procedure: sp_GetRoleList
-- Purpose: Get paginated role list with permission counts
-- Author: School Management System
-- Created: May 4, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetRoleList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT 
            r.Id,
            r.Name,
            r.Description,
            (SELECT COUNT(*) FROM RolePermissions WHERE RoleId = r.Id) AS PermissionCount,
            (SELECT COUNT(*) FROM UserRoles WHERE RoleId = r.Id) AS UserCount,

            COUNT(*) OVER () AS TotalRecords
        FROM 
Roles r WITH(NOLOCK)
        WHERE 
            r.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR r.Name LIKE '%' + @SearchTerm + '%'
                OR r.Description LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY r.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
