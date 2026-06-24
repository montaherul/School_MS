CREATE OR ALTER PROCEDURE [dbo].[sp_GetGuardianFees]
    @GuardianId INT,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM StudentGuardians WHERE GuardianId = @GuardianId AND StudentId = @StudentId AND IsDeleted = 0)
    BEGIN
        SELECT 0 AS TotalDue, 0 AS TotalPaid, 0 AS TotalInvoiced;
        RETURN;
    END

    -- Summary
    SELECT 
        ISNULL(SUM(CASE WHEN fi.Status <> 3 THEN fi.TotalAmount - fi.PaidAmount ELSE 0 END), 0) AS TotalDue,
        ISNULL(SUM(fi.PaidAmount), 0) AS TotalPaid,
        ISNULL(SUM(fi.TotalAmount), 0) AS TotalInvoiced
FROM FeeInvoices fi WITH(NOLOCK)
    WHERE fi.StudentId = @StudentId AND fi.IsDeleted = 0;

    -- Invoices
    SELECT 
        fi.Id,
        fi.InvoiceNo,
        fi.TotalAmount,
        fi.PaidAmount,
        (fi.TotalAmount - fi.PaidAmount) AS DueAmount,
        fi.Status,
        CASE fi.Status
            WHEN 1 THEN 'Unpaid'
            WHEN 2 THEN 'Partial'
            WHEN 3 THEN 'Paid'
            WHEN 4 THEN 'Waived'
            ELSE 'Unknown'
        END AS StatusName,
        fi.DueDate
FROM FeeInvoices fi WITH(NOLOCK)
    WHERE fi.StudentId = @StudentId AND fi.IsDeleted = 0
    ORDER BY fi.DueDate DESC, fi.Id DESC;

    -- Payments
    SELECT 
        p.Id AS ReceiptNo,
        p.Amount,
        p.Method,
        p.PaidAt,
        p.ReferenceNo AS Reference
FROM Payments p WITH(NOLOCK)
INNER JOIN FeeInvoices fi WITH(NOLOCK) ON p.FeeInvoiceId = fi.Id AND fi.StudentId = @StudentId AND fi.IsDeleted = 0
    WHERE p.IsDeleted = 0
    ORDER BY p.PaidAt DESC;
END
GO
