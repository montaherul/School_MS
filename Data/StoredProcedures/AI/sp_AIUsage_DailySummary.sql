CREATE PROCEDURE [dbo].[sp_AIUsage_DailySummary]
    @StudentId INT = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.[UsageDate],
        u.[Model],
        COUNT(DISTINCT u.[ConversationId]) AS [ConversationCount],
        COUNT(u.[Id]) AS [RequestCount],
        SUM(u.[PromptTokens]) AS [TotalPromptTokens],
        SUM(u.[CompletionTokens]) AS [TotalCompletionTokens],
        SUM(u.[TotalTokens]) AS [TotalTokens],
        SUM(u.[EstimatedCost]) AS [TotalCost],
        AVG(u.[LatencyMs]) AS [AvgLatencyMs]
    FROM [dbo].[AIUsage] u
    WHERE u.[IsDeleted] = 0
      AND (@StudentId IS NULL OR u.[StudentId] = @StudentId)
      AND (@StartDate IS NULL OR u.[UsageDate] >= @StartDate)
      AND (@EndDate IS NULL OR u.[UsageDate] <= @EndDate)
    GROUP BY u.[UsageDate], u.[Model]
    ORDER BY u.[UsageDate] DESC, u.[Model];
END
