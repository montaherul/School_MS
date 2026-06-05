/*
   Stored Procedure: sp_GetGuardianLeaveApplications
   Description: Retrieves leave applications submitted by a guardian for their children.
   Parameters:
     @GuardianId INT - ID of the guardian.
*/
CREATE PROCEDURE [dbo].[sp_GetGuardianLeaveApplications]
    @GuardianId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT sla.Id,
           sla.StudentId,
           sla.GuardianId,
           sla.LeaveTypeId,
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
    FROM dbo.StudentLeaveApplication sla
    INNER JOIN dbo.StudentGuardian sg ON sg.StudentId = sla.StudentId
    WHERE sg.GuardianId = @GuardianId;
END
