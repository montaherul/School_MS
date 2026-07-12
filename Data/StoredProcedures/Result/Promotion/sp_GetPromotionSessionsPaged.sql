CREATE OR ALTER PROCEDURE sp_GetPromotionSessionsPaged
    @PageNumber INT,
    @PageSize INT,
    @SearchTerm NVARCHAR(100) = NULL,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ps.Id, ps.SessionName, ay.Name AS AcademicYearName, ps.PromotionDate,
           ps.Status, ps.Remarks, ps.CreatedAt,
           COUNT(*) OVER() AS TotalRecords
    FROM PromotioSessions ps WITH(NOLOCK)
    INNER JOIN AcademicYears ay WITH(NOLOCK) ON ay.Id = ps.AcademicYearId AND ay.IsDeleted = 0
    WHERE ps.IsDeleted = 0
      AND (@SearchTerm IS NULL OR ps.SessionName LIKE '%' + @SearchTerm + '%')
      AND (@Status IS NULL OR ps.Status = @Status)
    ORDER BY ps.CreatedAt DESC
    OFFSET @PageSize * (@PageNumber - 1) ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
