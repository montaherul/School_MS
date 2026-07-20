CREATE PROCEDURE [dbo].[sp_AIPrompt_Upsert]
    @Id INT,
    @Name NVARCHAR(200),
    @Role NVARCHAR(50),
    @Prompt NVARCHAR(MAX),
    @IsActive BIT,
    @Category NVARCHAR(100),
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @Id = 0
        BEGIN
            -- If activating, deactivate existing active prompts with same Name
            IF @IsActive = 1
            BEGIN
                UPDATE [dbo].[AIPrompts]
                SET [IsActive] = 0,
                    [UpdatedBy] = @CreatedBy,
                    [UpdatedAt] = SYSUTCDATETIME()
                WHERE [Name] = @Name
                    AND [Role] = @Role
                    AND [IsDeleted] = 0
                    AND [IsActive] = 1;
            END

            DECLARE @NextVersion INT;
            SELECT @NextVersion = ISNULL(MAX([Version]), 0) + 1
            FROM [dbo].[AIPrompts]
            WHERE [Name] = @Name AND [Role] = @Role AND [IsDeleted] = 0;

            INSERT INTO [dbo].[AIPrompts] ([Name], [Role], [Prompt], [Version], [IsActive], [Category], [CreatedBy], [CreatedAt], [IsDeleted])
            VALUES (@Name, @Role, @Prompt, @NextVersion, @IsActive, @Category, @CreatedBy, SYSUTCDATETIME(), 0);

            SELECT SCOPE_IDENTITY() AS [Id];
        END
        ELSE
        BEGIN
            -- If activating, deactivate other prompts with same Name
            IF @IsActive = 1
            BEGIN
                UPDATE [dbo].[AIPrompts]
                SET [IsActive] = 0,
                    [UpdatedBy] = @CreatedBy,
                    [UpdatedAt] = SYSUTCDATETIME()
                WHERE [Name] = @Name
                    AND [Role] = @Role
                    AND [Id] <> @Id
                    AND [IsDeleted] = 0
                    AND [IsActive] = 1;
            END

            UPDATE [dbo].[AIPrompts]
            SET [Name] = @Name,
                [Role] = @Role,
                [Prompt] = @Prompt,
                [IsActive] = @IsActive,
                [Category] = @Category,
                [UpdatedBy] = @CreatedBy,
                [UpdatedAt] = SYSUTCDATETIME()
            WHERE [Id] = @Id AND [IsDeleted] = 0;

            SELECT @Id AS [Id];
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH;
END
