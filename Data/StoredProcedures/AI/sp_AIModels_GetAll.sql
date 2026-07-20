CREATE PROCEDURE [dbo].[sp_AIModels_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.[Id],
        m.[Name],
        m.[ProviderId],
        p.[Name] AS [ProviderName],
        m.[Role],
        m.[IsDefault],
        m.[MaxTokens],
        m.[Temperature],
        m.[IsEnabled]
    FROM [dbo].[AIModels] m
    INNER JOIN [dbo].[AIProviders] p ON m.[ProviderId] = p.[Id] AND p.[IsDeleted] = 0
    WHERE m.[IsDeleted] = 0
    ORDER BY m.[IsDefault] DESC, p.[Priority], m.[Name];
END
