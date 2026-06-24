CREATE OR ALTER PROCEDURE [dbo].[sp_GetTeacherAssignedExams]
    @TeacherId INT,
    @AcademicYearId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT e.Id AS ExamId, e.Name AS ExamName, e.Term, e.StartsOn, e.EndsOn, e.Status,
           ay.Id AS AcademicYearId, ay.Name AS AcademicYearName
FROM Exams e WITH(NOLOCK)
INNER JOIN AcademicYears ay WITH(NOLOCK) ON ay.Id = e.AcademicYearId AND ay.IsDeleted = 0
    WHERE e.IsDeleted = 0
      AND (e.Status = 1 OR e.Status = 2) -- Draft or Submitted
      AND EXISTS (
          SELECT 1 FROM TeacherClassAssignments tca
          WHERE tca.TeacherId = @TeacherId AND tca.IsActive = 1 AND tca.IsDeleted = 0
          AND tca.AcademicYearId = e.AcademicYearId
      )
      AND (@AcademicYearId IS NULL OR e.AcademicYearId = @AcademicYearId)
    ORDER BY e.StartsOn DESC;
END;
GO
