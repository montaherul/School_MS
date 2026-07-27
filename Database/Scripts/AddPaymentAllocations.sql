SET QUOTED_IDENTIFIER ON;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaymentAllocations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PaymentAllocations] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [PaymentId] INT NOT NULL,
        [FeeInvoiceId] INT NOT NULL,
        [AllocatedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Remarks] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(64) NOT NULL DEFAULT '',
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        [UpdatedBy] NVARCHAR(64) NULL,
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_PaymentAllocations] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_PaymentAllocations_Payments] FOREIGN KEY ([PaymentId]) REFERENCES [dbo].[Payments]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PaymentAllocations_FeeInvoices] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [dbo].[FeeInvoices]([Id]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_PaymentAllocations_PaymentId] ON [dbo].[PaymentAllocations] ([PaymentId]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_PaymentAllocations_FeeInvoiceId] ON [dbo].[PaymentAllocations] ([FeeInvoiceId]) WHERE [IsDeleted] = 0;
END
GO
