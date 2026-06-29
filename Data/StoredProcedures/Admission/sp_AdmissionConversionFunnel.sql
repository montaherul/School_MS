CREATE OR ALTER PROCEDURE sp_AdmissionConversionFunnel
    @DateFrom DATE = NULL,
    @DateTo DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);
    IF @DateFrom IS NULL SET @DateFrom = DATEADD(YEAR, -10, @Today);
    IF @DateTo IS NULL SET @DateTo = @Today;

    SELECT
        @DateFrom AS DateFrom,
        @DateTo AS DateTo,
        COUNT(*) AS TotalApplications,
        SUM(CASE WHEN a.AllDocumentsVerified = 1 THEN 1 ELSE 0 END) AS DocumentVerified,
        SUM(CASE WHEN a.Status > 1 THEN 1 ELSE 0 END) AS InterviewCompleted,
        SUM(CASE WHEN a.AdmissionFeePaid = 1 THEN 1 ELSE 0 END) AS FeePaid,
        SUM(CASE WHEN a.Status = 2 THEN 1 ELSE 0 END) AS Approved,
        SUM(CASE WHEN a.Status = 4 THEN 1 ELSE 0 END) AS Converted,
        CASE WHEN COUNT(*) > 0
            THEN CAST(SUM(CASE WHEN a.Status = 4 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100
            ELSE 0
        END AS ConversionRate
    FROM Admissions a WITH(NOLOCK)
    WHERE a.IsDeleted = 0
        AND CAST(a.CreatedAt AS DATE) >= @DateFrom
        AND CAST(a.CreatedAt AS DATE) <= @DateTo;
END;
GO
