CREATE OR ALTER PROCEDURE [dbo].[sp_BulkGenerateReportCards]
    @ExamId INT,
    @ClassId INT = NULL,
    @SectionId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ser.StudentId,
        s.FullName AS StudentName,
        s.RollNumber,
        c.Name AS ClassName,
        sec.Name AS SectionName,
        sg.Name AS GroupName,
        ser.TotalMarks,
        ser.TotalFullMarks,
        ser.Gpa,
        ser.Grade,
        ser.ClassPosition,
        ser.GroupPosition,
        CAST(ser.IsPassed AS BIT) AS IsPassed,
        ser.PublishedAt
FROM StudentExamResults ser WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON ser.StudentId = s.Id
INNER JOIN Classes c WITH(NOLOCK) ON s.ClassId = c.Id
LEFT JOIN Sections sec WITH(NOLOCK) ON s.SectionId = sec.Id
LEFT JOIN StudentGroups sg WITH(NOLOCK) ON s.StudentGroupId = sg.Id
    WHERE ser.ExamId = @ExamId
      AND ser.IsDeleted = 0
      AND (@ClassId IS NULL OR s.ClassId = @ClassId)
      AND (@SectionId IS NULL OR s.SectionId = @SectionId)
    ORDER BY c.Name, s.RollNumber;
END;
GO
