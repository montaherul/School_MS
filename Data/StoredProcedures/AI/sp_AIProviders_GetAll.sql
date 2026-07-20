CREATE OR ALTER PROCEDURE [dbo].[sp_AIProviders_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Name],
        [ProviderType],
        [BaseUrl],
        [ApiKeyEncrypted] AS [ApiKey],
        [IsEnabled],
        [Priority],
        [RetryCount],
        [TimeoutSeconds]
    FROM [dbo].[AIProviders]
    WHERE [IsDeleted] = 0
    ORDER BY [Priority], [Name];
END
