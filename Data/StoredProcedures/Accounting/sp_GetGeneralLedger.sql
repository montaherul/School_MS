CREATE PROCEDURE sp_GetGeneralLedger
    @AccountId INT = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL,
    @FinancialPeriodId INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        gl.Id,
        gl.AccountId,
        a.AccountCode,
        a.AccountName,
        gl.EntryDate,
        gl.JournalNo,
        gl.Description,
        gl.DebitAmount,
        gl.CreditAmount,
        gl.RunningBalance,
        COUNT(*) OVER() AS TotalRecords
    FROM GeneralLedgerEntries gl
    INNER JOIN ChartOfAccounts a ON gl.AccountId = a.Id
    WHERE gl.IsDeleted = 0
        AND (@AccountId IS NULL OR gl.AccountId = @AccountId)
        AND (@FromDate IS NULL OR gl.EntryDate >= @FromDate)
        AND (@ToDate IS NULL OR gl.EntryDate <= @ToDate)
        AND (@FinancialPeriodId IS NULL OR gl.FinancialPeriodId = @FinancialPeriodId)
    ORDER BY gl.EntryDate DESC, gl.Id DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
