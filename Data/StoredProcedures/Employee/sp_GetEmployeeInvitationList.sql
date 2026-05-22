-- ============================================================================
-- Stored Procedure: sp_GetEmployeeInvitationList
-- Purpose: Get paginated employee onboarding invitations with search
-- Author: School Management System
-- Created: May 23, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetEmployeeInvitationList
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@SearchTerm NVARCHAR(MAX) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

	WITH InvitationData AS (
		SELECT 
			i.Id,
			i.FullName,
			i.Email,
			i.Mobile,
			i.InvitationToken,
			i.DepartmentId,
			dept.Name AS DepartmentName,
			i.DesignationId,
			desig.Name AS DesignationName,
			i.JoiningDate,
			i.EmploymentType,
			i.Status,
			i.IsTeachingStaff,
			i.Remarks,
			i.ExpiresAt,
			i.IsUsed,
			i.IsApproved,
			i.InvitationStatus,
			i.CreatedAt
		FROM EmployeeInvitations i
		LEFT JOIN Departments dept ON i.DepartmentId = dept.Id
		LEFT JOIN Designations desig ON i.DesignationId = desig.Id
		WHERE i.IsDeleted = 0
		AND (
			@SearchTerm IS NULL OR 
			i.FullName LIKE '%' + @SearchTerm + '%' OR 
			i.Email LIKE '%' + @SearchTerm + '%' OR 
			i.Mobile LIKE '%' + @SearchTerm + '%'
		)
	),
	CountData AS (
		SELECT COUNT(*) AS TotalRecords FROM InvitationData
	)
	SELECT 
		i.*,
		c.TotalRecords
	FROM InvitationData i
	CROSS JOIN CountData c
	ORDER BY i.CreatedAt DESC
	OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
