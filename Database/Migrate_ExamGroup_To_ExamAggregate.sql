-- ============================================================
-- Data Migration: ExamGroup → Exam Aggregate Tables
-- This script migrates any existing ExamGroup data into the
-- new Exam, ExamClass, ExamSection, ExamSubject, 
-- ExamSubjectComponent tables BEFORE the old tables are dropped.
-- ============================================================
-- Run this BEFORE applying PhaseXX_55_AddExamAggregateTables migration
-- ============================================================

SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY
    PRINT '=== Migrating ExamGroup → Exam Aggregate Tables ===';

    -- ============================================================
    -- 1. Migrate ExamGroups → Exams
    -- ============================================================
    PRINT '1. Migrating ExamGroups → Exams...';

    MERGE INTO Exams AS target
    USING (
        SELECT 
            eg.Id,
            eg.Name,
            eg.Term,
            eg.Status,
            eg.AcademicYearId,
            eg.StartDate AS StartsOn,
            eg.EndDate AS EndsOn,
            eg.IsLocked,
            eg.LockedAt,
            eg.LockedByUserId,
            eg.IsPublished,
            0 AS IsArchived,
            NULL AS ArchivedAt,
            NULL AS ArchivedByUserId,
            NULL AS ArchiveReason,
            -- Get first class for backward-compat ClassId/SectionId
            (SELECT TOP 1 ClassId FROM ExamGroupClasses WHERE ExamGroupId = eg.Id AND IsDeleted = 0 ORDER BY SortOrder) AS ClassId,
            (SELECT TOP 1 s.SectionId FROM ExamGroupClasses egc 
             INNER JOIN ExamGroupSections s ON s.ExamGroupClassId = egc.Id AND s.IsDeleted = 0
             WHERE egc.ExamGroupId = eg.Id AND egc.IsDeleted = 0) AS SectionId,
            (SELECT StudentGroupId FROM SchoolClasses WHERE Id = (
                SELECT TOP 1 ClassId FROM ExamGroupClasses WHERE ExamGroupId = eg.Id AND IsDeleted = 0)) AS StudentGroupId,
            eg.CreatedBy,
            eg.CreatedAt,
            eg.UpdatedBy,
            eg.UpdatedAt,
            eg.IsDeleted
        FROM ExamGroups eg
        WHERE NOT EXISTS (SELECT 1 FROM Exams e WHERE e.Id = eg.Id)
    ) AS source
    ON target.Id = source.Id
    WHEN NOT MATCHED THEN
        INSERT (Id, Name, Term, Status, AcademicYearId, StartsOn, EndsOn, IsLocked, LockedAt, LockedByUserId,
                IsPublished, IsArchived, ArchivedAt, ArchivedByUserId, ArchiveReason,
                ClassId, SectionId, StudentGroupId, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
        VALUES (source.Id, source.Name, source.Term, source.Status, source.AcademicYearId, source.StartsOn, source.EndsOn,
                source.IsLocked, source.LockedAt, source.LockedByUserId,
                source.IsPublished, source.IsArchived, source.ArchivedAt, source.ArchivedByUserId, source.ArchiveReason,
                source.ClassId, source.SectionId, source.StudentGroupId,
                source.CreatedBy, source.CreatedAt, source.UpdatedBy, source.UpdatedAt, source.IsDeleted);

    PRINT '   Done.';

    -- ============================================================
    -- 2. Migrate ExamGroupClasses → ExamClasses
    -- ============================================================
    PRINT '2. Migrating ExamGroupClasses → ExamClasses...';

    MERGE INTO ExamClasses AS target
    USING (
        SELECT 
            egc.Id,
            egc.ExamGroupId AS ExamId,
            egc.ClassId,
            COALESCE(egc.ClassName, c.Name, '') AS ClassName,
            COALESCE(egc.SortOrder, c.DisplayOrder, 0) AS SortOrder,
            egc.CreatedBy,
            egc.CreatedAt,
            egc.UpdatedBy,
            egc.UpdatedAt,
            egc.IsDeleted
        FROM ExamGroupClasses egc
        INNER JOIN Classes c ON c.Id = egc.ClassId
        WHERE NOT EXISTS (SELECT 1 FROM ExamClasses ec WHERE ec.Id = egc.Id)
    ) AS source
    ON target.Id = source.Id
    WHEN NOT MATCHED THEN
        INSERT (Id, ExamId, ClassId, ClassName, SortOrder, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
        VALUES (source.Id, source.ExamId, source.ClassId, source.ClassName, source.SortOrder,
                source.CreatedBy, source.CreatedAt, source.UpdatedBy, source.UpdatedAt, source.IsDeleted);

    PRINT '   Done.';

    -- ============================================================
    -- 3. Migrate ExamGroupSections → ExamSections
    -- ============================================================
    PRINT '3. Migrating ExamGroupSections → ExamSections...';

    MERGE INTO ExamSections AS target
    USING (
        SELECT 
            egs.Id,
            egs.ExamGroupClassId AS ExamClassId,
            egs.SectionId,
            COALESCE(egs.SectionName, s.Name, '') AS SectionName,
            egs.CreatedBy,
            egs.CreatedAt,
            egs.UpdatedBy,
            egs.UpdatedAt,
            egs.IsDeleted
        FROM ExamGroupSections egs
        INNER JOIN Sections s ON s.Id = egs.SectionId
        WHERE NOT EXISTS (SELECT 1 FROM ExamSections es WHERE es.Id = egs.Id)
    ) AS source
    ON target.Id = source.Id
    WHEN NOT MATCHED THEN
        INSERT (Id, ExamClassId, SectionId, SectionName, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
        VALUES (source.Id, source.ExamClassId, source.SectionId, source.SectionName,
                source.CreatedBy, source.CreatedAt, source.UpdatedBy, source.UpdatedAt, source.IsDeleted);

    PRINT '   Done.';

    -- ============================================================
    -- 4. Migrate ExamGroupSubjects → ExamSubjects
    -- ============================================================
    PRINT '4. Migrating ExamGroupSubjects → ExamSubjects...';

    MERGE INTO ExamSubjects AS target
    USING (
        SELECT 
            egs.Id,
            egc.ExamGroupId AS ExamId,
            egs.SubjectId,
            egc.ClassId,
            NULL AS StudentGroupId,
            egs.FullMarks,
            egs.PassMarks,
            egs.IsOptional,
            egs.IsReligionSubject,
            egs.TeacherId,
            COALESCE(egs.SubjectName, sub.Name, '') AS SubjectName,
            COALESCE(egs.SubjectCode, sub.Code, '') AS SubjectCode,
            COALESCE(egs.SubjectType, '') AS SubjectType,
            COALESCE(egs.SubjectGroup, '') AS SubjectGroup,
            COALESCE(egs.TheoryMarks, egs.FullMarks, 100) AS TheoryMarks,
            COALESCE(egs.PracticalMarks, 0) AS PracticalMarks,
            egs.TeacherName,
            egs.TeacherEmployeeCode,
            COALESCE(egs.Credit, sub.Credit, 0) AS Credit,
            COALESCE(egs.NCTBCode, sub.NCTBCode, NULL) AS NCTBCode,
            NULL AS ExamClassId,
            1 AS IsActive,
            NULL AS ExamDate,
            NULL AS ExamStartTime,
            NULL AS ExamDuration,
            NULL AS RoomNumber,
            egs.CreatedBy,
            egs.CreatedAt,
            egs.UpdatedBy,
            egs.UpdatedAt,
            egs.IsDeleted
        FROM ExamGroupSubjects egs
        INNER JOIN ExamGroupClasses egc ON egc.Id = egs.ExamGroupClassId AND egc.IsDeleted = 0
        INNER JOIN Subjects sub ON sub.Id = egs.SubjectId
        WHERE NOT EXISTS (SELECT 1 FROM ExamSubjects es2 WHERE es2.Id = egs.Id)
    ) AS source
    ON target.Id = source.Id
    WHEN NOT MATCHED THEN
        INSERT (Id, ExamId, SubjectId, ClassId, StudentGroupId, FullMarks, PassMarks, IsOptional, IsReligionSubject,
                TeacherId, SubjectName, SubjectCode, SubjectType, SubjectGroup, TheoryMarks, PracticalMarks,
                TeacherName, TeacherEmployeeCode, Credit, NCTBCode, ExamClassId, IsActive,
                ExamDate, ExamStartTime, ExamDuration, RoomNumber,
                CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
        VALUES (source.Id, source.ExamId, source.SubjectId, source.ClassId, source.StudentGroupId,
                source.FullMarks, source.PassMarks, source.IsOptional, source.IsReligionSubject,
                source.TeacherId, source.SubjectName, source.SubjectCode, source.SubjectType, source.SubjectGroup,
                source.TheoryMarks, source.PracticalMarks,
                source.TeacherName, source.TeacherEmployeeCode, source.Credit, source.NCTBCode,
                source.ExamClassId, source.IsActive,
                source.ExamDate, source.ExamStartTime, source.ExamDuration, source.RoomNumber,
                source.CreatedBy, source.CreatedAt, source.UpdatedBy, source.UpdatedAt, source.IsDeleted);

    PRINT '   Done.';

    -- ============================================================
    -- 5. Migrate ExamGroupSubjectComponents → ExamSubjectComponents
    -- ============================================================
    PRINT '5. Migrating ExamGroupSubjectComponents → ExamSubjectComponents...';

    MERGE INTO ExamSubjectComponents AS target
    USING (
        SELECT 
            egsc.Id,
            egsc.ExamGroupSubjectId AS ExamSubjectId,
            egsc.ComponentId,
            egsc.MaxMarks,
            egsc.PassMarks,
            egsc.DisplayOrder,
            COALESCE(egsc.ComponentName, ec.Name, '') AS ComponentName,
            COALESCE(egsc.ComponentCode, ec.Code, '') AS ComponentCode,
            COALESCE(egsc.Weight, 0) AS Weight,
            egsc.CreatedBy,
            egsc.CreatedAt,
            egsc.UpdatedBy,
            egsc.UpdatedAt,
            egsc.IsDeleted
        FROM ExamGroupSubjectComponents egsc
        INNER JOIN ExamComponents ec ON ec.Id = egsc.ComponentId
        WHERE NOT EXISTS (SELECT 1 FROM ExamSubjectComponents esc WHERE esc.Id = egsc.Id)
    ) AS source
    ON target.Id = source.Id
    WHEN NOT MATCHED THEN
        INSERT (Id, ExamSubjectId, ComponentId, MaxMarks, PassMarks, DisplayOrder,
                ComponentName, ComponentCode, Weight,
                CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
        VALUES (source.Id, source.ExamSubjectId, source.ComponentId,
                source.MaxMarks, source.PassMarks, source.DisplayOrder,
                source.ComponentName, source.ComponentCode, source.Weight,
                source.CreatedBy, source.CreatedAt, source.UpdatedBy, source.UpdatedAt, source.IsDeleted);

    PRINT '   Done.';

    -- ============================================================
    -- 6. Validate migration
    -- ============================================================
    PRINT '=== Validation ===';
    DECLARE @ExamGroupCount INT, @MigratedExamCount INT;
    SELECT @ExamGroupCount = COUNT(*) FROM ExamGroups WHERE IsDeleted = 0;
    SELECT @MigratedExamCount = COUNT(*) FROM Exams e 
    WHERE EXISTS (SELECT 1 FROM ExamGroups eg WHERE eg.Id = e.Id);
    PRINT 'ExamGroups found: ' + CAST(@ExamGroupCount AS NVARCHAR);
    PRINT 'Exams migrated: ' + CAST(@MigratedExamCount AS NVARCHAR);

    DECLARE @ClassCount INT, @MigratedClassCount INT;
    SELECT @ClassCount = COUNT(*) FROM ExamGroupClasses WHERE IsDeleted = 0;
    SELECT @MigratedClassCount = COUNT(*) FROM ExamClasses ec
    WHERE EXISTS (SELECT 1 FROM ExamGroupClasses egc WHERE egc.Id = ec.Id);
    PRINT 'ExamGroupClasses found: ' + CAST(@ClassCount AS NVARCHAR);
    PRINT 'ExamClasses migrated: ' + CAST(@MigratedClassCount AS NVARCHAR);

    COMMIT TRANSACTION;
    PRINT '=== Migration completed successfully ===';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    PRINT 'ERROR: ' + @ErrorMessage;
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO
