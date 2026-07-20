CREATE PROCEDURE [dbo].[sp_AIKnowledgeBase_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Cascade soft delete to chunks
        UPDATE [dbo].[AIKnowledgeChunks]
        SET [IsDeleted] = 1,
            [UpdatedAt] = SYSUTCDATETIME()
        WHERE [KnowledgeBaseId] = @Id AND [IsDeleted] = 0;

        -- Soft delete the knowledge base
        UPDATE [dbo].[AIKnowledgeBases]
        SET [IsDeleted] = 1,
            [UpdatedAt] = SYSUTCDATETIME()
        WHERE [Id] = @Id;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH;
END
