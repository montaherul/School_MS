CREATE OR ALTER PROCEDURE [dbo].[sp_GetGuardianDashboard]
    @GuardianId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Statistics
    DECLARE @TotalChildren INT;
    DECLARE @TotalDue DECIMAL(18,2);
    DECLARE @UnreadNotifications INT;

    SELECT @TotalChildren = COUNT(*) FROM StudentGuardians WHERE GuardianId = @GuardianId AND IsDeleted = 0;

    SELECT @TotalDue = SUM(fi.TotalAmount - fi.PaidAmount)
FROM FeeInvoices fi WITH(NOLOCK)
JOIN StudentGuardians sg WITH(NOLOCK) ON fi.StudentId = sg.StudentId AND sg.IsDeleted = 0
    WHERE sg.GuardianId = @GuardianId AND fi.IsDeleted = 0 AND fi.Status <> 3; -- 3 = Paid

    SELECT @UnreadNotifications = COUNT(*)
FROM GuardianNotifications gn WITH(NOLOCK)
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
FROM StudentGuardians sg WITH(NOLOCK)
JOIN Students s WITH(NOLOCK) ON sg.StudentId = s.Id AND s.IsDeleted = 0
LEFT JOIN Attendance ar WITH(NOLOCK) ON s.Id = ar.StudentId AND ar.IsDeleted = 0
        AND ar.AttendanceDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
        AND ar.AttendanceDate < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
    WHERE sg.GuardianId = @GuardianId AND sg.IsDeleted = 0
    GROUP BY s.Id, s.FullName;

    -- Recent Notices
    SELECT TOP 5
        n.Id,
        n.Title,
        n.PublishAt AS PublishedAt,
        'General' AS Category
FROM Notices n WITH(NOLOCK)
    WHERE n.IsPublished = 1
      AND (n.AudienceRole = 'All' OR n.AudienceRole = 'Guardian' OR n.AudienceRole = 'Guardians')
    ORDER BY n.PublishAt DESC;
END
GO
