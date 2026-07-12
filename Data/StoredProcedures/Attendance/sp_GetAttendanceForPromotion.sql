CREATE OR ALTER PROCEDURE sp_GetAttendanceForPromotion
    @AcademicYearId INT,
    @ClassId INT = NULL,
    @SectionId INT = NULL,
    @MinPercentage DECIMAL(5,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @YearStart DATE = (SELECT TOP 1 StartsOn FROM AcademicYears WHERE Id = @AcademicYearId);
    DECLARE @YearEnd DATE = (SELECT TOP 1 EndsOn FROM AcademicYears WHERE Id = @AcademicYearId);

    IF @YearStart IS NULL
    BEGIN
        SELECT
            s.Id AS StudentId,
            s.StudentNo,
            s.FullName,
            s.RollNumber,
            c.Name AS ClassName,
            sec.Name AS SectionName,
            0 AS TotalSchoolDays,
            0 AS PresentDays,
            0 AS AbsentDays,
            0 AS LateDays,
            0 AS LeaveDays,
            0.00 AS AttendancePercentage,
            'N/A' AS EligibilityStatus
        FROM Students s WITH(NOLOCK)
        INNER JOIN Classes c WITH(NOLOCK) ON s.ClassId = c.Id
        INNER JOIN Sections sec WITH(NOLOCK) ON s.SectionId = sec.Id
        WHERE s.IsDeleted = 0 AND s.Status = 1
            AND (@ClassId IS NULL OR s.ClassId = @ClassId)
            AND (@SectionId IS NULL OR s.SectionId = @SectionId)
        ORDER BY c.Name, sec.Name, s.RollNumber;
        RETURN;
    END

    ;WITH AttendanceStats AS (
        SELECT
            s.Id AS StudentId,
            COUNT(DISTINCT a.AttendanceDate) AS TotalSchoolDays,
            COUNT(DISTINCT CASE WHEN a.Status = 1 THEN a.AttendanceDate END) AS PresentDays,
            COUNT(DISTINCT CASE WHEN a.Status = 2 THEN a.AttendanceDate END) AS AbsentDays,
            COUNT(DISTINCT CASE WHEN a.Status = 3 THEN a.AttendanceDate END) AS LateDays,
            COUNT(DISTINCT CASE WHEN a.Status = 4 THEN a.AttendanceDate END) AS LeaveDays
        FROM Students s WITH(NOLOCK)
        LEFT JOIN Attendance a WITH(NOLOCK)
            ON a.StudentId = s.Id
            AND a.AttendanceDate BETWEEN @YearStart AND @YearEnd
            AND a.IsDeleted = 0
        WHERE s.IsDeleted = 0 AND s.Status = 1
            AND (@ClassId IS NULL OR s.ClassId = @ClassId)
            AND (@SectionId IS NULL OR s.SectionId = @SectionId)
        GROUP BY s.Id
    )
    SELECT
        s.Id AS StudentId,
        s.StudentNo,
        s.FullName,
        s.RollNumber,
        c.Name AS ClassName,
        sec.Name AS SectionName,
        astats.TotalSchoolDays,
        astats.PresentDays,
        astats.AbsentDays,
        astats.LateDays,
        astats.LeaveDays,
        CASE
            WHEN astats.TotalSchoolDays > 0
                THEN ROUND(100.0 * (astats.PresentDays + astats.LateDays) / astats.TotalSchoolDays, 2)
            ELSE 0
        END AS AttendancePercentage,
        CASE
            WHEN astats.TotalSchoolDays = 0 THEN 'No Data'
            WHEN @MinPercentage IS NOT NULL
                AND ROUND(100.0 * (astats.PresentDays + astats.LateDays) / astats.TotalSchoolDays, 2) < @MinPercentage
                THEN 'Ineligible'
            WHEN astats.AbsentDays > 30 THEN 'Warning'
            ELSE 'Eligible'
        END AS EligibilityStatus
    FROM Students s WITH(NOLOCK)
    INNER JOIN Classes c WITH(NOLOCK) ON s.ClassId = c.Id
    INNER JOIN Sections sec WITH(NOLOCK) ON s.SectionId = sec.Id
    LEFT JOIN AttendanceStats astats ON s.Id = astats.StudentId
    WHERE s.IsDeleted = 0 AND s.Status = 1
        AND (@ClassId IS NULL OR s.ClassId = @ClassId)
        AND (@SectionId IS NULL OR s.SectionId = @SectionId)
    ORDER BY c.Name, sec.Name, s.RollNumber;
END;
