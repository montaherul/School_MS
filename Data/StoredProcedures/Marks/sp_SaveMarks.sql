CREATE OR ALTER PROCEDURE [dbo].[sp_SaveMarks]
    @ExamId INT,
    @StudentId INT,
    @SubjectId INT,
    @AcademicYearId INT,
    @ClassId INT,
    @SectionId INT,
    @StudentGroupId INT = NULL,
    @MarksObtained DECIMAL(18,2) = NULL,
    @Grade NVARCHAR(10) = NULL,
    @GradePoint DECIMAL(18,2) = NULL,
    @EnteredByTeacherId INT,
    @Status INT = 1,
    @FullMarks DECIMAL(18,2) = NULL,
    @PassMarks DECIMAL(18,2) = NULL,
    @ComponentValuesJson NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ExistingId INT;
    DECLARE @OldMarks DECIMAL(18,2);
    DECLARE @OldGrade NVARCHAR(10);
    DECLARE @OldGpa DECIMAL(18,2);

    SELECT @ExistingId = Id, @OldMarks = MarksObtained, @OldGrade = Grade
    FROM Marks
    WHERE ExamId = @ExamId AND StudentId = @StudentId AND SubjectId = @SubjectId AND IsDeleted = 0;

    IF @MarksObtained IS NOT NULL AND @MarksObtained > @FullMarks
    BEGIN
        RAISERROR('MarksObtained cannot exceed FullMarks', 16, 1);
        RETURN;
    END;

    IF @ExistingId IS NOT NULL
    BEGIN
        INSERT INTO ResultAuditLogs (ExamId, StudentId, SubjectId, OldMarks, NewMarks, OldGpa, NewGpa, ChangedByUserId, Reason, ChangeType, ChangedAt, CreatedAt, IsDeleted)
        VALUES (@ExamId, @StudentId, @SubjectId, @OldMarks, @MarksObtained, NULL, NULL, @EnteredByTeacherId, NULL, 'UPDATE', GETUTCDATE(), GETUTCDATE(), 0);

        UPDATE Marks
        SET MarksObtained = @MarksObtained,
            Grade = @Grade,
            GradePoint = @GradePoint,
            Status = @Status,
            UpdatedByUserId = @EnteredByTeacherId,
            UpdatedAt = GETUTCDATE()
        WHERE Id = @ExistingId;
    END
    ELSE
    BEGIN
        INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
            MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, CreatedByUserId, CreatedAt, IsDeleted)
        VALUES (@ExamId, @StudentId, @SubjectId, @AcademicYearId, @ClassId, @SectionId, @StudentGroupId,
            @MarksObtained, @Grade, @GradePoint, @EnteredByTeacherId, @Status, 0, @EnteredByTeacherId, GETUTCDATE(), 0);
    END;

    SELECT SCOPE_IDENTITY() AS Id;
END;
GO
