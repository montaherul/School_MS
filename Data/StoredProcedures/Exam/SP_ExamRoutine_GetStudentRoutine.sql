-- ============================================================================
-- Stored Procedure: SP_ExamRoutine_GetStudentRoutine
-- Purpose: Get published exam routine for a student's class/group
-- ============================================================================
CREATE OR ALTER PROCEDURE SP_ExamRoutine_GetStudentRoutine
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        es.Id AS ScheduleId,
        s.Name AS SubjectName,
        s.Code AS SubjectCode,
        es.ExamDate,
        es.StartsAt,
        es.EndsAt,
        es.RoomNo,
        es.Instructions,
        c.Name AS ClassName,
        sg.Name AS GroupName,
        sec.Name AS SectionName
    FROM ExamSchedules es
    INNER JOIN Exams e ON es.ExamId = e.Id
    INNER JOIN Subjects s ON es.SubjectId = s.Id
    INNER JOIN Classes c ON es.ClassId = c.Id
    LEFT JOIN StudentGroups sg ON es.StudentGroupId = sg.Id
    LEFT JOIN Sections sec ON es.SectionId = sec.Id
    INNER JOIN Students st ON st.Id = @StudentId
        AND st.ClassId = es.ClassId
        AND (es.StudentGroupId IS NULL OR es.StudentGroupId = st.StudentGroupId)
        AND (es.SectionId IS NULL OR es.SectionId = st.SectionId)
    WHERE e.Status = 5 -- Published
      AND e.IsDeleted = 0
      AND es.IsDeleted = 0
    ORDER BY es.ExamDate, es.StartsAt;
END;
GO
