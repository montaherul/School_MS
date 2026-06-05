CREATE OR ALTER PROCEDURE sp_GetAttendanceRevisionHistory
    @ClassId   INT,
    @SectionId INT,
    @AttendanceDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.Id,
        r.AttendanceRecordId,
        r.StudentId,
        s.FullName AS StudentName,
        s.StudentNo,
        r.AttendanceDate,
        r.OldStatus,
        r.NewStatus,
        r.Reason,
        r.ChangedBy,
        r.ChangedAt
    FROM
        AttendanceRevisions r
        LEFT JOIN Students s ON r.StudentId = s.Id
    WHERE
        r.IsDeleted = 0
        AND r.AttendanceDate = @AttendanceDate
        AND (@ClassId = 0 OR s.ClassId = @ClassId)
        AND (@SectionId = 0 OR s.SectionId = @SectionId)
    ORDER BY
        r.ChangedAt DESC;
END;
GO
