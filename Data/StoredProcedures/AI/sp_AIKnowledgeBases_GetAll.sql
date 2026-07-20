CREATE PROCEDURE [dbo].[sp_AIKnowledgeBases_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Name],
        [FilePath],
        [ContentType],
        [Size],
        [Version],
        [IsActive],
        [Description],
        [CreatedAt],
        [UpdatedAt]
    FROM [dbo].[AIKnowledgeBases]
    WHERE [IsDeleted] = 0
    ORDER BY [Name];
END
