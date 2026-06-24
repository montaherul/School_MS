/*
   Stored Procedure: sp_GetGuardianLeaveApplications
   Description: Retrieves leave applications submitted by a guardian for their children.
   Parameters:
     @GuardianId INT - ID of the guardian.
     @StudentId INT = NULL - Filter by student.
     @Status INT = NULL - Filter by approval status (0=Pending,1=Approved,2=Rejected).
     @PageNumber INT = 1
     @PageSize INT = 20
*/
CREATE OR ALTER PROCEDURE [dbo].[sp_GetGuardianLeaveApplications]
    @GuardianId INT,
    @StudentId INT = NULL,
    @Status INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
FROM dbo WITH(NOLOCK).StudentLeaveApplication sla
INNER JOIN dbo WITH(NOLOCK).StudentGuardian sg ON sg.StudentId = sla.StudentId
    WHERE sg.GuardianId = @GuardianId
      AND (@StudentId IS NULL OR sla.StudentId = @StudentId)
      AND (@Status IS NULL OR sla.ApprovalStatus = @Status);

    SELECT sla.Id,
           sla.StudentId,
           s.FullName AS StudentName,
           sla.GuardianId,
           sla.LeaveTypeId,
           lt.Name AS LeaveTypeName,
           sla.FromDate,
           sla.ToDate,
           sla.TotalDays,
           sla.Reason,
           sla.AttachmentPath,
           sla.ApprovalStatus,
           sla.ApprovedBy,
           sla.ApprovedAt,
           sla.Remarks,
           sla.CreatedAt
FROM dbo WITH(NOLOCK).StudentLeaveApplication sla
INNER JOIN dbo WITH(NOLOCK).StudentGuardian sg ON sg.StudentId = sla.StudentId
LEFT JOIN dbo WITH(NOLOCK).Students s ON sla.StudentId = s.Id
LEFT JOIN dbo WITH(NOLOCK).LeaveTypes lt ON sla.LeaveTypeId = lt.Id
    WHERE sg.GuardianId = @GuardianId
      AND (@StudentId IS NULL OR sla.StudentId = @StudentId)
      AND (@Status IS NULL OR sla.ApprovalStatus = @Status)
    ORDER BY sla.Id DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
