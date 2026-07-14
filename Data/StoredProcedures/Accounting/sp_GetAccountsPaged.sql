CREATE PROCEDURE sp_GetAccountsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(200) = NULL,
    @AccountType INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        a.Id,
        a.AccountCode,
        a.AccountName,
        a.Description,
        CASE a.AccountType
            WHEN 1 THEN 'Asset'
            WHEN 2 THEN 'Liability'
            WHEN 3 THEN 'Income'
            WHEN 4 THEN 'Expense'
            WHEN 5 THEN 'Equity'
        END AS AccountType,
        p.AccountName AS ParentAccount,
        a.IsActive,
        a.OpeningBalance,
        a.DisplayOrder,
        COUNT(*) OVER() AS TotalRecords
    FROM ChartOfAccounts a
    LEFT JOIN ChartOfAccounts p ON a.ParentAccountId = p.Id
    WHERE a.IsDeleted = 0
        AND (@SearchTerm IS NULL OR a.AccountName LIKE '%' + @SearchTerm + '%' OR a.AccountCode LIKE '%' + @SearchTerm + '%')
        AND (@AccountType IS NULL OR a.AccountType = @AccountType)
    ORDER BY a.AccountType, a.DisplayOrder, a.AccountCode
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
