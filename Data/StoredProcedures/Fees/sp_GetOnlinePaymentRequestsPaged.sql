CREATE OR ALTER PROCEDURE sp_GetOnlinePaymentRequestsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StatusFilter INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT 
        opr.Id,
        opr.StudentId,
        COALESCE(s.FullName, 'Unknown') AS StudentName,
        opr.FeeInvoiceId,
        COALESCE(fi.InvoiceNo, 'N/A') AS InvoiceNo,
        opr.Amount,
        CAST(opr.PaymentMethod AS INT) AS PaymentMethod,
        opr.ReferenceNo,
        CAST(opr.[Status] AS INT) AS [Status],
        opr.Remarks,
        opr.AdminNotes,
        opr.CreatedAt,
        opr.VerifiedAt,
        opr.VerifiedBy,
        opr.RejectedAt,
        opr.RejectedBy,
        COUNT(*) OVER () AS TotalRecords
    FROM OnlinePaymentRequests opr WITH(NOLOCK)
    LEFT JOIN Students s WITH(NOLOCK) ON opr.StudentId = s.Id AND s.IsDeleted = 0
    LEFT JOIN FeeInvoices fi WITH(NOLOCK) ON opr.FeeInvoiceId = fi.Id AND fi.IsDeleted = 0
    WHERE opr.IsDeleted = 0
        AND (@StatusFilter = 0 OR opr.[Status] = @StatusFilter)
        AND (@SearchTerm IS NULL OR @SearchTerm = ''
            OR s.FullName LIKE '%' + @SearchTerm + '%'
            OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%'
            OR opr.ReferenceNo LIKE '%' + @SearchTerm + '%')
    ORDER BY opr.CreatedAt DESC, opr.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
