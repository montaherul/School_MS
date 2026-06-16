CREATE OR ALTER PROCEDURE [dbo].[sp_PublishResults]
    @ExamId INT,
    @AcademicYearId INT,
    @PublishedByUserId INT,
    @LockResults BIT = 1,
    @Remarks NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    -- Create publication record
    MERGE ResultPublications AS target
    USING (SELECT @ExamId AS ExamId) AS source
    ON (target.ExamId = source.ExamId AND target.IsDeleted = 0)
    WHEN MATCHED THEN
        UPDATE SET
            Status = 5,
            PublishedAt = GETUTCDATE(),
            ApprovedByUserId = @PublishedByUserId,
            PublicationNotes = @Remarks,
            IsLocked = @LockResults,
            LockedAt = CASE WHEN @LockResults = 1 THEN GETUTCDATE() ELSE NULL END,
            UpdatedAt = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT (ExamId, AcademicYearId, Status, PublishedAt, ApprovedByUserId, PublicationNotes, IsLocked, LockedAt, CreatedAt, IsDeleted)
        VALUES (@ExamId, @AcademicYearId, 5, GETUTCDATE(), @PublishedByUserId, @Remarks, @LockResults,
            CASE WHEN @LockResults = 1 THEN GETUTCDATE() ELSE NULL END, GETUTCDATE(), 0);

    -- Update StudentExamResult status
    UPDATE StudentExamResults
    SET Status = 5,
        PublishedAt = GETUTCDATE()
    WHERE ExamId = @ExamId AND IsDeleted = 0;

    -- Update exam status
    UPDATE Exams
    SET Status = 5,
        IsLocked = CASE WHEN @LockResults = 1 THEN 1 ELSE IsLocked END,
        LockedAt = CASE WHEN @LockResults = 1 THEN GETUTCDATE() ELSE LockedAt END,
        LockedByUserId = CASE WHEN @LockResults = 1 THEN @PublishedByUserId ELSE LockedByUserId END
    WHERE Id = @ExamId;

    COMMIT TRANSACTION;

    SELECT 1 AS Success;
END;
GO
