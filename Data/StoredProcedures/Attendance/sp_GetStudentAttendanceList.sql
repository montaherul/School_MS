CREATE OR ALTER PROCEDURE sp_GetStudentAttendanceList
    @PageNumber     INT            = 1,
    @PageSize       INT            = 10,
    @SearchTerm     NVARCHAR(MAX)  = NULL,
    @ClassId        INT            = 0,
    @SectionId      INT            = 0,
    @StudentGroupId INT            = 0,
    @AttendanceDate DATE           = NULL,
    @Status         INT            = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    DECLARE @TargetDate DATE = COALESCE(@AttendanceDate, CAST(GETDATE() AS DATE));

    WITH FilteredStudents AS (
        SELECT
            std.Id AS StudentId,
            std.StudentNo,
            std.FullName AS StudentName,
            std.RollNumber,
            std.ClassId,
            c.Name AS ClassName,
            std.SectionId,
            sec.Name AS SectionName,
            std.StudentGroupId,
            ISNULL(g.Name, '') AS StudentGroupName
        FROM
Students std WITH(NOLOCK)
JOIN Classes c WITH(NOLOCK) ON std.ClassId = c.Id
JOIN Sections sec WITH(NOLOCK) ON std.SectionId = sec.Id
LEFT JOIN StudentGroups g WITH(NOLOCK) ON std.StudentGroupId = g.Id
        WHERE
            std.IsDeleted = 0
            AND std.Status = 1 -- Active
            AND (@ClassId = 0 OR std.ClassId = @ClassId)
            AND (@SectionId = 0 OR std.SectionId = @SectionId)
            AND (@StudentGroupId = 0 OR std.StudentGroupId = @StudentGroupId)
            AND (
                @SearchTerm IS NULL
                OR std.FullName LIKE '%' + @SearchTerm + '%'
                OR std.StudentNo LIKE '%' + @SearchTerm + '%'
            )
    ),
    AttendanceWithSession AS (
        SELECT
            fs.StudentId,
            fs.StudentNo,
            fs.StudentName,
            fs.RollNumber,
            fs.ClassId,
            fs.ClassName,
            fs.SectionId,
            fs.SectionName,
            fs.StudentGroupId,
            fs.StudentGroupName,
            a.Id AS AttendanceRecordId,
            a.Status AS AttendanceStatus,
            a.Remarks,
            a.CreatedBy AS MarkedBy,
            a.AttendanceDate,
            sess.Status AS SessionStatus
        FROM
FilteredStudents fs WITH(NOLOCK)
LEFT JOIN Attendance a WITH(NOLOCK) ON a.StudentId = fs.StudentId 
                AND a.IsDeleted = 0
                AND a.AttendanceDate = @TargetDate
LEFT JOIN AttendanceSessions sess WITH(NOLOCK) ON sess.SchoolClassId = fs.ClassId
                AND sess.SectionId = fs.SectionId
                AND sess.AttendanceDate = @TargetDate
                AND sess.IsDeleted = 0
                AND (
                    (fs.StudentGroupId IS NULL AND sess.StudentGroupId IS NULL)
                    OR (fs.StudentGroupId IS NOT NULL AND sess.StudentGroupId = fs.StudentGroupId)
                )
        WHERE
            (@Status = 0 OR ISNULL(a.Status, 1) = @Status)
    ),
    FinalCount AS (
        SELECT COUNT(*) AS TotalCount FROM AttendanceWithSession
    )
    SELECT
        Id = ISNULL(aws.AttendanceRecordId, 0),
        aws.StudentId,
        aws.StudentNo,
        aws.StudentName,
        aws.RollNumber,
        aws.ClassId,
        aws.ClassName,
        aws.SectionId,
        aws.SectionName,
        aws.StudentGroupId,
        aws.StudentGroupName,
        AttendanceDate = ISNULL(aws.AttendanceDate, @TargetDate),
        Status = ISNULL(aws.AttendanceStatus, 1), -- Default Present = 1
        Remarks = ISNULL(aws.Remarks, ''),
        MarkedBy = ISNULL(aws.MarkedBy, ''),
        SessionStatus = ISNULL(aws.SessionStatus, 1), -- Default Draft = 1
        TotalRecords = fc.TotalCount
    FROM
AttendanceWithSession aws WITH(NOLOCK),
        FinalCount fc
    ORDER BY
        TRY_CAST(aws.RollNumber AS INT) ASC,
        aws.StudentName ASC,
        aws.StudentId ASC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO
