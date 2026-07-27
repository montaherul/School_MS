-- ============================================================================
-- Stored Procedure: sp_GenerateMonthlyInvoices
-- Purpose: Auto-generate monthly invoices for all active students
-- who have fee assignments with Monthly/Quarterly/HalfYearly/Yearly frequency.
-- Set-based processing (no cursors). Configurable due days per fee structure.
-- Logs billing run to BillingRuns table.
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GenerateMonthlyInvoices
    @AcademicYearId INT,
    @DueDay INT = 10,
    @BatchSize INT = 500
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = CAST(@Now AS DATE);
    DECLARE @Year INT = YEAR(@Today);
    DECLARE @Month INT = MONTH(@Today);
    DECLARE @RunId INT;
    DECLARE @GeneratedCount INT = 0;
    DECLARE @StudentCount INT = 0;
    DECLARE @TotalAmount DECIMAL(18,2) = 0;

    -- Insert billing run log
    INSERT INTO BillingRuns (RunType, AcademicYearId, InvoicesGenerated, StudentsProcessed, TotalAmount, Status, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('Monthly', @AcademicYearId, 0, 0, 0, 'Running', 'auto-billing-system', @Now, 0);

    SET @RunId = SCOPE_IDENTITY();

    -- Temp table for students who qualify this month
    CREATE TABLE #BillingBatch (
        RowId INT IDENTITY(1,1) PRIMARY KEY,
        StudentId INT,
        FeeStructureId INT,
        FeeCategoryId INT,
        FeeName NVARCHAR(100),
        Amount DECIMAL(18,2),
        Frequency INT,
        DueDay INT,
        SchoolClassId INT
    );

    -- Insert students whose fee frequency says "bill now"
    INSERT INTO #BillingBatch (StudentId, FeeStructureId, FeeCategoryId, FeeName, Amount, Frequency, DueDay, SchoolClassId)
    SELECT
        s.Id,
        fs.Id,
        fs.FeeCategoryId,
        fs.FeeName,
        ISNULL(sfa.CustomAmount, fs.Amount),
        fs.Frequency,
        ISNULL(fs.DueDay, @DueDay),
        s.ClassId
    FROM Students s WITH(NOLOCK)
    JOIN StudentFeeAssignments sfa WITH(NOLOCK) ON sfa.StudentId = s.Id AND sfa.IsActive = 1 AND sfa.IsDeleted = 0
        AND (sfa.AcademicYearId IS NULL OR sfa.AcademicYearId = @AcademicYearId)
    JOIN FeeStructures fs WITH(NOLOCK) ON fs.Id = sfa.FeeStructureId AND fs.IsActive = 1 AND fs.IsDeleted = 0
    WHERE s.IsDeleted = 0
        AND (
            (fs.Frequency = 1) -- Monthly: always bill
            OR (fs.Frequency = 2 AND MONTH(@Today) % 3 = 1) -- Quarterly: bill every 3 months
            OR (fs.Frequency = 3 AND MONTH(@Today) % 6 = 1) -- HalfYearly: bill every 6 months
            OR (fs.Frequency = 4 AND MONTH(@Today) = 1)    -- Yearly: bill in January
        )
        -- Skip if already invoiced this month for same structure
        AND NOT EXISTS (
            SELECT 1 FROM FeeInvoices fi WITH(NOLOCK)
            JOIN FeeInvoiceItems fii WITH(NOLOCK) ON fii.FeeInvoiceId = fi.Id AND fii.FeeStructureId = fs.Id
            WHERE fi.StudentId = s.Id
                AND fi.AcademicYearId = @AcademicYearId
                AND fi.IsDeleted = 0
                AND MONTH(fi.CreatedAt) = @Month
                AND YEAR(fi.CreatedAt) = @Year
        );

    IF @@ROWCOUNT = 0
    BEGIN
        UPDATE BillingRuns SET Status = 'Skipped', CompletedAt = @Now WHERE Id = @RunId;
        DROP TABLE #BillingBatch;
        SELECT 0 AS GeneratedCount;
        RETURN;
    END;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Set-based invoice creation grouped by student
        DECLARE @Invoices TABLE (
            StudentId INT,
            InvoiceId INT,
            DueDay INT
        );

        -- Generate unique invoice numbers and create invoices in batch
        INSERT INTO FeeInvoices (InvoiceNo, StudentId, AcademicYearId, DueDate, TotalAmount, PaidAmount, DiscountAmount, LateFee, Status, Remarks, CreatedBy, CreatedAt, IsDeleted)
        OUTPUT INSERTED.StudentId, INSERTED.Id, INSERTED.DueDate
        INTO @Invoices (StudentId, InvoiceId, DueDay)
        SELECT
            'INV-' + FORMAT(@Today, 'yyyyMMdd') + '-' + FORMAT(bb.StudentId, 'D6') + '-' + FORMAT(@Month, 'D2') + '-' + FORMAT(ROW_NUMBER() OVER (ORDER BY bb.StudentId), 'D2'),
            bb.StudentId,
            @AcademicYearId,
            -- Calculate due date based on fee structure's DueDay
            CASE
                WHEN CAST(DATEFROMPARTS(@Year, @Month, MIN(bb.DueDay)) AS DATE) >= @Today
                THEN DATEFROMPARTS(@Year, @Month, MIN(bb.DueDay))
                ELSE DATEADD(MONTH, 1, DATEFROMPARTS(@Year, @Month, MIN(bb.DueDay)))
            END,
            0, 0, 0, 0, 5, 'Auto-generated monthly bill', 'auto-billing-system', @Now, 0
        FROM #BillingBatch bb
        GROUP BY bb.StudentId;

        -- Set-based invoice item creation
        INSERT INTO FeeInvoiceItems (FeeInvoiceId, FeeStructureId, FeeCategoryId, Description, Amount, DiscountAmount, NetAmount, CreatedBy, CreatedAt, IsDeleted)
        SELECT
            inv.InvoiceId,
            bb.FeeStructureId,
            bb.FeeCategoryId,
            bb.FeeName,
            bb.Amount,
            0,
            bb.Amount,
            'auto-billing-system',
            @Now,
            0
        FROM #BillingBatch bb
        JOIN @Invoices inv ON inv.StudentId = bb.StudentId;

        -- Update invoice totals in batch
        UPDATE fi
        SET fi.TotalAmount = item_sum.Total
        FROM FeeInvoices fi
        JOIN (
            SELECT fii.FeeInvoiceId, SUM(fii.NetAmount) AS Total
            FROM FeeInvoiceItems fii
            JOIN @Invoices inv ON inv.InvoiceId = fii.FeeInvoiceId
            GROUP BY fii.FeeInvoiceId
        ) item_sum ON item_sum.FeeInvoiceId = fi.Id;

        -- Create ledger entries in batch
        INSERT INTO FeeLedger (StudentId, FeeInvoiceId, FeePaymentId, TransactionType, Debit, Credit, Balance, Description, TransactionDate, CreatedBy, CreatedAt, IsDeleted)
        SELECT
            fi.StudentId,
            fi.Id,
            NULL,
            1,
            fi.TotalAmount,
            0,
            fi.TotalAmount,
            'Invoice generated: ' + fi.InvoiceNo,
            @Now,
            'auto-billing-system',
            @Now,
            0
        FROM FeeInvoices fi
        JOIN @Invoices inv ON inv.InvoiceId = fi.Id;

        -- Count results
        SELECT @GeneratedCount = COUNT(*), @StudentCount = COUNT(DISTINCT StudentId), @TotalAmount = ISNULL(SUM(TotalAmount), 0)
        FROM FeeInvoices WHERE Id IN (SELECT InvoiceId FROM @Invoices);

        -- Update billing run log
        UPDATE BillingRuns
        SET InvoicesGenerated = @GeneratedCount,
            StudentsProcessed = @StudentCount,
            TotalAmount = @TotalAmount,
            Status = 'Completed',
            CompletedAt = @Now
        WHERE Id = @RunId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        UPDATE BillingRuns
        SET Status = 'Failed',
            ErrorMessage = ERROR_MESSAGE(),
            CompletedAt = @Now
        WHERE Id = @RunId;

        DROP TABLE IF EXISTS #BillingBatch;
        THROW;
    END CATCH;

    DROP TABLE #BillingBatch;

    -- Return result set
    SELECT @GeneratedCount AS GeneratedCount, @StudentCount AS StudentsProcessed, @TotalAmount AS TotalAmount;
END;
GO
