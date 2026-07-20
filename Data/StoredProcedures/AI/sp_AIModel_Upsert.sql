CREATE PROCEDURE [dbo].[sp_AIModel_Upsert]
    @Id INT,
    @Name NVARCHAR(200),
    @ProviderId INT,
    @Role INT,
    @IsDefault BIT,
    @MaxTokens INT,
    @Temperature FLOAT,
    @IsEnabled BIT,
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id = 0
        BEGIN
            INSERT INTO [dbo].[AIModels] ([Name], [ProviderId], [Role], [IsDefault], [MaxTokens], [Temperature], [IsEnabled], [CreatedBy], [CreatedAt], [IsDeleted])
            VALUES (@Name, @ProviderId, @Role, @IsDefault, @MaxTokens, @Temperature, @IsEnabled, @CreatedBy, SYSUTCDATETIME(), 0);

            SELECT SCOPE_IDENTITY() AS [Id];
        END
        ELSE
        BEGIN
            UPDATE [dbo].[AIModels]
            SET [Name] = @Name,
                [ProviderId] = @ProviderId,
                [Role] = @Role,
                [IsDefault] = @IsDefault,
                [MaxTokens] = @MaxTokens,
                [Temperature] = @Temperature,
                [IsEnabled] = @IsEnabled,
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
