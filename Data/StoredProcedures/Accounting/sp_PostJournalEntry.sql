CREATE PROCEDURE sp_PostJournalEntry
    @JournalEntryId INT,
    @PostedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @JournalNo NVARCHAR(50), @EntryDate DATETIME, @FinancialPeriodId INT;

        SELECT @JournalNo = JournalNo, @EntryDate = EntryDate, @FinancialPeriodId = FinancialPeriodId
        FROM JournalEntries WHERE Id = @JournalEntryId AND IsDeleted = 0;

        IF @JournalNo IS NULL
            THROW 50000, 'Journal entry not found or already deleted.', 1;

        -- Validate debit = credit
        DECLARE @TotalDebit DECIMAL(18,2), @TotalCredit DECIMAL(18,2);
        SELECT @TotalDebit = SUM(Amount) FROM JournalEntryLines WHERE JournalEntryId = @JournalEntryId AND LineType = 1 AND IsDeleted = 0;
        SELECT @TotalCredit = SUM(Amount) FROM JournalEntryLines WHERE JournalEntryId = @JournalEntryId AND LineType = 2 AND IsDeleted = 0;

        IF @TotalDebit IS NULL OR @TotalCredit IS NULL OR @TotalDebit != @TotalCredit
        BEGIN
            ROLLBACK;
            THROW 50000, 'Journal entry is not balanced. Debit must equal Credit.', 1;
        END;

        -- Post each line to General Ledger
        DECLARE @AccountId INT, @LineType INT, @Amount DECIMAL(18,2), @Narration NVARCHAR(500);
        DECLARE @Debit DECIMAL(18,2), @Credit DECIMAL(18,2);
        DECLARE @RunningBalance DECIMAL(18,2);

        DECLARE line_cursor CURSOR FOR
            SELECT AccountId, LineType, Amount, Narration
            FROM JournalEntryLines
            WHERE JournalEntryId = @JournalEntryId AND IsDeleted = 0
            ORDER BY Id;

        OPEN line_cursor;
        FETCH NEXT FROM line_cursor INTO @AccountId, @LineType, @Amount, @Narration;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @Debit = CASE WHEN @LineType = 1 THEN @Amount ELSE 0 END;
            SET @Credit = CASE WHEN @LineType = 2 THEN @Amount ELSE 0 END;

            -- Get current running balance for account
            SELECT @RunningBalance = ISNULL((
                SELECT TOP 1 RunningBalance FROM GeneralLedgerEntries
                WHERE AccountId = @AccountId AND IsDeleted = 0
                ORDER BY EntryDate DESC, Id DESC
            ), 0);

            -- Determine effect on balance based on account type
            SET @RunningBalance = @RunningBalance + @Debit - @Credit;

            INSERT INTO GeneralLedgerEntries (
                AccountId, EntryDate, JournalEntryId, JournalNo, Description,
                DebitAmount, CreditAmount, RunningBalance, FinancialPeriodId,
                CreatedBy, CreatedAt, IsDeleted
            ) VALUES (
                @AccountId, @EntryDate, @JournalEntryId, @JournalNo, @Narration,
                @Debit, @Credit, @RunningBalance, @FinancialPeriodId,
                @PostedBy, GETUTCDATE(), 0
            );

            FETCH NEXT FROM line_cursor INTO @AccountId, @LineType, @Amount, @Narration;
        END;

        CLOSE line_cursor;
        DEALLOCATE line_cursor;

        -- Mark journal entry as posted
        UPDATE JournalEntries
        SET IsPosted = 1, PostedAt = GETUTCDATE(), PostedBy = @PostedBy,
            UpdatedAt = GETUTCDATE(), UpdatedBy = @PostedBy
        WHERE Id = @JournalEntryId;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH;
END
