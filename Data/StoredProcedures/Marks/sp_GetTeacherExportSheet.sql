CREATE OR ALTER PROCEDURE [dbo].[sp_GetTeacherExportSheet]
    @TeacherId INT,
    @ExamId INT,
    @SubjectId INT,
    @ClassId INT,
    @SectionId INT,
    @GroupId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Verify teacher assignment
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

    SELECT
        st.RollNumber,
        st.StudentNo,
        st.FullName AS StudentName,
        sc.Name AS ClassName,
        sec.Name AS SectionName,
        sg.Name AS GroupName,
        me.MarksObtained,
        me.WrittenMarks,
        me.MCQMarks,
        me.CQMarks,
        me.PracticalMarks,
        me.VivaMarks,
        me.LabMarks,
        me.OralMarks,
        me.AssignmentMarks,
        me.ContinuousAssessmentMarks,
        me.Grade,
        me.GradePoint,
        CASE WHEN me.Grade IS NOT NULL AND me.Grade != 'F' THEN 'Pass' ELSE 'Fail' END AS PassStatus,
        me.Status
    FROM Marks me
    INNER JOIN Students st ON st.Id = me.StudentId AND st.IsDeleted = 0
    INNER JOIN SchoolClasses sc ON sc.Id = me.ClassId
    INNER JOIN Sections sec ON sec.Id = me.SectionId
    LEFT JOIN StudentGroups sg ON sg.Id = st.StudentGroupId
    WHERE me.ExamId = @ExamId AND me.SubjectId = @SubjectId
      AND me.ClassId = @ClassId AND me.SectionId = @SectionId
      AND me.IsDeleted = 0
      AND (@GroupId IS NULL OR st.StudentGroupId = @GroupId)
    ORDER BY st.RollNumber;
END;
GO
