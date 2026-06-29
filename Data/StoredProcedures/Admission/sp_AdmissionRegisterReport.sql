CREATE OR ALTER PROCEDURE sp_AdmissionRegisterReport
    @DateFrom DATE = NULL,
    @DateTo DATE = NULL,
    @ClassId INT = NULL,
    @Status INT = NULL,
    @Gender NVARCHAR(20) = NULL,
    @Religion NVARCHAR(30) = NULL,
    @District NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);
    IF @DateFrom IS NULL SET @DateFrom = DATEADD(YEAR, -10, @Today);
    IF @DateTo IS NULL SET @DateTo = @Today;

    SELECT
        ROW_NUMBER() OVER (ORDER BY a.CreatedAt DESC, a.Id DESC) AS SerialNo,
        a.ApplicationNo,
        a.ApplicantName,
        a.ApplicantNameBangla AS NameBangla,
        a.DateOfBirth,
        a.Gender,
        a.FatherName,
        a.MotherName,
        a.ApplicantMobileNumber AS Mobile,
        a.Religion,
        c.Name AS AppliedClass,
        CASE a.[Status]
            WHEN 1 THEN 'Pending'
            WHEN 2 THEN 'Approved'
            WHEN 3 THEN 'Rejected'
            WHEN 4 THEN 'Converted'
            ELSE 'Unknown'
        END AS [Status],
        a.CreatedAt AS SubmittedAt
    FROM Admissions a WITH(NOLOCK)
    INNER JOIN Classes c WITH(NOLOCK) ON a.AppliedClassId = c.Id
    WHERE a.IsDeleted = 0
        AND CAST(a.CreatedAt AS DATE) >= @DateFrom
        AND CAST(a.CreatedAt AS DATE) <= @DateTo
        AND (@ClassId IS NULL OR a.AppliedClassId = @ClassId)
        AND (@Status IS NULL OR a.Status = @Status)
        AND (@Gender IS NULL OR a.Gender = @Gender)
        AND (@Religion IS NULL OR a.Religion = @Religion)
        AND (@District IS NULL OR a.PresentDistrict = @District OR a.PermanentDistrict = @District)
    ORDER BY a.CreatedAt DESC, a.Id DESC;
END;
GO
