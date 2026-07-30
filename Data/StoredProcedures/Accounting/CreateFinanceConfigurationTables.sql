-- Create Finance Configuration tables
-- Run this before SeedFinanceConfiguration.sql

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'FinanceSettings')
BEGIN
    CREATE TABLE FinanceSettings (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Key] NVARCHAR(100) NOT NULL,
        Value NVARCHAR(2000) NOT NULL,
        Description NVARCHAR(500) NULL,
        Category NVARCHAR(50) NOT NULL DEFAULT 'General',
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedBy NVARCHAR(64) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedBy NVARCHAR(64) NULL,
        UpdatedAt DATETIME2 NULL
    );
    CREATE UNIQUE INDEX IX_FinanceSettings_Key ON FinanceSettings ([Key]) WHERE IsDeleted = 0;
    PRINT 'Created FinanceSettings table';
END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AccountMappings')
BEGIN
    CREATE TABLE AccountMappings (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TransactionType NVARCHAR(100) NOT NULL,
        DebitAccountCode NVARCHAR(20) NOT NULL,
        CreditAccountCode NVARCHAR(20) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedBy NVARCHAR(64) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedBy NVARCHAR(64) NULL,
        UpdatedAt DATETIME2 NULL
    );
    CREATE UNIQUE INDEX IX_AccountMappings_TransactionType ON AccountMappings (TransactionType) WHERE IsDeleted = 0;
    PRINT 'Created AccountMappings table';
END

PRINT 'Finance configuration tables created.';
GO
