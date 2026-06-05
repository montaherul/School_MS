CREATE OR ALTER PROCEDURE sp_GetAttendanceSummary
    @StudentId INT = 0,
    @EmployeeId INT = 0,
    @ClassId INT = 0,
    @SectionId INT = 0,
    @StudentGroupId INT = 0,
    @AttendanceDate DATE = NULL,
    @Year INT = 0,
    @Month INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Daily class/section/group summary aligned with grid (roster + attendance join)
    IF @ClassId > 0 AND @SectionId > 0 AND @AttendanceDate IS NOT NULL
    BEGIN
        SELECT
            EffectiveStatus = ISNULL(a.Status, 1),
            CountValue = COUNT(*)
        FROM Students s
        LEFT JOIN Attendance a ON a.StudentId = s.Id
            AND a.AttendanceDate = @AttendanceDate
            AND a.IsDeleted = 0
        WHERE s.ClassId = @ClassId
          AND s.SectionId = @SectionId
          AND s.IsDeleted = 0
          AND s.Status = 1
          AND (@StudentGroupId = 0 OR s.StudentGroupId = @StudentGroupId)
        GROUP BY ISNULL(a.Status, 1);
        RETURN;
    END

    -- Daily employee summary (grid cards)
    IF @EmployeeId = -1 AND @AttendanceDate IS NOT NULL
    BEGIN
        SELECT
            Status,
            CountValue = COUNT(*)
        FROM EmployeeAttendances
        WHERE AttendanceDate = @AttendanceDate
          AND IsDeleted = 0
        GROUP BY Status;
        RETURN;
    END

    -- Monthly student summary
    IF @StudentId > 0 AND @Year > 0 AND @Month > 0
    BEGIN
        SELECT
            Status,
            CountValue = COUNT(*)
        FROM Attendance
        WHERE StudentId = @StudentId
          AND YEAR(AttendanceDate) = @Year
          AND MONTH(AttendanceDate) = @Month
          AND IsDeleted = 0
        GROUP BY Status;
        RETURN;
    END

    -- Monthly employee summary
    IF @EmployeeId > 0 AND @Year > 0 AND @Month > 0
    BEGIN
        SELECT
            Status,
            CountValue = COUNT(*)
        FROM EmployeeAttendances
        WHERE EmployeeId = @EmployeeId
          AND YEAR(AttendanceDate) = @Year
          AND MONTH(AttendanceDate) = @Month
          AND IsDeleted = 0
        GROUP BY Status;
    END
END;
GO
