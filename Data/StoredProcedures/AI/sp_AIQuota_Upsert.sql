CREATE PROCEDURE [dbo].[sp_AIQuota_Upsert]
    @Id INT,
    @Role NVARCHAR(50),
    @DailyLimit INT,
    @MinuteLimit INT,
    @MaxTokensPerRequest INT,
    @IsUnlimited BIT,
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id = 0
        BEGIN
            INSERT INTO [dbo].[AIQuotas] ([Role], [DailyLimit], [MinuteLimit], [MaxTokensPerRequest], [IsUnlimited], [CreatedBy], [CreatedAt], [IsDeleted])
            VALUES (@Role, @DailyLimit, @MinuteLimit, @MaxTokensPerRequest, @IsUnlimited, @CreatedBy, SYSUTCDATETIME(), 0);

            SELECT SCOPE_IDENTITY() AS [Id];
        END
        ELSE
        BEGIN
            UPDATE [dbo].[AIQuotas]
            SET [Role] = @Role,
                [DailyLimit] = @DailyLimit,
                [MinuteLimit] = @MinuteLimit,
                [MaxTokensPerRequest] = @MaxTokensPerRequest,
                [IsUnlimited] = @IsUnlimited,
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
