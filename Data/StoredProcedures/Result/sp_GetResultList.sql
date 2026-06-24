CREATE OR ALTER PROCEDURE [dbo].[sp_GetResultList]
    @ExamId INT = NULL,
    @ClassId INT = NULL,
    @SectionId INT = NULL,
    @StudentGroupId INT = NULL,
    @Status INT = NULL,
    @SearchTerm NVARCHAR(100) = NULL,
    @AcademicYearId INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Result Set 1: Paged Data
    SELECT 
        ser.Id,
        ser.ExamId,
        e.Name AS ExamName,
        e.Term,
        ser.StudentId,
        s.FullName AS StudentName,
        s.StudentNo,
        s.RollNumber,
        s.ClassId,
        cl.Name AS ClassName,
        s.SectionId,
        sec.Name AS SectionName,
        s.StudentGroupId,
        sg.Name AS GroupName,
        ser.TotalMarks,
        ser.TotalFullMarks,
        ser.Gpa,
        ser.Grade,
        ser.Position,
        ser.ClassPosition,
        ser.GroupPosition,
        ser.IsPassed,
        ser.FailedSubjectCount,
        ser.PassedSubjectCount,
        ser.Status,
        ser.PublishedAt
FROM StudentExamResults ser WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON ser.StudentId = s.Id
INNER JOIN Exams e WITH(NOLOCK) ON ser.ExamId = e.Id
INNER JOIN Classes cl WITH(NOLOCK) ON s.ClassId = cl.Id
LEFT JOIN Sections sec WITH(NOLOCK) ON s.SectionId = sec.Id
LEFT JOIN StudentGroups sg WITH(NOLOCK) ON s.StudentGroupId = sg.Id
    WHERE ser.IsDeleted = 0
      AND (@ExamId IS NULL OR ser.ExamId = @ExamId)
      AND (@ClassId IS NULL OR s.ClassId = @ClassId)
      AND (@SectionId IS NULL OR s.SectionId = @SectionId)
      AND (@StudentGroupId IS NULL OR s.StudentGroupId = @StudentGroupId)
      AND (@Status IS NULL OR ser.Status = @Status)
      AND (@AcademicYearId IS NULL OR e.AcademicYearId = @AcademicYearId)
      AND (@SearchTerm IS NULL OR s.FullName LIKE '%' + @SearchTerm + '%' OR s.StudentNo LIKE '%' + @SearchTerm + '%' OR s.RollNumber LIKE '%' + @SearchTerm + '%')
    ORDER BY s.ClassId, s.RollNumber, ser.Id
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    -- Result Set 2: Total Record Count
    SELECT COUNT(*) AS TotalCount
FROM StudentExamResults ser WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON ser.StudentId = s.Id
INNER JOIN Exams e WITH(NOLOCK) ON ser.ExamId = e.Id
INNER JOIN Classes cl WITH(NOLOCK) ON s.ClassId = cl.Id
LEFT JOIN Sections sec WITH(NOLOCK) ON s.SectionId = sec.Id
LEFT JOIN StudentGroups sg WITH(NOLOCK) ON s.StudentGroupId = sg.Id
    WHERE ser.IsDeleted = 0
      AND (@ExamId IS NULL OR ser.ExamId = @ExamId)
      AND (@ClassId IS NULL OR s.ClassId = @ClassId)
      AND (@SectionId IS NULL OR s.SectionId = @SectionId)
      AND (@StudentGroupId IS NULL OR s.StudentGroupId = @StudentGroupId)
      AND (@Status IS NULL OR ser.Status = @Status)
      AND (@AcademicYearId IS NULL OR e.AcademicYearId = @AcademicYearId)
      AND (@SearchTerm IS NULL OR s.FullName LIKE '%' + @SearchTerm + '%' OR s.StudentNo LIKE '%' + @SearchTerm + '%' OR s.RollNumber LIKE '%' + @SearchTerm + '%');
END;
GO
