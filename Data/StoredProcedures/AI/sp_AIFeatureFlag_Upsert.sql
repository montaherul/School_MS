CREATE PROCEDURE [dbo].[sp_AIFeatureFlag_Upsert]
    @Id INT,
    @Key NVARCHAR(200),
    @DisplayName NVARCHAR(200),
    @IsEnabled BIT,
    @Category NVARCHAR(100),
    @Description NVARCHAR(500),
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id = 0
        BEGIN
            INSERT INTO [dbo].[AIFeatureFlags] ([Key], [DisplayName], [IsEnabled], [Category], [Description], [CreatedBy], [CreatedAt], [IsDeleted])
            VALUES (@Key, @DisplayName, @IsEnabled, @Category, @Description, @CreatedBy, SYSUTCDATETIME(), 0);

            SELECT SCOPE_IDENTITY() AS [Id];
        END
        ELSE
        BEGIN
            UPDATE [dbo].[AIFeatureFlags]
            SET [Key] = @Key,
                [DisplayName] = @DisplayName,
                [IsEnabled] = @IsEnabled,
                [Category] = @Category,
                [Description] = @Description,
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
