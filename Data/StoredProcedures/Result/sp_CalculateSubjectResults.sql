CREATE OR ALTER PROCEDURE [dbo].[sp_CalculateSubjectResults]
    @ExamId INT,
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    MERGE StudentSubjectResults AS target
    USING (
        SELECT
            m.StudentId,
            m.SubjectId,
            m.AcademicYearId,
            m.ClassId,
            m.SectionId,
            m.StudentGroupId,
            m.MarksObtained,
            es.FullMarks,
            es.PassMarks,
            CASE
                WHEN es.IsOptional = 1 THEN 1
                ELSE 0
            END AS IsOptionalSubject,
            CASE
                WHEN sub.ReligionType IS NOT NULL THEN 1
                ELSE 0
            END AS IsReligionSubject,
            CASE
                WHEN m.MarksObtained >= es.PassMarks THEN 1
                ELSE 0
            END AS IsPassed
FROM Marks m WITH(NOLOCK)
INNER JOIN ExamSubjects es WITH(NOLOCK) ON m.ExamId = es.ExamId AND m.SubjectId = es.SubjectId AND es.IsDeleted = 0
INNER JOIN Subjects sub WITH(NOLOCK) ON m.SubjectId = sub.Id
        WHERE m.ExamId = @ExamId AND m.IsDeleted = 0
    ) AS source
    ON (
        target.ExamId = @ExamId
        AND target.StudentId = source.StudentId
        AND target.SubjectId = source.SubjectId
        AND target.IsDeleted = 0
    )
    WHEN MATCHED THEN
        UPDATE SET
            MarksObtained = source.MarksObtained,
            FullMarks = source.FullMarks,
            PassMarks = source.PassMarks,
            IsPassed = source.IsPassed,
            Grade = CASE
                WHEN source.MarksObtained >= 80 THEN 'A+'
                WHEN source.MarksObtained >= 70 THEN 'A'
                WHEN source.MarksObtained >= 60 THEN 'A-'
                WHEN source.MarksObtained >= 50 THEN 'B'
                WHEN source.MarksObtained >= 40 THEN 'C'
                WHEN source.MarksObtained >= 33 THEN 'D'
                ELSE 'F'
            END,
            GradePoint = CASE
                WHEN source.MarksObtained >= 80 THEN 5.00
                WHEN source.MarksObtained >= 70 THEN 4.00
                WHEN source.MarksObtained >= 60 THEN 3.50
                WHEN source.MarksObtained >= 50 THEN 3.00
                WHEN source.MarksObtained >= 40 THEN 2.00
                WHEN source.MarksObtained >= 33 THEN 1.00
                ELSE 0.00
            END,
            CalculatedAt = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
            IsOptionalSubject, IsReligionSubject, MarksObtained, FullMarks, PassMarks, IsPassed,
            Grade, GradePoint, CalculatedAt, CreatedAt, IsDeleted)
        VALUES (@ExamId, source.StudentId, source.SubjectId, @AcademicYearId, source.ClassId, source.SectionId, source.StudentGroupId,
            source.IsOptionalSubject, source.IsReligionSubject, source.MarksObtained, source.FullMarks, source.PassMarks, source.IsPassed,
            CASE
                WHEN source.MarksObtained >= 80 THEN 'A+'
                WHEN source.MarksObtained >= 70 THEN 'A'
                WHEN source.MarksObtained >= 60 THEN 'A-'
                WHEN source.MarksObtained >= 50 THEN 'B'
                WHEN source.MarksObtained >= 40 THEN 'C'
                WHEN source.MarksObtained >= 33 THEN 'D'
                ELSE 'F'
            END,
            CASE
                WHEN source.MarksObtained >= 80 THEN 5.00
                WHEN source.MarksObtained >= 70 THEN 4.00
                WHEN source.MarksObtained >= 60 THEN 3.50
                WHEN source.MarksObtained >= 50 THEN 3.00
                WHEN source.MarksObtained >= 40 THEN 2.00
                WHEN source.MarksObtained >= 33 THEN 1.00
                ELSE 0.00
            END,
            GETUTCDATE(), GETUTCDATE(), 0);

    SELECT @@ROWCOUNT AS SubjectResultsCalculated;
END;
GO
