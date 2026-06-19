CREATE OR ALTER PROCEDURE sp_GetTeacherDashboardMarkStatus
    @TeacherId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sub.Name AS SubjectName,
        e.Name AS ExamName,
        c.Name AS ClassName,
        sec.Name AS SectionName,
        COUNT(DISTINCT me.StudentId) AS TotalStudents,
        SUM(CASE WHEN me.MarksObtained > 0 THEN 1 ELSE 0 END) AS MarksEntered,
        COUNT(DISTINCT me.StudentId) - SUM(CASE WHEN me.MarksObtained > 0 THEN 1 ELSE 0 END) AS PendingCount,
        CAST(me.Status AS NVARCHAR(50)) AS [Status]
    FROM Marks me
    INNER JOIN Exams e ON e.Id = me.ExamId AND e.IsDeleted = 0
    INNER JOIN Subjects sub ON sub.Id = me.SubjectId AND sub.IsDeleted = 0
    INNER JOIN SchoolClasses c ON c.Id = me.ClassId AND c.IsDeleted = 0
    INNER JOIN Sections sec ON sec.Id = me.SectionId AND sec.IsDeleted = 0
    WHERE me.EnteredByTeacherId = @TeacherId
      AND me.IsDeleted = 0
    GROUP BY sub.Name, e.Name, c.Name, sec.Name, me.Status
    ORDER BY e.Name, sub.Name;
END;
