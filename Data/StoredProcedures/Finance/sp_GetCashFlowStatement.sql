CREATE OR ALTER PROCEDURE sp_GetCashFlowStatement
    @Year INT,
    @Month INT = 0,
    @PeriodType INT = 3
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @StartDate DATE, @EndDate DATE;
    IF @PeriodType = 1 AND @Month > 0
    BEGIN
        SET @StartDate = DATEFROMPARTS(@Year, @Month, 1);
        SET @EndDate = EOMONTH(@StartDate);
    END
    ELSE IF @PeriodType = 2
    BEGIN
        DECLARE @QuarterStart INT = ((@Month - 1) / 3) * 3 + 1;
        SET @StartDate = DATEFROMPARTS(@Year, @QuarterStart, 1);
        SET @EndDate = EOMONTH(DATEFROMPARTS(@Year, @QuarterStart + 2, 1));
    END
    ELSE
    BEGIN
        SET @StartDate = DATEFROMPARTS(@Year, 1, 1);
        SET @EndDate = DATEFROMPARTS(@Year, 12, 31);
    END

    SELECT @EndDate AS AsOfDate,
           CASE @PeriodType WHEN 1 THEN 'Monthly' WHEN 2 THEN 'Quarterly' ELSE 'Yearly' END AS PeriodName,
           0 AS NetCashFlow, 0 AS OpeningBalance, 0 AS ClosingBalance;

    -- Operating Activities
    SELECT 'Fee Collections' AS Label, ISNULL(SUM(e.Credit), 0) AS Amount, 0 AS IsTotal
    FROM GeneralLedgerEntries e
    JOIN ChartOfAccounts a ON e.AccountId = a.Id
    WHERE a.AccountType = 1 AND a.Code LIKE '1-00%'
      AND e.EntryDate >= @StartDate AND e.EntryDate <= @EndDate
    UNION ALL
    SELECT 'Admission Fees', ISNULL(SUM(je.Amount), 0), 0
    FROM JournalEntries j JOIN JournalEntryLines je ON j.Id = je.JournalEntryId
    WHERE j.EntryType = 2 AND j.EntryDate >= @StartDate AND j.EntryDate <= @EndDate
    UNION ALL
    SELECT 'Other Income', ISNULL(SUM(Amount), 0), 0
    FROM Expenses WHERE Status = 5 AND IsDeleted = 0
      AND ExpenseDate >= @StartDate AND ExpenseDate <= @EndDate
      AND Amount < 0
    UNION ALL
    SELECT 'Salary Paid', ISNULL(SUM(Amount), 0), 0
    FROM Expenses WHERE Status = 5 AND IsDeleted = 0
      AND ExpenseDate >= @StartDate AND ExpenseDate <= @EndDate
      AND Amount > 0 AND ExpenseCategoryId IN (SELECT Id FROM ExpenseCategories WHERE Name LIKE '%Salary%')
    UNION ALL
    SELECT 'Utilities', ISNULL(SUM(Amount), 0), 0
    FROM Expenses WHERE Status = 5 AND IsDeleted = 0
      AND ExpenseDate >= @StartDate AND ExpenseDate <= @EndDate
      AND Amount > 0 AND ExpenseCategoryId IN (SELECT Id FROM ExpenseCategories WHERE Name LIKE '%Utility%')
    UNION ALL
    SELECT 'Maintenance', ISNULL(SUM(Amount), 0), 0
    FROM Expenses WHERE Status = 5 AND IsDeleted = 0
      AND ExpenseDate >= @StartDate AND ExpenseDate <= @EndDate
      AND Amount > 0 AND ExpenseCategoryId IN (SELECT Id FROM ExpenseCategories WHERE Name LIKE '%Maintenance%')
    UNION ALL
    SELECT 'Net Operating Cash Flow', ISNULL(SUM(
        CASE WHEN a.AccountType IN (1, 4) THEN e.Credit - e.Debit
        ELSE e.Debit - e.Credit END
    ), 0), 1 AS IsTotal
    FROM GeneralLedgerEntries e
    JOIN ChartOfAccounts a ON e.AccountId = a.Id
    WHERE a.AccountType IN (1, 3, 4)
      AND e.EntryDate >= @StartDate AND e.EntryDate <= @EndDate;

    -- Investing Activities
    SELECT 'Equipment Purchase' AS Label, ISNULL(SUM(Amount), 0) AS Amount, 0 AS IsTotal
    FROM Expenses WHERE Status = 5 AND IsDeleted = 0
      AND ExpenseDate >= @StartDate AND ExpenseDate <= @EndDate
      AND ExpenseCategoryId IN (SELECT Id FROM ExpenseCategories WHERE Name LIKE '%Hardware%' OR Name LIKE '%Software%')
    UNION ALL
    SELECT 'Furniture & Fixtures', ISNULL(SUM(Amount), 0), 0
    FROM Expenses WHERE Status = 5 AND IsDeleted = 0
      AND ExpenseDate >= @StartDate AND ExpenseDate <= @EndDate
      AND ExpenseCategoryId IN (SELECT Id FROM ExpenseCategories WHERE Name LIKE '%Maintenance%')
    UNION ALL
    SELECT 'Net Investing Cash Flow', ISNULL(SUM(Amount), 0) * -1, 1
    FROM Expenses WHERE Status = 5 AND IsDeleted = 0
      AND ExpenseDate >= @StartDate AND ExpenseDate <= @EndDate
      AND Amount > 0;

    -- Financing Activities
    SELECT 'Loans Received' AS Label, 0 AS Amount, 0 AS IsTotal
    UNION ALL
    SELECT 'Capital Investment', 0, 0
    UNION ALL
    SELECT 'Loan Payments', 0, 0
    UNION ALL
    SELECT 'Net Financing Cash Flow', 0, 1;
END
GO
