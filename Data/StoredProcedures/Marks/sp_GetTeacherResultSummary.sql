CREATE OR ALTER PROCEDURE [dbo].[sp_GetTeacherResultSummary]
    @TeacherId INT,
    @ExamId INT,
    @SubjectId INT,
    @ClassId INT,
    @SectionId INT,
    @GroupId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalStudents INT, @MarksEntered INT, @PassCount INT, @FailCount INT;
    DECLARE @AvgMarks DECIMAL(10,2), @HighestMarks DECIMAL(10,2), @LowestMarks DECIMAL(10,2);

    SELECT @TotalStudents = COUNT(DISTINCT st.Id)
    FROM Students st
    WHERE st.IsDeleted = 0 AND st.Status = 1
      AND st.ClassId = @ClassId AND st.SectionId = @SectionId
      AND (@GroupId IS NULL OR st.StudentGroupId = @GroupId);

    SELECT @MarksEntered = COUNT(DISTINCT me.StudentId)
    FROM Marks me
    WHERE me.ExamId = @ExamId AND me.SubjectId = @SubjectId
      AND me.ClassId = @ClassId AND me.SectionId = @SectionId
      AND me.IsDeleted = 0 AND me.Status > 0;

    SELECT @PassCount = COUNT(DISTINCT me.StudentId),
           @AvgMarks = AVG(me.MarksObtained),
           @HighestMarks = MAX(me.MarksObtained),
           @LowestMarks = MIN(me.MarksObtained)
    FROM Marks me
    WHERE me.ExamId = @ExamId AND me.SubjectId = @SubjectId
      AND me.ClassId = @ClassId AND me.SectionId = @SectionId
      AND me.IsDeleted = 0 AND me.Status > 0 AND me.MarksObtained >= 0;

    SELECT @FailCount = @MarksEntered - @PassCount;

    SELECT
        @TotalStudents AS TotalStudents,
        @MarksEntered AS MarksEntered,
        @PassCount AS PassCount,
        @FailCount AS FailCount,
        ISNULL(@AvgMarks, 0) AS AvgMarks,
        ISNULL(@HighestMarks, 0) AS HighestMarks,
        ISNULL(@LowestMarks, 0) AS LowestMarks;

    -- Grade distribution
    SELECT me.Grade, COUNT(*) AS Count
    FROM Marks me
    WHERE me.ExamId = @ExamId AND me.SubjectId = @SubjectId
      AND me.ClassId = @ClassId AND me.SectionId = @SectionId
      AND me.IsDeleted = 0 AND me.Status > 0
    GROUP BY me.Grade
    ORDER BY me.Grade;
END;
GO
