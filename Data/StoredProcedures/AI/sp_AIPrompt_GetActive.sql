CREATE PROCEDURE [dbo].[sp_AIPrompt_GetActive]
    @Name NVARCHAR(200),
    @Role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Name],
        [Role],
        [Prompt],
        [Version],
        [IsActive],
        [Category],
        [CreatedAt],
        [UpdatedAt]
    FROM [dbo].[AIPrompts]
    WHERE [Name] = @Name
        AND [Role] = @Role
        AND [IsActive] = 1
        AND [IsDeleted] = 0;
END
