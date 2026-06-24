CREATE OR ALTER PROCEDURE [dbo].[sp_GetGroupReport]
    @ExamGroupKey NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.Id AS ExamId,
        e.Name AS ExamName,
        e.Term,
        e.StartsOn,
        e.EndsOn,
        e.ClassId,
        c.Name AS ClassName,
        e.StudentGroupId,
        sg.Name AS GroupName,
        e.Status AS ExamStatus,
        (SELECT COUNT(*) FROM ExamSubjects es WHERE es.ExamId = e.Id) AS SubjectCount,
        (SELECT COUNT(*) FROM StudentExamResults ser WHERE ser.ExamId = e.Id AND ser.IsDeleted = 0) AS TotalStudents,
        (SELECT COUNT(*) FROM StudentExamResults ser WHERE ser.ExamId = e.Id AND ser.IsDeleted = 0 AND ser.IsPassed = 1) AS PassedStudents,
        ROUND(
            (SELECT AVG(ser.Gpa) FROM StudentExamResults ser WHERE ser.ExamId = e.Id AND ser.IsDeleted = 0)
        , 2) AS AverageGPA
FROM Exams e WITH(NOLOCK)
INNER JOIN Classes c WITH(NOLOCK) ON e.ClassId = c.Id
LEFT JOIN StudentGroups sg WITH(NOLOCK) ON e.StudentGroupId = sg.Id
    WHERE 1=1
      AND (@ExamGroupKey IS NULL OR e.Name LIKE '%' + @ExamGroupKey + '%')
      AND e.IsDeleted = 0
    ORDER BY c.Name, sg.Name;
END;
GO
