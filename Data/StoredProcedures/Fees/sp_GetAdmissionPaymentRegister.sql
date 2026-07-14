CREATE OR ALTER PROCEDURE sp_GetAdmissionPaymentRegister
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @PaymentMethod NVARCHAR(50) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT r.Id, r.ReceiptNo, r.AdmissionApplicationId,
           r.Amount, r.PaymentMethod, r.GatewayTransactionId,
           r.ApplicantName, r.ReceiptDate, r.IsRefunded,
           r.RefundAmount, r.RefundedAt, r.RefundReason,
           a.ApplicationNo,
           COUNT(*) OVER() AS TotalRecords
    FROM AdmissionReceipts r WITH(NOLOCK)
    LEFT JOIN Admissions a WITH(NOLOCK) ON r.AdmissionApplicationId = a.Id AND a.IsDeleted = 0
    WHERE r.IsDeleted = 0
      AND (@FromDate IS NULL OR r.ReceiptDate >= @FromDate)
      AND (@ToDate IS NULL OR r.ReceiptDate < DATEADD(DAY, 1, @ToDate))
      AND (@PaymentMethod IS NULL OR r.PaymentMethod = @PaymentMethod)
    ORDER BY r.ReceiptDate DESC, r.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
