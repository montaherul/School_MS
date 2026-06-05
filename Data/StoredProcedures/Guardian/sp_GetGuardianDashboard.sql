CREATE OR ALTER PROCEDURE [dbo].[sp_GetGuardianDashboard]
    @GuardianId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Statistics
    DECLARE @TotalChildren INT;
    DECLARE @TotalDue DECIMAL(18,2);
    DECLARE @UnreadNotifications INT;

    SELECT @TotalChildren = COUNT(*) FROM StudentGuardians WHERE GuardianId = @GuardianId;

    SELECT @TotalDue = SUM(fi.TotalAmount - fi.PaidAmount)
    FROM FeeInvoices fi
    JOIN StudentGuardians sg ON fi.StudentId = sg.StudentId
    WHERE sg.GuardianId = @GuardianId AND fi.Status <> 3; -- 3 = Paid

    SELECT @UnreadNotifications = COUNT(*)
    FROM GuardianNotifications gn
    WHERE gn.GuardianId = @GuardianId AND gn.IsRead = 0;

    SELECT 
        @TotalChildren AS TotalChildren,
        ISNULL(@TotalDue, 0) AS TotalOutstandingFees,
        @UnreadNotifications AS UnreadNotifications;

    -- Children Attendance Summary (Current Month)
    SELECT 
        s.Id AS StudentId,
        s.FullName,
        COUNT(CASE WHEN ar.Status = 1 THEN 1 END) AS PresentCount, -- AttendanceStatus_Present
        COUNT(CASE WHEN ar.Status = 2 THEN 1 END) AS AbsentCount,  -- AttendanceStatus_Absent
        COUNT(ar.Id) AS TotalDays
    FROM StudentGuardians sg
    JOIN Students s ON sg.StudentId = s.Id
    LEFT JOIN Attendance ar ON s.Id = ar.StudentId 
        AND MONTH(ar.AttendanceDate) = MONTH(GETDATE()) 
        AND YEAR(ar.AttendanceDate) = YEAR(GETDATE())
    WHERE sg.GuardianId = @GuardianId
    GROUP BY s.Id, s.FullName;

    -- Recent Notices
    SELECT TOP 5
        n.Id,
        n.Title,
        n.PublishAt AS PublishedAt,
        'General' AS Category
    FROM Notices n
    WHERE n.IsPublished = 1
      AND (n.AudienceRole = 'All' OR n.AudienceRole = 'Guardian' OR n.AudienceRole = 'Guardians')
    ORDER BY n.PublishAt DESC;
END
GO
