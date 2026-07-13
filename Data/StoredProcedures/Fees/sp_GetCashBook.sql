-- ============================================================================
-- Stored Procedure: sp_GetCashBook
-- Purpose: Daily CASH BOOK derived from cash transactions only (Payments and
-- approved Refunds). Invoices, waivers, discounts and late fees are RECEIVABLE
-- entries and are intentionally excluded from the cash book.
--
-- Result set 1: Opening cash balance (cash received before @FromDate minus
--                approved refunds before @FromDate).
-- Result set 2: Daily cash-in / cash-out with running closing balance.
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetCashBook
    @FromDate DATE,
    @ToDate DATE,
    @AcademicYearId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Opening balance: cash received before the period minus approved refunds before the period
    DECLARE @OpeningBalance DECIMAL(18,2) = 0;

    SELECT @OpeningBalance = ISNULL(SUM(p.Amount + p.LateFee), 0)
    FROM Payments p WITH(NOLOCK)
    WHERE p.IsDeleted = 0
      AND CAST(p.PaidAt AS DATE) < @FromDate
      AND (@AcademicYearId IS NULL OR EXISTS (
          SELECT 1 FROM FeeInvoices fi WITH(NOLOCK)
          WHERE fi.Id = p.FeeInvoiceId AND fi.IsDeleted = 0 AND fi.AcademicYearId = @AcademicYearId));

    SELECT @OpeningBalance = @OpeningBalance - ISNULL(SUM(r.RefundAmount), 0)
    FROM FeeRefunds r WITH(NOLOCK)
    WHERE r.IsApproved = 1
      AND CAST(r.RefundDate AS DATE) < @FromDate
      AND (@AcademicYearId IS NULL OR EXISTS (
          SELECT 1 FROM Payments p2 WITH(NOLOCK)
          JOIN FeeInvoices fi2 WITH(NOLOCK) ON fi2.Id = p2.FeeInvoiceId
          WHERE p2.Id = r.FeePaymentId AND p2.IsDeleted = 0 AND fi2.IsDeleted = 0 AND fi2.AcademicYearId = @AcademicYearId));

    -- Result set 1: opening cash balance
    SELECT @OpeningBalance AS OpeningBalance;

    -- Result set 2: daily cash flow (payments = cash in, approved refunds = cash out)
    SELECT
        TxnDate,
        SUM(CashIn)  AS CashIn,
        SUM(CashOut) AS CashOut,
        SUM(CashIn) - SUM(CashOut) AS NetChange,
        SUM(PaymentCount) AS PaymentCount,
        SUM(RefundCount)  AS RefundCount,
        SUM(EntryCount)   AS EntryCount
    FROM (
        SELECT
            CAST(p.PaidAt AS DATE) AS TxnDate,
            ISNULL(p.Amount + p.LateFee, 0) AS CashIn,
            0 AS CashOut,
            1 AS PaymentCount,
            0 AS RefundCount,
            1 AS EntryCount
        FROM Payments p WITH(NOLOCK)
        WHERE p.IsDeleted = 0
          AND CAST(p.PaidAt AS DATE) BETWEEN @FromDate AND @ToDate
          AND (@AcademicYearId IS NULL OR EXISTS (
              SELECT 1 FROM FeeInvoices fi WITH(NOLOCK)
              WHERE fi.Id = p.FeeInvoiceId AND fi.IsDeleted = 0 AND fi.AcademicYearId = @AcademicYearId))
        UNION ALL
        SELECT
            CAST(r.RefundDate AS DATE) AS TxnDate,
            0 AS CashIn,
            ISNULL(r.RefundAmount, 0) AS CashOut,
            0 AS PaymentCount,
            1 AS RefundCount,
            1 AS EntryCount
        FROM FeeRefunds r WITH(NOLOCK)
        WHERE r.IsApproved = 1
          AND CAST(r.RefundDate AS DATE) BETWEEN @FromDate AND @ToDate
          AND (@AcademicYearId IS NULL OR EXISTS (
              SELECT 1 FROM Payments p2 WITH(NOLOCK)
              JOIN FeeInvoices fi2 WITH(NOLOCK) ON fi2.Id = p2.FeeInvoiceId
              WHERE p2.Id = r.FeePaymentId AND p2.IsDeleted = 0 AND fi2.IsDeleted = 0 AND fi2.AcademicYearId = @AcademicYearId))
    ) t
    GROUP BY TxnDate
    ORDER BY TxnDate;
END;
GO
