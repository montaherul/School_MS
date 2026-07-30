-- Seed Finance Configuration: default settings + account mappings
-- Run after Accounting migration (tables must exist)

-- ============================================================
-- SECTION 1: Default Finance Settings
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM FinanceSettings WHERE [Key] = 'FiscalYearStart' AND IsDeleted = 0)
    INSERT INTO FinanceSettings ([Key], Value, Description, Category, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('FiscalYearStart', '01-01', 'Fiscal year start date (MM-DD)', 'Fiscal', 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM FinanceSettings WHERE [Key] = 'FiscalYearEnd' AND IsDeleted = 0)
    INSERT INTO FinanceSettings ([Key], Value, Description, Category, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('FiscalYearEnd', '12-31', 'Fiscal year end date (MM-DD)', 'Fiscal', 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM FinanceSettings WHERE [Key] = 'AutoCreatePeriods' AND IsDeleted = 0)
    INSERT INTO FinanceSettings ([Key], Value, Description, Category, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('AutoCreatePeriods', 'true', 'Auto-create financial periods for new fiscal years', 'Fiscal', 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM FinanceSettings WHERE [Key] = 'GracePeriodDays' AND IsDeleted = 0)
    INSERT INTO FinanceSettings ([Key], Value, Description, Category, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('GracePeriodDays', '30', 'Grace period in days before late fee applies', 'Policy', 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM FinanceSettings WHERE [Key] = 'WriteOffThreshold' AND IsDeleted = 0)
    INSERT INTO FinanceSettings ([Key], Value, Description, Category, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('WriteOffThreshold', '1.00', 'Auto write-off threshold for small balances', 'Threshold', 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM FinanceSettings WHERE [Key] = 'DefaultDueDay' AND IsDeleted = 0)
    INSERT INTO FinanceSettings ([Key], Value, Description, Category, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('DefaultDueDay', '10', 'Default due day of month for invoices', 'Policy', 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM FinanceSettings WHERE [Key] = 'MinPaymentPercentage' AND IsDeleted = 0)
    INSERT INTO FinanceSettings ([Key], Value, Description, Category, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('MinPaymentPercentage', '0', 'Minimum payment percentage required (0 = no minimum)', 'Policy', 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM FinanceSettings WHERE [Key] = 'EnforcePeriodClosing' AND IsDeleted = 0)
    INSERT INTO FinanceSettings ([Key], Value, Description, Category, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('EnforcePeriodClosing', 'true', 'Block posting into closed financial periods', 'Policy', 0, 'system', GETUTCDATE());

-- ============================================================
-- SECTION 2: Default Account Mappings
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'FeeCollection' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('FeeCollection', '1-001', '1-101', 'Fee collection: Dr Cash, Cr Receivable', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'FeeWaiver' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('FeeWaiver', '1-101', '3-501', 'Fee waiver: Dr Receivable (negative), Cr Waiver', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'FeeDiscount' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('FeeDiscount', '4-201', '1-101', 'Fee discount: Dr Discount, Cr Receivable', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'FeeRefund' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('FeeRefund', '3-101', '1-001', 'Fee refund: Dr Income (reverse), Cr Cash', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'LateFee' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('LateFee', '1-101', '3-601', 'Late fee: Dr Receivable, Cr Late Fee Income', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'Fine' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('Fine', '1-101', '3-602', 'Fine: Dr Receivable, Cr Fine Income', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'BankReceipt' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('BankReceipt', '1-001', '3-101', 'Bank receipt: Dr Cash, Cr Income', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'BankPayment' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('BankPayment', '4-101', '1-001', 'Bank payment: Dr Expense, Cr Cash', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'AdmissionFee' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('AdmissionFee', '1-001', '3-201', 'Admission fee: Dr Cash, Cr Admission Fee Income', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'AdmissionRefund' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('AdmissionRefund', '3-201', '1-001', 'Admission refund: Dr Income (reverse), Cr Cash', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'WriteOff' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('WriteOff', '4-301', '1-101', 'Write-off: Dr Bad Debt, Cr Receivable', 1, 0, 'system', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AccountMappings WHERE TransactionType = 'BadDebt' AND IsDeleted = 0)
    INSERT INTO AccountMappings (TransactionType, DebitAccountCode, CreditAccountCode, Description, IsActive, IsDeleted, CreatedBy, CreatedAt)
    VALUES ('BadDebt', '4-301', '1-101', 'Bad debt: Dr Bad Debt, Cr Receivable', 1, 0, 'system', GETUTCDATE());

PRINT 'Finance configuration seed complete.';
GO
