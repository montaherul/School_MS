CREATE PROCEDURE [dbo].[sp_AISecurityPolicies_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Key],
        [Value],
        [Description],
        [Category]
    FROM [dbo].[AISecurityPolicies]
    WHERE [IsDeleted] = 0
    ORDER BY [Category], [Key];
END
