CREATE OR ALTER PROCEDURE sp_AdmissionRevenueReport
    @DateFrom DATE = NULL,
    @DateTo DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);
    IF @DateFrom IS NULL SET @DateFrom = DATEADD(YEAR, -10, @Today);
    IF @DateTo IS NULL SET @DateTo = @Today;

    -- Revenue summary
    SELECT
        ISNULL(SUM(fi.TotalAmount), 0) AS TotalInvoiceAmount,
        ISNULL(SUM(fi.PaidAmount), 0) AS TotalPaidAmount,
        ISNULL(SUM(fi.DueAmount), 0) AS TotalDueAmount,
        COUNT(*) AS TotalInvoices,
        SUM(CASE WHEN fi.Status = 3 THEN 1 ELSE 0 END) AS PaidInvoices,
        SUM(CASE WHEN fi.Status IN (1, 2) THEN 1 ELSE 0 END) AS PendingInvoices,
        CASE WHEN COUNT(*) > 0
            THEN CAST(SUM(CASE WHEN fi.Status = 3 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100
            ELSE 0
        END AS CollectionRate
    FROM FeeInvoices fi WITH(NOLOCK)
    WHERE fi.IsDeleted = 0
        AND fi.Remarks LIKE 'AdmissionApp_%'
        AND CAST(fi.CreatedAt AS DATE) >= @DateFrom
        AND CAST(fi.CreatedAt AS DATE) <= @DateTo;

    -- Revenue by class
    SELECT
        c.Name AS ClassName,
        ISNULL(SUM(fi.TotalAmount), 0) AS TotalInvoiceAmount,
        ISNULL(SUM(fi.PaidAmount), 0) AS TotalPaidAmount,
        COUNT(*) AS InvoiceCount,
        SUM(CASE WHEN fi.Status = 3 THEN 1 ELSE 0 END) AS PaidCount
    FROM FeeInvoices fi WITH(NOLOCK)
    INNER JOIN Admissions a WITH(NOLOCK) ON fi.Remarks LIKE 'AdmissionApp_' + CAST(a.Id AS NVARCHAR)
    INNER JOIN Classes c WITH(NOLOCK) ON a.AppliedClassId = c.Id
    WHERE fi.IsDeleted = 0
        AND a.IsDeleted = 0
        AND c.IsDeleted = 0
        AND fi.Remarks LIKE 'AdmissionApp_%'
        AND CAST(fi.CreatedAt AS DATE) >= @DateFrom
        AND CAST(fi.CreatedAt AS DATE) <= @DateTo
    GROUP BY c.Name, c.SortOrder
    ORDER BY c.SortOrder;

    -- Monthly revenue trend
    SELECT
        FORMAT(fi.CreatedAt, 'yyyy-MM') AS PeriodLabel,
        YEAR(fi.CreatedAt) AS PeriodYear,
        MONTH(fi.CreatedAt) AS PeriodMonth,
        ISNULL(SUM(fi.TotalAmount), 0) AS TotalInvoiceAmount,
        ISNULL(SUM(fi.PaidAmount), 0) AS TotalPaidAmount,
        COUNT(*) AS InvoiceCount
    FROM FeeInvoices fi WITH(NOLOCK)
    WHERE fi.IsDeleted = 0
        AND fi.Remarks LIKE 'AdmissionApp_%'
        AND CAST(fi.CreatedAt AS DATE) >= @DateFrom
        AND CAST(fi.CreatedAt AS DATE) <= @DateTo
    GROUP BY FORMAT(fi.CreatedAt, 'yyyy-MM'), YEAR(fi.CreatedAt), MONTH(fi.CreatedAt)
    ORDER BY PeriodYear, PeriodMonth;

    -- Scholarship/Waiver summary
    SELECT
        COUNT(*) AS TotalWaivers,
        ISNULL(SUM(fw.Amount), 0) AS TotalWaiverAmount,
        ISNULL(AVG(fw.Percentage), 0) AS AvgWaiverPercentage
    FROM FeeWaivers fw WITH(NOLOCK)
    INNER JOIN FeeInvoices fi WITH(NOLOCK) ON fw.FeeInvoiceId = fi.Id
    WHERE fw.IsDeleted = 0
        AND fi.IsDeleted = 0
        AND fi.Remarks LIKE 'AdmissionApp_%'
        AND CAST(fw.CreatedAt AS DATE) >= @DateFrom
        AND CAST(fw.CreatedAt AS DATE) <= @DateTo;
END;
GO
