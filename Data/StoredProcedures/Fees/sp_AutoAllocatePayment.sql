-- ============================================================================
-- Stored Procedure: sp_AutoAllocatePayment
-- Purpose: Auto-allocate unallocated payment amounts to open invoices
--          using proportional allocation. Single transaction with cursor.
-- Returns: Result set: PaymentsProcessed, AllocationsCreated, TotalAllocated
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_AutoAllocatePayment
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @PaymentsProcessed INT = 0;
    DECLARE @AllocationsCreated INT = 0;
    DECLARE @TotalAllocated DECIMAL(18,2) = 0;

    DECLARE @Now DATETIME2 = GETUTCDATE();

    -- Cursor variables
    DECLARE @PaymentId INT, @PaymentAmount DECIMAL(18,2), @StudentId INT;
    DECLARE @AlreadyAllocated DECIMAL(18,2), @Remaining DECIMAL(18,2);
    DECLARE @InvoiceId INT, @InvoiceDue DECIMAL(18,2), @TotalDue DECIMAL(18,2);
    DECLARE @Proportion DECIMAL(18,6), @AllocationAmount DECIMAL(18,2);
    DECLARE @InvoiceTotal DECIMAL(18,2), @InvoicePaid DECIMAL(18,2), @InvoiceStatus INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Declare cursor: unallocated payments with remaining > 0
        DECLARE payment_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT p.Id, p.Amount, fi.StudentId
            FROM Payments p WITH(NOLOCK)
            INNER JOIN FeeInvoices fi WITH(NOLOCK) ON fi.Id = p.FeeInvoiceId AND fi.IsDeleted = 0
            WHERE p.IsDeleted = 0 AND p.Amount > 0;

        OPEN payment_cursor;

        FETCH NEXT FROM payment_cursor INTO @PaymentId, @PaymentAmount, @StudentId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Calculate already allocated
            SELECT @AlreadyAllocated = ISNULL(SUM(pa.AllocatedAmount), 0)
            FROM PaymentAllocations pa WITH(NOLOCK)
            WHERE pa.PaymentId = @PaymentId AND pa.IsDeleted = 0;

            SET @Remaining = @PaymentAmount - @AlreadyAllocated;

            IF @Remaining > 0
            BEGIN
                -- Calculate total due across open invoices for this student
                SELECT @TotalDue = ISNULL(SUM(fi.TotalAmount - fi.PaidAmount), 0)
                FROM FeeInvoices fi WITH(NOLOCK)
                WHERE fi.StudentId = @StudentId
                  AND fi.IsDeleted = 0
                  AND (fi.Status = 1 OR fi.Status = 2) -- Issued or Partial
                  AND fi.TotalAmount > fi.PaidAmount;

                IF @TotalDue > 0
                BEGIN
                    -- Cursor over open invoices
                    DECLARE invoice_cursor CURSOR LOCAL FAST_FORWARD FOR
                        SELECT fi.Id, fi.TotalAmount, fi.PaidAmount, fi.Status
                        FROM FeeInvoices fi WITH(NOLOCK)
                        WHERE fi.StudentId = @StudentId
                          AND fi.IsDeleted = 0
                          AND (fi.Status = 1 OR fi.Status = 2)
                          AND fi.TotalAmount > fi.PaidAmount;

                    OPEN invoice_cursor;

                    FETCH NEXT FROM invoice_cursor INTO @InvoiceId, @InvoiceTotal, @InvoicePaid, @InvoiceStatus;

                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        SET @InvoiceDue = @InvoiceTotal - @InvoicePaid;
                        SET @Proportion = @InvoiceDue / @TotalDue;
                        SET @AllocationAmount = ROUND(@Remaining * @Proportion, 2);

                        IF @AllocationAmount > 0
                        BEGIN
                            -- Insert allocation
                            INSERT INTO PaymentAllocations (
                                PaymentId, FeeInvoiceId, AllocatedAmount, Remarks,
                                CreatedBy, CreatedAt
                            ) VALUES (
                                @PaymentId, @InvoiceId, @AllocationAmount,
                                'Auto-allocated from Payment #' + CAST(@PaymentId AS NVARCHAR(20)),
                                'system', @Now
                            );

                            -- Update invoice
                            SET @InvoicePaid = @InvoicePaid + @AllocationAmount;
                            IF @InvoicePaid >= @InvoiceTotal
                                SET @InvoiceStatus = 3; -- Paid
                            ELSE
                                SET @InvoiceStatus = 2; -- Partial

                            UPDATE FeeInvoices
                            SET PaidAmount = @InvoicePaid,
                                Status = @InvoiceStatus,
                                UpdatedAt = @Now
                            WHERE Id = @InvoiceId;

                            SET @AllocationsCreated = @AllocationsCreated + 1;
                            SET @TotalAllocated = @TotalAllocated + @AllocationAmount;
                        END

                        FETCH NEXT FROM invoice_cursor INTO @InvoiceId, @InvoiceTotal, @InvoicePaid, @InvoiceStatus;
                    END

                    CLOSE invoice_cursor;
                    DEALLOCATE invoice_cursor;

                    SET @PaymentsProcessed = @PaymentsProcessed + 1;
                END
            END

            FETCH NEXT FROM payment_cursor INTO @PaymentId, @PaymentAmount, @StudentId;
        END

        CLOSE payment_cursor;
        DEALLOCATE payment_cursor;

        COMMIT TRANSACTION;

        SELECT
            @PaymentsProcessed AS PaymentsProcessed,
            @AllocationsCreated AS AllocationsCreated,
            @TotalAllocated AS TotalAllocated;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION

        IF CURSOR_STATUS('local', 'payment_cursor') >= 0
        BEGIN
            CLOSE payment_cursor
            DEALLOCATE payment_cursor
        END

        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrSeverity INT = ERROR_SEVERITY()
        RAISERROR(@ErrMsg, @ErrSeverity, 1)
    END CATCH
END
GO
