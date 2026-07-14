CREATE PROCEDURE sp_GetTrialBalance
    @AsOfDate DATETIME = NULL,
    @FinancialPeriodId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @AsOfDate IS NULL SET @AsOfDate = GETUTCDATE();

    WITH AccountBalances AS (
        SELECT
            a.Id AS AccountId,
            a.AccountCode,
            a.AccountName,
            CASE a.AccountType
                WHEN 1 THEN 'Asset'
                WHEN 2 THEN 'Liability'
                WHEN 3 THEN 'Income'
                WHEN 4 THEN 'Expense'
                WHEN 5 THEN 'Equity'
            END AS AccountType,
            a.OpeningBalance,
            ISNULL(SUM(gl.DebitAmount), 0) AS TotalDebit,
            ISNULL(SUM(gl.CreditAmount), 0) AS TotalCredit
        FROM ChartOfAccounts a
        LEFT JOIN GeneralLedgerEntries gl ON a.Id = gl.AccountId
            AND gl.IsDeleted = 0
            AND gl.EntryDate <= @AsOfDate
            AND (@FinancialPeriodId IS NULL OR gl.FinancialPeriodId = @FinancialPeriodId)
        WHERE a.IsDeleted = 0 AND a.IsActive = 1
        GROUP BY a.Id, a.AccountCode, a.AccountName, a.AccountType, a.OpeningBalance
    )
    SELECT
        AccountId,
        AccountCode,
        AccountName,
        AccountType,
        CASE WHEN OpeningBalance > 0 THEN OpeningBalance ELSE 0 END AS OpeningDebit,
        CASE WHEN OpeningBalance < 0 THEN ABS(OpeningBalance) ELSE 0 END AS OpeningCredit,
        TotalDebit,
        TotalCredit,
        CASE
            WHEN AccountType IN (1, 4) THEN OpeningBalance + TotalDebit - TotalCredit
            ELSE OpeningBalance + TotalCredit - TotalDebit
        END AS ClosingBalance
    FROM AccountBalances
    WHERE OpeningBalance != 0 OR TotalDebit != 0 OR TotalCredit != 0
    ORDER BY AccountType, AccountCode;
END
