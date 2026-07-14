CREATE OR ALTER PROCEDURE sp_GetAdmissionDailyCollectionReport
    @CollectionDate DATE,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    DECLARE @NextDay DATE = DATEADD(DAY, 1, @CollectionDate);

    SELECT r.Id, r.ReceiptNo, r.AdmissionApplicationId,
           r.Amount, r.PaymentMethod, r.GatewayTransactionId,
           r.ApplicantName, r.ReceiptDate,
           a.ApplicationNo,
           COUNT(*) OVER() AS TotalRecords
    FROM AdmissionReceipts r WITH(NOLOCK)
    LEFT JOIN Admissions a WITH(NOLOCK) ON r.AdmissionApplicationId = a.Id AND a.IsDeleted = 0
    WHERE r.IsDeleted = 0 AND r.IsRefunded = 0
      AND r.ReceiptDate >= @CollectionDate AND r.ReceiptDate < @NextDay
    ORDER BY r.ReceiptDate DESC, r.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
