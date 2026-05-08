-- ============================================================================
-- Stored Procedure: sp_GetTeacherList
-- Purpose: Get paginated teacher list with filtering and count
-- Author: School Management System
-- Created: May 6, 2026
-- Updated: May 6, 2026 - Fixed status string-to-int conversion
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

    -- Convert status string name to its integer enum value
    -- TeacherStatus: Active=1, OnLeave=2, Resigned=3, Terminated=4, Inactive=5
    DECLARE @StatusInt INT = NULL;
    IF @Status IS NOT NULL
    BEGIN
        SET @StatusInt = CASE @Status
            WHEN 'Active'     THEN 1
            WHEN 'OnLeave'    THEN 2
            WHEN 'Resigned'   THEN 3
            WHEN 'Terminated' THEN 4
            WHEN 'Inactive'   THEN 5
            ELSE NULL
        END;
    END;

    WITH TeacherData AS (
        SELECT 
            t.Id,
            t.TeacherNo,
            t.FullName,
            t.Designation,
            t.Department,
            t.MobileNumber,
            t.[Status],
            t.ProfilePicturePath,
            t.IsDeleted,
            ROW_NUMBER() OVER (ORDER BY t.FullName ASC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Teachers t
        WHERE 
            t.IsDeleted = 0
            AND (
                @SearchTerm IS NULL 
                OR t.FullName LIKE '%' + @SearchTerm + '%'
                OR t.TeacherNo LIKE '%' + @SearchTerm + '%'
                OR t.MobileNumber LIKE '%' + @SearchTerm + '%'
                OR t.Designation LIKE '%' + @SearchTerm + '%'
            )
            AND (@Department IS NULL OR t.Department = @Department)
            AND (@StatusInt IS NULL OR t.[Status] = @StatusInt)
    )
    SELECT 
        Id,
        TeacherNo,
        FullName,
        Designation,
        Department,
        MobileNumber,
        [Status],
        ProfilePicturePath,
        TotalCount AS TotalRecords
    FROM 
        TeacherData
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
