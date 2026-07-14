CREATE OR ALTER PROCEDURE sp_GetAdmissionMonthlyCollectionReport
    @Year INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id, r.ReceiptNo, r.AdmissionApplicationId,
           r.Amount, r.PaymentMethod, r.GatewayTransactionId,
           r.ApplicantName, r.ReceiptDate,
           a.ApplicationNo
    FROM AdmissionReceipts r WITH(NOLOCK)
    LEFT JOIN Admissions a WITH(NOLOCK) ON r.AdmissionApplicationId = a.Id AND a.IsDeleted = 0
    WHERE r.IsDeleted = 0 AND r.IsRefunded = 0
      AND YEAR(r.ReceiptDate) = @Year AND MONTH(r.ReceiptDate) = @Month
    ORDER BY r.ReceiptDate DESC, r.Id DESC;

    SELECT COUNT(*) AS TotalCount,
           ISNULL(SUM(r.Amount), 0) AS TotalCollected
    FROM AdmissionReceipts r WITH(NOLOCK)
    WHERE r.IsDeleted = 0 AND r.IsRefunded = 0
      AND YEAR(r.ReceiptDate) = @Year AND MONTH(r.ReceiptDate) = @Month;
END;
GO
