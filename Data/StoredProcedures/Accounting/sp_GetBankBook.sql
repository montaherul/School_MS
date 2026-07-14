CREATE PROCEDURE sp_GetBankBook
    @AccountId INT = NULL,
    @BankAccountType INT = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL,
    @FinancialPeriodId INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    IF @FromDate IS NULL SET @FromDate = DATEADD(MONTH, -1, GETUTCDATE());
    IF @ToDate IS NULL SET @ToDate = GETUTCDATE();

    -- Get opening balance before the period
    DECLARE @OpeningBalance DECIMAL(18,2);
    SELECT @OpeningBalance = ISNULL(SUM(
        CASE WHEN TransactionType IN (1, 5) THEN Amount ELSE -Amount END
    ), 0)
    FROM BankTransactions
    WHERE IsDeleted = 0
        AND TransactionDate < @FromDate
        AND (@AccountId IS NULL OR AccountId = @AccountId)
        AND (@BankAccountType IS NULL OR BankAccountType = @BankAccountType);

    -- Paged results with running balance
    SELECT
        bt.Id,
        bt.AccountId,
        a.AccountName,
        CASE bt.BankAccountType
            WHEN 1 THEN 'Cash'
            WHEN 2 THEN 'Bank'
            WHEN 3 THEN 'bKash'
            WHEN 4 THEN 'Nagad'
            WHEN 5 THEN 'Rocket'
        END AS BankAccountType,
        bt.TransactionDate,
        CASE bt.TransactionType
            WHEN 1 THEN 'Deposit'
            WHEN 2 THEN 'Withdrawal'
            WHEN 3 THEN 'Transfer'
            WHEN 4 THEN 'Charge'
            WHEN 5 THEN 'Reconciliation'
        END AS TransactionType,
        bt.Amount,
        bt.ReferenceNo,
        bt.ChequeNo,
        bt.Description,
        bt.CounterParty,
        bt.IsReconciled,
        SUM(CASE WHEN bt2.TransactionType IN (1, 5) THEN bt2.Amount ELSE -bt2.Amount END)
            OVER (ORDER BY bt.TransactionDate, bt.Id) + ISNULL(@OpeningBalance, 0) AS RunningBalance,
        COUNT(*) OVER() AS TotalRecords
    FROM BankTransactions bt
    INNER JOIN ChartOfAccounts a ON bt.AccountId = a.Id
    LEFT JOIN BankTransactions bt2 ON bt2.Id <= bt.Id AND bt2.IsDeleted = 0
        AND (@AccountId IS NULL OR bt2.AccountId = @AccountId)
        AND (@BankAccountType IS NULL OR bt2.BankAccountType = @BankAccountType)
    WHERE bt.IsDeleted = 0
        AND bt.TransactionDate >= @FromDate AND bt.TransactionDate <= @ToDate
        AND (@AccountId IS NULL OR bt.AccountId = @AccountId)
        AND (@BankAccountType IS NULL OR bt.BankAccountType = @BankAccountType)
        AND (@FinancialPeriodId IS NULL OR bt.FinancialPeriodId = @FinancialPeriodId)
    ORDER BY bt.TransactionDate DESC, bt.Id DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Summary
    SELECT
        ISNULL(@OpeningBalance, 0) AS OpeningBalance,
        ISNULL(SUM(CASE WHEN TransactionType IN (1, 5) THEN Amount ELSE 0 END), 0) AS TotalDeposits,
        ISNULL(SUM(CASE WHEN TransactionType IN (2, 3, 4) THEN Amount ELSE 0 END), 0) AS TotalWithdrawals,
        ISNULL(@OpeningBalance, 0) + ISNULL(SUM(CASE WHEN TransactionType IN (1, 5) THEN Amount ELSE -Amount END), 0) AS ClosingBalance,
        ISNULL(SUM(CASE WHEN IsReconciled = 0 AND TransactionType IN (1, 5) THEN Amount ELSE 0 END), 0) AS UnclearedBalance
    FROM BankTransactions
    WHERE IsDeleted = 0
        AND TransactionDate >= @FromDate AND TransactionDate <= @ToDate
        AND (@AccountId IS NULL OR AccountId = @AccountId)
        AND (@BankAccountType IS NULL OR BankAccountType = @BankAccountType)
        AND (@FinancialPeriodId IS NULL OR FinancialPeriodId = @FinancialPeriodId);
END
