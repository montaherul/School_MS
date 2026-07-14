CREATE PROCEDURE sp_ReconcileBankTransactions
    @TransactionIds NVARCHAR(MAX),
    @ReconciledBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id INT;
    DECLARE @IdTable TABLE (Id INT PRIMARY KEY);

    INSERT INTO @IdTable (Id)
    SELECT value FROM STRING_SPLIT(@TransactionIds, ',');

    DECLARE cur CURSOR FOR SELECT Id FROM @IdTable;
    OPEN cur;
    FETCH NEXT FROM cur INTO @Id;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        UPDATE BankTransactions
        SET IsReconciled = 1, ReconciledAt = GETUTCDATE(), ReconciledBy = @ReconciledBy,
            UpdatedAt = GETUTCDATE(), UpdatedBy = @ReconciledBy
        WHERE Id = @Id AND IsDeleted = 0;

        FETCH NEXT FROM cur INTO @Id;
    END;

    CLOSE cur;
    DEALLOCATE cur;
END
