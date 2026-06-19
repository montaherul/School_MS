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
    WITH Filtered AS (
        SELECT
            n.Id,
            n.Title,
            n.Body,
            n.Channel,
            n.IsRead,
            n.SentAt,
            n.CreatedAt,
            ROW_NUMBER() OVER (ORDER BY n.CreatedAt DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM NotificationMessages n
        WHERE n.IsDeleted = 0
          AND n.UserId = @UserId
          AND (@IsRead = -1 OR n.IsRead = CAST(CASE WHEN @IsRead = 1 THEN 1 ELSE 0 END AS BIT))
          AND (@SearchTerm IS NULL OR n.Title LIKE '%' + @SearchTerm + '%' OR n.Body LIKE '%' + @SearchTerm + '%')
    )
    SELECT Id, Title, Body, Channel, IsRead, SentAt, CreatedAt,
           TotalCount AS TotalRecords
    FROM Filtered
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO