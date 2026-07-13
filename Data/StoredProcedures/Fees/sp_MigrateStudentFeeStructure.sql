-- ============================================================================
-- Stored Procedure: sp_MigrateStudentFeeStructure
-- Purpose: Auto-migrate a student's fee assignments on promotion.
-- Deactivates old class fee structures, assigns new class fee structures.
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_MigrateStudentFeeStructure
    @StudentId INT,
    @OldClassId INT,
    @NewClassId INT,
    @AcademicYearId INT,
    @CreatedBy NVARCHAR(64) = 'auto-migrate-system'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = SYSDATETIME();

    DECLARE @DeactivatedCount INT = 0;
    DECLARE @CreatedCount INT = 0;

    -- Deactivate old class fee assignments (soft delete)
    UPDATE sfa
    SET sfa.IsActive = 0, sfa.ValidTo = CAST(@Now AS DATE),
        sfa.UpdatedBy = @CreatedBy, sfa.UpdatedAt = @Now
    FROM StudentFeeAssignments sfa
    JOIN FeeStructures fs ON fs.Id = sfa.FeeStructureId
    WHERE sfa.StudentId = @StudentId
        AND fs.SchoolClassId = @OldClassId
        AND sfa.IsDeleted = 0
        AND sfa.IsActive = 1;
    SET @DeactivatedCount = @@ROWCOUNT;

    -- Assign new class fee structures
    INSERT INTO StudentFeeAssignments (StudentId, FeeStructureId, AcademicYearId, IsActive, ValidFrom, CreatedBy, CreatedAt, IsDeleted)
    SELECT @StudentId, fs.Id, @AcademicYearId, 1, CAST(@Now AS DATE), @CreatedBy, @Now, 0
    FROM FeeStructures fs WITH(NOLOCK)
    WHERE fs.SchoolClassId = @NewClassId
        AND fs.IsActive = 1
        AND fs.IsDeleted = 0
        AND (fs.AcademicYearId IS NULL OR fs.AcademicYearId = @AcademicYearId)
        AND NOT EXISTS (
            SELECT 1 FROM StudentFeeAssignments sfa WITH(NOLOCK)
            WHERE sfa.StudentId = @StudentId
                AND sfa.FeeStructureId = fs.Id
                AND sfa.IsDeleted = 0
        );
    SET @CreatedCount = @@ROWCOUNT;

    SELECT @DeactivatedCount AS Deactivated, @CreatedCount AS Created;
END;
GO
