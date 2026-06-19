CREATE OR ALTER PROCEDURE [dbo].[sp_Audit_LogAction]
    @UserId        INT,
    @Action        NVARCHAR(100),
    @Module        NVARCHAR(100) = NULL,
    @IpAddress     NVARCHAR(64)  = NULL,
    @Details       NVARCHAR(1000) = NULL,
    @AuditId       INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[AuditLogs] ([UserId], [Action], [Module], [IpAddress], [Details], [CreatedAt], [CreatedBy])
    VALUES (@UserId, @Action, @Module, @IpAddress, @Details, SYSDATETIME(), 'system');

    SET @AuditId = SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Audit_GetLogs]
    @Action         NVARCHAR(100) = NULL,
    @Module         NVARCHAR(100) = NULL,
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
    LEFT JOIN [dbo].[Users] u ON al.UserId = u.Id
    WHERE (@Action   IS NULL OR al.Action = @Action)
      AND (@Module   IS NULL OR al.Module = @Module)
      AND (@UserId   IS NULL OR al.UserId = @UserId)
      AND (@FromDate IS NULL OR al.CreatedAt >= @FromDate)
      AND (@ToDate   IS NULL OR al.CreatedAt <= @ToDate)
    ORDER BY al.CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[AuditLogs] al
    WHERE (@Action   IS NULL OR al.Action = @Action)
      AND (@Module   IS NULL OR al.Module = @Module)
      AND (@UserId   IS NULL OR al.UserId = @UserId)
      AND (@FromDate IS NULL OR al.CreatedAt >= @FromDate)
      AND (@ToDate   IS NULL OR al.CreatedAt <= @ToDate);
END;
GO
