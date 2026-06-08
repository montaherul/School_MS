CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamComponents]
    @IncludeInactive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        Code,
        Description,
        DisplayOrder,
        DefaultFullMarks,
        DefaultPassMarks,
        IsPractical,
        IsOptional,
        IsActive,
        CreatedAt,
        CreatedBy
    FROM ExamComponents
    WHERE IsDeleted = 0
      AND (@IncludeInactive = 1 OR IsActive = 1)
    ORDER BY DisplayOrder, Name;
END;
GO
