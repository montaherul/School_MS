-- ============================================================================
-- Stored Procedure: sp_GetExamsForAdmin
-- Purpose: Get active exams with summary stats
-- ============================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamsForAdmin]
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.Id, 
        e.Name, 
        e.Term, 
        e.StartsOn, 
        e.EndsOn, 
        e.Status,
        (SELECT COUNT(*) FROM StudentExamResults r WHERE r.ExamId = e.Id AND r.IsDeleted = 0) as StudentCount,
        (SELECT COUNT(*) FROM Marks m WHERE m.ExamId = e.Id AND m.Status = 4 AND m.IsDeleted = 0) as PublishedMarks
FROM Exams e WITH(NOLOCK)
    WHERE e.AcademicYearId = @AcademicYearId AND e.IsDeleted = 0
    ORDER BY e.StartsOn DESC;
END;
GO
