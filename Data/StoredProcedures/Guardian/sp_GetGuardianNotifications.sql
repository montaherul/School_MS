CREATE OR ALTER PROCEDURE [dbo].[sp_GetGuardianNotifications]
    @GuardianId INT,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) FROM GuardianNotifications WHERE GuardianId = @GuardianId AND IsDeleted = 0;

    SELECT 
        gn.Id,
        gn.Title,
        gn.Message,
        gn.Category,
        CAST(gn.IsRead AS BIT) AS IsRead,
        gn.ReadAt,
        gn.CreatedAt
FROM GuardianNotifications gn WITH(NOLOCK)
    WHERE gn.GuardianId = @GuardianId AND gn.IsDeleted = 0
    ORDER BY gn.IsRead ASC, gn.Id DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
