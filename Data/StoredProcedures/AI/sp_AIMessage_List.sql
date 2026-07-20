CREATE PROCEDURE [dbo].[sp_AIMessage_List]
    @ConversationId INT,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT m.[Id], m.[ConversationId], m.[Role], m.[Content], m.[CreatedAt]
    FROM [dbo].[AIMessages] m
    INNER JOIN [dbo].[AIConversations] c ON c.[Id] = m.[ConversationId]
    WHERE m.[ConversationId] = @ConversationId
      AND c.[StudentId] = @StudentId
      AND m.[IsDeleted] = 0
      AND c.[IsDeleted] = 0
    ORDER BY m.[CreatedAt] ASC;
END
