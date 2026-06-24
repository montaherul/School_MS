CREATE OR ALTER PROCEDURE sp_GetStudentNotificationsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @UserId INT,
    @IsRead INT = -1
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT
            n.Id,
            n.Title,
            n.Body,
            n.Channel,
            n.IsRead,
            n.SentAt,
            n.CreatedAt,

            COUNT(*) OVER () AS TotalRecords
FROM NotificationMessages n WITH(NOLOCK)
        WHERE n.IsDeleted = 0
          AND n.UserId = @UserId
          AND (@IsRead = -1 OR (@IsRead = 1 AND n.IsRead = 1) OR (@IsRead = 0 AND n.IsRead = 0))
          AND (@SearchTerm IS NULL OR n.Title LIKE '%' + @SearchTerm + '%' OR n.Body LIKE '%' + @SearchTerm + '%')
    
ORDER BY n.CreatedAt DESC, n.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO