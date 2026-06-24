-- Notification Queue for Email/SMS/In-App background processing
-- Supports: retry (3 attempts), status tracking, priority ordering

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'NotificationQueue')
BEGIN
    CREATE TABLE [dbo].[NotificationQueue] (
        [Id]             BIGINT           IDENTITY(1,1) PRIMARY KEY,
        [Channel]        NVARCHAR(20)     NOT NULL DEFAULT 'Email',  -- Email, SMS, InApp
        [Recipient]      NVARCHAR(256)    NOT NULL,
        [Subject]        NVARCHAR(500)    NULL,
        [Body]           NVARCHAR(MAX)    NULL,
        [Priority]       INT              NOT NULL DEFAULT 0,       -- higher = more urgent
        [Status]         NVARCHAR(20)     NOT NULL DEFAULT 'Pending', -- Pending, Sent, Failed
        [RetryCount]     INT              NOT NULL DEFAULT 0,
        [MaxRetries]     INT              NOT NULL DEFAULT 3,
        [LastError]      NVARCHAR(MAX)    NULL,
        [ScheduledAt]    DATETIME2        NULL,
        [SentAt]         DATETIME2        NULL,
        [CreatedAt]      DATETIME2        NOT NULL DEFAULT SYSDATETIME(),
        [CreatedBy]      INT              NULL,
        [ReferenceId]    INT              NULL,                     -- EntityId (e.g. ExamId)
        [ReferenceType]  NVARCHAR(100)    NULL,                     -- e.g. 'ResultPublished'
        INDEX [IX_NQ_Status] ([Status], [Priority] DESC),
        INDEX [IX_NQ_Scheduled] ([ScheduledAt]) WHERE [ScheduledAt] IS NOT NULL
    );
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Notification_Enqueue]
    @Channel        NVARCHAR(20),
    @Recipient      NVARCHAR(256),
    @Subject        NVARCHAR(500) = NULL,
    @Body           NVARCHAR(MAX) = NULL,
    @Priority       INT           = 0,
    @ScheduledAt    DATETIME2     = NULL,
    @CreatedBy      INT           = NULL,
    @ReferenceId    INT           = NULL,
    @ReferenceType  NVARCHAR(100) = NULL,
    @NotificationId BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[NotificationQueue]
        ([Channel], [Recipient], [Subject], [Body], [Priority], [Status],
         [ScheduledAt], [CreatedBy], [ReferenceId], [ReferenceType])
    VALUES
        (@Channel, @Recipient, @Subject, @Body, @Priority, 'Pending',
         @ScheduledAt, @CreatedBy, @ReferenceId, @ReferenceType);

    SET @NotificationId = SCOPE_IDENTITY();

    RETURN @NotificationId;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Notification_Dequeue]
    @BatchSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    -- Atomically claim next batch of pending notifications
    UPDATE TOP (@BatchSize) [dbo].[NotificationQueue]
    SET [Status] = 'Processing'
    OUTPUT INSERTED.*
    WHERE [Id] IN (
        SELECT TOP (@BatchSize) [Id]
        FROM [dbo].[NotificationQueue]
        WHERE [Status] = 'Pending'
          AND ([ScheduledAt] IS NULL OR [ScheduledAt] <= SYSDATETIME())
        ORDER BY [Priority] DESC, [CreatedAt] ASC
    );
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Notification_MarkSent]
    @NotificationId BIGINT,
    @Success        BIT = 1,
    @Error          NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Success = 1
    BEGIN
        UPDATE [dbo].[NotificationQueue]
        SET [Status] = 'Sent', [SentAt] = SYSDATETIME(), [LastError] = NULL
        WHERE [Id] = @NotificationId;
    END
    ELSE
    BEGIN
        UPDATE [dbo].[NotificationQueue]
        SET [RetryCount] = [RetryCount] + 1,
            [LastError] = @Error,
            [Status] = CASE WHEN [RetryCount] + 1 >= [MaxRetries] THEN 'Failed' ELSE 'Pending' END
        WHERE [Id] = @NotificationId;
    END
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Notification_GetStats]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Channel],
        [Status],
        COUNT(*) AS Count
    FROM [dbo].[NotificationQueue]
    GROUP BY [Channel], [Status]
    ORDER BY [Channel], [Status];

    SELECT
        COUNT(*) AS TotalPending,
        AVG(CASE WHEN [Status] = 'Failed' THEN [RetryCount] ELSE NULL END) AS AvgRetriesOnFailed
    FROM [dbo].[NotificationQueue]
    WHERE [Status] IN ('Pending', 'Failed');
END;
GO
