CREATE PROCEDURE [dbo].[sp_AISetting_Upsert]
    @Id INT,
    @Key NVARCHAR(200),
    @Value NVARCHAR(MAX),
    @Description NVARCHAR(500),
    @Category NVARCHAR(100),
    @DisplayOrder INT,
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id = 0
        BEGIN
            INSERT INTO [dbo].[AISettings] ([Key], [Value], [Description], [Category], [DisplayOrder], [CreatedBy], [CreatedAt], [IsDeleted])
            VALUES (@Key, @Value, @Description, @Category, @DisplayOrder, @CreatedBy, SYSUTCDATETIME(), 0);

            SELECT SCOPE_IDENTITY() AS [Id];
        END
        ELSE
        BEGIN
            UPDATE [dbo].[AISettings]
            SET [Key] = @Key,
                [Value] = @Value,
                [Description] = @Description,
                [Category] = @Category,
                [DisplayOrder] = @DisplayOrder,
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
