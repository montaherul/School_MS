CREATE OR ALTER PROCEDURE sp_GetStudentLedgerPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    WITH AllEntries AS (
        SELECT
            fl.Id,
            fl.TransactionDate,
            fl.TransactionType AS [Type],
            fl.Description,
            fl.Debit,
            fl.Credit,
            fl.Balance,
            ROW_NUMBER() OVER (ORDER BY fl.TransactionDate DESC, fl.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM FeeLedgers fl
        WHERE fl.IsDeleted = 0
          AND fl.StudentId = @StudentId
          AND (@SearchTerm IS NULL OR fl.Description LIKE '%' + @SearchTerm + '%')
    )
    SELECT Id, TransactionDate, [Type], Description, Debit, Credit,
           Balance, TotalCount AS TotalRecords
    FROM AllEntries
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO