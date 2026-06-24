CREATE OR ALTER PROCEDURE [dbo].[SP_System_DatabaseHealth]
AS
BEGIN
    SET NOCOUNT ON;

    -- Database size
    SELECT
        DB_NAME() AS DatabaseName,
        SUM(CAST(size AS BIGINT) * 8 / 1024) AS SizeMB,
        SUM(CAST(CASE WHEN type_desc = 'ROWS' THEN size ELSE 0 END AS BIGINT) * 8 / 1024) AS DataSizeMB,
        SUM(CAST(CASE WHEN type_desc = 'LOG' THEN size ELSE 0 END AS BIGINT) * 8 / 1024) AS LogSizeMB
FROM sys WITH(NOLOCK).database_files;

    -- Entity counts
    SELECT 'Students' AS EntityType, COUNT(*) AS TotalCount FROM [dbo].[Students] WHERE [IsDeleted] = 0
    UNION ALL
    SELECT 'Teachers', COUNT(*) FROM [dbo].[Teachers] WHERE [IsDeleted] = 0
    UNION ALL
    SELECT 'Exams', COUNT(*) FROM [dbo].[Exams] WHERE [IsDeleted] = 0
    UNION ALL
    SELECT 'StudentExamResults', COUNT(*) FROM [dbo].[StudentExamResults]
    UNION ALL
    SELECT 'ReportCardsGenerated', COUNT(*) FROM [dbo].[StudentExamResults] WHERE [Status] = 5
    UNION ALL
    SELECT 'AdmitCardsGenerated', COUNT(*) FROM [dbo].[AdmitCards]
    UNION ALL
    SELECT 'AttendanceRecords', COUNT(*) FROM [dbo].[Attendance] WHERE [IsDeleted] = 0;

    -- Recent activity timestamps
    SELECT
        (SELECT MAX([CreatedAt]) FROM [dbo].[AuditLogs] WHERE [Action] LIKE '%Backup%') AS LastBackup,
        (SELECT MAX([CreatedAt]) FROM [dbo].[AuditLogs] WHERE [Action] LIKE '%Restore%') AS LastRestore,
        (SELECT MAX([PublishedAt]) FROM [dbo].[StudentExamResults]) AS LastResultPublish,
        (SELECT MAX([CreatedAt]) FROM [dbo].[AuditLogs] WHERE [Action] LIKE '%Publish%') AS LastPublishAction,
        GETDATE() AS ReportGeneratedAt;

    -- Stored procedure health
    SELECT
        COUNT(*) AS TotalProcedures,
        SUM(CASE WHEN sm.[definition] IS NOT NULL THEN 1 ELSE 0 END) AS WithDefinition,
        SUM(CASE WHEN sm.[definition] IS NULL THEN 1 ELSE 0 END) AS DecompilationErrors
FROM sys WITH(NOLOCK).procedures sp
LEFT JOIN sys WITH(NOLOCK).sql_modules sm ON sp.[object_id] = sm.[object_id]
    WHERE sp.[is_ms_shipped] = 0;
END;
GO
