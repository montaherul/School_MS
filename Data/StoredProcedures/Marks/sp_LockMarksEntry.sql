CREATE OR ALTER PROCEDURE [dbo].[sp_LockMarksEntry]
    @ExamId INT,
    @SubjectId INT,
    @LockedByUserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Marks
    SET IsLocked = 1,
        LockedAt = GETUTCDATE(),
        Status = 6,
        UpdatedByUserId = @LockedByUserId,
        UpdatedAt = GETUTCDATE()
    WHERE ExamId = @ExamId AND SubjectId = @SubjectId AND IsDeleted = 0;

    INSERT INTO ResultLock (ExamId, LockedByUserId, LockedAt, CanUnlock, CreatedAt, IsDeleted)
    VALUES (@ExamId, @LockedByUserId, GETUTCDATE(), 1, GETUTCDATE(), 0);

    SELECT @@ROWCOUNT AS UpdatedCount;
END;
GO
