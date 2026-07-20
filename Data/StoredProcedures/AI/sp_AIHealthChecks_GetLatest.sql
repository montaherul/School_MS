CREATE PROCEDURE [dbo].[sp_AIHealthChecks_GetLatest]
AS
BEGIN
    SET NOCOUNT ON;

    WITH Ranked AS (
        SELECT
            [Id],
            [Component],
            [Status],
            [LastChecked],
            [ResponseTimeMs],
            [ErrorMessage],
            ROW_NUMBER() OVER (PARTITION BY [Component] ORDER BY [LastChecked] DESC) AS [rn]
        FROM [dbo].[AIHealthChecks]
        WHERE [IsDeleted] = 0
    )
    SELECT
        [Id],
        [Component],
        [Status],
        [LastChecked],
        [ResponseTimeMs],
        [ErrorMessage]
    FROM Ranked
    WHERE [rn] = 1
    ORDER BY [Component];
END
