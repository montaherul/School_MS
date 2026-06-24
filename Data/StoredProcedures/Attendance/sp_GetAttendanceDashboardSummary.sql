CREATE OR ALTER PROCEDURE sp_GetAttendanceDashboardSummary
    @Date DATE = NULL,
    @StudentId INT = 0,
    @GuardianId INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TargetDate DATE = COALESCE(@Date, CAST(GETDATE() AS DATE));
    
    -- Student Attendance Today
    DECLARE @StudentTotal INT = 0;
    DECLARE @StudentPresent INT = 0;
    DECLARE @StudentAbsent INT = 0;
    DECLARE @StudentLate INT = 0;
    DECLARE @StudentLeave INT = 0;
    DECLARE @StudentPct DECIMAL(5,2) = 0.00;
    
    -- If @GuardianId is provided, we filter by students linked to that guardian
    -- If @StudentId is provided, we filter by that specific student
    
    SELECT 
        @StudentTotal = COUNT(*),
        @StudentPresent = SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END),
        @StudentAbsent = SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END),
        @StudentLate = SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END),
        @StudentLeave = SUM(CASE WHEN Status = 4 THEN 1 ELSE 0 END)
FROM Attendance a WITH(NOLOCK)
    WHERE a.AttendanceDate = @TargetDate AND a.IsDeleted = 0
      AND (@StudentId = 0 OR a.StudentId = @StudentId)
      AND (@GuardianId = 0 OR a.StudentId IN (SELECT StudentId FROM StudentGuardians WHERE GuardianId = @GuardianId));
    
    IF @StudentTotal > 0
        SET @StudentPct = CAST((@StudentPresent + @StudentLate) AS DECIMAL(18,2)) / @StudentTotal * 100;
        
    -- Employee Attendance Today (Only for Admin views, i.e., when @StudentId = 0 and @GuardianId = 0)
    DECLARE @EmployeeTotal INT = 0;
    DECLARE @EmployeePresent INT = 0;
    DECLARE @EmployeeAbsent INT = 0;
    DECLARE @EmployeeLate INT = 0;
    DECLARE @EmployeeLeave INT = 0;
    DECLARE @EmployeePct DECIMAL(5,2) = 0.00;
    
    IF @StudentId = 0 AND @GuardianId = 0
    BEGIN
        SELECT 
            @EmployeeTotal = COUNT(*),
            @EmployeePresent = SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END),
            @EmployeeAbsent = SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END),
            @EmployeeLate = SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END),
            @EmployeeLeave = SUM(CASE WHEN Status = 4 THEN 1 ELSE 0 END)
FROM EmployeeAttendances WITH(NOLOCK)
        WHERE CAST(AttendanceDate AS DATE) = @TargetDate AND IsDeleted = 0;
        
        IF @EmployeeTotal > 0
            SET @EmployeePct = CAST((@EmployeePresent + @EmployeeLate) AS DECIMAL(18,2)) / @EmployeeTotal * 100;
    END

    -- Sessions counts
    DECLARE @PendingSessions INT = 0;
    DECLARE @LockedSessions INT = 0;
    DECLARE @ApprovedSessions INT = 0;
    
    IF @StudentId = 0 AND @GuardianId = 0
    BEGIN
        SELECT 
            @PendingSessions = SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END), -- Submitted / Pending Approval
            @LockedSessions = SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END), -- Locked
            @ApprovedSessions = SUM(CASE WHEN Status = 5 THEN 1 ELSE 0 END) -- Approved
FROM AttendanceSessions WITH(NOLOCK)
        WHERE AttendanceDate = @TargetDate AND IsDeleted = 0;
    END
    
    -- Return summary row
    SELECT
        TotalStudents = ISNULL(@StudentTotal, 0),
        StudentPresent = ISNULL(@StudentPresent, 0),
        StudentAbsent = ISNULL(@StudentAbsent, 0),
        StudentLate = ISNULL(@StudentLate, 0),
        StudentLeave = ISNULL(@StudentLeave, 0),
        StudentAttendancePercentage = ISNULL(@StudentPct, 0.00),
        TotalEmployees = ISNULL(@EmployeeTotal, 0),
        EmployeePresent = ISNULL(@EmployeePresent, 0),
        EmployeeAbsent = ISNULL(@EmployeeAbsent, 0),
        EmployeeLate = ISNULL(@EmployeeLate, 0),
        EmployeeLeave = ISNULL(@EmployeeLeave, 0),
        EmployeeAttendancePercentage = ISNULL(@EmployeePct, 0.00),
        ClassesMissingAttendance = 0,
        PendingSessions = ISNULL(@PendingSessions, 0),
        LockedSessions = ISNULL(@LockedSessions, 0),
        ApprovedSessions = ISNULL(@ApprovedSessions, 0);
END;
GO
