CREATE PROCEDURE [dbo].[sp_AIConversation_Get]
    @ConversationId INT,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.[Id], c.[StudentId], c.[Title], c.[Status], c.[IsPinned], c.[CreatedAt], c.[UpdatedAt]
    FROM [dbo].[AIConversations] c
    WHERE c.[Id] = @ConversationId AND c.[StudentId] = @StudentId AND c.[IsDeleted] = 0;
END
