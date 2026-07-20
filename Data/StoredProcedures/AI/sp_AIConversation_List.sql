CREATE PROCEDURE [dbo].[sp_AIConversation_List]
    @StudentId INT,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT COUNT(*) OVER() AS [TotalRecords],
           c.[Id],
           c.[Title],
           c.[Status],
           c.[IsPinned],
           c.[CreatedAt],
           (SELECT COUNT(*) FROM [dbo].[AIMessages] m WHERE m.[ConversationId] = c.[Id] AND m.[IsDeleted] = 0) AS [MessageCount]
    FROM [dbo].[AIConversations] c
    WHERE c.[StudentId] = @StudentId AND c.[IsDeleted] = 0 AND c.[Status] IN (1, 2)
    ORDER BY c.[IsPinned] DESC, c.[CreatedAt] DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
