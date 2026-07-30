-- ============================================================================
-- Stored Procedure: sp_GetCashBook
-- Purpose: Daily CASH BOOK derived from General Ledger entries against cash
-- accounts (1-001 Cash in Hand, 1-003 SSLCommerz Clearing). General Ledger is
-- the single source of truth for all cash movements.
--
-- Result set 1: Opening cash balance (net debit entries on cash accounts
--                before @FromDate, i.e. cash received minus cash disbursed).
-- Result set 2: Daily cash-in / cash-out with running closing balance.
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetCashBook
    @FromDate DATE,
    @ToDate DATE,
    @AcademicYearId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Opening balance: net debits on cash accounts before the period
    DECLARE @OpeningBalance DECIMAL(18,2) = 0;

    SELECT @OpeningBalance = ISNULL(SUM(gle.DebitAmount - gle.CreditAmount), 0)
    FROM GeneralLedgerEntries gle WITH(NOLOCK)
    JOIN ChartOfAccounts coa WITH(NOLOCK) ON coa.Id = gle.AccountId
    WHERE coa.AccountCode IN ('1-001', '1-003')
      AND coa.IsActive = 1
      AND CAST(gle.EntryDate AS DATE) < @FromDate
      AND gle.IsDeleted = 0;

    -- Result set 1: opening cash balance
    SELECT @OpeningBalance AS OpeningBalance;

    -- Result set 2: daily cash flow from General Ledger cash accounts
    SELECT
        CAST(gle.EntryDate AS DATE) AS TxnDate,
        ISNULL(SUM(gle.DebitAmount), 0) AS CashIn,
        ISNULL(SUM(gle.CreditAmount), 0) AS CashOut,
        ISNULL(SUM(gle.DebitAmount - gle.CreditAmount), 0) AS NetChange,
        COUNT(*) AS PaymentCount,
        0 AS RefundCount,
        COUNT(*) AS EntryCount
    FROM GeneralLedgerEntries gle WITH(NOLOCK)
    JOIN ChartOfAccounts coa WITH(NOLOCK) ON coa.Id = gle.AccountId
    WHERE coa.AccountCode IN ('1-001', '1-003')
      AND coa.IsActive = 1
      AND CAST(gle.EntryDate AS DATE) BETWEEN @FromDate AND @ToDate
      AND gle.IsDeleted = 0
    GROUP BY CAST(gle.EntryDate AS DATE)
    ORDER BY CAST(gle.EntryDate AS DATE);
END;
GO
