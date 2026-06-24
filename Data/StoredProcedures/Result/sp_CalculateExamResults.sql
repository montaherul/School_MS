CREATE OR ALTER PROCEDURE [dbo].[sp_CalculateExamResults]
    @ExamId INT,
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    MERGE StudentExamResults AS target
    USING (
        SELECT
            ssr.StudentId,
            ssr.ClassId,
            ssr.SectionId,
            ssr.StudentGroupId,
            SUM(ssr.MarksObtained) AS TotalMarks,
            SUM(ssr.FullMarks) AS TotalFullMarks,
            ROUND(AVG(CAST(ssr.GradePoint AS DECIMAL(18,4))), 2) AS Gpa,
            CASE
                WHEN AVG(CAST(ssr.GradePoint AS DECIMAL(18,4))) >= 5.00 THEN 'A+'
                WHEN AVG(CAST(ssr.GradePoint AS DECIMAL(18,4))) >= 4.00 THEN 'A'
                WHEN AVG(CAST(ssr.GradePoint AS DECIMAL(18,4))) >= 3.50 THEN 'A-'
                WHEN AVG(CAST(ssr.GradePoint AS DECIMAL(18,4))) >= 3.00 THEN 'B'
                WHEN AVG(CAST(ssr.GradePoint AS DECIMAL(18,4))) >= 2.00 THEN 'C'
                WHEN AVG(CAST(ssr.GradePoint AS DECIMAL(18,4))) >= 1.00 THEN 'D'
                ELSE 'F'
            END AS Grade,
            CASE WHEN MIN(CASE WHEN ssr.IsPassed = 0 THEN 0 ELSE 1 END) = 0 THEN 0 ELSE 1 END AS IsPassed,
            SUM(CASE WHEN ssr.IsPassed = 0 THEN 1 ELSE 0 END) AS FailedSubjectCount,
            SUM(CASE WHEN ssr.IsPassed = 1 THEN 1 ELSE 0 END) AS PassedSubjectCount
FROM StudentSubjectResults ssr WITH(NOLOCK)
        WHERE ssr.ExamId = @ExamId AND ssr.IsDeleted = 0
        GROUP BY ssr.StudentId, ssr.ClassId, ssr.SectionId, ssr.StudentGroupId
    ) AS source
    ON (
        target.ExamId = @ExamId
        AND target.StudentId = source.StudentId
        AND target.IsDeleted = 0
    )
    WHEN MATCHED THEN
        UPDATE SET
            ClassId = source.ClassId,
            SectionId = source.SectionId,
            StudentGroupId = source.StudentGroupId,
            TotalMarks = source.TotalMarks,
            TotalFullMarks = source.TotalFullMarks,
            Gpa = source.Gpa,
            Grade = source.Grade,
            IsPassed = source.IsPassed,
            FailedSubjectCount = source.FailedSubjectCount,
            PassedSubjectCount = source.PassedSubjectCount,
            CalculatedAt = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT (ExamId, StudentId, AcademicYearId, ClassId, SectionId, StudentGroupId,
            TotalMarks, TotalFullMarks, Gpa, Grade, IsPassed, FailedSubjectCount, PassedSubjectCount,
            Position, ClassPosition, Status, CalculatedAt, CreatedAt, IsDeleted)
        VALUES (@ExamId, source.StudentId, @AcademicYearId, source.ClassId, source.SectionId, source.StudentGroupId,
            source.TotalMarks, source.TotalFullMarks, source.Gpa, source.Grade, source.IsPassed,
            source.FailedSubjectCount, source.PassedSubjectCount,
            0, 0, 4, GETUTCDATE(), GETUTCDATE(), 0);

    SELECT @@ROWCOUNT AS ExamResultsCalculated;
END;
GO
