CREATE OR ALTER PROCEDURE sp_GetDiscountReport
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT fd.Id, fd.Name, CAST(fd.DiscountType AS NVARCHAR(20)) AS DiscountType,
           fd.Value, c.Name AS ClassName, fc.Name AS FeeCategoryName,
           fd.IsActive, COUNT(*) OVER() AS TotalRecords
    FROM FeeDiscounts fd
    LEFT JOIN Classes c ON fd.SchoolClassId = c.Id
    LEFT JOIN FeeCategories fc ON fd.FeeCategoryId = fc.Id
    WHERE fd.IsDeleted = 0
    ORDER BY fd.Name, fd.Id
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
