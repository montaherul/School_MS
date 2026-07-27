SET QUOTED_IDENTIFIER ON;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Scholarships]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Scholarships] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL DEFAULT '',
        [Description] NVARCHAR(500) NULL,
        [DiscountType] INT NOT NULL DEFAULT 0,
        [Value] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [SchoolClassId] INT NULL,
        [FeeCategoryId] INT NULL,
        [FeeTypeId] INT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [ValidFrom] DATE NULL,
        [ValidTo] DATE NULL,
        [CreatedBy] NVARCHAR(64) NOT NULL DEFAULT '',
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        [UpdatedBy] NVARCHAR(64) NULL,
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Scholarships] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE INDEX [IX_Scholarships_SchoolClassId] ON [dbo].[Scholarships] ([SchoolClassId]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Scholarships_FeeCategoryId] ON [dbo].[Scholarships] ([FeeCategoryId]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Scholarships_IsActive] ON [dbo].[Scholarships] ([IsActive]) WHERE [IsDeleted] = 0;
END
GO
