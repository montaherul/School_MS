CREATE OR ALTER PROCEDURE sp_BulkPromoteStudents
    @SessionId INT,
    @FromClassId INT,
    @ToClassId INT,
    @AcademicYearId INT,
    @ProcessedByUserId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO PromotionHistories (StudentId, FromClassId, ToClassId, AcademicYearId,
            PromotioSessionId, Status, PromotedAt, PromotedByUserId, CreatedAt, CreatedBy)
        SELECT s.Id, @FromClassId, @ToClassId, @AcademicYearId,
               @SessionId, 'Promoted', GETUTCDATE(), @ProcessedByUserId, GETUTCDATE(), 'System'
        FROM Students s
        INNER JOIN FinalResults fr ON fr.StudentId = s.Id AND fr.AcademicYearId = @AcademicYearId
        WHERE s.ClassId = @FromClassId AND s.IsDeleted = 0 AND s.Status = 'Active'
          AND fr.IsPassed = 1 AND fr.PromotioStatus = 'Pending';
        
        SELECT @@ROWCOUNT AS PromotedCount;
        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH;
END;
