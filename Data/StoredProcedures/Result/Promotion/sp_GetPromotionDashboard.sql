CREATE OR ALTER PROCEDURE sp_GetPromotionDashboard
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(*) FROM Students s WHERE s.IsDeleted = 0 AND s.Status = 'Active' AND EXISTS (SELECT 1 FROM FinalResults fr WHERE fr.StudentId = s.Id AND fr.AcademicYearId = @AcademicYearId)) AS TotalStudents,
        (SELECT COUNT(*) FROM FinalResults fr WHERE fr.AcademicYearId = @AcademicYearId AND fr.IsPassed = 1 AND fr.PromotioStatus = 'Pending') AS EligibleForPromotion,
        (SELECT COUNT(*) FROM FinalResults fr WHERE fr.AcademicYearId = @AcademicYearId AND fr.IsPassed = 0) AS FailedStudents,
        (SELECT COUNT(*) FROM FinalResults fr WHERE fr.AcademicYearId = @AcademicYearId AND fr.PromotioStatus = 'Promoted') AS AlreadyPromoted,
        (SELECT COUNT(*) FROM PromotionHistories ph WHERE ph.AcademicYearId = @AcademicYearId AND ph.IsDeleted = 0) AS TotalPromotions,
        (SELECT COUNT(*) FROM PromotioSessions ps WHERE ps.AcademicYearId = @AcademicYearId AND ps.IsDeleted = 0 AND ps.Status = 'Draft') AS DraftSessions,
        (SELECT COUNT(*) FROM PromotioSessions ps WHERE ps.AcademicYearId = @AcademicYearId AND ps.IsDeleted = 0 AND ps.Status = 'Completed') AS CompletedSessions;
END;
