CREATE OR ALTER PROCEDURE sp_GetAttendanceSessions
    @PageNumber     INT            = 1,
    @PageSize       INT            = 25,
    @SearchTerm     NVARCHAR(MAX)  = NULL,
    @ClassId        INT            = 0,
    @SectionId      INT            = 0,
    @StudentGroupId INT            = 0,
    @Status         INT            = 0,
    @AttendanceDate DATE           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH FilteredSessions AS (
        SELECT
            s.Id,
            s.AttendanceDate,
            s.SchoolClassId,
            c.Name AS ClassName,
            s.SectionId,
            sec.Name AS SectionName,
            s.StudentGroupId,
            ISNULL(g.Name, '') AS StudentGroupName,
            s.Status AS StatusValue,
            s.CreatedBy AS SubmittedBy,
            s.CreatedAt AS SubmittedAt,
            s.LockedBy,
            s.LockedAt,
            s.UpdatedAt
        FROM
            AttendanceSessions s
            JOIN Classes c ON s.SchoolClassId = c.Id
            JOIN Sections sec ON s.SectionId = sec.Id
            LEFT JOIN StudentGroups g ON s.StudentGroupId = g.Id
        WHERE
            s.IsDeleted = 0
            AND (@ClassId = 0 OR s.SchoolClassId = @ClassId)
            AND (@SectionId = 0 OR s.SectionId = @SectionId)
            AND (@StudentGroupId = 0 OR s.StudentGroupId = @StudentGroupId)
            AND (@Status = 0 OR CAST(s.Status AS INT) = @Status)
            AND (@AttendanceDate IS NULL OR s.AttendanceDate = @AttendanceDate)
            AND (
                @SearchTerm IS NULL
                OR c.Name LIKE '%' + @SearchTerm + '%'
                OR sec.Name LIKE '%' + @SearchTerm + '%'
                OR s.CreatedBy LIKE '%' + @SearchTerm + '%'
            )
    ),
    SessionsWithCounts AS (
        SELECT
            fs.*,
            (
                SELECT COUNT(*) 
                FROM Attendance a
                INNER JOIN Students st ON st.Id = a.StudentId AND st.IsDeleted = 0
                WHERE a.SchoolClassId = fs.SchoolClassId 
                  AND a.SectionId = fs.SectionId 
                  AND a.AttendanceDate = fs.AttendanceDate 
                  AND a.IsDeleted = 0
                  AND (fs.StudentGroupId IS NULL OR st.StudentGroupId = fs.StudentGroupId)
            ) AS TotalStudents,
            (
                SELECT COUNT(*) 
                FROM Attendance a
                INNER JOIN Students st ON st.Id = a.StudentId AND st.IsDeleted = 0
                WHERE a.SchoolClassId = fs.SchoolClassId 
                  AND a.SectionId = fs.SectionId 
                  AND a.AttendanceDate = fs.AttendanceDate 
                  AND a.Status = 1 -- Present
                  AND a.IsDeleted = 0
                  AND (fs.StudentGroupId IS NULL OR st.StudentGroupId = fs.StudentGroupId)
            ) AS Present,
            (
                SELECT COUNT(*) 
                FROM Attendance a
                INNER JOIN Students st ON st.Id = a.StudentId AND st.IsDeleted = 0
                WHERE a.SchoolClassId = fs.SchoolClassId 
                  AND a.SectionId = fs.SectionId 
                  AND a.AttendanceDate = fs.AttendanceDate 
                  AND a.Status = 2 -- Absent
                  AND a.IsDeleted = 0
                  AND (fs.StudentGroupId IS NULL OR st.StudentGroupId = fs.StudentGroupId)
            ) AS Absent
        FROM
            FilteredSessions fs
    ),
    FinalCount AS (
        SELECT COUNT(*) AS TotalCount FROM SessionsWithCounts
    )
    SELECT
        swc.Id,
        swc.AttendanceDate,
        swc.SchoolClassId,
        swc.ClassName,
        swc.SectionId,
        swc.SectionName,
        swc.StudentGroupId,
        swc.StudentGroupName,
        swc.StatusValue,
        CASE swc.StatusValue
            WHEN 1 THEN 'Draft'
            WHEN 2 THEN 'Submitted'
            WHEN 3 THEN 'Locked'
            WHEN 4 THEN 'Revised'
            WHEN 5 THEN 'Approved'
            ELSE 'Draft'
        END AS Status,
        swc.SubmittedBy,
        swc.SubmittedAt,
        swc.LockedBy,
        swc.LockedAt,
        swc.UpdatedAt,
        swc.TotalStudents,
        swc.Present,
        swc.Absent,
        TotalRecords = fc.TotalCount
    FROM
        SessionsWithCounts swc,
        FinalCount fc
    ORDER BY
        swc.AttendanceDate DESC,
        swc.Id DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO
