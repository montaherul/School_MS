CREATE OR ALTER PROCEDURE [dbo].[sp_CalculateMerit]
    @ExamGroupKey NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Determine exams: all matching the group key, or all exams if null
    DECLARE @Exams TABLE (Id INT, ClassId INT, StudentGroupId INT NULL, Name NVARCHAR(500));

    INSERT INTO @Exams
    SELECT Id, ClassId, StudentGroupId, Name
FROM Exams WITH(NOLOCK)
    WHERE IsDeleted = 0
      AND (@ExamGroupKey IS NULL OR Name LIKE '%' + @ExamGroupKey + '%');

    -- Process ALL classes in the exam group â€” no FirstOrDefault
    -- Class Position: rank within each (ExamId, ClassId)
    UPDATE ser
    SET ClassPosition = ranked.NewPosition
FROM StudentExamResults ser WITH(NOLOCK)
    INNER JOIN (
        SELECT
            r.Id,
            RANK() OVER (
                PARTITION BY r.ExamId, s.ClassId
                ORDER BY r.Gpa DESC, r.TotalMarks DESC
            ) AS NewPosition
FROM StudentExamResults r WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON r.StudentId = s.Id
        WHERE r.ExamId IN (SELECT Id FROM @Exams)
          AND r.IsDeleted = 0
          AND s.IsDeleted = 0
    ) ranked ON ser.Id = ranked.Id;

    -- Section Position: rank within (ExamId, SectionId)
    UPDATE ser
    SET Position = ranked.NewPosition
FROM StudentExamResults ser WITH(NOLOCK)
    INNER JOIN (
        SELECT
            r.Id,
            RANK() OVER (
                PARTITION BY r.ExamId, s.SectionId
                ORDER BY r.Gpa DESC, r.TotalMarks DESC
            ) AS NewPosition
FROM StudentExamResults r WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON r.StudentId = s.Id
        WHERE r.ExamId IN (SELECT Id FROM @Exams)
          AND r.IsDeleted = 0
          AND s.IsDeleted = 0
    ) ranked ON ser.Id = ranked.Id;

    -- Group Position: rank within (ExamId, StudentGroupId)
    UPDATE ser
    SET GroupPosition = ranked.NewPosition
FROM StudentExamResults ser WITH(NOLOCK)
    INNER JOIN (
        SELECT
            r.Id,
            RANK() OVER (
                PARTITION BY r.ExamId, s.StudentGroupId
                ORDER BY r.Gpa DESC, r.TotalMarks DESC
            ) AS NewPosition
FROM StudentExamResults r WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON r.StudentId = s.Id
        WHERE r.ExamId IN (SELECT Id FROM @Exams)
          AND r.IsDeleted = 0
          AND s.IsDeleted = 0
          AND s.StudentGroupId IS NOT NULL
    ) ranked ON ser.Id = ranked.Id;

    SELECT COUNT(*) AS MeritPositionsCalculated
FROM StudentExamResults WITH(NOLOCK)
    WHERE ExamId IN (SELECT Id FROM @Exams) AND IsDeleted = 0;
END;
GO
