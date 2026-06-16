CREATE OR ALTER PROCEDURE [dbo].[SP_System_DashboardMetrics]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AvgQueryTime DECIMAL(10,4);
    DECLARE @SlowQueries INT;
    DECLARE @FailedRequests INT;
    DECLARE @PublishedResults INT;
    DECLARE @PendingResults INT;
    DECLARE @TotalUsersOnline INT;

    -- Execution stats from DMV (requires VIEW SERVER STATE)
    BEGIN TRY
        SELECT @AvgQueryTime = ISNULL(AVG(total_elapsed_time / 1000.0 / NULLIF(execution_count, 0)), 0),
               @SlowQueries = COUNT(CASE WHEN total_elapsed_time / 1000.0 / NULLIF(execution_count, 0) > 2000 THEN 1 END)
        FROM sys.dm_exec_query_stats
        WHERE last_execution_time >= DATEADD(HOUR, -1, GETDATE());

        SELECT @TotalUsersOnline = COUNT(DISTINCT session_id)
        FROM sys.dm_exec_sessions
        WHERE is_user_process = 1
          AND last_request_start_time >= DATEADD(MINUTE, -15, GETDATE());
    END TRY
    BEGIN CATCH
        SELECT @AvgQueryTime = 0, @SlowQueries = 0, @TotalUsersOnline = 0;
    END CATCH;

    -- Application metrics
    SELECT @PublishedResults = COUNT(*) FROM [dbo].[StudentExamResults] WHERE [IsPublished] = 1;
    SELECT @PendingResults   = COUNT(*) FROM [dbo].[StudentExamResults] WHERE [IsPublished] = 0 OR [IsPublished] IS NULL;

    -- Result set
    SELECT
        @AvgQueryTime       AS AvgQueryTimeMs,
        @SlowQueries        AS SlowQueries,
        @FailedRequests     AS FailedRequests,
        @PublishedResults   AS PublishedResults,
        @PendingResults     AS PendingResults,
        @TotalUsersOnline   AS TotalUsersOnline,
        GETDATE()           AS SampledAt;

    -- Exam breakdown
    SELECT
        e.[Id]             AS ExamId,
        e.[ExamName],
        e.[ExamType],
        e.[Status],
        COUNT(DISTINCT ser.[StudentId]) AS TotalStudents,
        COUNT(DISTINCT CASE WHEN ser.[IsPublished] = 1 THEN ser.[StudentId] END) AS PublishedStudents,
        AVG(ser.[GradePoint]) AS AvgGPA
    FROM [dbo].[Exams] e
    LEFT JOIN [dbo].[StudentExamResults] ser ON e.[Id] = ser.[ExamId]
    GROUP BY e.[Id], e.[ExamName], e.[ExamType], e.[Status]
    ORDER BY e.[Id] DESC;

    -- Recent activity
    SELECT TOP 20
        [Action],
        [Entity],
        [Timestamp],
        [UserId]
    FROM [dbo].[AuditLogs]
    ORDER BY [Timestamp] DESC;
END;
GO
