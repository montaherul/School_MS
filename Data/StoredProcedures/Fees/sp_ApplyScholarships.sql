-- ============================================================================
-- Stored Procedure: sp_ApplyScholarships
-- Purpose: Batch apply active scholarships to eligible students.
--          Mirrors ScholarshipEngineService.RunAsync() logic in SQL.
--          Single transaction with cursor — same pattern as sp_GenerateMonthlyInvoices.
-- Returns: Result set: StudentsProcessed, ScholarshipsApplied, TotalDiscountAmount
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_ApplyScholarships
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StudentsProcessed INT = 0;
    DECLARE @ScholarshipsApplied INT = 0;
    DECLARE @TotalDiscountAmount DECIMAL(18,2) = 0;

    DECLARE @Now DATETIME2 = GETUTCDATE();
    DECLARE @Today DATE = CAST(@Now AS DATE);

    -- Cursor variables
    DECLARE @StudentId INT, @ClassId INT, @FeeStructureId INT, @CustomAmount DECIMAL(18,2);
    DECLARE @CurrentAmount DECIMAL(18,2), @DiscountAmount DECIMAL(18,2);
    DECLARE @ScholarshipId INT, @ScholarshipName NVARCHAR(100), @ScholarshipValue DECIMAL(18,2);
    DECLARE @DiscountType INT, @MatchFound BIT;
    DECLARE @Description NVARCHAR(500);

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Declare cursor: active students with their fee assignments
        DECLARE student_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT s.Id, s.ClassId, sfa.FeeStructureId, sfa.CustomAmount
            FROM Students s WITH(NOLOCK)
            INNER JOIN StudentFeeAssignments sfa WITH(NOLOCK)
                ON sfa.StudentId = s.Id AND sfa.IsActive = 1 AND sfa.IsDeleted = 0
            WHERE s.IsDeleted = 0;

        OPEN student_cursor;

        FETCH NEXT FROM student_cursor INTO @StudentId, @ClassId, @FeeStructureId, @CustomAmount;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @StudentsProcessed = @StudentsProcessed + 1;

            -- Get base amount from FeeStructure or custom override
            SELECT @CurrentAmount = ISNULL(@CustomAmount, Amount)
            FROM FeeStructures WITH(NOLOCK)
            WHERE Id = @FeeStructureId AND IsDeleted = 0;

            IF @CurrentAmount IS NULL OR @CurrentAmount <= 0
            BEGIN
                FETCH NEXT FROM student_cursor INTO @StudentId, @ClassId, @FeeStructureId, @CustomAmount;
                CONTINUE;
            END

            -- Find matching active scholarship
            SELECT TOP 1 @ScholarshipId = sh.Id, @ScholarshipName = sh.Name,
                         @ScholarshipValue = sh.Value, @DiscountType = CAST(sh.DiscountType AS INT)
            FROM Scholarships sh WITH(NOLOCK)
            WHERE sh.IsActive = 1 AND sh.IsDeleted = 0
              AND (sh.ValidFrom IS NULL OR sh.ValidFrom <= CAST(@Now AS DATE))
              AND (sh.ValidTo IS NULL OR sh.ValidTo >= CAST(@Now AS DATE))
              AND (sh.SchoolClassId IS NULL OR sh.SchoolClassId = @ClassId)
              AND (sh.FeeCategoryId IS NULL OR sh.FeeCategoryId = @FeeStructureId)
              AND (sh.FeeTypeId IS NULL OR sh.FeeTypeId = @FeeStructureId)
            ORDER BY sh.SchoolClassId DESC, sh.FeeCategoryId DESC, sh.FeeTypeId DESC;

            IF @ScholarshipId IS NOT NULL
            BEGIN
                -- Calculate discount
                IF @DiscountType = 1 -- Percentage
                    SET @DiscountAmount = @CurrentAmount * (@ScholarshipValue / 100.0);
                ELSE -- Fixed
                    SET @DiscountAmount = @ScholarshipValue;

                IF @DiscountAmount > @CurrentAmount
                    SET @DiscountAmount = @CurrentAmount;

                IF @DiscountAmount > 0
                BEGIN
                    -- Dedup: check if already applied today for this student
                    IF NOT EXISTS (
                        SELECT 1 FROM FeeLedgers WITH(NOLOCK)
                        WHERE StudentId = @StudentId
                          AND TransactionType = 3 -- Discount
                          AND Description LIKE '%' + @ScholarshipName + '%'
                          AND CAST(CreatedAt AS DATE) = @Today
                    )
                    BEGIN
                        SET @Description = 'Scholarship: ' + @ScholarshipName
                            + ' (' + CAST(@ScholarshipValue AS NVARCHAR(20))
                            + CASE WHEN @DiscountType = 1 THEN '%' ELSE ' fixed' END + ')';

                        INSERT INTO FeeLedgers (
                            StudentId, FeeInvoiceId, TransactionType,
                            Debit, Credit, Balance, Description,
                            TransactionDate, CreatedBy, CreatedAt
                        ) VALUES (
                            @StudentId, NULL, 3, -- Discount
                            0, @DiscountAmount, -@DiscountAmount, @Description,
                            @Now, 'system', @Now
                        );

                        SET @ScholarshipsApplied = @ScholarshipsApplied + 1;
                        SET @TotalDiscountAmount = @TotalDiscountAmount + @DiscountAmount;
                    END
                END
            END

            SET @ScholarshipId = NULL;

            FETCH NEXT FROM student_cursor INTO @StudentId, @ClassId, @FeeStructureId, @CustomAmount;
        END

        CLOSE student_cursor;
        DEALLOCATE student_cursor;

        COMMIT TRANSACTION;

        -- Return result
        SELECT
            @StudentsProcessed AS StudentsProcessed,
            @ScholarshipsApplied AS ScholarshipsApplied,
            @TotalDiscountAmount AS TotalDiscountAmount;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION

        IF CURSOR_STATUS('local', 'student_cursor') >= 0
        BEGIN
            CLOSE student_cursor
            DEALLOCATE student_cursor
        END

        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrSeverity INT = ERROR_SEVERITY()
        RAISERROR(@ErrMsg, @ErrSeverity, 1)
    END CATCH
END
GO
