CREATE OR ALTER PROCEDURE sp_GetTeacherDashboardPendingResults
    @TeacherId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS PendingCount
    FROM Marks
    WHERE EnteredByTeacherId = @TeacherId
      AND IsDeleted = 0
      AND [Status] = 0; -- Draft
END;
