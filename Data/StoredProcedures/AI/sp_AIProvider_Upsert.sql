CREATE PROCEDURE [dbo].[sp_AIProvider_Upsert]
    @Id INT,
    @Name NVARCHAR(200),
    @ProviderType INT,
    @BaseUrl NVARCHAR(500),
    @ApiKeyEncrypted NVARCHAR(500),
    @IsEnabled BIT,
    @Priority INT,
    @RetryCount INT,
    @TimeoutSeconds INT,
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id = 0
        BEGIN
            INSERT INTO [dbo].[AIProviders] ([Name], [ProviderType], [BaseUrl], [ApiKeyEncrypted], [IsEnabled], [Priority], [RetryCount], [TimeoutSeconds], [CreatedBy], [CreatedAt], [IsDeleted])
            VALUES (@Name, @ProviderType, @BaseUrl, @ApiKeyEncrypted, @IsEnabled, @Priority, @RetryCount, @TimeoutSeconds, @CreatedBy, SYSUTCDATETIME(), 0);

            SELECT SCOPE_IDENTITY() AS [Id];
        END
        ELSE
        BEGIN
            UPDATE [dbo].[AIProviders]
            SET [Name] = @Name,
                [ProviderType] = @ProviderType,
                [BaseUrl] = @BaseUrl,
                [ApiKeyEncrypted] = @ApiKeyEncrypted,
                [IsEnabled] = @IsEnabled,
                [Priority] = @Priority,
                [RetryCount] = @RetryCount,
                [TimeoutSeconds] = @TimeoutSeconds,
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
