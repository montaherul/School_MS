CREATE PROCEDURE [dbo].[sp_AISecurityPolicy_Upsert]
    @Id INT,
    @Key NVARCHAR(200),
    @Value NVARCHAR(MAX),
    @Description NVARCHAR(500),
    @Category NVARCHAR(100),
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id = 0
        BEGIN
            INSERT INTO [dbo].[AISecurityPolicies] ([Key], [Value], [Description], [Category], [CreatedBy], [CreatedAt], [IsDeleted])
            VALUES (@Key, @Value, @Description, @Category, @CreatedBy, SYSUTCDATETIME(), 0);

            SELECT SCOPE_IDENTITY() AS [Id];
        END
        ELSE
        BEGIN
            UPDATE [dbo].[AISecurityPolicies]
            SET [Key] = @Key,
                [Value] = @Value,
                [Description] = @Description,
                [Category] = @Category,
                [UpdatedBy] = @CreatedBy,
                [UpdatedAt] = SYSUTCDATETIME()
            WHERE [Id] = @Id AND [IsDeleted] = 0;

            SELECT @Id AS [Id];
        END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
