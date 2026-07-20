CREATE PROCEDURE [dbo].[sp_AIConversation_Delete]
    @ConversationId INT,
    @StudentId INT,
    @UpdatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[AIMessages]
    SET [IsDeleted] = 1, [UpdatedBy] = @UpdatedBy, [UpdatedAt] = SYSUTCDATETIME()
    WHERE [ConversationId] = @ConversationId AND [IsDeleted] = 0;

    UPDATE [dbo].[AIConversations]
    SET [Status] = 4, [IsDeleted] = 1, [UpdatedBy] = @UpdatedBy, [UpdatedAt] = SYSUTCDATETIME()
    WHERE [Id] = @ConversationId AND [StudentId] = @StudentId AND [IsDeleted] = 0;
END
