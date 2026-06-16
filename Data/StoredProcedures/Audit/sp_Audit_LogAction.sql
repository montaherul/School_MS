CREATE OR ALTER PROCEDURE [dbo].[sp_Audit_LogAction]
    @UserId        INT,
    @Action        NVARCHAR(100),
    @Entity        NVARCHAR(100),
    @EntityId      INT           = NULL,
    @OldValue      NVARCHAR(MAX) = NULL,
    @NewValue      NVARCHAR(MAX) = NULL,
    @Reason        NVARCHAR(500) = NULL,
    @AuditId       INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[AuditLogs] ([UserId], [Action], [Entity], [EntityId], [OldValue], [NewValue], [Reason], [Timestamp])
    VALUES (@UserId, @Action, @Entity, @EntityId, @OldValue, @NewValue, @Reason, SYSDATETIME());

    SET @AuditId = SCOPE_IDENTITY();

    RETURN @AuditId;
END;
GO

-- Verify AuditLogs table exists
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AuditLogs')
BEGIN
    CREATE TABLE [dbo].[AuditLogs] (
        [AuditId]    INT             IDENTITY(1,1) PRIMARY KEY,
        [UserId]     INT             NOT NULL,
        [Action]     NVARCHAR(100)   NOT NULL,
        [Entity]     NVARCHAR(100)   NOT NULL,
        [EntityId]   INT             NULL,
        [OldValue]   NVARCHAR(MAX)   NULL,
        [NewValue]   NVARCHAR(MAX)   NULL,
        [Reason]     NVARCHAR(500)   NULL,
        [Timestamp]  DATETIME2       NOT NULL DEFAULT SYSDATETIME(),
        INDEX [IX_AuditLogs_Timestamp] ([Timestamp] DESC),
        INDEX [IX_AuditLogs_Action] ([Action]),
        INDEX [IX_AuditLogs_EntityId] ([EntityId])
    );
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Audit_GetLogs]
    @Action         NVARCHAR(100) = NULL,
    @Entity         NVARCHAR(100) = NULL,
    @UserId         INT           = NULL,
    @FromDate       DATETIME2     = NULL,
    @ToDate         DATETIME2     = NULL,
    @PageNumber     INT           = 1,
    @PageSize       INT           = 50
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT al.*, u.UserName, u.Email
    FROM [dbo].[AuditLogs] al
    LEFT JOIN [dbo].[Users] u ON al.UserId = u.UserId
    WHERE (@Action   IS NULL OR al.Action = @Action)
      AND (@Entity   IS NULL OR al.Entity = @Entity)
      AND (@UserId   IS NULL OR al.UserId = @UserId)
      AND (@FromDate IS NULL OR al.Timestamp >= @FromDate)
      AND (@ToDate   IS NULL OR al.Timestamp <= @ToDate)
    ORDER BY al.Timestamp DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[AuditLogs] al
    WHERE (@Action   IS NULL OR al.Action = @Action)
      AND (@Entity   IS NULL OR al.Entity = @Entity)
      AND (@UserId   IS NULL OR al.UserId = @UserId)
      AND (@FromDate IS NULL OR al.Timestamp >= @FromDate)
      AND (@ToDate   IS NULL OR al.Timestamp <= @ToDate);
END;
GO
