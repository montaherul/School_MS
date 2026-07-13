-- ============================================================================
-- Stored Procedure: sp_CopyFeeStructureForAcademicYear
-- Purpose: Copy previous year's fee structures to a new academic year.
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_CopyFeeStructureForAcademicYear
    @FromAcademicYearId INT,
    @ToAcademicYearId INT,
    @CreatedBy NVARCHAR(64) = 'auto-copy-system'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = SYSDATETIME();
    DECLARE @Copied INT = 0;

    INSERT INTO FeeStructures (SchoolClassId, FeeCategoryId, AcademicYearId, FeeName, Description, Amount, IsRecurring, Frequency, DueDay, IsActive, CreatedBy, CreatedAt, IsDeleted)
    SELECT
        fs.SchoolClassId,
        fs.FeeCategoryId,
        @ToAcademicYearId,
        fs.FeeName,
        fs.Description,
        fs.Amount,
        fs.IsRecurring,
        fs.Frequency,
        fs.DueDay,
        fs.IsActive,
        @CreatedBy,
        @Now,
        0
    FROM FeeStructures fs WITH(NOLOCK)
    WHERE fs.AcademicYearId = @FromAcademicYearId
        AND fs.IsDeleted = 0
        AND NOT EXISTS (
            SELECT 1 FROM FeeStructures fs2 WITH(NOLOCK)
            WHERE fs2.SchoolClassId = fs.SchoolClassId
                AND fs2.FeeCategoryId = fs.FeeCategoryId
                AND fs2.AcademicYearId = @ToAcademicYearId
                AND fs2.FeeName = fs.FeeName
                AND fs2.IsDeleted = 0
        );

    SET @Copied = @@ROWCOUNT;
    SELECT @Copied AS CopiedCount;
END;
GO
