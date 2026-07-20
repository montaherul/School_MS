CREATE PROCEDURE [dbo].[sp_AIQuotas_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Role],
        [DailyLimit],
        [MinuteLimit],
        [MaxTokensPerRequest],
        [IsUnlimited]
    FROM [dbo].[AIQuotas]
    WHERE [IsDeleted] = 0
    ORDER BY [Role];
END
