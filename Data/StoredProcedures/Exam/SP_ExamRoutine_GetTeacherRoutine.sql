-- ============================================================================
-- Stored Procedure: SP_ExamRoutine_GetTeacherRoutine
-- Purpose: Get exam schedule + invigilation duties for a teacher
-- ============================================================================
CREATE OR ALTER PROCEDURE SP_ExamRoutine_GetTeacherRoutine
    @TeacherId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Subject exams where teacher is assigned
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
        sec.Name AS SectionName,
        'Assigned Subject' AS DutyType
    FROM ExamSchedules es
    INNER JOIN Exams e ON es.ExamId = e.Id
    INNER JOIN Subjects s ON es.SubjectId = s.Id
    INNER JOIN Classes c ON es.ClassId = c.Id
    LEFT JOIN StudentGroups sg ON es.StudentGroupId = sg.Id
    LEFT JOIN Sections sec ON es.SectionId = sec.Id
    INNER JOIN ExamSubjects exs ON exs.ExamId = es.ExamId 
        AND exs.SubjectId = es.SubjectId 
        AND exs.ClassId = es.ClassId
    WHERE exs.TeacherId = @TeacherId
      AND exs.IsDeleted = 0
      AND e.IsDeleted = 0
      AND es.IsDeleted = 0
      AND (e.Status = 5 OR e.Status = 4 OR e.Status = 3) -- Published/Locked/Active
    UNION
    -- All schedules in the teacher's assigned classes (invigilation context)
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
        sec.Name AS SectionName,
        'Invigilation' AS DutyType
    FROM ExamSchedules es
    INNER JOIN Exams e ON es.ExamId = e.Id
    INNER JOIN Subjects s ON es.SubjectId = s.Id
    INNER JOIN Classes c ON es.ClassId = c.Id
    LEFT JOIN StudentGroups sg ON es.StudentGroupId = sg.Id
    LEFT JOIN Sections sec ON es.SectionId = sec.Id
    INNER JOIN TeacherClassAssignments tca ON tca.ClassId = es.ClassId
    WHERE tca.TeacherId = @TeacherId
      AND tca.IsDeleted = 0
      AND e.IsDeleted = 0
      AND es.IsDeleted = 0
      AND (e.Status = 5 OR e.Status = 4 OR e.Status = 3)
    ORDER BY es.ExamDate, es.StartsAt;
END;
GO
