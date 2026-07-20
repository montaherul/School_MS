-- AI Module: Enterprise Tables
-- ========================================

-- AIConversations: Stores chat sessions
CREATE TABLE [dbo].[AIConversations] (
    [Id]            INT            IDENTITY(1,1) NOT NULL,
    [StudentId]     INT            NOT NULL,
    [Title]         NVARCHAR(200)  NOT NULL DEFAULT N'New Chat',
    [Status]        INT            NOT NULL DEFAULT 2, -- 1=Draft, 2=Active, 3=Archived, 4=Deleted
    [IsPinned]      BIT            NOT NULL DEFAULT 0,
    [CreatedBy]     NVARCHAR(64)   NOT NULL DEFAULT N'system',
    [CreatedAt]     DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedBy]     NVARCHAR(64)   NULL,
    [UpdatedAt]     DATETIME2      NULL,
    [IsDeleted]     BIT            NOT NULL DEFAULT 0,
    CONSTRAINT [PK_AIConversations] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_AIConversations_Students] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students]([Id]) ON DELETE NO ACTION
);

CREATE NONCLUSTERED INDEX [IX_AIConversations_StudentId] ON [dbo].[AIConversations] ([StudentId]) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_AIConversations_CreatedAt] ON [dbo].[AIConversations] ([CreatedAt] DESC) WHERE [IsDeleted] = 0;

-- AIMessages: Individual messages within conversations
CREATE TABLE [dbo].[AIMessages] (
    [Id]              INT            IDENTITY(1,1) NOT NULL,
    [ConversationId]  INT            NOT NULL,
    [Role]            NVARCHAR(20)   NOT NULL, -- 'user' or 'assistant'
    [Content]         NVARCHAR(MAX)  NOT NULL,
    [PromptTokens]    INT            NULL,
    [CompletionTokens] INT           NULL,
    [Model]           NVARCHAR(100)  NULL,
    [LatencyMs]       INT            NULL,
    [CreatedBy]       NVARCHAR(64)   NOT NULL DEFAULT N'system',
    [CreatedAt]       DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedBy]       NVARCHAR(64)   NULL,
    [UpdatedAt]       DATETIME2      NULL,
    [IsDeleted]       BIT            NOT NULL DEFAULT 0,
    CONSTRAINT [PK_AIMessages] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_AIMessages_AIConversations] FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[AIConversations]([Id]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX [IX_AIMessages_ConversationId] ON [dbo].[AIMessages] ([ConversationId], [CreatedAt]) WHERE [IsDeleted] = 0;

-- AIUsage: Token usage and cost tracking
CREATE TABLE [dbo].[AIUsage] (
    [Id]              INT            IDENTITY(1,1) NOT NULL,
    [StudentId]       INT            NOT NULL,
    [ConversationId]  INT            NULL,
    [MessageId]       INT            NULL,
    [Model]           NVARCHAR(100)  NOT NULL DEFAULT N'gpt-4o-mini',
    [PromptTokens]    INT            NOT NULL DEFAULT 0,
    [CompletionTokens] INT           NOT NULL DEFAULT 0,
    [TotalTokens]     INT            NOT NULL DEFAULT 0,
    [EstimatedCost]   DECIMAL(18,6)  NOT NULL DEFAULT 0,
    [LatencyMs]       INT            NULL,
    [UsageDate]       DATE           NOT NULL DEFAULT CAST(SYSUTCDATETIME() AS DATE),
    [CreatedBy]       NVARCHAR(64)   NOT NULL DEFAULT N'system',
    [CreatedAt]       DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedBy]       NVARCHAR(64)   NULL,
    [UpdatedAt]       DATETIME2      NULL,
    [IsDeleted]       BIT            NOT NULL DEFAULT 0,
    CONSTRAINT [PK_AIUsage] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_AIUsage_Students] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AIUsage_AIConversations] FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[AIConversations]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AIUsage_AIMessages] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[AIMessages]([Id]) ON DELETE NO ACTION
);

CREATE NONCLUSTERED INDEX [IX_AIUsage_StudentId] ON [dbo].[AIUsage] ([StudentId], [UsageDate]) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_AIUsage_UsageDate] ON [dbo].[AIUsage] ([UsageDate]) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_AIUsage_ConversationId] ON [dbo].[AIUsage] ([ConversationId]) WHERE [IsDeleted] = 0;
