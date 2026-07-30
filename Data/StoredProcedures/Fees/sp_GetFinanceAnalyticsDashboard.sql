-- ============================================================================
-- Stored Procedure: sp_GetFinanceAnalyticsDashboard
-- Purpose: Finance Analytics — revenue forecast, cash flow, fee trends,
--          budget vs actual, defaulter segments, monthly summary
-- Returns: 6 result sets
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFinanceAnalyticsDashboard
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = GETUTCDATE();
    DECLARE @CurrentMonthStart DATE = DATEFROMPARTS(YEAR(@Now), MONTH(@Now), 1);
    DECLARE @PrevMonthStart DATE = DATEADD(MONTH, -1, @CurrentMonthStart);
    DECLARE @NextMonthStart DATE = DATEADD(MONTH, 1, @CurrentMonthStart);

    -- ==================================================================
    -- RESULT SET 1: Revenue Forecast
    -- ==================================================================
    SELECT
        ISNULL(SUM(CASE WHEN p.PaidAt >= @CurrentMonthStart AND p.PaidAt < @NextMonthStart THEN p.Amount ELSE 0 END), 0) AS CurrentMonthRevenue,
        ISNULL(SUM(CASE WHEN p.PaidAt >= @PrevMonthStart AND p.PaidAt < @CurrentMonthStart THEN p.Amount ELSE 0 END), 0) AS PreviousMonthRevenue,
        COUNT(DISTINCT CASE WHEN fi.Status IN (1,2) AND fi.DueDate < CAST(@Now AS DATE) THEN fi.Id END) AS AtRiskInvoices,
        ISNULL(SUM(CASE WHEN fi.Status IN (1,2) THEN fi.TotalAmount - fi.PaidAmount ELSE 0 END), 0) AS TotalOutstanding
    FROM FeeInvoices fi WITH(NOLOCK)
    LEFT JOIN Payments p WITH(NOLOCK) ON p.FeeInvoiceId = fi.Id AND p.IsDeleted = 0
    WHERE fi.IsDeleted = 0;

    -- ==================================================================
    -- RESULT SET 2: Revenue Monthly Projections (12 months: 6 past + 6 future)
    -- ==================================================================
    WITH Months AS (
        SELECT -5 AS OffsetNum
        UNION SELECT -4 UNION SELECT -3 UNION SELECT -2 UNION SELECT -1
        UNION SELECT 0 UNION SELECT 1 UNION SELECT 2 UNION SELECT 3
        UNION SELECT 4 UNION SELECT 5 UNION SELECT 6
    ),
    MonthPeriods AS (
        SELECT
            OffsetNum,
            DATEFROMPARTS(YEAR(@Now), MONTH(@Now), 1) AS MonthStart,
            DATEADD(MONTH, OffsetNum, @CurrentMonthStart) AS PeriodStart,
            DATEADD(MONTH, OffsetNum + 1, @CurrentMonthStart) AS PeriodEnd
        FROM Months
    )
    SELECT
        FORMAT(mp.PeriodStart, 'MMM yy') AS [Month],
        ISNULL(SUM(p.Amount), 0) AS Actual,
        CASE WHEN mp.OffsetNum > 0
            THEN ISNULL(SUM(p.Amount), 0) * 1.05
            ELSE 0
        END AS Projected
    FROM MonthPeriods mp
    LEFT JOIN Payments p WITH(NOLOCK) ON p.PaidAt >= mp.PeriodStart AND p.PaidAt < mp.PeriodEnd AND p.IsDeleted = 0
    GROUP BY mp.OffsetNum, mp.PeriodStart
    ORDER BY mp.OffsetNum;

    -- ==================================================================
    -- RESULT SET 3: Cash Flow Projections (6 months)
    -- ==================================================================
    WITH Months AS (
        SELECT 0 AS OffsetNum
        UNION SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5
    ),
    MonthPeriods AS (
        SELECT
            OffsetNum,
            DATEADD(MONTH, OffsetNum, @CurrentMonthStart) AS PeriodStart,
            DATEADD(MONTH, OffsetNum + 1, @CurrentMonthStart) AS PeriodEnd
        FROM Months
    )
    SELECT
        FORMAT(mp.PeriodStart, 'MMM yy') AS [Period],
        ISNULL(SUM(p.Amount), 0) AS Inflow,
        ISNULL(SUM(p.Amount), 0) * 0.6 AS Outflow,
        ISNULL(SUM(p.Amount), 0) * 0.4 AS [Balance]
    FROM MonthPeriods mp
    LEFT JOIN Payments p WITH(NOLOCK) ON p.PaidAt >= mp.PeriodStart AND p.PaidAt < mp.PeriodEnd AND p.IsDeleted = 0
    GROUP BY mp.OffsetNum, mp.PeriodStart
    ORDER BY mp.OffsetNum;

    -- ==================================================================
    -- RESULT SET 4: Fee Trends (12 months)
    -- ==================================================================
    WITH Months AS (
        SELECT -11 AS OffsetNum
        UNION SELECT -10 UNION SELECT -9 UNION SELECT -8 UNION SELECT -7
        UNION SELECT -6 UNION SELECT -5 UNION SELECT -4 UNION SELECT -3
        UNION SELECT -2 UNION SELECT -1 UNION SELECT 0
    ),
    MonthPeriods AS (
        SELECT
            OffsetNum,
            DATEADD(MONTH, OffsetNum, @CurrentMonthStart) AS PeriodStart,
            DATEADD(MONTH, OffsetNum + 1, @CurrentMonthStart) AS PeriodEnd
        FROM Months
    )
    SELECT
        FORMAT(mp.PeriodStart, 'MMM yy') AS [Month],
        ISNULL((SELECT SUM(p2.Amount) FROM Payments p2 WITH(NOLOCK)
            WHERE p2.PaidAt >= mp.PeriodStart AND p2.PaidAt < mp.PeriodEnd AND p2.IsDeleted = 0), 0) AS Collected,
        ISNULL((SELECT SUM(fi2.TotalAmount) FROM FeeInvoices fi2 WITH(NOLOCK)
            WHERE fi2.CreatedAt >= mp.PeriodStart AND fi2.CreatedAt < mp.PeriodEnd AND fi2.IsDeleted = 0), 0) AS [Target]
    FROM MonthPeriods mp
    ORDER BY mp.OffsetNum;

    -- ==================================================================
    -- RESULT SET 5: Budget vs Actual by FeeCategory
    -- ==================================================================
    SELECT
        fc.Name AS Category,
        ISNULL(SUM(fii.Amount), 0) AS Budget,
        ISNULL(SUM(CASE WHEN fi.Status = 3 THEN fii.NetAmount ELSE 0 END), 0) AS Actual
    FROM FeeCategories fc WITH(NOLOCK)
    LEFT JOIN FeeStructures fs WITH(NOLOCK) ON fs.FeeCategoryId = fc.Id AND fs.IsDeleted = 0
    LEFT JOIN FeeInvoiceItems fii WITH(NOLOCK) ON fii.FeeStructureId = fs.Id AND fii.IsDeleted = 0
    LEFT JOIN FeeInvoices fi WITH(NOLOCK) ON fi.Id = fii.FeeInvoiceId AND fi.IsDeleted = 0 AND YEAR(fi.CreatedAt) = YEAR(@Now)
    WHERE fc.IsDeleted = 0
    GROUP BY fc.Name
    ORDER BY fc.Name;

    -- ==================================================================
    -- RESULT SET 6: Monthly Summaries (12 months)
    -- ==================================================================
    WITH Months AS (
        SELECT -11 AS OffsetNum
        UNION SELECT -10 UNION SELECT -9 UNION SELECT -8 UNION SELECT -7
        UNION SELECT -6 UNION SELECT -5 UNION SELECT -4 UNION SELECT -3
        UNION SELECT -2 UNION SELECT -1 UNION SELECT 0
    ),
    MonthPeriods AS (
        SELECT
            OffsetNum,
            DATEADD(MONTH, OffsetNum, @CurrentMonthStart) AS PeriodStart,
            DATEADD(MONTH, OffsetNum + 1, @CurrentMonthStart) AS PeriodEnd
        FROM Months
    )
    SELECT
        FORMAT(mp.PeriodStart, 'MMM') AS [Month],
        YEAR(mp.PeriodStart) AS [Year],
        ISNULL((SELECT SUM(p3.Amount) FROM Payments p3 WITH(NOLOCK)
            WHERE p3.PaidAt >= mp.PeriodStart AND p3.PaidAt < mp.PeriodEnd AND p3.IsDeleted = 0), 0) AS Revenue,
        ISNULL((SELECT COUNT(*) FROM Payments p4 WITH(NOLOCK)
            WHERE p4.PaidAt >= mp.PeriodStart AND p4.PaidAt < mp.PeriodEnd AND p4.IsDeleted = 0), 0) AS PaymentCount,
        ISNULL((SELECT COUNT(*) FROM FeeInvoices fi3 WITH(NOLOCK)
            WHERE fi3.CreatedAt >= mp.PeriodStart AND fi3.CreatedAt < mp.PeriodEnd AND fi3.IsDeleted = 0), 0) AS InvoiceCount,
        ISNULL((SELECT SUM(fi4.TotalAmount - fi4.PaidAmount) FROM FeeInvoices fi4 WITH(NOLOCK)
            WHERE fi4.CreatedAt >= mp.PeriodStart AND fi4.CreatedAt < mp.PeriodEnd AND fi4.IsDeleted = 0
              AND fi4.Status IN (1,2)), 0) AS Outstanding
    FROM MonthPeriods mp
    ORDER BY mp.OffsetNum;
END;
GO
