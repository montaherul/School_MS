-- AI Admin Module: Enterprise Tables (Phase XX+105)
-- ============================================================

-- AISettings: Key-value configuration store
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AISettings')
BEGIN
    CREATE TABLE [dbo].[AISettings] (
        [Id]            INT            IDENTITY(1,1) NOT NULL,
        [Key]           NVARCHAR(200)  NOT NULL,
        [Value]         NVARCHAR(MAX)  NOT NULL,
        [Description]   NVARCHAR(500)  NULL,
        [Category]      NVARCHAR(100)  NOT NULL DEFAULT N'General',
        [IsEncrypted]   BIT            NOT NULL DEFAULT 0,
        [DisplayOrder]  INT            NOT NULL DEFAULT 0,
        [CreatedBy]     NVARCHAR(64)   NOT NULL DEFAULT N'system',
        [CreatedAt]     DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy]     NVARCHAR(64)   NULL,
        [UpdatedAt]     DATETIME2      NULL,
        [IsDeleted]     BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AISettings] PRIMARY KEY CLUSTERED ([Id])
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_AISettings_Key] ON [dbo].[AISettings] ([Key]) WHERE [IsDeleted] = 0;
END
GO

-- AIProviders: LLM provider configurations
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AIProviders')
BEGIN
    CREATE TABLE [dbo].[AIProviders] (
        [Id]              INT            IDENTITY(1,1) NOT NULL,
        [Name]            NVARCHAR(100)  NOT NULL,
        [ProviderType]    INT            NOT NULL DEFAULT 1, -- 1=OpenAI, 2=AzureOpenAI, 3=Gemini, 4=Claude, 5=Ollama
        [BaseUrl]         NVARCHAR(500)  NULL,
        [ApiKeyEncrypted] NVARCHAR(500)  NULL,
        [IsEnabled]       BIT            NOT NULL DEFAULT 1,
        [Priority]        INT            NOT NULL DEFAULT 0,
        [RetryCount]      INT            NOT NULL DEFAULT 3,
        [TimeoutSeconds]  INT            NOT NULL DEFAULT 60,
        [CreatedBy]       NVARCHAR(64)   NOT NULL DEFAULT N'system',
        [CreatedAt]       DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy]       NVARCHAR(64)   NULL,
        [UpdatedAt]       DATETIME2      NULL,
        [IsDeleted]       BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AIProviders] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- AIModels: Model configurations per provider and role
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AIModels')
BEGIN
    CREATE TABLE [dbo].[AIModels] (
        [Id]          INT            IDENTITY(1,1) NOT NULL,
        [Name]        NVARCHAR(100)  NOT NULL,
        [ProviderId]  INT            NOT NULL,
        [Role]        INT            NOT NULL DEFAULT 1, -- 1=Student, 2=Teacher, 3=Admin
        [IsDefault]   BIT            NOT NULL DEFAULT 0,
        [MaxTokens]   INT            NOT NULL DEFAULT 2048,
        [Temperature] FLOAT          NOT NULL DEFAULT 0.7,
        [IsEnabled]   BIT            NOT NULL DEFAULT 1,
        [CreatedBy]   NVARCHAR(64)   NOT NULL DEFAULT N'system',
        [CreatedAt]   DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy]   NVARCHAR(64)   NULL,
        [UpdatedAt]   DATETIME2      NULL,
        [IsDeleted]   BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AIModels] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_AIModels_AIProviders] FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[AIProviders]([Id]) ON DELETE NO ACTION
    );
END
GO

-- AIPrompts: Versioned prompt templates
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AIPrompts')
BEGIN
    CREATE TABLE [dbo].[AIPrompts] (
        [Id]        INT            IDENTITY(1,1) NOT NULL,
        [Name]      NVARCHAR(200)  NOT NULL,
        [Role]      NVARCHAR(50)   NOT NULL DEFAULT N'Student',
        [Prompt]    NVARCHAR(MAX)  NOT NULL,
        [Version]   INT            NOT NULL DEFAULT 1,
        [IsActive]  BIT            NOT NULL DEFAULT 1,
        [Category]  NVARCHAR(100)  NULL,
        [CreatedBy] NVARCHAR(64)   NOT NULL DEFAULT N'system',
        [CreatedAt] DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy] NVARCHAR(64)   NULL,
        [UpdatedAt] DATETIME2      NULL,
        [IsDeleted] BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AIPrompts] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- AIFeatureFlags: Dynamic feature toggles
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AIFeatureFlags')
BEGIN
    CREATE TABLE [dbo].[AIFeatureFlags] (
        [Id]          INT            IDENTITY(1,1) NOT NULL,
        [Key]         NVARCHAR(100)  NOT NULL,
        [DisplayName] NVARCHAR(200)  NOT NULL,
        [IsEnabled]   BIT            NOT NULL DEFAULT 1,
        [Category]    NVARCHAR(100)  NULL,
        [Description] NVARCHAR(500)  NULL,
        [CreatedBy]   NVARCHAR(64)   NOT NULL DEFAULT N'system',
        [CreatedAt]   DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy]   NVARCHAR(64)   NULL,
        [UpdatedAt]   DATETIME2      NULL,
        [IsDeleted]   BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AIFeatureFlags] PRIMARY KEY CLUSTERED ([Id])
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_AIFeatureFlags_Key] ON [dbo].[AIFeatureFlags] ([Key]) WHERE [IsDeleted] = 0;
END
GO

-- AIQuotas: Role-based rate limits
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AIQuotas')
BEGIN
    CREATE TABLE [dbo].[AIQuotas] (
        [Id]                  INT           IDENTITY(1,1) NOT NULL,
        [Role]                NVARCHAR(50)  NOT NULL,
        [DailyLimit]          INT           NULL,
        [MinuteLimit]         INT           NULL,
        [MaxTokensPerRequest] INT           NULL,
        [IsUnlimited]         BIT           NOT NULL DEFAULT 0,
        [CreatedBy]           NVARCHAR(64)  NOT NULL DEFAULT N'system',
        [CreatedAt]           DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy]           NVARCHAR(64)  NULL,
        [UpdatedAt]           DATETIME2     NULL,
        [IsDeleted]           BIT           NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AIQuotas] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- AISecurityPolicies: Security configuration rules
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AISecurityPolicies')
BEGIN
    CREATE TABLE [dbo].[AISecurityPolicies] (
        [Id]          INT            IDENTITY(1,1) NOT NULL,
        [Key]         NVARCHAR(100)  NOT NULL,
        [Value]       NVARCHAR(MAX)  NOT NULL,
        [Description] NVARCHAR(500)  NULL,
        [Category]    NVARCHAR(100)  NULL,
        [CreatedBy]   NVARCHAR(64)   NOT NULL DEFAULT N'system',
        [CreatedAt]   DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy]   NVARCHAR(64)   NULL,
        [UpdatedAt]   DATETIME2      NULL,
        [IsDeleted]   BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AISecurityPolicies] PRIMARY KEY CLUSTERED ([Id])
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_AISecurityPolicies_Key] ON [dbo].[AISecurityPolicies] ([Key]) WHERE [IsDeleted] = 0;
END
GO

-- AIAuditLogs: Track all AI configuration changes
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AIAuditLogs')
BEGIN
    CREATE TABLE [dbo].[AIAuditLogs] (
        [Id]          INT            IDENTITY(1,1) NOT NULL,
        [Action]      NVARCHAR(100)  NOT NULL,
        [EntityType]  NVARCHAR(100)  NOT NULL,
        [EntityId]    INT            NULL,
        [OldValue]    NVARCHAR(MAX)  NULL,
        [NewValue]    NVARCHAR(MAX)  NULL,
        [IpAddress]   NVARCHAR(50)   NULL,
        [UserAgent]   NVARCHAR(500)  NULL,
        [CreatedBy]   NVARCHAR(64)   NOT NULL DEFAULT N'system',
        [CreatedAt]   DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy]   NVARCHAR(64)   NULL,
        [UpdatedAt]   DATETIME2      NULL,
        [IsDeleted]   BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AIAuditLogs] PRIMARY KEY CLUSTERED ([Id])
    );
    CREATE NONCLUSTERED INDEX [IX_AIAuditLogs_EntityType_EntityId] ON [dbo].[AIAuditLogs] ([EntityType], [EntityId]);
    CREATE NONCLUSTERED INDEX [IX_AIAuditLogs_CreatedAt] ON [dbo].[AIAuditLogs] ([CreatedAt] DESC);
END
GO

-- AIKnowledgeBases: RAG document metadata
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AIKnowledgeBases')
BEGIN
    CREATE TABLE [dbo].[AIKnowledgeBases] (
        [Id]          INT            IDENTITY(1,1) NOT NULL,
        [Name]        NVARCHAR(300)  NOT NULL,
        [FilePath]    NVARCHAR(1000) NULL,
        [ContentType] NVARCHAR(100)  NOT NULL DEFAULT N'text',
        [Size]        BIGINT         NOT NULL DEFAULT 0,
        [Version]     INT            NOT NULL DEFAULT 1,
        [IsActive]    BIT            NOT NULL DEFAULT 1,
        [Description] NVARCHAR(500)  NULL,
        [CreatedBy]   NVARCHAR(64)   NOT NULL DEFAULT N'system',
        [CreatedAt]   DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy]   NVARCHAR(64)   NULL,
        [UpdatedAt]   DATETIME2      NULL,
        [IsDeleted]   BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AIKnowledgeBases] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- AIKnowledgeChunks: Document chunks for RAG
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AIKnowledgeChunks')
BEGIN
    CREATE TABLE [dbo].[AIKnowledgeChunks] (
        [Id]              INT            IDENTITY(1,1) NOT NULL,
        [KnowledgeBaseId] INT            NOT NULL,
        [Content]         NVARCHAR(MAX)  NOT NULL,
        [ChunkIndex]      INT            NOT NULL DEFAULT 0,
        [TokenCount]      INT            NOT NULL DEFAULT 0,
        [CreatedBy]       NVARCHAR(64)   NOT NULL DEFAULT N'system',
        [CreatedAt]       DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy]       NVARCHAR(64)   NULL,
        [UpdatedAt]       DATETIME2      NULL,
        [IsDeleted]       BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AIKnowledgeChunks] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_AIKnowledgeChunks_AIKnowledgeBases] FOREIGN KEY ([KnowledgeBaseId]) REFERENCES [dbo].[AIKnowledgeBases]([Id]) ON DELETE CASCADE
    );
END
GO

-- AIHealthChecks: Component health monitoring
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AIHealthChecks')
BEGIN
    CREATE TABLE [dbo].[AIHealthChecks] (
        [Id]             INT           IDENTITY(1,1) NOT NULL,
        [Component]      NVARCHAR(100) NOT NULL,
        [Status]         NVARCHAR(50)  NOT NULL DEFAULT N'Healthy',
        [LastChecked]    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
        [ResponseTimeMs] INT           NULL,
        [ErrorMessage]   NVARCHAR(MAX) NULL,
        [CreatedBy]      NVARCHAR(64)  NOT NULL DEFAULT N'system',
        [CreatedAt]      DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy]      NVARCHAR(64)  NULL,
        [UpdatedAt]      DATETIME2     NULL,
        [IsDeleted]      BIT           NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AIHealthChecks] PRIMARY KEY CLUSTERED ([Id])
    );
    CREATE NONCLUSTERED INDEX [IX_AIHealthChecks_LastChecked] ON [dbo].[AIHealthChecks] ([LastChecked] DESC);
END
GO

-- AIDashboardCache: Cached dashboard data
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AIDashboardCaches')
BEGIN
    CREATE TABLE [dbo].[AIDashboardCaches] (
        [Id]        INT            IDENTITY(1,1) NOT NULL,
        [Key]       NVARCHAR(100)  NOT NULL,
        [JsonData]  NVARCHAR(MAX)  NOT NULL,
        [ExpiresAt] DATETIME2      NOT NULL,
        [CreatedBy] NVARCHAR(64)   NOT NULL DEFAULT N'system',
        [CreatedAt] DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedBy] NVARCHAR(64)   NULL,
        [UpdatedAt] DATETIME2      NULL,
        [IsDeleted] BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AIDashboardCaches] PRIMARY KEY CLUSTERED ([Id])
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_AIDashboardCaches_Key] ON [dbo].[AIDashboardCaches] ([Key]);
END
GO

PRINT 'All 11 AI admin tables created.';
GO
