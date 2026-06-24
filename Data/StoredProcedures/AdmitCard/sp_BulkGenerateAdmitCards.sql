CREATE OR ALTER PROCEDURE [dbo].[sp_BulkGenerateAdmitCards]
    @ExamId INT,
    @ClassId INT = NULL,
    @SectionId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Students TABLE (StudentId INT);

    INSERT INTO @Students
    SELECT s.Id
FROM Students s WITH(NOLOCK)
    WHERE s.Status = 1
      AND s.IsDeleted = 0
      AND (@ClassId IS NULL OR s.ClassId = @ClassId)
      AND (@SectionId IS NULL OR s.SectionId = @SectionId);

    INSERT INTO AdmitCards (ExamId, StudentId, CardNo, IsIssued, IsGenerated, IssuedAt, PrintedAt, CreatedAt, IsDeleted)
    SELECT
        @ExamId,
        s.StudentId,
        'AC-' + CAST(@ExamId AS NVARCHAR) + '-' + CAST(s.StudentId AS NVARCHAR),
        1, 1, GETUTCDATE(), GETUTCDATE(), GETUTCDATE(), 0
    FROM @Students s
    WHERE NOT EXISTS (
        SELECT 1 FROM AdmitCards ac
        WHERE ac.ExamId = @ExamId AND ac.StudentId = s.StudentId AND ac.IsDeleted = 0
    );

    SELECT @@ROWCOUNT AS CardsGenerated;
END;
GO
