CREATE PROCEDURE [dbo].[sp_AISetting_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE [dbo].[AISettings]
        SET [IsDeleted] = 1,
            [UpdatedAt] = SYSUTCDATETIME()
        WHERE [Id] = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
