CREATE OR ALTER PROCEDURE sp_AdmissionClassDemand
    @DateFrom DATE = NULL,
    @DateTo DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);
    IF @DateFrom IS NULL SET @DateFrom = DATEADD(YEAR, -10, @Today);
    IF @DateTo IS NULL SET @DateTo = @Today;

    SELECT
        c.Name AS ClassName,
        c.SortOrder,
        COUNT(*) AS TotalApplications,
        SUM(CASE WHEN a.Status = 4 THEN 1 ELSE 0 END) AS ConvertedCount,
        SUM(CASE WHEN a.Status = 1 THEN 1 ELSE 0 END) AS PendingCount,
        SUM(CASE WHEN a.Status = 2 THEN 1 ELSE 0 END) AS ApprovedCount,
        CASE WHEN COUNT(*) > 0
            THEN CAST(SUM(CASE WHEN a.Status = 4 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100
            ELSE 0
        END AS ConversionRate,
        COUNT(DISTINCT a.Gender) AS GenderCount,
        COUNT(DISTINCT a.Religion) AS ReligionDiversity
    FROM Admissions a WITH(NOLOCK)
    INNER JOIN Classes c WITH(NOLOCK) ON a.AppliedClassId = c.Id
    WHERE a.IsDeleted = 0
        AND c.IsDeleted = 0
        AND CAST(a.CreatedAt AS DATE) >= @DateFrom
        AND CAST(a.CreatedAt AS DATE) <= @DateTo
    GROUP BY c.Name, c.SortOrder
    ORDER BY c.SortOrder;

    -- Class-wise gender breakdown
    SELECT
        c.Name AS ClassName,
        a.Gender,
        COUNT(*) AS Count
    FROM Admissions a WITH(NOLOCK)
    INNER JOIN Classes c WITH(NOLOCK) ON a.AppliedClassId = c.Id
    WHERE a.IsDeleted = 0
        AND c.IsDeleted = 0
        AND CAST(a.CreatedAt AS DATE) >= @DateFrom
        AND CAST(a.CreatedAt AS DATE) <= @DateTo
    GROUP BY c.Name, a.Gender
    ORDER BY c.Name, a.Gender;
END;
GO
