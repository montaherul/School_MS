CREATE PROCEDURE [dbo].[sp_AIProvider_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE [dbo].[AIProviders]
        SET [IsDeleted] = 1,
            [UpdatedAt] = SYSUTCDATETIME()
        WHERE [Id] = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
