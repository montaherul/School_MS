CREATE PROCEDURE [dbo].[sp_AIUsage_Insert]
    @StudentId INT,
    @ConversationId INT = NULL,
    @MessageId INT = NULL,
    @Model NVARCHAR(100),
    @PromptTokens INT,
    @CompletionTokens INT,
    @TotalTokens INT,
    @EstimatedCost DECIMAL(18,6),
    @LatencyMs INT = NULL,
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[AIUsage] ([StudentId], [ConversationId], [MessageId], [Model], [PromptTokens], [CompletionTokens], [TotalTokens], [EstimatedCost], [LatencyMs], [CreatedBy])
    VALUES (@StudentId, @ConversationId, @MessageId, @Model, @PromptTokens, @CompletionTokens, @TotalTokens, @EstimatedCost, @LatencyMs, @CreatedBy);

    SELECT SCOPE_IDENTITY() AS [Id];
END
