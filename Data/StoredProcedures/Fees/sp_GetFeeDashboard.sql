-- ============================================================================
-- Stored Procedure: sp_GetFeeDashboard
-- Purpose: Get fee dashboard aggregate data
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeDashboard
    @AcademicYearId INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalAssigned DECIMAL(18,2), @TotalCollected DECIMAL(18,2),
            @TotalOutstanding DECIMAL(18,2), @TotalDiscounted DECIMAL(18,2),
            @TotalInvoices INT, @TotalPayments INT, @OverdueInvoices INT,
            @CollectionRate DECIMAL(5,2);

    SELECT 
        @TotalAssigned = ISNULL(SUM(TotalAmount), 0),
        @TotalCollected = ISNULL(SUM(PaidAmount), 0),
        @TotalDiscounted = ISNULL(SUM(DiscountAmount), 0),
        @TotalInvoices = COUNT(*),
        @OverdueInvoices = SUM(CASE WHEN [Status] IN (1, 2) AND DueDate < GETDATE() THEN 1 ELSE 0 END)
FROM FeeInvoices WITH(NOLOCK)
    WHERE IsDeleted = 0
      AND (@AcademicYearId = 0 OR AcademicYearId = @AcademicYearId);

    SELECT @TotalPayments = COUNT(*)
FROM Payments p WITH(NOLOCK)
INNER JOIN FeeInvoices fi WITH(NOLOCK) ON p.FeeInvoiceId = fi.Id
    WHERE p.IsDeleted = 0
      AND (@AcademicYearId = 0 OR fi.AcademicYearId = @AcademicYearId);

    SET @TotalOutstanding = @TotalAssigned - @TotalCollected;
    SET @CollectionRate = CASE WHEN @TotalAssigned > 0 THEN (@TotalCollected / @TotalAssigned) * 100 ELSE 0 END;

    SELECT 
        @TotalAssigned AS TotalAssigned,
        @TotalCollected AS TotalCollected,
        @TotalOutstanding AS TotalOutstanding,
        @TotalDiscounted AS TotalDiscounted,
        @TotalInvoices AS TotalInvoices,
        @TotalPayments AS TotalPayments,
        @OverdueInvoices AS OverdueInvoices,
        @CollectionRate AS CollectionRate;

    -- Monthly collection trend (last 6 months)
    SELECT 
        YEAR(PaidAt) AS [Year],
        MONTH(PaidAt) AS [Month],
        SUM(Amount) AS Collected,
        COUNT(*) AS TransactionCount
FROM Payments p WITH(NOLOCK)
INNER JOIN FeeInvoices fi WITH(NOLOCK) ON p.FeeInvoiceId = fi.Id
    WHERE p.IsDeleted = 0
      AND p.PaidAt >= DATEADD(MONTH, -6, GETUTCDATE())
      AND (@AcademicYearId = 0 OR fi.AcademicYearId = @AcademicYearId)
    GROUP BY YEAR(PaidAt), MONTH(PaidAt)
    ORDER BY [Year] DESC, [Month] DESC;

    -- Payment method breakdown
    SELECT 
        p.Method,
        COUNT(*) AS Count,
        SUM(p.Amount) AS Total
FROM Payments p WITH(NOLOCK)
INNER JOIN FeeInvoices fi WITH(NOLOCK) ON p.FeeInvoiceId = fi.Id
    WHERE p.IsDeleted = 0
      AND (@AcademicYearId = 0 OR fi.AcademicYearId = @AcademicYearId)
    GROUP BY p.Method;

    -- Due soon invoices (next 7 days)
    SELECT TOP 10
        fi.Id, fi.InvoiceNo, s.FullName AS StudentName,
        fi.DueDate, fi.TotalAmount, fi.PaidAmount,
        (fi.TotalAmount - fi.PaidAmount) AS DueAmount,
        DATEDIFF(DAY, GETDATE(), fi.DueDate) AS DaysRemaining
FROM FeeInvoices fi WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON fi.StudentId = s.Id
    WHERE fi.IsDeleted = 0
      AND fi.[Status] IN (1, 2)
      AND fi.DueDate BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY, 7, CAST(GETDATE() AS DATE))
      AND (@AcademicYearId = 0 OR fi.AcademicYearId = @AcademicYearId)
    ORDER BY fi.DueDate;
END;
GO
