CREATE PROCEDURE [dbo].[sp_AIModel_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE [dbo].[AIModels]
        SET [IsDeleted] = 1,
            [UpdatedAt] = SYSUTCDATETIME()
        WHERE [Id] = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
