-- ============================================================================
-- Stored Procedure: sp_GetAttendanceList
-- Purpose: Get paginated attendance list with date/class/section/student/group/status filters and sorting
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetAttendanceList
    @PageNumber     INT            = 1,
    @PageSize       INT            = 10,
    @SearchTerm     NVARCHAR(MAX)  = NULL,
    @StudentId      INT            = 0,
    @ClassId        INT            = 0,
    @SectionId      INT            = 0,
    @StudentGroupId INT            = 0,
    @Status         INT            = 0,
    @AttendanceDate DATE           = NULL,
    @SortColumn     NVARCHAR(50)   = NULL,
    @SortDirection  NVARCHAR(10)   = 'DESC'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        a.Id,
        a.StudentId,
        s.FullName          AS StudentName,
        s.StudentNo         AS StudentNo,
        a.SchoolClassId,
        c.Name              AS ClassName,
        a.SectionId,
        sec.Name            AS SectionName,
        a.[Status],
        a.Remarks,
        CAST(a.AttendanceDate AS DATE) AS AttendanceDate,
        COUNT(*) OVER () AS TotalRecords
    FROM
Attendance a WITH(NOLOCK)
JOIN Students s WITH(NOLOCK)   ON a.StudentId      = s.Id
JOIN Classes c WITH(NOLOCK)   ON a.SchoolClassId  = c.Id
JOIN Sections sec WITH(NOLOCK) ON a.SectionId      = sec.Id
    WHERE
        a.IsDeleted = 0
        AND (@StudentId      = 0    OR a.StudentId     = @StudentId)
        AND (@ClassId        = 0    OR a.SchoolClassId = @ClassId)
        AND (@SectionId      = 0    OR a.SectionId     = @SectionId)
        AND (@StudentGroupId = 0    OR s.StudentGroupId = @StudentGroupId)
        AND (@Status         = 0    OR CAST(a.Status AS INT) = @Status)
        AND (@AttendanceDate IS NULL OR CAST(a.AttendanceDate AS DATE) = @AttendanceDate)
        AND (
            @SearchTerm IS NULL
            OR s.FullName    LIKE '%' + @SearchTerm + '%'
            OR s.StudentNo   LIKE '%' + @SearchTerm + '%'
            OR a.Remarks     LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY
        CASE WHEN @SortDirection = 'ASC' THEN
            CASE 
                WHEN @SortColumn = 'StudentName' THEN StudentName
                WHEN @SortColumn = 'ClassName'   THEN ClassName
                WHEN @SortColumn = 'SectionName' THEN SectionName
                ELSE NULL
            END
        END ASC,
        CASE WHEN @SortDirection = 'DESC' THEN
            CASE 
                WHEN @SortColumn = 'StudentName' THEN StudentName
                WHEN @SortColumn = 'ClassName'   THEN ClassName
                WHEN @SortColumn = 'SectionName' THEN SectionName
                ELSE NULL
            END
        END DESC,
        CASE WHEN @SortDirection = 'ASC' THEN
            CASE 
                WHEN @SortColumn = 'AttendanceDate' THEN AttendanceDate
                ELSE NULL
            END
        END ASC,
        CASE WHEN @SortDirection = 'DESC' THEN
            CASE 
                WHEN @SortColumn = 'AttendanceDate' THEN AttendanceDate
                ELSE NULL
            END
        END DESC,
        CASE WHEN @SortColumn IS NULL THEN AttendanceDate END DESC,
        Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
