CREATE OR ALTER PROCEDURE sp_GetAttendanceHistory
    @StudentId INT,
    @Year INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        StudentId,
        AttendanceDate,
        Status,
        Remarks
FROM Attendance WITH(NOLOCK)
    WHERE StudentId = @StudentId
      AND YEAR(AttendanceDate) = @Year
      AND MONTH(AttendanceDate) = @Month
      AND IsDeleted = 0
    ORDER BY AttendanceDate;
END;
GO