CREATE OR ALTER PROCEDURE sp_GetStudentsForAttendance
    @ClassId INT,
    @SectionId INT,
    @StudentGroupId INT = NULL,
    @AttendanceDate DATE,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    DECLARE @Fetch INT = @PageSize;

    -- First result set: total count
    SELECT COUNT(*) AS TotalRecords
    FROM Students s WITH(NOLOCK)
    WHERE s.ClassId = @ClassId
        AND s.SectionId = @SectionId
        AND s.Status = 1
        AND s.IsDeleted = 0
        AND (@StudentGroupId IS NULL OR s.StudentGroupId = @StudentGroupId);

    -- Second result set: paged student list with attendance status
    SELECT
        ISNULL(a.Id, 0) AS Id,
        s.Id AS StudentId,
        ISNULL(s.StudentNo, '') AS StudentNo,
        ISNULL(s.FullName, '') AS StudentName,
        ISNULL(CAST(s.RollNumber AS NVARCHAR(10)), '') AS RollNumber,
        @ClassId AS ClassId,
        ISNULL(c.Name, '') AS ClassName,
        @SectionId AS SectionId,
        ISNULL(sec.Name, '') AS SectionName,
        s.StudentGroupId,
        ISNULL(sg.Name, '') AS StudentGroupName,
        @AttendanceDate AS AttendanceDate,
        ISNULL(a.Status, 1) AS Status,
        CASE ISNULL(a.Status, 1)
            WHEN 1 THEN 'Present'
            WHEN 2 THEN 'Absent'
            WHEN 3 THEN 'Late'
            WHEN 4 THEN 'Leave'
            ELSE 'Present'
        END AS StatusName,
        ISNULL(a.Remarks, '') AS Remarks,
        0 AS TotalRecords
    FROM Students s WITH(NOLOCK)
    INNER JOIN Classes c WITH(NOLOCK) ON s.ClassId = c.Id
    INNER JOIN Sections sec WITH(NOLOCK) ON s.SectionId = sec.Id
    LEFT JOIN StudentGroups sg WITH(NOLOCK) ON s.StudentGroupId = sg.Id
    LEFT JOIN Attendance a WITH(NOLOCK)
        ON a.StudentId = s.Id
        AND a.AttendanceDate = @AttendanceDate
        AND a.IsDeleted = 0
    WHERE s.ClassId = @ClassId
        AND s.SectionId = @SectionId
        AND s.Status = 1
        AND s.IsDeleted = 0
        AND (@StudentGroupId IS NULL OR s.StudentGroupId = @StudentGroupId)
    ORDER BY s.RollNumber, s.FullName
    OFFSET @Offset ROWS
    FETCH NEXT @Fetch ROWS ONLY;
END;
