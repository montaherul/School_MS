CREATE OR ALTER PROCEDURE sp_AdmissionDashboard
    @DateFrom DATE = NULL,
    @DateTo DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);
    DECLARE @WeekStart DATE = DATEADD(DAY, -(DATEPART(WEEKDAY, @Today) - 1), @Today);
    DECLARE @MonthStart DATE = DATEFROMPARTS(YEAR(@Today), MONTH(@Today), 1);
    DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);

    IF @DateFrom IS NULL SET @DateFrom = @YearStart;
    IF @DateTo IS NULL SET @DateTo = @Today;

    -- Today's stats
    SELECT COUNT(*) AS TodayApplications FROM Admissions WITH(NOLOCK) 
        WHERE CAST(CreatedAt AS DATE) = @Today AND IsDeleted = 0;
    
    -- This Week
    SELECT COUNT(*) AS WeekApplications FROM Admissions WITH(NOLOCK) 
        WHERE CAST(CreatedAt AS DATE) >= @WeekStart AND IsDeleted = 0;
    
    -- This Month
    SELECT COUNT(*) AS MonthApplications FROM Admissions WITH(NOLOCK) 
        WHERE CAST(CreatedAt AS DATE) >= @MonthStart AND IsDeleted = 0;

    -- Status breakdown
    SELECT 
        SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS PendingVerification,
        SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) AS Approved,
        SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END) AS Rejected,
        SUM(CASE WHEN Status = 4 THEN 1 ELSE 0 END) AS Converted
    FROM Admissions WITH(NOLOCK) WHERE IsDeleted = 0;

    -- Monthly trend (last 12 months)
    SELECT 
        YEAR(CreatedAt) AS Year,
        MONTH(CreatedAt) AS Month,
        COUNT(*) AS Count,
        SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS PendingCount,
        SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) AS ApprovedCount,
        SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END) AS RejectedCount,
        SUM(CASE WHEN Status = 4 THEN 1 ELSE 0 END) AS ConvertedCount
    FROM Admissions WITH(NOLOCK) 
    WHERE CreatedAt >= DATEADD(MONTH, -12, @Today) AND IsDeleted = 0
    GROUP BY YEAR(CreatedAt), MONTH(CreatedAt)
    ORDER BY Year, Month;

    -- Class-wise distribution
    SELECT 
        c.Name AS ClassName,
        COUNT(*) AS Total,
        SUM(CASE WHEN a.Status = 4 THEN 1 ELSE 0 END) AS Converted
    FROM Admissions a WITH(NOLOCK)
    INNER JOIN Classes c WITH(NOLOCK) ON a.AppliedClassId = c.Id
    WHERE a.IsDeleted = 0 AND c.IsDeleted = 0
    GROUP BY c.Name, c.SortOrder
    ORDER BY c.SortOrder;

    -- Gender distribution
    SELECT Gender, COUNT(*) AS Count 
    FROM Admissions WITH(NOLOCK) 
    WHERE IsDeleted = 0
    GROUP BY Gender;

    -- Religion distribution
    SELECT Religion, COUNT(*) AS Count 
    FROM Admissions WITH(NOLOCK) 
    WHERE IsDeleted = 0
    GROUP BY Religion;

    -- District distribution
    SELECT PresentDistrict AS District, COUNT(*) AS Count 
    FROM Admissions WITH(NOLOCK) 
    WHERE IsDeleted = 0 AND PresentDistrict IS NOT NULL
    GROUP BY PresentDistrict
    ORDER BY Count DESC;

    -- Conversion rate
    SELECT 
        COUNT(*) AS TotalApplications,
        SUM(CASE WHEN Status = 4 THEN 1 ELSE 0 END) AS ConvertedCount,
        CASE WHEN COUNT(*) > 0 
            THEN CAST(SUM(CASE WHEN Status = 4 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100 
            ELSE 0 
        END AS ConversionRate
    FROM Admissions WITH(NOLOCK) WHERE IsDeleted = 0;

    -- Revenue summary (from fee invoices for admission apps)
    SELECT 
        ISNULL(SUM(TotalAmount), 0) AS TotalInvoiceAmount,
        ISNULL(SUM(PaidAmount), 0) AS TotalPaidAmount,
        COUNT(*) AS TotalInvoices,
        SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END) AS PaidInvoices
    FROM FeeInvoices WITH(NOLOCK)
    WHERE Remarks LIKE 'AdmissionApp_%' AND IsDeleted = 0;

    -- Application source (distinct days with counts)
    SELECT 
        CAST(CreatedAt AS DATE) AS Date,
        COUNT(*) AS Count
    FROM Admissions WITH(NOLOCK)
    WHERE CreatedAt >= @DateFrom AND CreatedAt <= @DateTo AND IsDeleted = 0
    GROUP BY CAST(CreatedAt AS DATE)
    ORDER BY Date;

    -- Top classes by application volume
    SELECT TOP 5
        c.Name AS ClassName,
        COUNT(*) AS ApplicationCount
    FROM Admissions a WITH(NOLOCK)
    INNER JOIN Classes c WITH(NOLOCK) ON a.AppliedClassId = c.Id
    WHERE a.IsDeleted = 0 AND c.IsDeleted = 0
    GROUP BY c.Name, c.SortOrder
    ORDER BY ApplicationCount DESC;
END;
GO
