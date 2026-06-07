CREATE OR ALTER PROCEDURE [dbo].[sp_GetGuardianChildren]
    @GuardianId INT,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.Id AS StudentId,
        s.StudentNo,
        s.FullName,
        ISNULL(c.Name, 'N/A') AS ClassName,
        ISNULL(sec.Name, 'N/A') AS SectionName,
        s.RollNumber,
        s.ProfilePicturePath,
        CASE sg.Relationship
            WHEN 1 THEN 'Father'
            WHEN 2 THEN 'Mother'
            WHEN 3 THEN 'LegalGuardian'
            WHEN 4 THEN 'Grandfather'
            WHEN 5 THEN 'Grandmother'
            WHEN 6 THEN 'Uncle'
            WHEN 7 THEN 'Aunt'
            WHEN 8 THEN 'Brother'
            WHEN 9 THEN 'Sister'
            WHEN 10 THEN 'Other'
            ELSE 'Other'
        END AS Relationship,
        CAST(sg.IsPrimaryGuardian AS BIT) AS IsPrimaryGuardian,
        ISNULL(atts.TotalDays, 0) AS TotalDays,
        ISNULL(atts.PresentCount, 0) AS PresentCount,
        ISNULL(atts.AbsentCount, 0) AS AbsentCount,
        ISNULL(atts.LateCount, 0) AS LateCount,
        CASE WHEN ISNULL(atts.TotalDays, 0) = 0 THEN 0
             ELSE CAST(ROUND((CAST(ISNULL(atts.PresentCount, 0) + ISNULL(atts.LateCount, 0) AS FLOAT) / atts.TotalDays) * 100, 2) AS DECIMAL(10,2))
        END AS AttendancePercentage,
        ISNULL(fees.Outstanding, 0) AS OutstandingFees,
        ISNULL(unr.UnreadCount, 0) AS UnreadNotificationCount
    FROM StudentGuardians sg
    JOIN Students s ON sg.StudentId = s.Id
    LEFT JOIN Classes c ON s.ClassId = c.Id
    LEFT JOIN Sections sec ON s.SectionId = sec.Id
    OUTER APPLY (
        SELECT 
            COUNT(*) AS TotalDays,
            SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS PresentCount,
            SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) AS AbsentCount,
            SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END) AS LateCount
        FROM Attendance a 
        WHERE a.StudentId = s.Id AND a.IsDeleted = 0
          AND (@FromDate IS NULL OR a.AttendanceDate >= @FromDate)
          AND (@ToDate IS NULL OR a.AttendanceDate <= @ToDate)
    ) atts
    OUTER APPLY (
        SELECT ISNULL(SUM(fi.TotalAmount - fi.PaidAmount), 0) AS Outstanding
        FROM FeeInvoices fi
        WHERE fi.StudentId = s.Id AND fi.IsDeleted = 0 AND fi.Status <> 3
    ) fees
    OUTER APPLY (
        SELECT COUNT(*) AS UnreadCount
        FROM GuardianNotifications gn
        WHERE gn.GuardianId = sg.GuardianId AND gn.IsRead = 0 AND gn.IsDeleted = 0
    ) unr
    WHERE sg.GuardianId = @GuardianId AND sg.IsDeleted = 0 AND s.IsDeleted = 0
    ORDER BY sg.IsPrimaryGuardian DESC, s.FullName;
END
GO
