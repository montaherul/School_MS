-- ============================================================================
-- FinanceInitialization.sql
-- Enterprise Finance & Fee Management Data Initialization
-- School ERP — SQL Server
-- ============================================================================
-- PURPOSE: Initialize finance data for ALL existing non-deleted students.
--          Creates StudentFeeAssignments, FeeInvoices, FeeInvoiceItems,
--          and FeeLedger entries based on active FeeStructures.
--
-- SAFETY:  Idempotent — can be run multiple times without duplicates.
--          All inserts protected by existence checks.
--          Wrapped in TRANSACTION with TRY/CATCH ROLLBACK.
--
-- TARGET:  1000+ / 5000+ / 10000+ students (set-based, no cursors/loops)
-- ============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- ============================================================================
-- DECLARATIONS
-- ============================================================================
DECLARE @ActiveAcademicYearId INT,
        @ActiveYearName NVARCHAR(30),
        @YearStartsOn DATE,
        @YearEndsOn DATE,
        @Today DATE,
        @DefaultDueDay INT = 15,
        @ScriptStart DATETIME2 = SYSDATETIME(),
        @AssignmentCount INT = 0,
        @InvoiceCount INT = 0,
        @ItemCount INT = 0,
        @LedgerCount INT = 0,
        @ErrorMsg NVARCHAR(4000);

-- ============================================================================
-- DETECT ACTIVE ACADEMIC YEAR
-- ============================================================================
SELECT TOP 1
    @ActiveAcademicYearId = Id,
    @ActiveYearName = Name,
    @YearStartsOn = CAST(StartsOn AS DATE),
    @YearEndsOn = CAST(EndsOn AS DATE)
FROM AcademicYears
WHERE IsActive = 1
  AND IsDeleted = 0;

IF @ActiveAcademicYearId IS NULL
BEGIN
    RAISERROR('No active AcademicYear found. Aborting.', 16, 1);
    RETURN;
END

SET @Today = CAST(SYSDATETIME() AS DATE);

PRINT '=== FINANCE INITIALIZATION SCRIPT ===';
PRINT 'Started: ' + CONVERT(NVARCHAR(30), @ScriptStart, 120);
PRINT 'Active Academic Year: ' + @ActiveYearName + ' (ID=' + CAST(@ActiveAcademicYearId AS NVARCHAR) + ')';
PRINT 'Date Range: ' + CONVERT(NVARCHAR(10), @YearStartsOn, 120) + ' to ' + CONVERT(NVARCHAR(10), @YearEndsOn, 120);
PRINT '';

-- ============================================================================
-- BEGIN TRANSACTION
-- ============================================================================
BEGIN TRANSACTION;

BEGIN TRY

    -- ========================================================================
    -- STEP 2: CREATE StudentFeeAssignments
    -- ========================================================================
    ;WITH ActiveFeeStructures AS (
        SELECT
            fs.Id AS FeeStructureId,
            fs.SchoolClassId,
            fs.Amount,
            fs.FeeName,
            fs.DueDay,
            fs.Frequency,
            fs.IsRecurring
        FROM FeeStructures fs
        WHERE fs.IsDeleted = 0
          AND fs.IsActive = 1
    ),
    StudentsToAssign AS (
        SELECT
            s.Id AS StudentId,
            s.ClassId,
            s.SectionId,
            afs.FeeStructureId,
            afs.Amount,
            afs.FeeName,
            afs.DueDay,
            afs.Frequency,
            afs.IsRecurring
        FROM Students s
        INNER JOIN ActiveFeeStructures afs ON afs.SchoolClassId = s.ClassId
        WHERE s.IsDeleted = 0
          AND s.Status = 1  -- StudentStatus.Active
    )
    INSERT INTO StudentFeeAssignments (
        StudentId, FeeStructureId, AcademicYearId,
        CustomAmount, IsActive, ValidFrom, ValidTo, IsDeleted,
        CreatedBy, CreatedAt
    )
    SELECT
        sta.StudentId,
        sta.FeeStructureId,
        @ActiveAcademicYearId,
        NULL,                   -- CustomAmount (use FeeStructure default)
        1,                      -- IsActive
        CAST(@YearStartsOn AS DATE),   -- ValidFrom
        CAST(@YearEndsOn AS DATE),     -- ValidTo
        0,                      -- IsDeleted
        'SYSTEM-INIT',          -- CreatedBy
        @ScriptStart            -- CreatedAt
    FROM StudentsToAssign sta
    WHERE NOT EXISTS (
        SELECT 1
        FROM StudentFeeAssignments sfa
        WHERE sfa.StudentId = sta.StudentId
          AND sfa.FeeStructureId = sta.FeeStructureId
          AND sfa.AcademicYearId = @ActiveAcademicYearId
          AND sfa.IsDeleted = 0
    );

    SET @AssignmentCount = @@ROWCOUNT;
    PRINT 'Step 2: Created ' + CAST(@AssignmentCount AS NVARCHAR) + ' StudentFeeAssignment(s).';

    -- ========================================================================
    -- STEP 3: CREATE FeeInvoices
    -- Uses existing max invoice number for the year to avoid collisions
    -- ========================================================================
    DECLARE @ExistingMaxSeq INT;
    SELECT @ExistingMaxSeq = MAX(CAST(SUBSTRING(InvoiceNo, CHARINDEX('-', InvoiceNo, 5) + 1, LEN(InvoiceNo)) AS INT))
    FROM FeeInvoices
    WHERE InvoiceNo LIKE 'INV-' + CAST(YEAR(@YearStartsOn) AS NVARCHAR(4)) + '-%'
      AND IsDeleted = 0;

    SET @ExistingMaxSeq = ISNULL(@ExistingMaxSeq, 0);

    ;WITH StudentFees AS (
        SELECT
            sfa.StudentId,
            sfa.AcademicYearId,
            SUM(ISNULL(sfa.CustomAmount, fs.Amount)) AS TotalAmount,
            MIN(ISNULL(fs.DueDay, @DefaultDueDay)) AS DueDay
        FROM StudentFeeAssignments sfa
        INNER JOIN FeeStructures fs ON fs.Id = sfa.FeeStructureId AND fs.IsDeleted = 0
        WHERE sfa.IsDeleted = 0
          AND sfa.IsActive = 1
          AND sfa.AcademicYearId = @ActiveAcademicYearId
        GROUP BY sfa.StudentId, sfa.AcademicYearId
    ),
    NewInvoices AS (
        SELECT
            sf.StudentId,
            sf.AcademicYearId,
            sf.TotalAmount,
            sf.DueDay,
            ROW_NUMBER() OVER (ORDER BY sf.StudentId) AS RowNum
        FROM StudentFees sf
        WHERE NOT EXISTS (
            SELECT 1
            FROM FeeInvoices fi
            WHERE fi.StudentId = sf.StudentId
              AND (fi.AcademicYearId = sf.AcademicYearId OR (fi.AcademicYearId IS NULL AND sf.AcademicYearId IS NULL))
              AND fi.IsDeleted = 0
        )
    )
    INSERT INTO FeeInvoices (
        InvoiceNo, StudentId, AcademicYearId,
        DueDate, TotalAmount, PaidAmount, DiscountAmount, LateFee,
        Status, Remarks, IsDeleted,
        CreatedBy, CreatedAt
    )
    SELECT
        'INV-' + CAST(YEAR(@YearStartsOn) AS NVARCHAR(4))
            + '-' + RIGHT('000000' + CAST(@ExistingMaxSeq + ni.RowNum AS NVARCHAR(6)), 6),
        ni.StudentId,
        ni.AcademicYearId,
        DATEADD(DAY, ni.DueDay - 1, @YearStartsOn),
        ni.TotalAmount,
        0,                  -- PaidAmount
        0,                  -- DiscountAmount
        0,                  -- LateFee
        1,                  -- Status = PaymentStatus.Unpaid
        'Initial invoice via FinanceInitialization.sql',
        0,                  -- IsDeleted
        'SYSTEM-INIT',
        @ScriptStart
    FROM NewInvoices ni;

    SET @InvoiceCount = @@ROWCOUNT;
    PRINT 'Step 3: Created ' + CAST(@InvoiceCount AS NVARCHAR) + ' FeeInvoice(s).';

    -- ========================================================================
    -- STEP 4: CREATE FeeInvoiceItems
    -- ========================================================================
    INSERT INTO FeeInvoiceItems (
        FeeInvoiceId, FeeStructureId, FeeCategoryId,
        Description, Amount, DiscountAmount, NetAmount, IsDeleted,
        CreatedBy, CreatedAt
    )
    SELECT
        fi.Id,
        sfa.FeeStructureId,
        fs.FeeCategoryId,
        ISNULL(fs.FeeName, 'Fee Item'),
        ISNULL(sfa.CustomAmount, fs.Amount),
        0,                                          -- DiscountAmount
        ISNULL(sfa.CustomAmount, fs.Amount),        -- NetAmount
        0,                                          -- IsDeleted
        'SYSTEM-INIT',
        @ScriptStart
    FROM FeeInvoices fi
    INNER JOIN StudentFeeAssignments sfa
        ON sfa.StudentId = fi.StudentId
        AND sfa.AcademicYearId = fi.AcademicYearId
        AND sfa.IsDeleted = 0
        AND sfa.IsActive = 1
    INNER JOIN FeeStructures fs
        ON fs.Id = sfa.FeeStructureId
        AND fs.IsDeleted = 0
    WHERE fi.IsDeleted = 0
      AND fi.CreatedBy = 'SYSTEM-INIT'
      AND fi.CreatedAt = @ScriptStart
      AND NOT EXISTS (
          SELECT 1
          FROM FeeInvoiceItems fii
          WHERE fii.FeeInvoiceId = fi.Id
            AND fii.FeeStructureId = sfa.FeeStructureId
            AND fii.IsDeleted = 0
      );

    SET @ItemCount = @@ROWCOUNT;
    PRINT 'Step 4: Created ' + CAST(@ItemCount AS NVARCHAR) + ' FeeInvoiceItem(s).';

    -- ========================================================================
    -- STEP 5: CREATE FeeLedger ENTRIES
    -- ========================================================================
    INSERT INTO FeeLedgers (
        StudentId, FeeInvoiceId, FeePaymentId,
        TransactionType, Debit, Credit, Balance,
        Description, TransactionDate, IsDeleted,
        CreatedBy, CreatedAt
    )
    SELECT
        fi.StudentId,
        fi.Id,
        NULL,                       -- FeePaymentId (no payment yet)
        1,                          -- TransactionType = FeeLedgerType.Invoice
        fi.TotalAmount,             -- Debit
        0,                          -- Credit
        fi.TotalAmount,             -- Balance (initial = TotalAmount)
        'Invoice created: ' + fi.InvoiceNo,
        @ScriptStart,               -- TransactionDate
        0,                          -- IsDeleted
        'SYSTEM-INIT',
        @ScriptStart
    FROM FeeInvoices fi
    WHERE fi.IsDeleted = 0
      AND fi.CreatedBy = 'SYSTEM-INIT'
      AND fi.CreatedAt = @ScriptStart
      AND NOT EXISTS (
          SELECT 1
          FROM FeeLedgers fl
          WHERE fl.FeeInvoiceId = fi.Id
            AND fl.TransactionType = 1  -- FeeLedgerType.Invoice
            AND fl.IsDeleted = 0
      );

    SET @LedgerCount = @@ROWCOUNT;
    PRINT 'Step 5: Created ' + CAST(@LedgerCount AS NVARCHAR) + ' FeeLedger entry(ies).';
    PRINT '';

    -- ========================================================================
    -- COMMIT
    -- ========================================================================
    COMMIT TRANSACTION;

    PRINT '=== INITIALIZATION COMPLETE ===';
    PRINT 'Duration: ' + CAST(DATEDIFF(SECOND, @ScriptStart, SYSDATETIME()) AS NVARCHAR) + ' seconds';
    PRINT '';

END TRY
BEGIN CATCH
    SET @ErrorMsg = ERROR_MESSAGE();
    PRINT 'ERROR: ' + @ErrorMsg;
    PRINT 'Line: ' + CAST(ERROR_LINE() AS NVARCHAR);

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT 'Transaction rolled back. No data was modified.';
    RETURN;
END CATCH

-- ============================================================================
-- VALIDATION QUERIES — Run after success
-- ============================================================================
PRINT '=== VALIDATION ===';
PRINT '';

-- Counts
SELECT 'StudentFeeAssignments' AS [Table], COUNT(*) AS [Count]
FROM StudentFeeAssignments
WHERE IsDeleted = 0 AND AcademicYearId = @ActiveAcademicYearId
UNION ALL
SELECT 'FeeInvoices', COUNT(*)
FROM FeeInvoices
WHERE IsDeleted = 0 AND AcademicYearId = @ActiveAcademicYearId
UNION ALL
SELECT 'FeeInvoiceItems', COUNT(*)
FROM FeeInvoiceItems fii
INNER JOIN FeeInvoices fi ON fi.Id = fii.FeeInvoiceId AND fi.IsDeleted = 0
WHERE fii.IsDeleted = 0 AND fi.AcademicYearId = @ActiveAcademicYearId
UNION ALL
SELECT 'FeeLedgers (Invoice)', COUNT(*)
FROM FeeLedgers fl
INNER JOIN FeeInvoices fi ON fi.Id = fl.FeeInvoiceId AND fi.IsDeleted = 0
WHERE fl.IsDeleted = 0 AND fl.TransactionType = 1 AND fi.AcademicYearId = @ActiveAcademicYearId
ORDER BY [Table];

PRINT '';
PRINT '=== VERIFICATION ===';
PRINT '';

-- Students without assignment
SELECT 'Students without Assignment' AS [Check],
       COUNT(*) AS [Count]
FROM Students s
WHERE s.IsDeleted = 0
  AND s.Status = 1
  AND NOT EXISTS (
      SELECT 1 FROM StudentFeeAssignments sfa
      WHERE sfa.StudentId = s.Id
        AND sfa.AcademicYearId = @ActiveAcademicYearId
        AND sfa.IsDeleted = 0
  );

-- Students without invoice
SELECT 'Students without Invoice' AS [Check],
       COUNT(*) AS [Count]
FROM Students s
WHERE s.IsDeleted = 0
  AND s.Status = 1
  AND NOT EXISTS (
      SELECT 1 FROM FeeInvoices fi
      WHERE fi.StudentId = s.Id
        AND fi.AcademicYearId = @ActiveAcademicYearId
        AND fi.IsDeleted = 0
  );

-- Invoices without items
SELECT 'Invoices without Items' AS [Check],
       COUNT(*) AS [Count]
FROM FeeInvoices fi
WHERE fi.IsDeleted = 0
  AND fi.AcademicYearId = @ActiveAcademicYearId
  AND NOT EXISTS (
      SELECT 1 FROM FeeInvoiceItems fii
      WHERE fii.FeeInvoiceId = fi.Id
        AND fii.IsDeleted = 0
  );

-- Invoices without ledger
SELECT 'Invoices without Ledger' AS [Check],
       COUNT(*) AS [Count]
FROM FeeInvoices fi
WHERE fi.IsDeleted = 0
  AND fi.AcademicYearId = @ActiveAcademicYearId
  AND NOT EXISTS (
      SELECT 1 FROM FeeLedgers fl
      WHERE fl.FeeInvoiceId = fi.Id
        AND fl.TransactionType = 1
        AND fl.IsDeleted = 0
  );

PRINT '';
PRINT '=== SCRIPT ENDED ===';
GO
