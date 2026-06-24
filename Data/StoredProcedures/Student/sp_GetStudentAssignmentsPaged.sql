CREATE OR ALTER PROCEDURE sp_GetStudentAssignmentsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT,
    @ClassId INT,
    @SectionId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT
            at.Id,
            at.Title,
            at.Instructions,
            at.Deadline,
            at.[Status] AS AssignmentStatus,
            s.Name AS SubjectName,
            t.FullName AS TeacherName,
            CASE WHEN asub.Id IS NOT NULL THEN 1 ELSE 0 END AS IsSubmitted,
            asub.SubmittedAt,
            asub.Marks,
            asub.Feedback,

            COUNT(*) OVER () AS TotalRecords
FROM AssignmentTasks at WITH(NOLOCK)
INNER JOIN Subjects s WITH(NOLOCK) ON at.SubjectId = s.Id
INNER JOIN Teachers t WITH(NOLOCK) ON at.TeacherProfileId = t.Id
LEFT JOIN AssignmentSubmissions asub WITH(NOLOCK) ON asub.AssignmentTaskId = at.Id AND asub.StudentId = @StudentId AND asub.IsDeleted = 0
        WHERE at.IsDeleted = 0
          AND at.SchoolClassId = @ClassId
          AND at.SectionId = @SectionId
          AND (@SearchTerm IS NULL OR at.Title LIKE '%' + @SearchTerm + '%' OR s.Name LIKE '%' + @SearchTerm + '%')
    
ORDER BY at.Deadline ASC, at.Id ASC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO