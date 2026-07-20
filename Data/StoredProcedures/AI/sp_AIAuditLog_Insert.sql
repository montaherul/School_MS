CREATE PROCEDURE [dbo].[sp_AIAuditLog_Insert]
    @Action NVARCHAR(200),
    @EntityType NVARCHAR(100),
    @EntityId INT = NULL,
    @OldValue NVARCHAR(MAX) = NULL,
    @NewValue NVARCHAR(MAX) = NULL,
    @IpAddress NVARCHAR(50) = NULL,
    @UserAgent NVARCHAR(500) = NULL,
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [dbo].[AIAuditLogs] ([Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [UserAgent], [CreatedBy], [CreatedAt], [IsDeleted])
        VALUES (@Action, @EntityType, @EntityId, @OldValue, @NewValue, @IpAddress, @UserAgent, @CreatedBy, SYSUTCDATETIME(), 0);

        SELECT SCOPE_IDENTITY() AS [Id];
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
