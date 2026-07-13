-- ============================================================================
-- Stored Procedure: sp_AutoAssignStudentFeeStructure
-- Purpose: Auto-assign fee structures to a student on admission
-- or to a new student. Finds matching fee structures by class.
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_AutoAssignStudentFeeStructure
    @StudentId INT,
    @AcademicYearId INT,
    @CreatedBy NVARCHAR(64) = 'auto-assign-system'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ClassId INT, @Now DATETIME2 = SYSDATETIME();

    SELECT @ClassId = ClassId FROM Students WHERE Id = @StudentId AND IsDeleted = 0;
    IF @ClassId IS NULL RETURN;

    -- Assign all active fee structures for this class
    INSERT INTO StudentFeeAssignments (StudentId, FeeStructureId, AcademicYearId, IsActive, ValidFrom, CreatedBy, CreatedAt, IsDeleted)
    SELECT @StudentId, fs.Id, @AcademicYearId, 1, CAST(@Now AS DATE), @CreatedBy, @Now, 0
    FROM FeeStructures fs WITH(NOLOCK)
    WHERE fs.SchoolClassId = @ClassId
        AND fs.IsActive = 1
        AND fs.IsDeleted = 0
        AND (fs.AcademicYearId IS NULL OR fs.AcademicYearId = @AcademicYearId)
        AND NOT EXISTS (
            SELECT 1 FROM StudentFeeAssignments sfa WITH(NOLOCK)
            WHERE sfa.StudentId = @StudentId
                AND sfa.FeeStructureId = fs.Id
                AND sfa.IsDeleted = 0
        );
END;
GO
