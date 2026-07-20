CREATE PROCEDURE [dbo].[sp_AIDashboardStats]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(SYSUTCDATETIME() AS DATE);
    DECLARE @Now DATETIME = SYSUTCDATETIME();
    DECLARE @TwentyFourHoursAgo DATETIME = DATEADD(HOUR, -24, @Now);
    DECLARE @MonthStart DATE = DATEFROMPARTS(YEAR(@Today), MONTH(@Today), 1);

    SELECT
        ISNULL(SUM(u.[TotalTokens]), 0) AS [TotalTokens],
        ISNULL(SUM(u.[PromptTokens]), 0) AS [PromptTokens],
        ISNULL(SUM(u.[CompletionTokens]), 0) AS [CompletionTokens],
        ISNULL(SUM(CASE WHEN CAST(u.[CreatedAt] AS DATE) = @Today THEN 1 ELSE 0 END), 0) AS [TotalAIRequests],
        ISNULL(SUM(CASE WHEN CAST(u.[CreatedAt] AS DATE) = @Today THEN u.[EstimatedCost] ELSE 0 END), 0) AS [DailyCost],
        ISNULL(SUM(CASE WHEN u.[CreatedAt] >= @MonthStart THEN u.[EstimatedCost] ELSE 0 END), 0) AS [MonthlyCost],
        ISNULL(AVG(u.[LatencyMs]), 0) AS [AvgResponseTime],
        CASE
            WHEN ISNULL(COUNT(CASE WHEN u.[CreatedAt] >= @TwentyFourHoursAgo THEN 1 END), 0) = 0 THEN 0
            ELSE ISNULL(COUNT(CASE WHEN u.[CreatedAt] >= @TwentyFourHoursAgo AND u.[LatencyMs] IS NULL THEN 1 END), 0) * 100.0 /
                 NULLIF(COUNT(CASE WHEN u.[CreatedAt] >= @TwentyFourHoursAgo THEN 1 END), 0)
        END AS [ErrorRate],
        ISNULL((SELECT COUNT(DISTINCT [StudentId]) FROM [dbo].[AIUsage] WHERE [IsDeleted] = 0), 0) AS [ActiveUsers],
        ISNULL((SELECT COUNT(DISTINCT [StudentId]) FROM [dbo].[AIUsage] WHERE CAST([CreatedAt] AS DATE) = @Today AND [IsDeleted] = 0), 0) AS [StudentsToday]
    FROM [dbo].[AIUsage] u
    WHERE u.[IsDeleted] = 0;
END
