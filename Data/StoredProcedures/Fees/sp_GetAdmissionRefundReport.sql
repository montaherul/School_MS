CREATE OR ALTER PROCEDURE sp_GetAdmissionRefundReport
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT r.Id, r.ReceiptNo, r.AdmissionApplicationId,
           r.Amount AS OriginalAmount,
           r.RefundAmount, r.RefundedAt, r.RefundReason, r.RefundedBy,
           r.ApplicantName, r.PaymentMethod,
           a.ApplicationNo,
           COUNT(*) OVER() AS TotalRecords
    FROM AdmissionReceipts r WITH(NOLOCK)
    LEFT JOIN Admissions a WITH(NOLOCK) ON r.AdmissionApplicationId = a.Id AND a.IsDeleted = 0
    WHERE r.IsDeleted = 0 AND r.IsRefunded = 1
      AND (@FromDate IS NULL OR r.RefundedAt >= @FromDate)
      AND (@ToDate IS NULL OR r.RefundedAt < DATEADD(DAY, 1, @ToDate))
    ORDER BY r.RefundedAt DESC, r.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
