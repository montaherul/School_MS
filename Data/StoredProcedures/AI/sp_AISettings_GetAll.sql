CREATE PROCEDURE [dbo].[sp_AISettings_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Key],
        [Value],
        [Description],
        [Category],
        [DisplayOrder],
        [CreatedAt],
        [UpdatedAt]
    FROM [dbo].[AISettings]
    WHERE [IsDeleted] = 0
    ORDER BY [Category], [DisplayOrder];
END
