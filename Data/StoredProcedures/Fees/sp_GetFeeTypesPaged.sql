CREATE OR ALTER PROCEDURE sp_GetFeeTypesPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    SELECT ft.Id, ft.Name, ft.Description, ft.DisplayOrder, ft.IsActive,
           COUNT(*) OVER() AS TotalRecords
    FROM FeeTypes ft
    WHERE ft.IsDeleted = 0
      AND (@SearchTerm IS NULL OR ft.Name LIKE '%' + @SearchTerm + '%')
    ORDER BY ft.DisplayOrder, ft.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
