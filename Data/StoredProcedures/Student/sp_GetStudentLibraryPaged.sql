CREATE OR ALTER PROCEDURE sp_GetStudentLibraryPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


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

            COUNT(*) OVER () AS TotalRecords
FROM BookIssues bi WITH(NOLOCK)
INNER JOIN Books b WITH(NOLOCK) ON bi.BookId = b.Id AND b.IsDeleted = 0
        WHERE bi.IsDeleted = 0
          AND bi.StudentId = @StudentId
          AND (@SearchTerm IS NULL OR b.Title LIKE '%' + @SearchTerm + '%' OR b.Author LIKE '%' + @SearchTerm + '%')
    
ORDER BY bi.IssueDate DESC, bi.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO