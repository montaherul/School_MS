CREATE OR ALTER PROCEDURE [dbo].[sp_GetTeacherMarksEntrySheet]
    @TeacherId INT,
    @ExamId INT,
    @ClassId INT,
    @SectionId INT,
    @SubjectId INT,
    @GroupId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Verify teacher is assigned to this subject/class/section
    IF NOT EXISTS (
        SELECT 1 FROM TeacherSubjectAssignments
        WHERE TeacherId = @TeacherId AND SubjectId = @SubjectId
          AND ClassId = @ClassId AND SectionId = @SectionId
          AND (@GroupId IS NULL OR GroupId = @GroupId)
          AND IsActive = 1 AND IsDeleted = 0
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS Authorized;
        RETURN;
    END

    SELECT CAST(1 AS BIT) AS Authorized;

    -- Student list
    SELECT st.Id AS StudentId, st.StudentNo, st.FullName AS StudentName,
           st.RollNumber, sc.Name AS ClassName, sec.Name AS SectionName,
           sg.Name AS GroupName
    FROM Students st
    INNER JOIN SchoolClasses sc ON sc.Id = @ClassId
    INNER JOIN Sections sec ON sec.Id = @SectionId AND sec.IsDeleted = 0
    LEFT JOIN StudentGroups sg ON sg.Id = @GroupId AND sg.IsDeleted = 0
    WHERE st.IsDeleted = 0 AND st.Status = 1
      AND st.ClassId = @ClassId AND st.SectionId = @SectionId
      AND (@GroupId IS NULL OR st.StudentGroupId = @GroupId)
    ORDER BY st.RollNumber;

    -- Existing marks
    SELECT me.StudentId, me.MarksObtained, me.Grade, me.GradePoint,
           me.WrittenMarks, me.MCQMarks, me.CQMarks, me.PracticalMarks,
           me.VivaMarks, me.LabMarks, me.OralMarks, me.AssignmentMarks,
           me.ContinuousAssessmentMarks, me.CompetencyMarks, me.BehaviourMarks,
           me.ParticipationMarks, me.ComponentValues, me.Status, me.IsLocked
    FROM Marks me
    WHERE me.ExamId = @ExamId AND me.SubjectId = @SubjectId
      AND me.ClassId = @ClassId AND me.SectionId = @SectionId
      AND me.IsDeleted = 0
      AND (@GroupId IS NULL OR EXISTS (
          SELECT 1 FROM Students s2
          WHERE s2.Id = me.StudentId
            AND (@GroupId IS NULL OR s2.StudentGroupId = @GroupId)
      ));
END;
GO
