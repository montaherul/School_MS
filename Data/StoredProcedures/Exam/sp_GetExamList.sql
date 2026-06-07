CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamList]
    @AcademicYearId INT,
    @SearchTerm NVARCHAR(100) = NULL,
    @Status INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @SortColumn NVARCHAR(50) = 'CreatedAt',
    @SortDirection NVARCHAR(4) = 'DESC'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT COUNT(*) AS TotalCount
    FROM Exams e
    WHERE e.IsDeleted = 0
      AND e.AcademicYearId = @AcademicYearId
      AND (@SearchTerm IS NULL OR e.Name LIKE '%' + @SearchTerm + '%')
      AND (@Status IS NULL OR e.Status = @Status);

    SELECT 
        e.Id,
        e.Name,
        e.Term,
        e.StartsOn,
        e.EndsOn,
        e.Status,
        e.AcademicYearId,
        e.StudentGroupId,
        e.IsLocked,
        e.CreatedAt,
        e.CreatedBy,
        (SELECT COUNT(*) FROM ExamSubjects es WHERE es.ExamId = e.Id) AS SubjectCount,
        (SELECT COUNT(*) FROM StudentExamResults ser WHERE ser.ExamId = e.Id AND ser.IsDeleted = 0) AS StudentResultCount
    FROM Exams e
    WHERE e.IsDeleted = 0
      AND e.AcademicYearId = @AcademicYearId
      AND (@SearchTerm IS NULL OR e.Name LIKE '%' + @SearchTerm + '%')
      AND (@Status IS NULL OR e.Status = @Status)
    ORDER BY 
        CASE WHEN @SortColumn = 'Name' AND @SortDirection = 'ASC' THEN e.Name END ASC,
        CASE WHEN @SortColumn = 'Name' AND @SortDirection = 'DESC' THEN e.Name END DESC,
        CASE WHEN @SortColumn = 'StartsOn' AND @SortDirection = 'ASC' THEN CONVERT(NVARCHAR(10), e.StartsOn, 112) END ASC,
        CASE WHEN @SortColumn = 'StartsOn' AND @SortDirection = 'DESC' THEN CONVERT(NVARCHAR(10), e.StartsOn, 112) END DESC,
        CASE WHEN @SortColumn = 'Status' AND @SortDirection = 'ASC' THEN RIGHT('000' + CAST(e.Status AS NVARCHAR), 3) END ASC,
        CASE WHEN @SortColumn = 'Status' AND @SortDirection = 'DESC' THEN RIGHT('000' + CAST(e.Status AS NVARCHAR), 3) END DESC,
        e.CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO