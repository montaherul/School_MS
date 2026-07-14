CREATE PROCEDURE sp_GetIncomeStatement
    @FromDate DATETIME,
    @ToDate DATETIME,
    @FinancialPeriodId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Income accounts (AccountType = 3)
    SELECT
        a.Id AS AccountId,
        a.AccountCode,
        a.AccountName,
        ISNULL(SUM(gl.CreditAmount - gl.DebitAmount), 0) AS Amount
    FROM ChartOfAccounts a
    INNER JOIN GeneralLedgerEntries gl ON a.Id = gl.AccountId
        AND gl.IsDeleted = 0
        AND gl.EntryDate >= @FromDate
        AND gl.EntryDate <= @ToDate
        AND (@FinancialPeriodId IS NULL OR gl.FinancialPeriodId = @FinancialPeriodId)
    WHERE a.IsDeleted = 0 AND a.IsActive = 1 AND a.AccountType = 3
    GROUP BY a.Id, a.AccountCode, a.AccountName
    HAVING ISNULL(SUM(gl.CreditAmount - gl.DebitAmount), 0) != 0
    ORDER BY a.AccountCode;

    -- Separator: empty row
    SELECT CAST(NULL AS INT) AS AccountId, CAST(NULL AS NVARCHAR(20)) AS AccountCode,
           CAST(NULL AS NVARCHAR(200)) AS AccountName, CAST(NULL AS DECIMAL(18,2)) AS Amount
    WHERE 1 = 0;

    -- Expense accounts (AccountType = 4)
    SELECT
        a.Id AS AccountId,
        a.AccountCode,
        a.AccountName,
        ISNULL(SUM(gl.DebitAmount - gl.CreditAmount), 0) AS Amount
    FROM ChartOfAccounts a
    INNER JOIN GeneralLedgerEntries gl ON a.Id = gl.AccountId
        AND gl.IsDeleted = 0
        AND gl.EntryDate >= @FromDate
        AND gl.EntryDate <= @ToDate
        AND (@FinancialPeriodId IS NULL OR gl.FinancialPeriodId = @FinancialPeriodId)
    WHERE a.IsDeleted = 0 AND a.IsActive = 1 AND a.AccountType = 4
    GROUP BY a.Id, a.AccountCode, a.AccountName
    HAVING ISNULL(SUM(gl.DebitAmount - gl.CreditAmount), 0) != 0
    ORDER BY a.AccountCode;
END
