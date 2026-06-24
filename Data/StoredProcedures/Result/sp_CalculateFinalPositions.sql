-- ============================================================
-- Phase 5: Calculate Final Result Positions (School, Class, Section, Group)
-- Uses configurable ranking rules with tie-breaking.
-- All 4 position types computed via SQL window functions.
-- ============================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_CalculateFinalPositions]
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- School-wide position
    ;WITH SchoolRank AS (
        SELECT 
            fr.Id,
            ROW_NUMBER() OVER (
                PARTITION BY fr.AcademicYearId
                ORDER BY fr.FinalGpa DESC, fr.WeightedTotalMarks DESC, s.RollNumber ASC
            ) AS SchoolPosition
FROM FinalResults fr WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON fr.StudentId = s.Id
        WHERE fr.AcademicYearId = @AcademicYearId
            AND fr.IsDeleted = 0
    )
    UPDATE fr
    SET fr.FinalPosition = sr.SchoolPosition,
        fr.UpdatedAt = GETUTCDATE()
FROM FinalResults fr WITH(NOLOCK)
INNER JOIN SchoolRank sr WITH(NOLOCK) ON fr.Id = sr.Id;

    -- Class position
    ;WITH ClassRank AS (
        SELECT 
            fr.Id,
            ROW_NUMBER() OVER (
                PARTITION BY fr.AcademicYearId, fr.SchoolClassId
                ORDER BY fr.FinalGpa DESC, fr.WeightedTotalMarks DESC, s.RollNumber ASC
            ) AS ClassPosition
FROM FinalResults fr WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON fr.StudentId = s.Id
        WHERE fr.AcademicYearId = @AcademicYearId
            AND fr.IsDeleted = 0
    )
    UPDATE fr
    SET fr.FinalClassPosition = cr.ClassPosition,
        fr.UpdatedAt = GETUTCDATE()
FROM FinalResults fr WITH(NOLOCK)
INNER JOIN ClassRank cr WITH(NOLOCK) ON fr.Id = cr.Id;

    -- Section position
    ;WITH SectionRank AS (
        SELECT 
            fr.Id,
            ROW_NUMBER() OVER (
                PARTITION BY fr.AcademicYearId, fr.SchoolClassId, fr.SectionId
                ORDER BY fr.FinalGpa DESC, fr.WeightedTotalMarks DESC, s.RollNumber ASC
            ) AS SectionPosition
FROM FinalResults fr WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON fr.StudentId = s.Id
        WHERE fr.AcademicYearId = @AcademicYearId
            AND fr.IsDeleted = 0
    )
    UPDATE fr
    SET fr.FinalSectionPosition = sr.SectionPosition,
        fr.UpdatedAt = GETUTCDATE()
FROM FinalResults fr WITH(NOLOCK)
INNER JOIN SectionRank sr WITH(NOLOCK) ON fr.Id = sr.Id;

    -- Group position (only for students with assigned groups)
    ;WITH GroupRank AS (
        SELECT 
            fr.Id,
            ROW_NUMBER() OVER (
                PARTITION BY fr.AcademicYearId, fr.StudentGroupId
                ORDER BY fr.FinalGpa DESC, fr.WeightedTotalMarks DESC, s.RollNumber ASC
            ) AS GroupPosition
FROM FinalResults fr WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON fr.StudentId = s.Id
        WHERE fr.AcademicYearId = @AcademicYearId
            AND fr.IsDeleted = 0
            AND fr.StudentGroupId IS NOT NULL
    )
    UPDATE fr
    SET fr.FinalGroupPosition = gr.GroupPosition,
        fr.UpdatedAt = GETUTCDATE()
FROM FinalResults fr WITH(NOLOCK)
INNER JOIN GroupRank gr WITH(NOLOCK) ON fr.Id = gr.Id;
END
GO
