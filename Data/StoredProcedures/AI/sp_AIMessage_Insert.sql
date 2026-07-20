CREATE PROCEDURE [dbo].[sp_AIMessage_Insert]
    @ConversationId INT,
    @Role NVARCHAR(20),
    @Content NVARCHAR(MAX),
    @PromptTokens INT = NULL,
    @CompletionTokens INT = NULL,
    @Model NVARCHAR(100) = NULL,
    @LatencyMs INT = NULL,
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[AIMessages] ([ConversationId], [Role], [Content], [PromptTokens], [CompletionTokens], [Model], [LatencyMs], [CreatedBy])
    VALUES (@ConversationId, @Role, @Content, @PromptTokens, @CompletionTokens, @Model, @LatencyMs, @CreatedBy);

    SELECT SCOPE_IDENTITY() AS [Id];
END
