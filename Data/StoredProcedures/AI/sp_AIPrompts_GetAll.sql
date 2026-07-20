CREATE PROCEDURE [dbo].[sp_AIPrompts_GetAll]
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
    WHERE [IsDeleted] = 0
    ORDER BY [Name], [Version] DESC;
END
