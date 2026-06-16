CREATE OR ALTER PROCEDURE [dbo].[sp_UnpublishResults]
    @ExamId INT,
    @UnpublishedByUserId INT,
    @Reason NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Reason IS NULL OR LTRIM(RTRIM(@Reason)) = ''
    BEGIN
        RAISERROR('Reason is mandatory for unpublish operation', 16, 1);
        RETURN;
    END;

    BEGIN TRANSACTION;

    -- Update ResultPublications
    UPDATE ResultPublications
    SET Status = 7,
        PublishedAt = NULL,
        IsLocked = 0,
        UpdatedAt = GETUTCDATE()
    WHERE ExamId = @ExamId AND IsDeleted = 0;

    -- Update StudentExamResults
    UPDATE StudentExamResults
    SET Status = 4,
        PublishedAt = NULL
    WHERE ExamId = @ExamId AND IsDeleted = 0;

    -- Update exam status
    UPDATE Exams
    SET Status = 7,
        IsLocked = 0,
        LockedAt = NULL,
        LockedByUserId = NULL
    WHERE Id = @ExamId;

    -- Audit
    INSERT INTO ResultAuditLogs (ExamId, StudentId, SubjectId, OldMarks, NewMarks, ChangedByUserId, Reason, ChangeType, ChangedAt, CreatedAt, IsDeleted)
    VALUES (@ExamId, NULL, NULL, NULL, NULL, @UnpublishedByUserId, @Reason, 'UNPUBLISH', GETUTCDATE(), GETUTCDATE(), 0);

    COMMIT TRANSACTION;

    SELECT 1 AS Success;
END;
GO
