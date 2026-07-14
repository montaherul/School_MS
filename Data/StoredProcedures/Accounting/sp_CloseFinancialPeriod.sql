CREATE PROCEDURE sp_CloseFinancialPeriod
    @FinancialPeriodId INT,
    @ClosedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Status INT;
        SELECT @Status = Status FROM FinancialPeriods WHERE Id = @FinancialPeriodId AND IsDeleted = 0;

        IF @Status IS NULL
            THROW 50000, 'Financial period not found.', 1;

        IF @Status = 3
            THROW 50000, 'Financial period is already closed.', 1;

        -- Verify no unposted journal entries
        IF EXISTS (
            SELECT 1 FROM JournalEntries
            WHERE FinancialPeriodId = @FinancialPeriodId AND IsDeleted = 0 AND IsPosted = 0
        )
        BEGIN
            ROLLBACK;
            THROW 50000, 'Cannot close period: there are unposted journal entries. Post or delete them first.', 1;
        END;

        -- Update status to Closed
        UPDATE FinancialPeriods
        SET Status = 3, ClosedAt = GETUTCDATE(), ClosedBy = @ClosedBy,
            IsActive = 0, UpdatedAt = GETUTCDATE(), UpdatedBy = @ClosedBy
        WHERE Id = @FinancialPeriodId;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH;
END
