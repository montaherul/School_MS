CREATE PROCEDURE [dbo].[sp_AIAuditLog_GetPaged]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @EntityType NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        [Id],
        [Action],
        [EntityType],
        [EntityId],
        [OldValue],
        [NewValue],
        [IpAddress],
        [UserAgent],
        [CreatedBy],
        [CreatedAt],
        COUNT(*) OVER() AS [TotalRecords]
    FROM [dbo].[AIAuditLogs]
    WHERE [IsDeleted] = 0
        AND (@EntityType IS NULL OR [EntityType] = @EntityType)
    ORDER BY [CreatedAt] DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
