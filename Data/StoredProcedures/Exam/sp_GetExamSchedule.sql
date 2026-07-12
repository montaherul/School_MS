CREATE OR ALTER PROCEDURE sp_GetExamSchedule
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Get all exam schedules with subject details for conflict detection
    SELECT
        sch.Id AS ScheduleId,
        sch.SubjectId,
        sub.Name AS SubjectName,
        sub.Code AS SubjectCode,
        c.Name AS ClassName,
        s.Name AS SectionName,
        sg.Name AS StudentGroupName,
        sch.ExamDate,
        sch.StartsAt,
        sch.EndsAt,
        sch.RoomNo,
        sch.RoomName,
        sch.BuildingName,
        sch.ShiftName,
        sch.Instructions
    FROM ExamSchedules sch
    INNER JOIN Subjects sub ON sub.Id = sch.SubjectId
    INNER JOIN Classes c ON c.Id = sch.ClassId
    LEFT JOIN Sections s ON s.Id = sch.SectionId
    LEFT JOIN StudentGroups sg ON sg.Id = sch.StudentGroupId
    WHERE sch.ExamId = @ExamId AND sch.IsDeleted = 0
    ORDER BY sch.ExamDate, sch.StartsAt, c.DisplayOrder;

    -- Detect room conflicts
    SELECT
        sch1.ExamDate,
        sch1.RoomNo,
        sch1.RoomName,
        sch1.SubjectId AS Subject1Id,
        sub1.Name AS Subject1Name,
        sch2.SubjectId AS Subject2Id,
        sub2.Name AS Subject2Name,
        sch1.StartsAt AS Start1,
        sch1.EndsAt AS End1,
        sch2.StartsAt AS Start2,
        sch2.EndsAt AS End2
    FROM ExamSchedules sch1
    INNER JOIN ExamSchedules sch2 ON 
        sch2.ExamId = sch1.ExamId 
        AND sch2.Id > sch1.Id
        AND sch2.ExamDate = sch1.ExamDate 
        AND sch2.RoomNo = sch1.RoomNo
        AND sch2.StartsAt < sch1.EndsAt 
        AND sch2.EndsAt > sch1.StartsAt
    INNER JOIN Subjects sub1 ON sub1.Id = sch1.SubjectId
    INNER JOIN Subjects sub2 ON sub2.Id = sch2.SubjectId
    WHERE sch1.ExamId = @ExamId AND sch1.IsDeleted = 0 AND sch2.IsDeleted = 0;

    -- Classes/subjects without schedule
    SELECT
        esj.Id AS ExamSubjectId,
        esj.SubjectName,
        c.Name AS ClassName
    FROM ExamSubjects esj
    INNER JOIN Classes c ON c.Id = esj.ClassId
    LEFT JOIN ExamSchedules sch ON sch.ExamId = @ExamId AND sch.SubjectId = esj.SubjectId AND sch.IsDeleted = 0
    WHERE esj.ExamId = @ExamId AND esj.IsDeleted = 0 AND sch.Id IS NULL
    ORDER BY c.DisplayOrder, esj.SubjectName;
END;
