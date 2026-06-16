CREATE OR ALTER PROCEDURE [dbo].[sp_UnlockMarksEntry]
    @ExamId INT,
    @SubjectId INT,
    @UnlockedByUserId INT,
    @Reason NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Reason IS NULL OR LTRIM(RTRIM(@Reason)) = ''
    BEGIN
        RAISERROR('Reason is mandatory for unlock operation', 16, 1);
        RETURN;
    END;

    UPDATE Marks
    SET IsLocked = 0,
        LockedAt = NULL,
        Status = 1,
        UpdatedByUserId = @UnlockedByUserId,
        UpdatedAt = GETUTCDATE()
    WHERE ExamId = @ExamId AND SubjectId = @SubjectId AND IsDeleted = 0;

    INSERT INTO ResultAuditLogs (ExamId, StudentId, SubjectId, OldMarks, NewMarks, ChangedByUserId, Reason, ChangeType, ChangedAt, CreatedAt, IsDeleted)
    VALUES (@ExamId, NULL, @SubjectId, NULL, NULL, @UnlockedByUserId, @Reason, 'UNLOCK', GETUTCDATE(), GETUTCDATE(), 0);

    SELECT @@ROWCOUNT AS UpdatedCount;
END;
GO
