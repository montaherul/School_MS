-- ============================================================================
-- Stored Procedure: sp_GenerateMonthlyInvoices
-- Purpose: Auto-generate monthly invoices for all active students
-- who have fee assignments with Monthly/Quarterly/HalfYearly/Yearly frequency.
-- Called by scheduler or manually.
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
    DECLARE @DueDate DATE = DATEFROMPARTS(@Year, @Month, @DueDay);

    -- If due day already passed this month, use next month
    IF @Today > @DueDate
        SET @DueDate = DATEADD(MONTH, 1, @DueDate);

    -- Temp table for students who qualify this month
    CREATE TABLE #BillingBatch (
        StudentId INT,
        FeeStructureId INT,
        FeeCategoryId INT,
        FeeName NVARCHAR(100),
        Amount DECIMAL(18,2),
        Frequency INT,
        AcademicYearId INT,
        SchoolClassId INT
    );

    -- Insert students whose fee frequency says "bill now"
    INSERT INTO #BillingBatch (StudentId, FeeStructureId, FeeCategoryId, FeeName, Amount, Frequency, AcademicYearId, SchoolClassId)
    SELECT
        s.Id,
        fs.Id,
        fs.FeeCategoryId,
        fs.FeeName,
        ISNULL(sfa.CustomAmount, fs.Amount),
        fs.Frequency,
        @AcademicYearId,
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

    DECLARE @GeneratedCount INT = 0;
    DECLARE @Count INT = (SELECT COUNT(*) FROM #BillingBatch);

    IF @Count = 0 RETURN;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Generate invoices grouped by student
        DECLARE @StudentId INT, @FeeStructureId INT, @FeeCategoryId INT,
                @FeeName NVARCHAR(100), @Amount DECIMAL(18,2),
                @InvoiceId INT, @InvoiceNo NVARCHAR(40);

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT DISTINCT StudentId FROM #BillingBatch;

        OPEN cur;
        FETCH NEXT FROM cur INTO @StudentId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Generate invoice number
            SET @InvoiceNo = 'INV-' + FORMAT(@Today, 'yyyyMMdd') + '-' + FORMAT(@StudentId, 'D6') + '-' + FORMAT(@Month, 'D2');

            -- Create invoice
            INSERT INTO FeeInvoices (InvoiceNo, StudentId, AcademicYearId, DueDate, TotalAmount, PaidAmount, DiscountAmount, LateFee, Status, Remarks, CreatedBy, CreatedAt, IsDeleted)
            VALUES (@InvoiceNo, @StudentId, @AcademicYearId, @DueDate, 0, 0, 0, 0, 5, 'Auto-generated monthly bill', 'auto-billing-system', @Now, 0);

            SET @InvoiceId = SCOPE_IDENTITY();

            -- Add invoice items for each fee structure assigned to this student
            DECLARE item_cursor CURSOR LOCAL FAST_FORWARD FOR
                SELECT FeeStructureId, FeeCategoryId, FeeName, Amount
                FROM #BillingBatch
                WHERE StudentId = @StudentId;

            OPEN item_cursor;
            FETCH NEXT FROM item_cursor INTO @FeeStructureId, @FeeCategoryId, @FeeName, @Amount;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                INSERT INTO FeeInvoiceItems (FeeInvoiceId, FeeStructureId, FeeCategoryId, Description, Amount, DiscountAmount, NetAmount, CreatedBy, CreatedAt, IsDeleted)
                VALUES (@InvoiceId, @FeeStructureId, @FeeCategoryId, @FeeName, @Amount, 0, @Amount, 'auto-billing-system', @Now, 0);

                -- Update invoice total
                UPDATE FeeInvoices SET TotalAmount = TotalAmount + @Amount WHERE Id = @InvoiceId;

                FETCH NEXT FROM item_cursor INTO @FeeStructureId, @FeeCategoryId, @FeeName, @Amount;
            END;

            CLOSE item_cursor;
            DEALLOCATE item_cursor;

            -- Create ledger entry for invoice
            INSERT INTO FeeLedger (StudentId, FeeInvoiceId, FeePaymentId, TransactionType, Debit, Credit, Balance, Description, TransactionDate, CreatedBy, CreatedAt, IsDeleted)
            SELECT @StudentId, @InvoiceId, NULL, 1, fi.TotalAmount, 0, fi.TotalAmount, 'Invoice generated: ' + fi.InvoiceNo, @Now, 'auto-billing-system', @Now, 0
            FROM FeeInvoices fi WHERE fi.Id = @InvoiceId;

            SET @GeneratedCount = @GeneratedCount + 1;

            FETCH NEXT FROM cur INTO @StudentId;
        END;

        CLOSE cur;
        DEALLOCATE cur;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;

    DROP TABLE #BillingBatch;

    -- Return result set
    SELECT @GeneratedCount AS GeneratedCount;
END;
GO
