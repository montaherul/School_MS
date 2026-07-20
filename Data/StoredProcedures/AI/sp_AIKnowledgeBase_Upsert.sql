CREATE PROCEDURE [dbo].[sp_AIKnowledgeBase_Upsert]
    @Id INT = 0,
    @Name NVARCHAR(200),
    @FilePath NVARCHAR(500) = NULL,
    @ContentType NVARCHAR(50) = 'text',
    @Size BIGINT = 0,
    @Description NVARCHAR(500) = NULL,
    @IsActive BIT = 1,
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id > 0
    BEGIN
        UPDATE [dbo].[AIKnowledgeBases]
        SET [Name] = @Name,
            [FilePath] = @FilePath,
            [ContentType] = @ContentType,
            [Size] = @Size,
            [Description] = @Description,
            [IsActive] = @IsActive,
            [Version] = [Version] + 1,
            [UpdatedBy] = @CreatedBy,
            [UpdatedAt] = SYSUTCDATETIME()
        WHERE [Id] = @Id AND [IsDeleted] = 0;

        SELECT @Id AS [Id];
    END
    ELSE
    BEGIN
        INSERT INTO [dbo].[AIKnowledgeBases] ([Name], [FilePath], [ContentType], [Size], [Description], [IsActive], [CreatedBy])
        VALUES (@Name, @FilePath, @ContentType, @Size, @Description, @IsActive, @CreatedBy);

        SELECT SCOPE_IDENTITY() AS [Id];
    END
END
