CREATE OR ALTER PROCEDURE sp_AdmissionTrendAnalysis
    @DateFrom DATE = NULL,
    @DateTo DATE = NULL,
    @GroupBy NVARCHAR(10) = 'Month' -- 'Month' or 'Year'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);
    IF @DateFrom IS NULL SET @DateFrom = DATEADD(YEAR, -2, @Today);
    IF @DateTo IS NULL SET @DateTo = @Today;

    IF @GroupBy = 'Year'
    BEGIN
        SELECT
            YEAR(a.CreatedAt) AS PeriodLabel,
            YEAR(a.CreatedAt) AS PeriodYear,
            0 AS PeriodMonth,
            COUNT(*) AS TotalApplications,
            SUM(CASE WHEN a.Status = 1 THEN 1 ELSE 0 END) AS PendingCount,
            SUM(CASE WHEN a.Status = 2 THEN 1 ELSE 0 END) AS ApprovedCount,
            SUM(CASE WHEN a.Status = 3 THEN 1 ELSE 0 END) AS RejectedCount,
            SUM(CASE WHEN a.Status = 4 THEN 1 ELSE 0 END) AS ConvertedCount,
            CASE WHEN COUNT(*) > 0
                THEN CAST(SUM(CASE WHEN a.Status = 4 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100
                ELSE 0
            END AS ConversionRate
        FROM Admissions a WITH(NOLOCK)
        WHERE a.IsDeleted = 0
            AND CAST(a.CreatedAt AS DATE) >= @DateFrom
            AND CAST(a.CreatedAt AS DATE) <= @DateTo
        GROUP BY YEAR(a.CreatedAt)
        ORDER BY PeriodYear;
    END
    ELSE
    BEGIN
        SELECT
            FORMAT(a.CreatedAt, 'yyyy-MM') AS PeriodLabel,
            YEAR(a.CreatedAt) AS PeriodYear,
            MONTH(a.CreatedAt) AS PeriodMonth,
            COUNT(*) AS TotalApplications,
            SUM(CASE WHEN a.Status = 1 THEN 1 ELSE 0 END) AS PendingCount,
            SUM(CASE WHEN a.Status = 2 THEN 1 ELSE 0 END) AS ApprovedCount,
            SUM(CASE WHEN a.Status = 3 THEN 1 ELSE 0 END) AS RejectedCount,
            SUM(CASE WHEN a.Status = 4 THEN 1 ELSE 0 END) AS ConvertedCount,
            CASE WHEN COUNT(*) > 0
                THEN CAST(SUM(CASE WHEN a.Status = 4 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100
                ELSE 0
            END AS ConversionRate
        FROM Admissions a WITH(NOLOCK)
        WHERE a.IsDeleted = 0
            AND CAST(a.CreatedAt AS DATE) >= @DateFrom
            AND CAST(a.CreatedAt AS DATE) <= @DateTo
        GROUP BY FORMAT(a.CreatedAt, 'yyyy-MM'), YEAR(a.CreatedAt), MONTH(a.CreatedAt)
        ORDER BY PeriodYear, PeriodMonth;
    END
END;
GO
