-- ============================================================================
-- Stored Procedure: sp_GenerateOneTimeFeeInvoices
-- Purpose: Batch-generate invoices for one-time fees (Frequency = 0)
-- that have not yet been invoiced for the student.
-- One-time fees are billed only once per StudentId + FeeStructureId.
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GenerateOneTimeFeeInvoices
    @AcademicYearId INT,
    @DueDay INT = 30,
    @BatchSize INT = 500
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = CAST(@Now AS DATE);
    DECLARE @DueDate DATE = DATEADD(DAY, @DueDay, @Today);
    DECLARE @RunId INT;
    DECLARE @GeneratedCount INT = 0;
    DECLARE @StudentCount INT = 0;
    DECLARE @TotalAmount DECIMAL(18,2) = 0;

    -- Insert billing run log
    INSERT INTO BillingRuns (RunType, AcademicYearId, InvoicesGenerated, StudentsProcessed, TotalAmount, Status, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('OneTime', @AcademicYearId, 0, 0, 0, 'Running', 'auto-billing-system', @Now, 0);

    SET @RunId = SCOPE_IDENTITY();

    -- Temp table for one-time fees that haven't been invoiced
    CREATE TABLE #OneTimeBatch (
        RowId INT IDENTITY(1,1) PRIMARY KEY,
        StudentId INT,
        FeeStructureId INT,
        FeeCategoryId INT,
        FeeName NVARCHAR(100),
        Amount DECIMAL(18,2),
        SchoolClassId INT
    );

    INSERT INTO #OneTimeBatch (StudentId, FeeStructureId, FeeCategoryId, FeeName, Amount, SchoolClassId)
    SELECT
        s.Id,
        fs.Id,
        fs.FeeCategoryId,
        fs.FeeName,
        ISNULL(sfa.CustomAmount, fs.Amount),
        s.ClassId
    FROM Students s WITH(NOLOCK)
    JOIN StudentFeeAssignments sfa WITH(NOLOCK) ON sfa.StudentId = s.Id AND sfa.IsActive = 1 AND sfa.IsDeleted = 0
        AND (sfa.AcademicYearId IS NULL OR sfa.AcademicYearId = @AcademicYearId)
    JOIN FeeStructures fs WITH(NOLOCK) ON fs.Id = sfa.FeeStructureId AND fs.IsActive = 1 AND fs.IsDeleted = 0
    WHERE s.IsDeleted = 0
        AND fs.Frequency = 0 -- One-time
        -- Skip if already invoiced
        AND NOT EXISTS (
            SELECT 1 FROM FeeInvoices fi WITH(NOLOCK)
            JOIN FeeInvoiceItems fii WITH(NOLOCK) ON fii.FeeInvoiceId = fi.Id AND fii.FeeStructureId = fs.Id
            WHERE fi.StudentId = s.Id
                AND fi.AcademicYearId = @AcademicYearId
                AND fi.IsDeleted = 0
        );

    IF @@ROWCOUNT = 0
    BEGIN
        UPDATE BillingRuns SET Status = 'Skipped', CompletedAt = @Now WHERE Id = @RunId;
        DROP TABLE #OneTimeBatch;
        SELECT 0 AS GeneratedCount;
        RETURN;
    END;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Invoices TABLE (StudentId INT, InvoiceId INT);

        -- Create invoices grouped by student
        INSERT INTO FeeInvoices (InvoiceNo, StudentId, AcademicYearId, DueDate, TotalAmount, PaidAmount, DiscountAmount, LateFee, Status, Remarks, CreatedBy, CreatedAt, IsDeleted)
        OUTPUT INSERTED.StudentId, INSERTED.Id INTO @Invoices
        SELECT
            'INV-OT-' + FORMAT(@Today, 'yyyyMMdd') + '-' + FORMAT(bb.StudentId, 'D6') + '-' + FORMAT(ROW_NUMBER() OVER (ORDER BY bb.StudentId), 'D3'),
            bb.StudentId,
            @AcademicYearId,
            @DueDate,
            0, 0, 0, 0, 5,
            'One-time fee: ' + STRING_AGG(bb.FeeName, ', '),
            'auto-billing-system', @Now, 0
        FROM #OneTimeBatch bb
        GROUP BY bb.StudentId;

        -- Create invoice items
        INSERT INTO FeeInvoiceItems (FeeInvoiceId, FeeStructureId, FeeCategoryId, Description, Amount, DiscountAmount, NetAmount, CreatedBy, CreatedAt, IsDeleted)
        SELECT
            inv.InvoiceId,
            bb.FeeStructureId,
            bb.FeeCategoryId,
            bb.FeeName,
            bb.Amount,
            0,
            bb.Amount,
            'auto-billing-system', @Now, 0
        FROM #OneTimeBatch bb
        JOIN @Invoices inv ON inv.StudentId = bb.StudentId;

        -- Update totals
        UPDATE fi
        SET fi.TotalAmount = item_sum.Total
        FROM FeeInvoices fi
        JOIN (SELECT fii.FeeInvoiceId, SUM(fii.NetAmount) AS Total FROM FeeInvoiceItems fii JOIN @Invoices inv ON inv.InvoiceId = fii.FeeInvoiceId GROUP BY fii.FeeInvoiceId) item_sum
            ON item_sum.FeeInvoiceId = fi.Id;

        -- Ledger entries
        INSERT INTO FeeLedger (StudentId, FeeInvoiceId, FeePaymentId, TransactionType, Debit, Credit, Balance, Description, TransactionDate, CreatedBy, CreatedAt, IsDeleted)
        SELECT fi.StudentId, fi.Id, NULL, 1, fi.TotalAmount, 0, fi.TotalAmount, 'One-time invoice: ' + fi.InvoiceNo, @Now, 'auto-billing-system', @Now, 0
        FROM FeeInvoices fi JOIN @Invoices inv ON inv.InvoiceId = fi.Id;

        SELECT @GeneratedCount = COUNT(*), @StudentCount = COUNT(DISTINCT StudentId), @TotalAmount = ISNULL(SUM(TotalAmount), 0)
        FROM FeeInvoices WHERE Id IN (SELECT InvoiceId FROM @Invoices);

        UPDATE BillingRuns
        SET InvoicesGenerated = @GeneratedCount, StudentsProcessed = @StudentCount, TotalAmount = @TotalAmount, Status = 'Completed', CompletedAt = @Now
        WHERE Id = @RunId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        UPDATE BillingRuns SET Status = 'Failed', ErrorMessage = ERROR_MESSAGE(), CompletedAt = @Now WHERE Id = @RunId;
        DROP TABLE IF EXISTS #OneTimeBatch;
        THROW;
    END CATCH;

    DROP TABLE #OneTimeBatch;
    SELECT @GeneratedCount AS GeneratedCount, @StudentCount AS StudentsProcessed, @TotalAmount AS TotalAmount;
END;
GO
