CREATE OR ALTER PROCEDURE sp_Student_GetPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @SortField NVARCHAR(50) = 'StudentNo',
    @SortDirection NVARCHAR(10) = 'ASC',
    @ClassId INT = NULL,
    @SectionId INT = NULL,
    @Status INT = NULL,
    @CurrentUserId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Whitelist Sort Fields
    IF @SortField NOT IN ('StudentNo', 'FullName', 'RollNumber', 'ClassName', 'SectionName')
    BEGIN
        SET @SortField = 'StudentNo';
    END

    -- 2. Base Query
    ;WITH StudentData AS (
        SELECT 
            s.Id,
            s.StudentNo,
            s.FullName,
            s.RollNumber,
            s.Status,
            c.Name AS ClassName,
            sec.Name AS SectionName,
            s.ProfilePicturePath
        FROM Students s
        JOIN Classes c ON s.ClassId = c.Id
        JOIN Sections sec ON s.SectionId = sec.Id
        WHERE s.IsDeleted = 0
          AND (@ClassId IS NULL OR s.ClassId = @ClassId)
          AND (@SectionId IS NULL OR s.SectionId = @SectionId)
          AND (@Status IS NULL OR s.Status = @Status)
          AND (@Search IS NULL OR (
                s.FullName LIKE '%' + @Search + '%' OR
                s.StudentNo LIKE '%' + @Search + '%'
          ))
    )
    -- 3. Paged Result
    SELECT * FROM (
        SELECT 
            *,
            COUNT(*) OVER() AS TotalCount
        FROM StudentData
    ) AS Result
    ORDER BY 
        CASE WHEN @SortField = 'StudentNo' AND @SortDirection = 'ASC' THEN StudentNo END ASC,
        CASE WHEN @SortField = 'StudentNo' AND @SortDirection = 'DESC' THEN StudentNo END DESC,
        CASE WHEN @SortField = 'FullName' AND @SortDirection = 'ASC' THEN FullName END ASC,
        CASE WHEN @SortField = 'FullName' AND @SortDirection = 'DESC' THEN FullName END DESC,
        CASE WHEN @SortField = 'RollNumber' AND @SortDirection = 'ASC' THEN RollNumber END ASC,
        CASE WHEN @SortField = 'RollNumber' AND @SortDirection = 'DESC' THEN RollNumber END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
