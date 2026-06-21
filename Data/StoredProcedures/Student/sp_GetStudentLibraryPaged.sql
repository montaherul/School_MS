CREATE OR ALTER PROCEDURE sp_GetStudentLibraryPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    WITH Filtered AS (
        SELECT
            bi.Id,
            b.Title AS BookTitle,
            b.Author,
            b.AccessionNo,
            bi.IssueDate,
            bi.DueDate,
            bi.ReturnedDate,
            bi.FineAmount,
            CASE WHEN bi.ReturnedDate IS NULL THEN 'Issued' ELSE 'Returned' END AS [Status],
            ROW_NUMBER() OVER (ORDER BY bi.IssueDate DESC, bi.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM BookIssues bi
        INNER JOIN Books b ON bi.BookId = b.Id AND b.IsDeleted = 0
        WHERE bi.IsDeleted = 0
          AND bi.StudentId = @StudentId
          AND (@SearchTerm IS NULL OR b.Title LIKE '%' + @SearchTerm + '%' OR b.Author LIKE '%' + @SearchTerm + '%')
    )
    SELECT Id, BookTitle, Author, AccessionNo, IssueDate, DueDate,
           ReturnedDate, FineAmount, [Status], TotalCount AS TotalRecords
    FROM Filtered
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO