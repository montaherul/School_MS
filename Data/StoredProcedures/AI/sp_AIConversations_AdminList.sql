CREATE PROCEDURE [dbo].[sp_AIConversations_AdminList]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @Search NVARCHAR(200) = NULL,
    @StatusFilter INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        c.[Id],
        c.[StudentId],
        s.[FullName] AS [StudentName],
        c.[Title],
        c.[Status],
        c.[IsPinned],
        (SELECT COUNT(*) FROM [dbo].[AIMessages] m WHERE m.[ConversationId] = c.[Id] AND m.[IsDeleted] = 0) AS [MessageCount],
        c.[CreatedAt],
        c.[UpdatedAt],
        COUNT(*) OVER() AS [TotalRecords]
    FROM [dbo].[AIConversations] c
    INNER JOIN [dbo].[Students] s ON c.[StudentId] = s.[Id] AND s.[IsDeleted] = 0
    WHERE c.[IsDeleted] = 0
        AND (@Search IS NULL OR c.[Title] LIKE '%' + @Search + '%' OR s.[FullName] LIKE '%' + @Search + '%')
        AND (@StatusFilter IS NULL OR c.[Status] = @StatusFilter)
    ORDER BY c.[IsPinned] DESC, c.[CreatedAt] DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
