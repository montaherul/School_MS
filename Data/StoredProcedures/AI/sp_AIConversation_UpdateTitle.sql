CREATE PROCEDURE [dbo].[sp_AIConversation_UpdateTitle]
    @ConversationId INT,
    @StudentId INT,
    @Title NVARCHAR(200),
    @UpdatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[AIConversations]
    SET [Title] = @Title, [UpdatedBy] = @UpdatedBy, [UpdatedAt] = SYSUTCDATETIME()
    WHERE [Id] = @ConversationId AND [StudentId] = @StudentId AND [IsDeleted] = 0;
END
