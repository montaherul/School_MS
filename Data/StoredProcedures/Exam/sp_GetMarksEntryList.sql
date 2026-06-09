CREATE OR ALTER PROCEDURE [dbo].[sp_GetMarksEntryList]
    @ExamId INT,
    @ClassId INT,
    @SectionId INT,
    @SubjectId INT,
    @OptionalSubjectId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.Id AS StudentId,
        s.FullName AS StudentName,
        s.StudentNo,
        s.RollNumber,
        s.ClassId,
        s.SectionId,
        sc.Name AS ClassName,
        sec.Name AS SectionName,
        m.Id AS MarkId,
        m.MarksObtained,
        m.WrittenMarks,
        m.MCQMarks,
        m.CQMarks,
        m.PracticalMarks,
        m.AssignmentMarks,
        m.VivaMarks,
        m.LabMarks,
        m.ContinuousAssessmentMarks,
        m.OralMarks,
        m.CompetencyMarks,
        m.BehaviourMarks,
        m.ParticipationMarks,
        m.ComponentValues,
        m.Grade,
        m.GradePoint,
        m.IsLocked,
        m.Status AS MarkStatus,
        CASE WHEN m.Id IS NOT NULL THEN 1 ELSE 0 END AS HasEntry
    FROM Students s
    INNER JOIN Classes sc ON s.ClassId = sc.Id
    INNER JOIN Sections sec ON s.SectionId = sec.Id
    LEFT JOIN Marks m ON s.Id = m.StudentId 
        AND m.ExamId = @ExamId 
        AND m.SubjectId = @SubjectId
        AND m.IsDeleted = 0
    WHERE s.ClassId = @ClassId
      AND s.SectionId = @SectionId
      AND s.Status = 1 -- Active per StudentStatus enum
      AND s.IsDeleted = 0
      AND (@OptionalSubjectId IS NULL OR s.OptionalSubjectId = @OptionalSubjectId)
    ORDER BY s.RollNumber;
END;
GO