-- ============================================================================
-- Stored Procedure: sp_GetAttendanceList
-- Purpose: Get paginated attendance list with date/class/section/student filters
-- Updated: May 2026 — added @ClassId, @SectionId, @AttendanceDate filters
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetAttendanceList
    @PageNumber     INT            = 1,
    @PageSize       INT            = 10,
    @SearchTerm     NVARCHAR(MAX)  = NULL,
    @StudentId      INT            = 0,
    @ClassId        INT            = 0,
    @SectionId      INT            = 0,
    @AttendanceDate DATE           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH FilteredAttendance AS (
        SELECT
            a.Id,
            a.StudentId,
            s.FullName          AS StudentName,
            a.SchoolClassId,
            c.Name              AS ClassName,
            a.SectionId,
            sec.Name            AS SectionName,
            a.[Status],
            a.Remarks,
            CAST(a.AttendanceDate AS DATE) AS AttendanceDate,
            COUNT(*) OVER ()    AS TotalCount
        FROM
            Attendance a
            JOIN Students   s   ON a.StudentId      = s.Id
            JOIN Classes    c   ON a.SchoolClassId  = c.Id
            JOIN Sections   sec ON a.SectionId      = sec.Id
        WHERE
            a.IsDeleted = 0
            AND (@StudentId      = 0    OR a.StudentId     = @StudentId)
            AND (@ClassId        = 0    OR a.SchoolClassId = @ClassId)
            AND (@SectionId      = 0    OR a.SectionId     = @SectionId)
            AND (@AttendanceDate IS NULL OR CAST(a.AttendanceDate AS DATE) = @AttendanceDate)
            AND (
                @SearchTerm IS NULL
                OR s.FullName    LIKE '%' + @SearchTerm + '%'
                OR s.StudentNo   LIKE '%' + @SearchTerm + '%'
                OR a.Remarks     LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT
        Id,
        StudentId,
        StudentName,
        SchoolClassId,
        ClassName,
        SectionId,
        SectionName,
        [Status],
        Remarks,
        AttendanceDate,
        TotalCount AS TotalRecords
    FROM
        FilteredAttendance
    ORDER BY
        AttendanceDate DESC,
        Id DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO
