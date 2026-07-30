-- Seed Chart of Accounts for Finance Posting Engine
-- Required by FinancePostingService for Discount, Late Fee, Fine posting
-- Execute after Accounting migration has created the ChartOfAccounts table

IF NOT EXISTS (SELECT 1 FROM ChartOfAccounts WHERE AccountCode = '4-201' AND IsDeleted = 0)
BEGIN
    INSERT INTO ChartOfAccounts (AccountCode, AccountName, Description, AccountType, IsActive, OpeningBalance, DisplayOrder, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('4-201', N'Discount Allowed', N'Discounts given on fee collections', 4, 1, 0, 201, 'system', GETUTCDATE(), 0);
    PRINT 'Created account: 4-201 Discount Allowed';
END

IF NOT EXISTS (SELECT 1 FROM ChartOfAccounts WHERE AccountCode = '3-601' AND IsDeleted = 0)
BEGIN
    INSERT INTO ChartOfAccounts (AccountCode, AccountName, Description, AccountType, IsActive, OpeningBalance, DisplayOrder, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('3-601', N'Late Fee Income', N'Late fee charges collected from overdue invoices', 3, 1, 0, 601, 'system', GETUTCDATE(), 0);
    PRINT 'Created account: 3-601 Late Fee Income';
END

IF NOT EXISTS (SELECT 1 FROM ChartOfAccounts WHERE AccountCode = '3-602' AND IsDeleted = 0)
BEGIN
    INSERT INTO ChartOfAccounts (AccountCode, AccountName, Description, AccountType, IsActive, OpeningBalance, DisplayOrder, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('3-602', N'Fine Income', N'Fine/penalty charges collected from students', 3, 1, 0, 602, 'system', GETUTCDATE(), 0);
    PRINT 'Created account: 3-602 Fine Income';
END

IF NOT EXISTS (SELECT 1 FROM ChartOfAccounts WHERE AccountCode = '4-301' AND IsDeleted = 0)
BEGIN
    INSERT INTO ChartOfAccounts (AccountCode, AccountName, Description, AccountType, IsActive, OpeningBalance, DisplayOrder, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('4-301', N'Bad Debt Expense', N'Write-offs and bad debt expense', 4, 1, 0, 301, 'system', GETUTCDATE(), 0);
    PRINT 'Created account: 4-301 Bad Debt Expense';
END

PRINT 'Accounting posting accounts seed complete.';
GO
