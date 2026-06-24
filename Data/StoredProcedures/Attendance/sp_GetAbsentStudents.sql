CREATE OR ALTER PROCEDURE sp_GetAbsentStudents
    @ClassId INT = 0,
    @SectionId INT = 0,
    @Date DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TargetDate DATE = COALESCE(@Date, CAST(GETDATE() AS DATE));

    SELECT
        a.Id,
        a.StudentId,
        s.FullName AS StudentName,
        s.StudentNo,
        s.RollNumber,
        c.Name AS ClassName,
        sec.Name AS SectionName,
        a.Remarks,
        a.AttendanceDate
    FROM
Attendance a WITH(NOLOCK)
JOIN Students s WITH(NOLOCK) ON a.StudentId = s.Id
JOIN Classes c WITH(NOLOCK) ON a.SchoolClassId = c.Id
JOIN Sections sec WITH(NOLOCK) ON a.SectionId = sec.Id
    WHERE
        a.IsDeleted = 0
        AND a.Status = 2 -- Absent
        AND (@ClassId = 0 OR a.SchoolClassId = @ClassId)
        AND (@SectionId = 0 OR a.SectionId = @SectionId)
        AND (a.AttendanceDate = @TargetDate)
    ORDER BY
        s.FullName;
END;
GO
