CREATE PROCEDURE [dbo].[sp_AIDashboardCharts]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME = SYSUTCDATETIME();
    DECLARE @TwentyFourHoursAgo DATETIME = DATEADD(HOUR, -24, @Now);
    DECLARE @ThirtyDaysAgo DATETIME = DATEADD(DAY, -30, @Now);

    -- Result Set 1: RequestsPerHour (last 24 hours)
    WITH HoursCTE AS (
        SELECT TOP 24
            DATEADD(HOUR, -n, DATEADD(HOUR, DATEDIFF(HOUR, 0, @Now), 0)) AS [HourStart]
        FROM (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n FROM [dbo].[AISettings]) numbers
    )
    SELECT
        h.[HourStart] AS [Hour],
        ISNULL(COUNT(u.[Id]), 0) AS [RequestCount]
    FROM HoursCTE h
    LEFT JOIN [dbo].[AIUsage] u ON u.[CreatedAt] >= h.[HourStart]
        AND u.[CreatedAt] < DATEADD(HOUR, 1, h.[HourStart])
        AND u.[IsDeleted] = 0
    GROUP BY h.[HourStart]
    ORDER BY h.[HourStart];

    -- Result Set 2: DailyCost (last 30 days)
    WITH DaysCTE AS (
        SELECT TOP 30
            CAST(DATEADD(DAY, -n, @Now) AS DATE) AS [Date]
        FROM (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n FROM [dbo].[AISettings]) numbers
    )
    SELECT
        d.[Date],
        ISNULL(SUM(u.[EstimatedCost]), 0) AS [Cost]
    FROM DaysCTE d
    LEFT JOIN [dbo].[AIUsage] u ON CAST(u.[CreatedAt] AS DATE) = d.[Date] AND u.[IsDeleted] = 0
    GROUP BY d.[Date]
    ORDER BY d.[Date];

    -- Result Set 3: TopSubjects (top 10 by estimated subject from AIUsage or title keywords)
    SELECT TOP 10
        ISNULL(NULLIF(LEFT(c.[Title], 50), ''), 'General') AS [Subject],
        COUNT(DISTINCT c.[Id]) AS [ConversationCount]
    FROM [dbo].[AIConversations] c
    INNER JOIN [dbo].[AIUsage] u ON u.[ConversationId] = c.[Id] AND u.[IsDeleted] = 0
    WHERE c.[IsDeleted] = 0
    GROUP BY ISNULL(NULLIF(LEFT(c.[Title], 50), ''), 'General')
    ORDER BY COUNT(DISTINCT c.[Id]) DESC;
END
