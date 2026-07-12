CREATE OR ALTER PROCEDURE sp_GetPromotionHistory
    @StudentId INT = NULL,
    @ClassId INT = NULL,
    @AcademicYearId INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ph.Id, s.FullName AS StudentName, s.StudentNo, 
           fc.Name AS FromClassName, tc.Name AS ToClassName,
           ay.Name AS AcademicYearName, ph.Status, ph.PromotedAt,
           ps.SessionName, ph.Remarks,
           COUNT(*) OVER() AS TotalRecords
    FROM PromotionHistories ph WITH(NOLOCK)
    INNER JOIN Students s WITH(NOLOCK) ON s.Id = ph.StudentId
    INNER JOIN SchoolClasses fc WITH(NOLOCK) ON fc.Id = ph.FromClassId
    INNER JOIN SchoolClasses tc WITH(NOLOCK) ON tc.Id = ph.ToClassId
    INNER JOIN AcademicYears ay WITH(NOLOCK) ON ay.Id = ph.AcademicYearId
    LEFT JOIN PromotioSessions ps WITH(NOLOCK) ON ps.Id = ph.PromotioSessionId
    WHERE ph.IsDeleted = 0
      AND (@StudentId IS NULL OR ph.StudentId = @StudentId)
      AND (@ClassId IS NULL OR ph.FromClassId = @ClassId)
      AND (@AcademicYearId IS NULL OR ph.AcademicYearId = @AcademicYearId)
    ORDER BY ph.PromotedAt DESC
    OFFSET @PageSize * (@PageNumber - 1) ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
