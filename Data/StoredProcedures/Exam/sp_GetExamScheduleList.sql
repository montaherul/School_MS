CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamScheduleList]
    @ExamId INT = NULL,
    @ClassId INT = NULL,
    @StudentGroupId INT = NULL,
    @SectionId INT = NULL,
    @SubjectId INT = NULL,
    @AcademicYearId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        esch.Id,
        esch.ExamId,
        e.Name AS ExamName,
        esch.SubjectId,
        sub.Name AS SubjectName,
        sub.Code AS SubjectCode,
        esch.ClassId,
        c.Name AS ClassName,
        esch.StudentGroupId,
        sg.Name AS StudentGroupName,
        esch.SectionId,
        sec.Name AS SectionName,
        esch.ExamDate,
        esch.StartsAt,
        esch.EndsAt,
        esch.RoomNo,
        esch.Instructions
FROM ExamSchedules esch WITH(NOLOCK)
INNER JOIN Exams e WITH(NOLOCK) ON esch.ExamId = e.Id
INNER JOIN Subjects sub WITH(NOLOCK) ON esch.SubjectId = sub.Id
LEFT JOIN SchoolClasses c WITH(NOLOCK) ON esch.ClassId = c.Id
LEFT JOIN StudentGroups sg WITH(NOLOCK) ON esch.StudentGroupId = sg.Id
LEFT JOIN Sections sec WITH(NOLOCK) ON esch.SectionId = sec.Id
    WHERE e.IsDeleted = 0
      AND (@ExamId IS NULL OR esch.ExamId = @ExamId)
      AND (@ClassId IS NULL OR esch.ClassId = @ClassId)
      AND (@StudentGroupId IS NULL OR esch.StudentGroupId = @StudentGroupId)
      AND (@SectionId IS NULL OR esch.SectionId = @SectionId)
      AND (@SubjectId IS NULL OR esch.SubjectId = @SubjectId)
      AND (@AcademicYearId IS NULL OR e.AcademicYearId = @AcademicYearId)
    ORDER BY esch.ExamDate, esch.StartsAt;
END;
GO
