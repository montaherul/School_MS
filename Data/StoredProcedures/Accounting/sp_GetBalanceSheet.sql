CREATE PROCEDURE sp_GetBalanceSheet
    @AsOfDate DATETIME = NULL,
    @FinancialPeriodId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @AsOfDate IS NULL SET @AsOfDate = GETUTCDATE();

    -- Assets (AccountType = 1)
    SELECT
        a.Id AS AccountId,
        a.AccountCode,
        a.AccountName,
        a.OpeningBalance + ISNULL(SUM(gl.DebitAmount - gl.CreditAmount), 0) AS Amount
    FROM ChartOfAccounts a
    LEFT JOIN GeneralLedgerEntries gl ON a.Id = gl.AccountId
        AND gl.IsDeleted = 0
        AND gl.EntryDate <= @AsOfDate
        AND (@FinancialPeriodId IS NULL OR gl.FinancialPeriodId = @FinancialPeriodId)
    WHERE a.IsDeleted = 0 AND a.IsActive = 1 AND a.AccountType = 1
    GROUP BY a.Id, a.AccountCode, a.AccountName, a.OpeningBalance
    HAVING a.OpeningBalance + ISNULL(SUM(gl.DebitAmount - gl.CreditAmount), 0) != 0
    ORDER BY a.AccountCode;

    -- Liabilities (AccountType = 2)
    SELECT
        a.Id AS AccountId,
        a.AccountCode,
        a.AccountName,
        a.OpeningBalance + ISNULL(SUM(gl.CreditAmount - gl.DebitAmount), 0) AS Amount
    FROM ChartOfAccounts a
    LEFT JOIN GeneralLedgerEntries gl ON a.Id = gl.AccountId
        AND gl.IsDeleted = 0
        AND gl.EntryDate <= @AsOfDate
        AND (@FinancialPeriodId IS NULL OR gl.FinancialPeriodId = @FinancialPeriodId)
    WHERE a.IsDeleted = 0 AND a.IsActive = 1 AND a.AccountType = 2
    GROUP BY a.Id, a.AccountCode, a.AccountName, a.OpeningBalance
    HAVING a.OpeningBalance + ISNULL(SUM(gl.CreditAmount - gl.DebitAmount), 0) != 0
    ORDER BY a.AccountCode;

    -- Equity (AccountType = 5)
    SELECT
        a.Id AS AccountId,
        a.AccountCode,
        a.AccountName,
        a.OpeningBalance + ISNULL(SUM(gl.CreditAmount - gl.DebitAmount), 0) AS Amount
    FROM ChartOfAccounts a
    LEFT JOIN GeneralLedgerEntries gl ON a.Id = gl.AccountId
        AND gl.IsDeleted = 0
        AND gl.EntryDate <= @AsOfDate
        AND (@FinancialPeriodId IS NULL OR gl.FinancialPeriodId = @FinancialPeriodId)
    WHERE a.IsDeleted = 0 AND a.IsActive = 1 AND a.AccountType = 5
    GROUP BY a.Id, a.AccountCode, a.AccountName, a.OpeningBalance
    HAVING a.OpeningBalance + ISNULL(SUM(gl.CreditAmount - gl.DebitAmount), 0) != 0
    ORDER BY a.AccountCode;

    -- Net Income (transfer from Income - Expense)
    DECLARE @NetIncome DECIMAL(18,2);

    SELECT @NetIncome = ISNULL(SUM(gl.CreditAmount - gl.DebitAmount), 0)
    FROM ChartOfAccounts a
    INNER JOIN GeneralLedgerEntries gl ON a.Id = gl.AccountId
        AND gl.IsDeleted = 0
        AND gl.EntryDate <= @AsOfDate
        AND (@FinancialPeriodId IS NULL OR gl.FinancialPeriodId = @FinancialPeriodId)
    WHERE a.IsDeleted = 0 AND a.AccountType = 3;

    SELECT @NetIncome = @NetIncome - ISNULL(SUM(gl.DebitAmount - gl.CreditAmount), 0)
    FROM ChartOfAccounts a
    INNER JOIN GeneralLedgerEntries gl ON a.Id = gl.AccountId
        AND gl.IsDeleted = 0
        AND gl.EntryDate <= @AsOfDate
        AND (@FinancialPeriodId IS NULL OR gl.FinancialPeriodId = @FinancialPeriodId)
    WHERE a.IsDeleted = 0 AND a.AccountType = 4;

    IF @NetIncome != 0
        SELECT 0 AS AccountId, 'NET' AS AccountCode, 'Net Profit / (Loss)' AS AccountName, @NetIncome AS Amount;
END
