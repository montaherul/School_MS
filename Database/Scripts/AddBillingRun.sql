-- ============================================================================
-- Script: AddBillingRun
-- Purpose: Create BillingRuns table for billing run history
-- ============================================================================

SET QUOTED_IDENTIFIER ON;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BillingRuns]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BillingRuns] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [RunType] NVARCHAR(50) NOT NULL DEFAULT '',
        [AcademicYearId] INT NOT NULL DEFAULT 0,
        [InvoicesGenerated] INT NOT NULL DEFAULT 0,
        [StudentsProcessed] INT NOT NULL DEFAULT 0,
        [TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [CompletedAt] DATETIME2 NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Completed',
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [CreatedBy] NVARCHAR(64) NOT NULL DEFAULT '',
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        [UpdatedBy] NVARCHAR(64) NULL,
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_BillingRuns] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE INDEX [IX_BillingRuns_RunType] ON [dbo].[BillingRuns] ([RunType]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_BillingRuns_AcademicYearId] ON [dbo].[BillingRuns] ([AcademicYearId]) WHERE [IsDeleted] = 0;
END

GO

-- Unique index on Payments(FeeInvoiceId, ReferenceNo) if not exists
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Payments_FeeInvoiceId_ReferenceNo')
BEGIN
    CREATE UNIQUE INDEX [IX_Payments_FeeInvoiceId_ReferenceNo] ON [dbo].[Payments] ([FeeInvoiceId], [ReferenceNo]) WHERE [IsDeleted] = 0 AND [ReferenceNo] IS NOT NULL;
END
GO
