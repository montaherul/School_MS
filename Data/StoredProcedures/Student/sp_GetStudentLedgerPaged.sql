CREATE OR ALTER PROCEDURE sp_GetStudentLedgerPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT
            fl.Id,
            fl.TransactionDate,
            fl.TransactionType AS [Type],
            fl.Description,
            fl.Debit,
            fl.Credit,
            fl.Balance,

            COUNT(*) OVER () AS TotalRecords
FROM FeeLedgers fl WITH(NOLOCK)
        WHERE fl.IsDeleted = 0
          AND fl.StudentId = @StudentId
          AND (@SearchTerm IS NULL OR fl.Description LIKE '%' + @SearchTerm + '%')
    
ORDER BY fl.TransactionDate DESC, fl.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO