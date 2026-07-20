CREATE PROCEDURE [dbo].[sp_AIFeatureFlags_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Key],
        [DisplayName],
        [IsEnabled],
        [Category],
        [Description]
    FROM [dbo].[AIFeatureFlags]
    WHERE [IsDeleted] = 0
    ORDER BY [Category], [DisplayName];
END
