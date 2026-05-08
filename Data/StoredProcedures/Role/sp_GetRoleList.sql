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

    WITH RoleData AS (
        SELECT 
            r.Id,
            r.Name,
            r.Description,
            (SELECT COUNT(*) FROM RolePermissions WHERE RoleId = r.Id) AS PermissionCount,
            (SELECT COUNT(*) FROM UserRoles WHERE RoleId = r.Id) AS UserCount,
            ROW_NUMBER() OVER (ORDER BY r.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Roles r
        WHERE 
            r.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR r.Name LIKE '%' + @SearchTerm + '%'
                OR r.Description LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        Name,
        Description,
        PermissionCount,
        UserCount,
        TotalCount AS TotalRecords
    FROM 
        RoleData
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
