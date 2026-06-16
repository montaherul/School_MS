CREATE OR ALTER PROCEDURE [dbo].[sp_RecalculateResults]
    @ExamId INT,
    @AcademicYearId INT,
    @RecalculatedByUserId INT,
    @Reason NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    -- 1. Recalculate subject results
    EXEC sp_CalculateSubjectResults @ExamId, @AcademicYearId;

    -- 2. Recalculate exam results
    EXEC sp_CalculateExamResults @ExamId, @AcademicYearId;

    -- 3. Recalculate merit positions for this exam's group
    DECLARE @ExamName NVARCHAR(500);
    SELECT @ExamName = Name FROM Exams WHERE Id = @ExamId;
    EXEC sp_CalculateMerit @ExamGroupKey = @ExamName;

    -- 4. Audit trail
    INSERT INTO ResultAuditLogs (ExamId, StudentId, SubjectId, OldMarks, NewMarks, ChangedByUserId, Reason, ChangeType, ChangedAt, CreatedAt, IsDeleted)
    VALUES (@ExamId, NULL, NULL, NULL, NULL, @RecalculatedByUserId, @Reason, 'RECALCULATE', GETUTCDATE(), GETUTCDATE(), 0);

    COMMIT TRANSACTION;

    SELECT 1 AS Success;
END;
GO
