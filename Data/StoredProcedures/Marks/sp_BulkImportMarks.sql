CREATE OR ALTER PROCEDURE [dbo].[sp_BulkImportMarks]
    @ExamId INT,
    @AcademicYearId INT,
    @ClassId INT,
    @SectionId INT,
    @StudentGroupId INT = NULL,
    @EnteredByTeacherId INT,
    @MarksData NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SuccessCount INT = 0;
    DECLARE @ErrorCount INT = 0;
    DECLARE @Errors TABLE (RowNumber INT, Message NVARCHAR(500));

    DECLARE @Marks TABLE (
        RowNumber INT IDENTITY(1,1),
        StudentId INT,
        SubjectId INT,
        MarksObtained DECIMAL(18,2),
        FullMarks DECIMAL(18,2),
        Grade NVARCHAR(10),
        GradePoint DECIMAL(18,2)
    );

    INSERT INTO @Marks (StudentId, SubjectId, MarksObtained, FullMarks, Grade, GradePoint)
    SELECT
        StudentId, SubjectId, MarksObtained, FullMarks, Grade, GradePoint
FROM OPENJSON WITH(NOLOCK)(@MarksData)
    WITH (
        StudentId INT '$.studentId',
        SubjectId INT '$.subjectId',
        MarksObtained DECIMAL(18,2) '$.marksObtained',
        FullMarks DECIMAL(18,2) '$.fullMarks',
        Grade NVARCHAR(10) '$.grade',
        GradePoint DECIMAL(18,2) '$.gradePoint'
    );

    DECLARE @Row INT = 1;
    DECLARE @MaxRow INT;
    DECLARE @Sid INT, @SubId INT, @MarkVal DECIMAL(18,2), @Fm DECIMAL(18,2), @Gr NVARCHAR(10), @Gp DECIMAL(18,2);

    SELECT @MaxRow = MAX(RowNumber) FROM @Marks;

    WHILE @Row <= @MaxRow
    BEGIN
        SELECT @Sid = StudentId, @SubId = SubjectId, @MarkVal = MarksObtained, @Fm = FullMarks, @Gr = Grade, @Gp = GradePoint
        FROM @Marks WHERE RowNumber = @Row;

        IF @MarkVal > @Fm
        BEGIN
            INSERT INTO @Errors VALUES (@Row, 'MarksObtained exceeds FullMarks for StudentId=' + CAST(@Sid AS NVARCHAR) + ', SubjectId=' + CAST(@SubId AS NVARCHAR));
            SET @ErrorCount = @ErrorCount + 1;
            SET @Row = @Row + 1;
            CONTINUE;
        END;

        MERGE Marks AS target
        USING (SELECT @ExamId AS ExamId, @Sid AS StudentId, @SubId AS SubjectId) AS source
        ON (target.ExamId = source.ExamId AND target.StudentId = source.StudentId AND target.SubjectId = source.SubjectId AND target.IsDeleted = 0)
        WHEN MATCHED THEN
            UPDATE SET MarksObtained = @MarkVal, Grade = @Gr, GradePoint = @Gp, Status = 1, UpdatedByUserId = @EnteredByTeacherId, UpdatedAt = GETUTCDATE()
        WHEN NOT MATCHED THEN
            INSERT (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId, MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, CreatedByUserId, CreatedAt, IsDeleted)
            VALUES (@ExamId, @Sid, @SubId, @AcademicYearId, @ClassId, @SectionId, @StudentGroupId, @MarkVal, @Gr, @Gp, @EnteredByTeacherId, 1, 0, @EnteredByTeacherId, GETUTCDATE(), 0);

        SET @SuccessCount = @SuccessCount + 1;
        SET @Row = @Row + 1;
    END;

    SELECT @SuccessCount AS SuccessCount, @ErrorCount AS ErrorCount;
    SELECT RowNumber, Message FROM @Errors;
END;
GO
